/* NemeStats browser QA support; GPL-3.0-or-later. */

// Append observerSource to the actual SignalR response, before any UI setup.
// This installs into the current document only; navigation creates a fresh ledger.
export function installObserver(window) {
    const $ = window.jQuery;
    if (!$ || typeof $.hubConnection !== 'function') {
        throw new Error('Observer requires actual jQuery and SignalR before UI setup');
    }
    if (window.__nemeStatsObserver) {
        throw new Error('Observer already installed in this document');
    }
    const connections = [];
    const proxies = [];
    const joins = [];
    const events = [];
    const waiters = new Set();
    let sequence = 0;
    let invocationId = 0;
    const originalHubConnection = $.hubConnection;

    function identity(proxyRecord) {
        return {
            connectionObjectId: proxyRecord.connectionRecord.connectionObjectId,
            connectionId: proxyRecord.connectionRecord.connection.id || null,
            proxyId: proxyRecord.proxyId,
            hubName: proxyRecord.hubName
        };
    }
    function matches(row, selector) {
        return Object.entries(selector).every(([key, value]) => {
            if (key === 'hubName' || key === 'eventName') {
                return String(row[key]).toLowerCase() === String(value).toLowerCase();
            }
            if (key === 'markerId') return row.args[0] === value;
            return row[key] === value;
        });
    }
    function notify() {
        for (const check of [...waiters]) check();
    }
    function recordEvent(proxyRecord, eventName, args) {
        events.push({ ...identity(proxyRecord), sequence: ++sequence,
            eventName, args });
        notify();
    }
    function observeProxy(proxy, connectionRecord) {
        if (proxies.some(record => record.proxy === proxy)) return;
        const record = { proxy, connectionRecord, proxyId: proxies.length + 1,
            hubName: proxy.hubName };
        proxies.push(record);
        const originalInvoke = proxy.invoke;
        proxy.invoke = function () {
            const args = Array.from(arguments);
            // Let the original invocation throw/return unchanged before observing it.
            const promise = originalInvoke.apply(this, arguments);
            const id = ++invocationId;
            if (String(args[0]).toLowerCase() === 'join') {
                const join = { ...identity(record), joinId: id,
                    groupKey: args[1], args, promise, status: 'pending',
                    sequence: ++sequence };
                joins.push(join);
                promise.done(function () {
                    Object.assign(join, identity(record), { status: 'succeeded',
                        result: Array.from(arguments), settledSequence: ++sequence });
                    notify();
                });
                promise.fail(function () {
                    Object.assign(join, identity(record), { status: 'failed',
                        failure: Array.from(arguments), settledSequence: ++sequence });
                    notify();
                });
                notify();
            }
            return promise;
        };
        // Do not replace, invoke or wrap the product callbacks. Observe the same proxy.
        const productEvent = {
            notificationshub: 'NewAchievementUnlocked',
            longrunningtaskhub: 'BGGImportDetailsProgress'
        }[String(record.hubName).toLowerCase()];
        for (const eventName of [productEvent, 'QaMarker'].filter(Boolean)) {
            proxy.on(eventName, function () {
                recordEvent(record, eventName, Array.from(arguments));
            });
        }
    }
    function observedHubConnection() {
        const connection = originalHubConnection.apply(this, arguments);
        if (!connections.some(record => record.connection === connection)) {
            const record = { connection, connectionObjectId: connections.length + 1 };
            connections.push(record);
            const originalCreateHubProxy = connection.createHubProxy;
            connection.createHubProxy = function () {
                const proxy = originalCreateHubProxy.apply(this, arguments);
                observeProxy(proxy, record);
                return proxy;
            };
        }
        return connection;
    }
    // Preserve SignalR's public factory properties, notably fn and prototype.
    for (const key of Reflect.ownKeys(originalHubConnection)) {
        if (['length', 'name', 'arguments', 'caller'].includes(key)) continue;
        Object.defineProperty(observedHubConnection, key,
            Object.getOwnPropertyDescriptor(originalHubConnection, key));
    }
    $.hubConnection = observedHubConnection;

    function boundedWait(find, options = {}) {
        const timeoutMs = options.timeoutMs ?? 30000;
        if (!Number.isFinite(timeoutMs) || timeoutMs <= 0 || timeoutMs > 30000) {
            throw new Error('Observer timeoutMs must be in (0, 30000]');
        }
        return new Promise((resolve, reject) => {
            let timer;
            const cleanup = () => {
                window.clearTimeout(timer);
                waiters.delete(check);
                options.signal?.removeEventListener('abort', abort);
            };
            const abort = () => {
                cleanup();
                reject(new Error('Observer subscription aborted'));
            };
            const check = () => {
                const result = find();
                if (!result) return;
                cleanup();
                if (result.error) reject(result.error);
                else resolve(result.value);
            };
            timer = window.setTimeout(() => {
                cleanup();
                reject(new Error('Observer subscription timed out'));
            }, timeoutMs);
            waiters.add(check);
            options.signal?.addEventListener('abort', abort, { once: true });
            if (options.signal?.aborted) abort();
            else check();
        });
    }
    const api = {
        // Live references are intentionally available for strict identity evidence.
        connections, proxies, joins, events,
        get sequence() { return sequence; },
        waitForJoin(selector, options) {
            return boundedWait(() => {
                const join = [...joins].reverse().find(row => matches(row, selector));
                if (!join || join.status === 'pending') return null;
                if (join.status === 'failed') {
                    const error = new Error('Original UI Join failed');
                    error.join = join;
                    return { error };
                }
                return { value: join };
            }, options);
        },
        // Subscribe before sending. Replay requires an explicit afterSequence cursor.
        waitForEvent(selector, options = {}) {
            const afterSequence = options.afterSequence ?? sequence;
            return boundedWait(() => {
                const event = events.find(row => row.sequence > afterSequence &&
                    matches(row, selector));
                return event ? { value: event } : null;
            }, options);
        },
        snapshot() {
            return {
                sequence,
                connections: connections.map(record => ({
                    connectionObjectId: record.connectionObjectId,
                    connectionId: record.connection.id || null })),
                proxies: proxies.map(record => identity(record)),
                joins: joins.map(({ promise, ...row }) => row),
                events: events.map(row => ({ ...row }))
            };
        }
    };
    window.__nemeStatsObserver = api;
    return api;
}

export const observerSource = '(' + installObserver.toString() + ')(window);';

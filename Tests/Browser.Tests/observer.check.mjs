/* NemeStats browser QA support; GPL-3.0-or-later. */
import assert from 'node:assert/strict';
import vm from 'node:vm';
import { observerSource } from './observer.mjs';

// Actual-shaped fake only: this does not establish browser/SignalR runtime success.
const timers = new Map();
let timerId = 0;
const calls = [];
const callbacks = new Map();
const results = [];
function invocationResult() {
    const done = [];
    const fail = [];
    const result = {
        done(callback) { done.push(callback); return this; },
        fail(callback) { fail.push(callback); return this; },
        resolve(...args) { for (const callback of done) callback.apply(proxy, args); },
        reject(...args) { for (const callback of fail) callback.apply(proxy, args); }
    };
    return result;
}
const proxy = {
    hubName: 'notificationshub',
    on(name, callback) {
        const list = callbacks.get(name.toLowerCase()) || [];
        list.push(callback);
        callbacks.set(name.toLowerCase(), list);
        return this;
    },
    invoke(...args) {
        calls.push({ method: 'invoke', receiver: this, args });
        const result = invocationResult();
        results.push(result);
        return result;
    }
};
const connection = {
    id: null,
    createHubProxy(...args) {
        calls.push({ method: 'createHubProxy', receiver: this, args });
        return proxy;
    }
};
proxy.connection = connection;
function hubConnection(...args) {
    calls.push({ method: 'hubConnection', receiver: this, args });
    return connection;
}
hubConnection.fn = hubConnection.prototype;
const window = {
    jQuery: { hubConnection },
    setTimeout(callback) { const id = ++timerId; timers.set(id, callback); return id; },
    clearTimeout(id) { timers.delete(id); }
};
vm.runInNewContext(observerSource, { window });
const observer = window.__nemeStatsObserver;
assert.strictEqual(window.jQuery.hubConnection.fn, hubConnection.fn);
assert.strictEqual(window.jQuery.hubConnection.prototype, hubConnection.prototype);
const options = { logging: false };
assert.strictEqual(window.jQuery.hubConnection('/signalr', options), connection);
assert.strictEqual(calls[0].receiver, window.jQuery);
assert.strictEqual(calls[0].args[1], options);
assert.strictEqual(connection.createHubProxy('notificationsHub'), proxy);
assert.strictEqual(calls[1].receiver, connection);
assert.deepEqual(calls[1].args, ['notificationsHub']);
assert.strictEqual(connection.createHubProxy('notificationsHub'), proxy);
assert.equal(callbacks.get('qamarker').length, 1, 'cached proxies observed once');
assert.strictEqual(observer.connections[0].connection, connection);
assert.strictEqual(observer.proxies[0].proxy, proxy);

const selector = { hubName: 'notificationsHub', groupKey: 'owned-user' };
const membership = observer.waitForJoin(selector);
const promise = proxy.invoke('join', 'owned-user');
assert.strictEqual(promise, results[0], 'original invoke result returned');
assert.strictEqual(observer.joins[0].promise, promise, 'original Join result observed');
assert.strictEqual(calls.at(-1).receiver, proxy);
assert.deepEqual(calls.at(-1).args, ['join', 'owned-user']);
assert.equal(observer.joins[0].status, 'pending');
assert.equal(timers.size, 1, 'pending Join has not released subscription');
connection.id = 'actual-connection-id';
promise.resolve('joined');
assert.strictEqual(await membership, observer.joins[0]);
assert.strictEqual(await observer.waitForJoin(selector), observer.joins[0],
    'already-recorded original success is a barrier');
assert.equal(observer.joins[0].connectionId, connection.id);
assert.equal(timers.size, 0);

const failedPromise = proxy.invoke('Join', 'owned-user');
assert.strictEqual(failedPromise, results[1]);
const failedWait = observer.waitForJoin(selector);
const rejected = assert.rejects(failedWait, error =>
    error.message === 'Original UI Join failed' && error.join.promise === failedPromise);
failedPromise.reject(new Error('server rejected membership'));
await rejected;
await assert.rejects(observer.waitForJoin(selector), /Original UI Join failed/);
assert.equal(observer.joins[1].status, 'failed');
assert.equal(calls.filter(call => call.method === 'invoke').length, 2, 'no extra Join');
assert.strictEqual(proxy.invoke('unrelated', options), results[2]);
assert.equal(observer.joins.length, 2, 'non-Join result is returned but not a barrier');

let productCalls = 0;
proxy.on('NewAchievementUnlocked', function (...args) {
    assert.strictEqual(this, proxy);
    assert.deepEqual(args, [7, 'Title', 'icon', 'Description', 'Gold']);
    productCalls++;
});
function emit(name, ...args) {
    for (const callback of callbacks.get(name.toLowerCase())) callback.apply(proxy, args);
}
const eventWait = observer.waitForEvent({ proxyId: 1, eventName: 'NewAchievementUnlocked' });
emit('NewAchievementUnlocked', 7, 'Title', 'icon', 'Description', 'Gold');
assert.equal((await eventWait).connectionId, connection.id);
assert.equal(productCalls, 1, 'observer never invokes product callback');
const markerWait = observer.waitForEvent({ proxyId: 1, eventName: 'QaMarker', markerId: 'm2' });
emit('QaMarker', 'm1');
assert.equal(timers.size, 1, 'wrong marker cannot release subscription');
emit('QaMarker', 'm2');
assert.equal((await markerWait).args[0], 'm2');
const timeout = observer.waitForEvent({ eventName: 'QaMarker' });
const timeoutRejected = assert.rejects(timeout, /timed out/);
assert.equal(timers.size, 1, 'recorded marker is not replayed by default');
[...timers.values()][0]();
await timeoutRejected;
const controller = new AbortController();
const cancelled = observer.waitForEvent({ eventName: 'QaMarker' }, { signal: controller.signal });
const cancelledRejected = assert.rejects(cancelled, /aborted/);
controller.abort();
await cancelledRejected;
assert.equal(timers.size, 0);
assert.throws(() => observer.waitForJoin(selector, { timeoutMs: 30001 }), /timeoutMs/);
assert.equal(observer.snapshot().joins[0].promise, undefined);
assert.equal(JSON.parse(JSON.stringify(observer.snapshot())).events.length, 3);
console.log('PASS actual-shaped fake: original identities/receivers/args, Join success/failure, event/marker bounds');

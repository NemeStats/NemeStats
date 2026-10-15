/* NemeStats browser QA support; GPL-3.0-or-later. */
import assert from 'node:assert/strict';
import { readFile, writeFile, mkdir, rm, access } from 'node:fs/promises';
import { watch } from 'node:fs';
import { resolve, dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';
import { spawn, execFile } from 'node:child_process';
import { promisify } from 'node:util';
import { createHash, randomUUID } from 'node:crypto';
import https from 'node:https';
import net from 'node:net';
import { loadOmowright } from 'file:///C:/Users/brian/.omo/binary-runtime/5.1.29/plugin/skills/browser/scripts/omowright.mjs';
import { observerSource } from './observer.mjs';

const exec = promisify(execFile);
const here = dirname(fileURLToPath(import.meta.url));
const args = Object.fromEntries(process.argv.slice(2).reduce((rows, value, i, all) => {
    if (value.startsWith('--')) rows.push([value.slice(2),
        ['--acceptance', '--check-seam', '--check-dependencies', '--check-routing', '--diagnostic-comparison', '--seed-only', '--negative-control'].includes(value) ? true : all[i + 1]]);
    return rows;
}, []));
const bounded = (promise, label) => {
    let timeout;
    return Promise.race([promise, new Promise((_, reject) => {
        timeout = setTimeout(() => reject(new Error(label + ' timed out')), 30000);
    })]).finally(() => clearTimeout(timeout));
};
const task = args.acceptance ? 10 : args['check-routing'] ? 9 : args.mode === 'migrate' || args['diagnostic-comparison'] ? 7 :
    args['check-dependencies'] ? 6 : args['check-seam'] ? 2 : args.mode === 'upgraded' ? 8 : 3;
const prefix = 'task-' + task + '-' + (task === 10 ? args.stage + '-' : '') + args.bundle + '-' + args.viewport +
    (task === 7 ? '-' + args.mode : '') +
    (args.mode === 'upgraded' && args['seed-only'] ? '-seed-only' : '');
const output = { task, mode: args.mode, attemptId: randomUUID(), runtime: process.version, assertions: [],
    sends: [], blockedExternalRequests: [], status: 'running' };
let manifest, browser, host, profile, hostExit, browserExit, ca, activePage, pageSession, probeProfile, controlSocket;
const modernizrUrl = 'https://cdnjs.cloudflare.com/ajax/libs/modernizr/2.8.3/modernizr.min.js';
let modernizrBytes;
let diagnosticMigrate;
const migratePath = join(here, '../../.omo/evidence/jquery-3-migration/wave2/task-7/jquery-migrate-3.5.2.js');
const migrateOfficialHash = '4e115c36bfefd71295b79726a252486ae507bed5c53b0c2a8933fb21b80ff045';
const diagnosticSnapshot = page => main(page, `({
    url:location.pathname,time:performance.now(),readyState:document.readyState,
    jquery:window.jQuery?.fn.jquery,migrateVersion:window.jQuery?.migrateVersion || null,
    migrateMessages:Array.from(window.jQuery?.migrateWarnings || []),
    injectionOrder:window.__qaDiagnosticOrder || null,
    legacyHtmlPrefilterEnabled:window.jQuery?.migrateIsPatchEnabled?.('self-closed-tags') || false
})`);
let failedJoinProbe = false;
let forcedFallback = false, controlledRequest = null;
const allRoutes = [];
const browserBoundarySource = `window.ga=function(){
    window.__qaBrowserAnalyticsCapture(JSON.stringify(Array.from(arguments)));
};
window.addEventListener('load',function(){
    const element=document.querySelector('.bggdescription[data-readmore]');
    if(!element)return;
    window.__qaReadmoreMeasured=new Promise((resolve,reject)=>{
        const observer=new MutationObserver(rows=>{
            const clone=node=>node!==element&&node.nodeType===1&&node.matches('.bggdescription');
            if(!rows.some(row=>Array.from(row.addedNodes).some(clone))||
                !rows.some(row=>Array.from(row.removedNodes).some(clone)))return;
            observer.disconnect();clearTimeout(timer);
            resolve({source:'Original readmore load/resize clone measurement mutation',
                height:element.getBoundingClientRect().height,style:element.getAttribute('style')});
        });
        const timer=setTimeout(()=>{observer.disconnect();reject(new Error('Original readmore measurement timeout'));},30000);
        observer.observe(element.parentElement,{childList:true});
    });
    window.__qaReadmoreMeasured.catch(()=>{});
});`;
async function readAuthorizedModernizr() {
    modernizrBytes = await bounded(new Promise((resolveBytes, reject) => {
        // One exact HTTPS GET with normal certificate validation; never follow redirects.
        const req = https.get(modernizrUrl, response => {
            if (response.statusCode !== 200) {
                response.resume();
                return reject(new Error('Exact Modernizr GET returned ' + response.statusCode));
            }
            const chunks = [];
            response.on('data', chunk => chunks.push(chunk));
            response.on('end', () => resolveBytes(Buffer.concat(chunks)));
            response.on('error', reject);
        });
        req.on('error', reject);
    }), 'Authorized exact Modernizr GET');
    output.authorizedStaticResource = { url: modernizrUrl, method: 'GET', status: 200,
        bytes: modernizrBytes.length, sha256: createHash('sha256').update(modernizrBytes).digest('hex'),
        redirectsFollowed: 0, browserFulfillments: 0, hostOutboundRelaxed: false };
}
const check = (id, actual, expected = true) => {
    try { assert.deepEqual(actual, expected, id); }
    catch (error) { output.assertions.push({ id, passed: false }); throw error; }
    output.assertions.push({ id, passed: true });
};
async function request(url, options = {}) {
    const target = new URL(url);
    assert.equal(target.origin, manifest.httpsOrigin, 'Owned HTTPS requests only');
    return bounded(new Promise((resolveRequest, reject) => {
        const req = https.request(target, { ca, method: options.method || 'GET',
            headers: options.headers || {} }, response => {
            const bytes = [];
            response.on('data', data => bytes.push(data));
            response.on('end', () => resolveRequest({ status: response.statusCode,
                headers: response.headers, body: Buffer.concat(bytes).toString('utf8') }));
        });
        req.on('error', reject);
        req.end(options.body);
    }), 'HTTPS request');
}
async function pipe(command, payload = {}, secret = manifest.qaPipeSecret) {
    const requestId = randomUUID();
    if (!controlSocket || controlSocket.destroyed) {
        controlSocket = net.createConnection('\\\\.\\pipe\\' + manifest.qaPipeName);
        const ownedSocket = controlSocket;
        ownedSocket.once('close', () => {
            if (controlSocket === ownedSocket) controlSocket = null;
        });
        await bounded(new Promise((resolveConnected, reject) => {
            controlSocket.once('connect', resolveConnected);
            controlSocket.once('error', reject);
        }), 'Local authenticated pipe connection');
    }
    return bounded(new Promise((resolvePipe, reject) => {
        const socket = controlSocket;
        let buffer = '';
        const onError = error => { cleanup(); reject(error); };
        const cleanup = () => { socket.off('data', onData); socket.off('error', onError); };
        const onData = chunk => {
            buffer += chunk;
            if (!buffer.includes('\n')) return;
            cleanup();
            try {
                const response = JSON.parse(buffer.split('\n')[0]);
                assert.equal(response.requestId, requestId);
                resolvePipe(response);
            } catch (error) { reject(error); }
        };
        socket.on('data', onData);
        socket.once('error', onError);
        socket.write(JSON.stringify({ requestId, runId: manifest.runId, secret, command, payload }) + '\n');
    }), 'Authenticated pipe ' + command);
}
async function main(page, expression) {
    const result = await bounded(page.cdp.send('Runtime.evaluate', {
        expression, returnByValue: true, awaitPromise: true
    }, pageSession), 'Main document evaluation');
    if (result.exceptionDetails) throw new Error(result.exceptionDetails.text + ': ' +
        (result.exceptionDetails.exception?.description || ''));
    return result.result.value;
}
async function responseSignal(page, path, method) {
    const sessionId = pageSession;
    await page.cdp.send('Network.enable', {}, sessionId);
    const tracked = new Set();
    const cleanup = [];
    const promise = new Promise(resolveResponse => {
        cleanup.push(page.cdp.on('Network.requestWillBeSent', (event, meta) => {
            if (meta?.sessionId !== sessionId) return;
            if (event.redirectResponse && tracked.has(event.requestId))
                resolveResponse({ status: event.redirectResponse.status, redirected: true });
            if (event.request.method === method && new URL(event.request.url).pathname === path)
                tracked.add(event.requestId);
        }));
        cleanup.push(page.cdp.on('Network.responseReceived', (event, meta) => {
            if (meta?.sessionId === sessionId && tracked.has(event.requestId))
                resolveResponse({ status: event.response.status, redirected: false });
        }));
    });
    return { promise: bounded(promise, method + ' response').finally(() => cleanup.forEach(off => { off(); })) };
}
async function loadSignal(page) {
    const sessionId = pageSession;
    let off;
    const promise = new Promise(resolveLoad => {
        off = page.cdp.on('Page.loadEventFired', (_, meta) => {
            if (meta?.sessionId === sessionId) resolveLoad(true);
        });
    });
    return { promise: bounded(promise, 'Actual navigation load').finally(() => off()) };
}
async function navigate(page, url) {
    const loaded = await loadSignal(page);
    await page.cdp.send('Page.navigate', { url }, pageSession);
    await loaded.promise;
}
async function capture(page, name) {
    const width = args.viewport === 'desktop' ? 1440 : 390;
    const height = args.viewport === 'desktop' ? 900 : 844;
    check('actual-viewport-' + name, await main(page,
        '({width:innerWidth,height:innerHeight})'), { width, height });
    const image = await page.cdp.send('Page.captureScreenshot', {
        format: 'png', fromSurface: true, captureBeyondViewport: false
    }, pageSession);
    const bytes = Buffer.from(image.data, 'base64');
    check('screenshot-dimensions-' + name,
        { width: bytes.readUInt32BE(16), height: bytes.readUInt32BE(20) }, { width, height });
    const relative = task === 7 ? 'task-7/' + args.mode + '/' + output.attemptId + '/screenshots/' + name + '.png' :
        task === 8 || task === 9 || task === 10 ? 'task-' + task + '/' + args.stage + '/' + args.bundle + '-' + args.viewport + '/' + output.attemptId + '/screenshots/' + name + '.png' :
        task === 6 ? 'upgraded/' + args.bundle + '-' + args.viewport + '/screenshots/' + name + '.png' :
        task === 2 ? 'task-2-' + args.viewport + '-' + name + '.png' :
        'baseline/' + args.bundle + '-' + args.viewport + '/screenshots/' + name + '.png';
    await mkdir(dirname(join(manifest.evidenceRoot, relative)), { recursive: true });
    await writeFile(join(manifest.evidenceRoot, relative), bytes);
    output.screenshots ??= [];
    output.screenshots.push({ name, path: relative, width, height });
}
async function installRoutes(page, omowright) {
    const routes = omowright.createRoutes(page);
    await routes.route('*', async route => {
        const url = new URL(route.request.url);
        if (url.origin !== manifest.httpsOrigin) {
            if (url.href === modernizrUrl && route.request.method === 'GET') {
                output.authorizedStaticResource.browserFulfillments++;
                return route.fulfill({ status: 200, contentType: 'application/javascript',
                    body: modernizrBytes.toString('utf8') +
                        (forcedFallback ? '\nModernizr.inputtypes.date=false;' : '') });
            }
            output.blockedExternalRequests.push({ origin: url.origin, path: url.pathname, method: route.request.method });
            return route.abort('BlockedByClient');
        }
        if (controlledRequest && url.pathname.toLowerCase() === controlledRequest.path.toLowerCase() &&
            route.request.method === controlledRequest.method) {
            const failure = controlledRequest;
            controlledRequest = null;
            if ((args['check-dependencies'] || args.acceptance) && url.pathname === '/signalr/negotiate') {
                // The request is paused before its controlled response. Observe the
                // original start Deferred, never create another start or Join.
                await main(page, `(() => {
                    const o=window.__nemeStatsObserver;
                    const connection=o.connections.find(r=>r.connection._deferral)?.connection;
                    if(!connection)throw new Error('Missing original in-flight start Deferred');
                    window.__qaNegotiateFailure=new Promise((resolve,reject)=>{
                        connection._deferral.fail(error=>resolve({
                            rejected:true,message:error.message,state:connection.state,
                            joins:o.joins.length,connected:connection.state===jQuery.signalR.connectionState.connected
                        }));
                        connection._deferral.done(()=>reject(new Error('Controlled negotiate unexpectedly connected')));
                    });
                })()`);
            }
            output.controlledRequests ??= [];
            output.controlledRequests.push({ path: url.pathname, method: route.request.method,
                status: failure.status, requestData: route.request.postData || null });
            return route.fulfill({ status: failure.status, contentType: 'application/json',
                body: JSON.stringify({ message: 'Owned controlled request failure' }) });
        }
        if ((task === 7 || task === 8 || task === 10) &&
            (/^\/Scripts\/jquery-3\.7\.1\.js$|^\/bundles\/jquery$/i.test(url.pathname))) {
            const actual = await request(url.href);
            check('diagnostic-core-response', actual.status, 200);
            if (args.mode === 'upgraded') {
                output.cleanCoreResponses ??= [];
                output.cleanCoreResponses.push({scenarioId:output.currentScenario || 'startup',
                    path:url.pathname,productSha256:createHash('sha256').update(actual.body).digest('hex'),
                    deliveredSha256:createHash('sha256').update(actual.body).digest('hex'),
                    diagnosticBytes:0});
                return route.fulfill({status:200,contentType:'application/javascript',body:actual.body});
            }
            const before = `\nwindow.__qaDiagnosticOrder={jquery:jQuery.fn.jquery,cookie:typeof jQuery.cookie,
                signalr:typeof jQuery.signalR,time:performance.now(),readyState:document.readyState};\n`;
            const after = `\njQuery.migrateDisablePatches('self-closed-tags');
                window.__qaDiagnosticOrder.migrate=jQuery.migrateVersion;\n`;
            let body;
            let splitOffset = actual.body.length;
            if (args.bundle === 'optimized') {
                const boundary = ',function(n){typeof define=="function"&&define.amd?define(["jquery"],n):typeof exports=="object"?n(require("jquery")):n(jQuery)}';
                check('diagnostic-one-native-cookie-boundary', actual.body.split(boundary).length - 1, 1);
                splitOffset = actual.body.indexOf(boundary);
                // The native bundle joins the independent cookie UMD by comma.
                // End that expression before inserting a separate official diagnostic script,
                // then preserve the anonymous cookie factory's expression context.
                body = actual.body.slice(0, splitOffset) + ';' + before + diagnosticMigrate +
                    after + '(' + actual.body.slice(splitOffset + 1) + ');';
            } else body = actual.body + before + diagnosticMigrate + after;
            output.diagnosticResponses ??= [];
            output.diagnosticResponses.push({scenarioId:output.currentScenario || 'startup',
                path:url.pathname,productSha256:createHash('sha256').update(actual.body).digest('hex'),
                deliveredSha256:createHash('sha256').update(body).digest('hex'),splitOffset,
                migrateSha256:migrateOfficialHash,officialBytesUnmodified:true});
            return route.fulfill({status:200,contentType:'application/javascript',body});
        }
        if (/\/(?:Scripts\/jquery\.signalR-[^/]+\.js|bundles\/signalr)(?:\?|$)/i.test(url.href)) {
            const actual = await request(url.href);
            output.observerResponses ??= [];
            output.observerResponses.push({ path: url.pathname, status: actual.status,
                productSha256: createHash('sha256').update(actual.body).digest('hex'),
                deliveredSha256: createHash('sha256').update(actual.body + '\n' +
                    browserBoundarySource + '\n' + observerSource).digest('hex') });
            if (actual.status !== 200) return route.abort('Failed');
            return route.fulfill({ status: 200, contentType: 'application/javascript',
                body: actual.body + '\n' + browserBoundarySource + '\n' + observerSource });
        }
        if (failedJoinProbe && url.pathname === '/GamingGroup/GetGamingGroupGameDefinitions/' &&
            route.request.method === 'GET') {
            failedJoinProbe = false;
            const cookies = await page.cdp.send('Network.getCookies',
                { urls: [manifest.httpsOrigin] }, pageSession);
            const actual = await request(url.href, { headers: {
                Cookie: cookies.cookies.map(cookie => cookie.name + '=' + cookie.value).join('; ')
            } });
            check('failed-join-probe-original-response', actual.status, 200);
            const attribute = 'data-gaminggroupid="' + manifest.fixtures.gamingGroupId + '"';
            check('failed-join-probe-one-authoritative-attribute', actual.body.split(attribute).length - 1, 1);
            const body = actual.body.replace(attribute, '');
            output.failedJoinBoundary = { path: url.pathname, method: 'GET',
                originalStatus: actual.status, removedOwnedGroupId: manifest.fixtures.gamingGroupId,
                originalSha256: createHash('sha256').update(actual.body).digest('hex'),
                controlledSha256: createHash('sha256').update(body).digest('hex'),
                change: 'Only the original authoritative group attribute is removed in one local response' };
            return route.fulfill({ status: 200, contentType: 'text/html', body });
        }
        return route.continue();
    });
    allRoutes.push(routes);
    return routes;
}
async function observeSend(page, hub, command, payload, predicate) {
    // Subscribe to event AND live DOM mutations in the main document before IPC send.
    await main(page, `(() => {
        const observer = window.__nemeStatsObserver;
        const join = observer.joins.find(j => j.hubName.toLowerCase() === ${JSON.stringify(hub.toLowerCase())} && j.status === 'succeeded');
        if (!join) throw new Error('No original successful UI Join');
        window.__seamEvent = observer.waitForEvent({hubName:${JSON.stringify(hub)}, eventName:${JSON.stringify(command === 'SendProgress' ? 'BGGImportDetailsProgress' : 'NewAchievementUnlocked')}});
        window.__seamMarker = observer.waitForEvent({hubName:${JSON.stringify(hub)}, eventName:'QaMarker',markerId:${JSON.stringify(payload.markerId)}});
        window.__seamDom = new Promise((resolveDom,reject) => {
            const test = ${predicate};
            const mutations = new MutationObserver(() => { if (test()) { mutations.disconnect(); clearTimeout(timer); resolveDom(true); } });
            const timer = setTimeout(() => {mutations.disconnect();reject(new Error('DOM mutation timed out'));},30000);
            mutations.observe(document.body,{subtree:true,childList:true,attributes:true,characterData:true});
        });
    })()`);
    const receipt = await pipe(command, payload);
    check('send-' + command + '-enqueue', receipt.success);
    output.sends.push(receipt.result);
    const event = await main(page, 'window.__seamEvent');
    const marker = await main(page, 'window.__seamMarker');
    check('send-' + command + '-original-payload', event.args, command === 'SendProgress' ?
        [payload.currentGamesImported, payload.gamesToImport, payload.currentGameName] :
        [String(payload.achievementId), payload.name, payload.iconClass, payload.description, payload.level]);
    check('send-' + command + '-marker-correlated', marker.args, [payload.markerId]);
    check('send-' + command + '-real-dom', await main(page, 'window.__seamDom'));
    check('send-' + command + '-same-domain', receipt.result.appDomainId, output.activation.appDomainId);
    check('send-' + command + '-same-worker', receipt.result.pid, output.activation.pid);
    output.sends.at(-1).browserEvent = event;
    output.sends.at(-1).browserMarker = marker;
}
async function selectPage(page) {
    activePage = page;
    pageSession = await page.resolveSessionId();
}
async function installPageLedger(page) {
    const sessionId = pageSession;
    output.dialogs ??= [];
    output.networkFailures ??= [];
    output.browserAnalyticsCalls ??= [];
    if (task === 7 || task === 8 || task === 10) {
        output.consoleCalls ??= [];
        output.networkTimeline ??= [];
        page.cdp.on('Runtime.consoleAPICalled', (event, meta) => {
            if (meta?.sessionId === sessionId) output.consoleCalls.push({
                scenarioId:output.currentScenario || 'startup',type:event.type,timestamp:event.timestamp,
                args:event.args.map(arg=>arg.value ?? arg.description),stackTrace:event.stackTrace || null
            });
        });
        page.cdp.on('Network.requestWillBeSent', (event, meta) => {
            if (meta?.sessionId === sessionId && new URL(event.request.url).origin === manifest.httpsOrigin)
                output.networkTimeline.push({event:'request',scenarioId:output.currentScenario || 'startup',
                    requestId:event.requestId,path:new URL(event.request.url).pathname,method:event.request.method,
                    timestamp:event.timestamp,initiator:event.initiator});
        });
        page.cdp.on('Network.responseReceived', (event, meta) => {
            if (meta?.sessionId === sessionId && new URL(event.response.url).origin === manifest.httpsOrigin)
                output.networkTimeline.push({event:'response',scenarioId:output.currentScenario || 'startup',
                    requestId:event.requestId,path:new URL(event.response.url).pathname,status:event.response.status,
                    timestamp:event.timestamp});
        });
        page.cdp.on('Network.loadingFailed', (event, meta) => {
            if (meta?.sessionId === sessionId) output.networkTimeline.push({
                event:'failed',scenarioId:output.currentScenario || 'startup',requestId:event.requestId,
                timestamp:event.timestamp,errorText:event.errorText,blockedReason:event.blockedReason});
        });
    }
    await page.cdp.send('Runtime.addBinding', { name: '__qaBrowserAnalyticsCapture' }, sessionId);
    page.cdp.on('Runtime.bindingCalled', (event, meta) => {
        if (meta?.sessionId === sessionId && event.name === '__qaBrowserAnalyticsCapture')
            output.browserAnalyticsCalls.push({
                scenarioId: output.currentScenario || 'startup',
                boundary: 'External browser ga API; captured no-op, original app wrapper retained',
                args: JSON.parse(event.payload)
            });
    });
    page.cdp.on('Runtime.exceptionThrown', (event, meta) => {
        if (meta?.sessionId === sessionId) output.consoleExceptions.push({
            scenarioId: output.currentScenario || 'startup', text: event.exceptionDetails.text,
            description: event.exceptionDetails.exception?.description,
            ...(task === 7 || task === 8 || task === 10 ? {timestamp:event.timestamp,details:event.exceptionDetails} : {}) });
    });
    page.cdp.on('Page.javascriptDialogOpening', (event, meta) => {
        if (meta?.sessionId === sessionId) output.dialogs.push({
            scenarioId: output.currentScenario || 'startup', type: event.type, message: event.message });
    });
    page.cdp.on('Network.responseReceived', (event, meta) => {
        if ((args['check-dependencies'] || args.acceptance) && meta?.sessionId === sessionId) {
            const url = new URL(event.response.url);
            if (url.origin === manifest.httpsOrigin && /^\/signalr\/(?:negotiate|start|connect)$/.test(url.pathname)) {
                output.signalRResponses ??= [];
                output.signalRResponses.push({path:url.pathname,status:event.response.status,
                    transport:url.searchParams.get('transport')});
            }
        }
        if ((args['check-dependencies'] || args.acceptance) && meta?.sessionId === sessionId &&
            (/\/Scripts\/jquery-(?:\d+\.){2}\d+(?:\.min)?\.js(?:\?|$)/i.test(event.response.url) ||
             /\/bundles\/jquery(?:\?|$)/i.test(event.response.url) ||
             /\/Scripts\/jquery\.signalR-[^/]+\.js(?:\?|$)/i.test(event.response.url) ||
             /\/bundles\/signalr(?:\?|$)/i.test(event.response.url))) {
            output.loadedDependencies ??= [];
            output.loadedDependencies.push({ requestId:event.requestId,url:event.response.url,
                status:event.response.status });
        }
        if (meta?.sessionId !== sessionId || event.response.status < 400) return;
        const url = new URL(event.response.url);
        if (url.origin === manifest.httpsOrigin) output.networkFailures.push({
            scenarioId: output.currentScenario || 'startup', path: url.pathname, status: event.response.status });
    });
    await page.cdp.send('Network.enable', {}, sessionId);
}
async function armDom(page, predicate) {
    const key = 'dom' + randomUUID().replaceAll('-', '');
    await main(page, `(() => {
        window.__qaSignals ??= {};
        const test=${predicate};
        window.__qaSignals[${JSON.stringify(key)}]=new Promise((resolve,reject)=>{
            const observer=new MutationObserver(()=>{if(test()){observer.disconnect();clearTimeout(timer);resolve(test());}});
            const timer=setTimeout(()=>{observer.disconnect();reject(new Error('Required DOM state timeout'));},30000);
            observer.observe(document.documentElement,{subtree:true,childList:true,attributes:true,characterData:true});
            if(test()){observer.disconnect();clearTimeout(timer);resolve(test());}
        });
        window.__qaSignals[${JSON.stringify(key)}].catch(()=>{});
    })()`);
    return () => main(page, 'window.__qaSignals[' + JSON.stringify(key) + ']');
}
async function fill(page, selector, value) {
    await main(page, `(() => {
        const input=document.querySelector(${JSON.stringify(selector)});
        if(!input) throw new Error('Missing actual input '+${JSON.stringify(selector)});
        input.focus();
        const prototype=input instanceof HTMLTextAreaElement ? HTMLTextAreaElement.prototype :
            input instanceof HTMLSelectElement ? HTMLSelectElement.prototype : HTMLInputElement.prototype;
        Object.getOwnPropertyDescriptor(prototype,'value').set.call(input,${JSON.stringify(String(value))});
        input.dispatchEvent(new Event('input',{bubbles:true}));
        input.dispatchEvent(new Event('change',{bubbles:true}));
    })()`);
}
async function click(page, selector) {
    await main(page, `(() => {
        const e=document.querySelector(${JSON.stringify(selector)});
        if(!e)throw new Error('Missing actual control '+${JSON.stringify(selector)});
        e.click();
    })()`);
}
async function armAjax(page, path, method = 'GET') {
    const key = 'ajax' + randomUUID().replaceAll('-', '');
    await main(page, `(() => {
        window.__qaSignals ??= {};
        window.__qaSignals[${JSON.stringify(key)}]=new Promise((resolve,reject)=>{
            const handler=(event,xhr,options)=>{
                if(new URL(options.url,location.href).pathname.toLowerCase()!==${JSON.stringify(path.toLowerCase())} ||
                    options.type.toUpperCase()!==${JSON.stringify(method)})return;
                jQuery(document).off('ajaxComplete',handler);clearTimeout(timer);
                const data=new URLSearchParams(options.type.toUpperCase()==='GET' ?
                    new URL(options.url,location.href).search :
                    (typeof options.data==='string' ? options.data : ''));
                data.delete('__RequestVerificationToken');
                data.delete('_');
                const responseText=(xhr.responseText || '').replace(
                    /<input\\b[^>]*\\bname="__RequestVerificationToken"[^>]*>/gi,
                    tag=>tag.replace(/\\bvalue="[^"]*"/i,'value="[redacted]"'));
                resolve({path:new URL(options.url,location.href).pathname,method:options.type,
                    data:data.toString(),status:xhr.status,response:xhr.responseJSON ??
                        responseText.slice(0,20000)});
            };
            const timer=setTimeout(()=>{jQuery(document).off('ajaxComplete',handler);reject(new Error('Required AJAX completion timeout'));},30000);
            jQuery(document).on('ajaxComplete',handler);
        });
        window.__qaSignals[${JSON.stringify(key)}].catch(()=>{});
    })()`);
    return async () => {
        const receipt = await main(page, 'window.__qaSignals[' + JSON.stringify(key) + ']');
        output.requests ??= [];
        output.requests.push({ scenarioId: output.currentScenario, ...receipt });
        return receipt;
    };
}
async function invalidForm(page, selector, trigger, savePath) {
    await main(page, `(() => {
        window.__invalidPosts=0;
        window.__invalidSend=(event,xhr,options)=>{
            if(options.type.toUpperCase()==='POST' &&
                new URL(options.url,location.href).pathname.toLowerCase()===${JSON.stringify(savePath.toLowerCase())})
                window.__invalidPosts++;
        };
        jQuery(document).on('ajaxSend',window.__invalidSend);
        window.__invalidSignal=new Promise((resolve,reject)=>{
            const timer=setTimeout(()=>reject(new Error('Validation event timeout')),30000);
            jQuery(${JSON.stringify(selector)}).one('invalid-form.validate',()=>{clearTimeout(timer);resolve(true);});
        });
        window.__invalidSignal.catch(()=>{});
    })()`);
    await trigger();
    check(output.currentScenario + '-validation-event', await main(page, 'window.__invalidSignal'));
    check(output.currentScenario + '-no-save-post', await main(page, 'window.__invalidPosts'), 0);
    check(output.currentScenario + '-rendered-errors', await main(page,
        `!!document.querySelector(${JSON.stringify(selector + ' .field-validation-error, ' + selector + ' .input-validation-error')})`));
    await main(page, "void jQuery(document).off('ajaxSend',window.__invalidSend)");
}
async function groupPage(page, tab = 'players') {
    await navigate(page, manifest.httpsOrigin + '/GamingGroup');
    const loaded = await armDom(page, "()=>!!document.querySelector('.createOrUpdatePartial #btnSubmitCreatePlayer')");
    await loaded();
    if (tab === 'games') {
        const response = await armAjax(page, '/GamingGroup/GetGamingGroupGameDefinitions/');
        const rendered = await armDom(page, "()=>!!document.querySelector('#gameNameInput') && !!jQuery('#js-createGameForm').data('validator')");
        await click(page, '#gamesTab');
        check(output.currentScenario + '-games-response', (await response()).status, 200);
        await rendered();
        const join = await main(page, `(async()=>{
            const row=await window.__nemeStatsObserver.waitForJoin({hubName:'longRunningTaskHub'});
            return {...row,promise:undefined,transport:window.__nemeStatsObserver.connections.find(c=>c.connection.id===row.connectionId).connection.transport.name};
        })()`);
        check(output.currentScenario + '-original-progress-join', join.status, 'succeeded');
        return join;
    }
}
async function loginPage(page, userName) {
    await navigate(page, manifest.httpsOrigin + '/Account/Login');
    await fill(page, '#UserName', userName);
    await fill(page, '#Password', manifest.qaFixturePassword);
    const response = await responseSignal(page, '/Account/Login', 'POST');
    const loaded = await loadSignal(page);
    await main(page, "document.querySelector('#Password').form.requestSubmit()");
    check(output.currentScenario + '-login-post', (await response.promise).status, 302);
    await loaded.promise;
}
async function persistedGames(page, groupId = manifest.fixtures.gamingGroupId) {
    const receipt = await main(page, `(async()=>{
        const response=await fetch('/GamingGroup/GetCurrentUserGamingGroupGameDefinitions?id='+${JSON.stringify(String(groupId))},{cache:'no-store'});
        const html=await response.text();
        const document=new DOMParser().parseFromString(html,'text/html');
        return {status:response.status,games:Array.from(document.querySelectorAll('a.name'))
            .map(e=>({name:e.textContent,path:e.getAttribute('href')}))};
    })()`);
    check(output.currentScenario + '-independent-persistence-get', receipt.status, 200);
    return receipt.games;
}
async function scenario(page, id, action) {
    output.currentScenario = id;
    const start = output.assertions.length, consoleStart = output.consoleExceptions.length,
        networkStart = output.networkFailures.length;
    const row = { id, startedUtc: new Date().toISOString(), status: 'running' };
    if (task === 7 || task === 8 || task === 9 || task === 10) row.diagnosticBefore = await diagnosticSnapshot(page);
    output.scenarios.push(row);
    console.log(JSON.stringify({ scenario: id, status: 'running' }));
    try {
        await action();
        if (task === 8 || task === 9 || task === 10) {
            check(id + '-no-console-exceptions', output.consoleExceptions.slice(consoleStart), []);
            const runtime = await diagnosticSnapshot(page);
            check(id + '-migrate-free', runtime.migrateVersion, null);
            check(id + '-secure-html-prefilter', runtime.legacyHtmlPrefilterEnabled, false);
        }
        row.status = 'passed';
    } catch (error) {
        row.status = args.mode === 'baseline' ? 'baseline-failure' : 'observed-failure';
        row.error = error.message;
        row.classification = args.mode === 'baseline' ? 'Observed baseline failure on jQuery 2.2.4; not repaired or waived' :
            'Observed upgraded behavior failure; classification requires frozen baseline comparison';
    }
    row.assertions = output.assertions.slice(start);
    row.consoleExceptions = output.consoleExceptions.slice(consoleStart);
    row.networkFailures = output.networkFailures.slice(networkStart);
    const expectedFailures = {
        'player-create-error': ['/player/save'],
        'bgg-autocomplete-error': ['/gamedefinition/searchboardgamegeekhttpget'],
        'game-create-error': ['/gamedefinition/ajaxcreate'],
        'played-game-error': ['/playedgame/save'],
        'rank-vote-error': ['/api/votablefeatures/'],
        'bgg-import-boundary-failure': ['/gamedefinition/importfrombgg']
    }[id] || [];
    row.unexpectedFailedRequests = row.networkFailures.filter(item=>!expectedFailures.includes(item.path.toLowerCase()));
    if (row.unexpectedFailedRequests.length && row.status === 'passed') {
        row.status = 'baseline-failure';
        row.classification = 'Observed unexpected failed application request; not silently counted as passed';
    }
    if (row.consoleExceptions.length && row.status === 'passed') {
        row.status = 'baseline-failure';
        row.classification = 'Observed baseline console exception; behavior assertions are retained separately';
    }
    row.finishedUtc = new Date().toISOString();
    if (task === 7 || task === 8 || task === 9 || task === 10) {
        row.diagnosticAfter = await diagnosticSnapshot(page);
        row.durationMs = Date.parse(row.finishedUtc) - Date.parse(row.startedUtc);
    }
    console.log(JSON.stringify({ scenario: id, status: row.status, error: row.error }));
    await selectPage(page);
    await capture(page, id).catch(error => { row.screenshotError = error.message; row.status = 'baseline-failure'; });
}
async function runBaseline(page, ownedBrowser, omowright) {
    const coverage = JSON.parse(await readFile(join(here, 'coverage.json'), 'utf8'));
    output.coverageSchemaVersion = coverage.schemaVersion;
    output.fixtureIds = manifest.fixtures;
    output.scenarios = [];
    output.sourceOnlyInventory = coverage.rows.filter(row => row.status === 'source-only-inventory');
    check('cell-fixture-selection', manifest.qaFixtureCell, args.bundle + '-' + args.viewport);
    check('served-bundle-mode', await main(page, `Array.from(document.scripts).some(s=>new URL(s.src||location.href).pathname==='/bundles/jquery')`),
        args.bundle === 'optimized');
    await scenario(page, 'account-login', async () => {
        check('account-login-observed-protected-post', output.protectedPost.status, 302);
        const cookies = await page.cdp.send('Network.getCookies', { urls: [manifest.httpsOrigin] }, pageSession);
        check('account-login-real-secure-cookie', cookies.cookies.some(c=>c.name==='.AspNet.ApplicationCookie'&&c.secure));
        await groupPage(page);
        check('account-login-rendered-owned-identity', await main(page,
            "document.querySelector('#currentuserid').getAttribute('data-id')"), manifest.fixtures.userId);
    });
    await scenario(page, 'registration-validation', async () => {
        await navigate(page, manifest.httpsOrigin + '/Account/Register');
        await invalidForm(page, 'form[action="/Account/Register"]',
            () => main(page, "void jQuery('form[action=\"/Account/Register\"]').trigger('submit')"), '/Account/Register');
        await fill(page, '#UserName', 'qa_validation_' + output.attemptId.replaceAll('-', ''));
        await fill(page, '#EmailAddress', 'synthetic@example.invalid');
        await fill(page, '#Password', manifest.qaFixturePassword);
        await fill(page, '#ConfirmPassword', 'different');
        await invalidForm(page, 'form[action="/Account/Register"]',
            () => main(page, "void jQuery('form[action=\"/Account/Register\"]').trigger('submit')"), '/Account/Register');
        await fill(page, '#ConfirmPassword', manifest.qaFixturePassword);
        check('registration-valid-inputs-accepted', await main(page, "jQuery('form[action=\"/Account/Register\"]').valid()"));
    });
    await scenario(page, 'account-invalid-login', async () => {
        await navigate(page, manifest.httpsOrigin + '/Account/Login');
        await fill(page, '#UserName', manifest.fixtures.userName);
        await fill(page, '#Password', 'Wrong_owned_password_7');
        const response = await responseSignal(page, '/Account/Login', 'POST');
        const loaded = await loadSignal(page);
        await main(page, "document.querySelector('#Password').form.requestSubmit()");
        const receipt = await response.promise;
        await loaded.promise;
        check('invalid-login-real-post', receipt.status, 200);
        check('invalid-login-visible-error', await main(page, "!!document.querySelector('.validation-summary-errors')"));
        await loginPage(page, manifest.fixtures.userName);
    });
    let createdPlayerId;
    const playerName = 'UI player ' + output.attemptId;
    await scenario(page, 'player-create-invalid', async () => {
        await groupPage(page);
        await invalidForm(page, '.createOrUpdatePartial form', () => click(page, '#btnSubmitCreatePlayer'), '/player/save');
    });
    await scenario(page, 'player-create', async () => {
        await groupPage(page);
        await fill(page, '.createOrUpdatePartial #Name', playerName);
        const response = await armAjax(page, '/player/save', 'POST');
        const rendered = await armDom(page, `()=>document.querySelector('#playersList')?.textContent.includes(${JSON.stringify(playerName)})`);
        await click(page, '#btnSubmitCreatePlayer');
        const saved = await response();
        check('player-create-status', saved.status, 200);
        check('player-create-payload-name', new URLSearchParams(saved.data).get('Name'), playerName);
        createdPlayerId = saved.response.Id;
        check('player-create-id', Number.isInteger(createdPlayerId));
        await rendered();
        // List.js removes off-page rows once this retained owned group grows.
        // Read the exact saved row through its real server-rendered edit GET.
        const persistedResponse = await responseSignal(page, '/Player/Edit/' + createdPlayerId, 'GET');
        await navigate(page, manifest.httpsOrigin + '/Player/Edit/' + createdPlayerId);
        check('player-create-independent-get-status', (await persistedResponse.promise).status, 200);
        check('player-create-persisted-new-request', await main(page,
            'document.querySelector("#Name").value'), playerName);
    });
    await scenario(page, 'player-create-error', async () => {
        await groupPage(page);
        await fill(page, '.createOrUpdatePartial #Name', 'Controlled player ' + output.attemptId);
        controlledRequest = { path: '/player/save', method: 'POST', status: 500 };
        const response = await armAjax(page, '/player/save', 'POST');
        const rendered = await armDom(page, "()=>!!document.querySelector('.createOrUpdatePartial .field-validation-error')");
        await click(page, '#btnSubmitCreatePlayer');
        check('player-error-controlled-500', (await response()).status, 500);
        await rendered();
        check('player-error-input-retained', await main(page,
            'document.querySelector(".createOrUpdatePartial #Name").value.startsWith("Controlled player ")'));
    });
    await scenario(page, 'player-edit', async () => {
        assert.ok(createdPlayerId, 'Actual created player required');
        await navigate(page, manifest.httpsOrigin + '/Player/Edit/' + createdPlayerId);
        await fill(page, '#Name', playerName + ' edited');
        const response = await responseSignal(page, '/Player/Edit/' + createdPlayerId, 'POST');
        const loaded = await loadSignal(page);
        await main(page, "document.querySelector('#Name').form.requestSubmit()");
        check('player-edit-real-post', (await response.promise).status, 302);
        await loaded.promise;
        await navigate(page, manifest.httpsOrigin + '/Player/Edit/' + createdPlayerId);
        check('player-edit-persisted', await main(page, "document.querySelector('#Name').value"), playerName + ' edited');
    });
    let createdGameId;
    const gameName = 'UI game ' + output.attemptId;
    await scenario(page, 'game-create-invalid', async () => {
        await groupPage(page, 'games');
        await invalidForm(page, '#js-createGameForm', () => click(page, '#createGameDefinitionFromPartial'), '/gamedefinition/ajaxcreate');
    });
    await scenario(page, 'bgg-autocomplete', async () => {
        await groupPage(page, 'games');
        const response = await armAjax(page, '/GameDefinition/SearchBoardGameGeekHttpGet');
        const menu = await armDom(page, "()=>!!document.querySelector('.ui-autocomplete .ui-menu-item')");
        await fill(page, '#gameNameInput', 'Browser QA BGG');
        const receipt = await response();
        check('bgg-autocomplete-real-response', receipt.status, 200);
        check('bgg-autocomplete-fixture-not-empty', receipt.response.length > 0);
        await menu();
        await click(page, '.ui-autocomplete .ui-menu-item-wrapper');
        check('bgg-selection-hidden-id', await main(page, "document.querySelector('#BoardGameGeekGameDefinitionId').value"),
            String(receipt.response[0].BoardGameId));
        await main(page, `void (window.__autocompleteCommitted=new Promise(resolve=>
            jQuery('#gameNameInput').one('autocompletechange',()=>resolve(true))))`);
        await main(page, "document.querySelector('#gameNameInput').blur()");
        await main(page, 'window.__autocompleteCommitted');
        await fill(page, '#gameNameInput', '');
        await main(page, `void (window.__autocompleteChanged=new Promise(resolve=>
            jQuery('#gameNameInput').one('autocompletechange',()=>resolve(true))))`);
        await main(page, "document.querySelector('#gameNameInput').blur()");
        await main(page, 'window.__autocompleteChanged');
        check('bgg-empty-value-clears-id', await main(page, "document.querySelector('#BoardGameGeekGameDefinitionId').value"), '');
    });
    await scenario(page, 'bgg-autocomplete-uncommitted-clear', async () => {
        await groupPage(page, 'games');
        const response = await armAjax(page, '/GameDefinition/SearchBoardGameGeekHttpGet');
        const menu = await armDom(page, "()=>!!document.querySelector('.ui-autocomplete .ui-menu-item')");
        await fill(page, '#gameNameInput', 'Browser QA BGG');
        check('bgg-uncommitted-real-lookup', (await response()).status, 200);
        await menu();
        await click(page, '.ui-autocomplete .ui-menu-item-wrapper');
        await fill(page, '#gameNameInput', '');
        await main(page, "document.querySelector('#gameNameInput').blur()");
        check('bgg-uncommitted-clear-known-stale-id', await main(page,
            "document.querySelector('#BoardGameGeekGameDefinitionId').value"), '910003');
        output.knownBaselineDefects ??= [];
        output.knownBaselineDefects.push({
            id: 'uncommitted-autocomplete-empty-value',
            scenarioId: 'bgg-autocomplete-uncommitted-clear',
            classification: 'Observed baseline defect; no save is triggered and no repair is claimed',
            finding: 'Clearing the selected text before its first blur leaves the selected BGG ID. jQuery UI previous equals the empty value, so the original change callback does not execute.'
        });
    });
    await scenario(page, 'bgg-autocomplete-empty', async () => {
        await groupPage(page, 'games');
        const response = await armAjax(page, '/GameDefinition/SearchBoardGameGeekHttpGet');
        await fill(page, '#gameNameInput', 'Browser QA empty');
        const receipt = await response();
        check('bgg-empty-response-status', receipt.status, 200);
        check('bgg-empty-result', receipt.response, []);
        check('bgg-empty-loader-cleared', await main(page, "!document.querySelector('#gameNameInput').classList.contains('autocomplete-loading')"));
        check('bgg-empty-menu-hidden', await main(page, "!jQuery('.ui-autocomplete').is(':visible')"));
    });
    await scenario(page, 'bgg-autocomplete-error', async () => {
        await groupPage(page, 'games');
        controlledRequest = { path: '/GameDefinition/SearchBoardGameGeekHttpGet', method: 'GET', status: 500 };
        const response = await armAjax(page, '/GameDefinition/SearchBoardGameGeekHttpGet');
        await fill(page, '#gameNameInput', 'Browser QA error');
        check('bgg-error-controlled-status', (await response()).status, 500);
        check('bgg-error-original-dialog', output.dialogs.some(row=>row.scenarioId==='bgg-autocomplete-error'&&row.type==='alert'));
        check('bgg-error-loader-cleared', await main(page, "!document.querySelector('#gameNameInput').classList.contains('autocomplete-loading')"));
    });
    await scenario(page, 'game-create', async () => {
        await groupPage(page, 'games');
        await fill(page, '#gameNameInput', gameName);
        const response = await armAjax(page, '/gamedefinition/ajaxcreate', 'POST');
        const loadedPartial = await armAjax(page, '/GamingGroup/GetCurrentUserGamingGroupGameDefinitions');
        const rendered = await armDom(page, `()=>document.querySelector('#gameDefinitionsList')?.textContent.includes(${JSON.stringify(gameName)})&&document.querySelector('#js-gamedefinitions')?.getBoundingClientRect().height>0`);
        await click(page, '#createGameDefinitionFromPartial');
        const receipt = await response();
        check('game-create-real-post', receipt.status, 200);
        check('game-create-payload', new URLSearchParams(receipt.data).get('Name'), gameName);
        check('game-create-original-ajax-load-response', (await loadedPartial()).status, 200);
        await rendered();
        await groupPage(page, 'games');
        // Read persisted group HTML independently, not the optimistic client callback.
        const persisted = (await persistedGames(page)).find(game=>game.name===gameName)?.path;
        check('game-create-persisted-link', typeof persisted, 'string');
        createdGameId = Number(persisted.split('/').at(-1));
        check('game-create-persisted-id', Number.isInteger(createdGameId) && createdGameId > 0);
    });
    await scenario(page, 'game-create-error', async () => {
        await groupPage(page, 'games');
        await fill(page, '#gameNameInput', 'Controlled game ' + output.attemptId);
        controlledRequest = { path: '/gamedefinition/ajaxcreate', method: 'POST', status: 400 };
        const response = await armAjax(page, '/gamedefinition/ajaxcreate', 'POST');
        await click(page, '#createGameDefinitionFromPartial');
        check('game-error-controlled-status', (await response()).status, 400);
        check('game-error-original-dialog', output.dialogs.some(row=>row.scenarioId==='game-create-error'&&row.type==='alert'));
        check('game-error-input-retained', await main(page, "document.querySelector('#gameNameInput').value.startsWith('Controlled game ')"));
    });
    await scenario(page, 'table-pagination-search-navigation', async () => {
        await groupPage(page, 'games');
        if (args.viewport === 'desktop') {
            check('pagination-has-second-page', await main(page, "document.querySelectorAll('#gameDefinitionsList .pagination li').length>1"));
            const changed = await armDom(page, "()=>!!document.querySelector('#gameDefinitionsList .pagination li:nth-child(2).active')");
            await click(page, '#gameDefinitionsList .pagination li:nth-child(2) a');
            await changed();
            check('pagination-page-is-bounded', await main(page, "document.querySelectorAll('#gameDefinitionsList tbody tr').length<=20"));
        } else {
            check('mobile-shows-full-unpaginated-list', await main(page, "document.querySelectorAll('#gameDefinitionsList tbody tr').length>=21"));
        }
        const narrowed = await armDom(page, `()=>Array.from(document.querySelectorAll('#gameDefinitionsList tbody tr')).every(r=>r.textContent.includes(${JSON.stringify(manifest.fixtures.gameDefinitionName)})) && document.querySelectorAll('#gameDefinitionsList tbody tr').length===1`);
        await fill(page, '#gameDefinitionsList .search', manifest.fixtures.gameDefinitionName);
        await main(page, "document.querySelector('#gameDefinitionsList .search').dispatchEvent(new KeyboardEvent('keyup',{bubbles:true,key:'e'}))");
        await narrowed();
        const loaded = await loadSignal(page);
        await click(page, '#gameDefinitionsList tbody a.name');
        await loaded.promise;
        check('actual-game-navigation', await main(page, 'location.pathname'), '/GameDefinition/Details/' + manifest.fixtures.gameDefinitionId);
    });
    let playedId;
    await scenario(page, 'played-game-invalid', async () => {
        await navigate(page, manifest.httpsOrigin + '/PlayedGame/Create');
        await main(page, "window.__invalidPlayedPosts=0;void jQuery(document).on('ajaxSend',window.__invalidPlayedSend=(e,x,o)=>{if(o.type==='POST'&&new URL(o.url,location.href).pathname.toLowerCase()==='/playedgame/save')window.__invalidPlayedPosts++;})");
        const rendered = await armDom(page, "()=>document.querySelector('#record-played-game .alert-danger')?.textContent.includes('You must set')");
        await click(page, '#btn-setDateCustom');
        await rendered();
        check('played-invalid-no-save-post', await main(page, 'window.__invalidPlayedPosts'), 0);
        await main(page, "void jQuery(document).off('ajaxSend',window.__invalidPlayedSend)");
        check('played-invalid-no-request', await main(page, "document.querySelector('#record-played-game').__vue__.serverRequestInProgress"), false);
    });
    await scenario(page, 'group-game-typeahead', async () => {
        await navigate(page, manifest.httpsOrigin + '/PlayedGame/Create');
        await click(page, '#btn-setDateToday');
        const response = await armAjax(page, '/gamedefinition/SearchGameDefinition');
        const menu = await armDom(page, "()=>!!document.querySelector('#search-game-definition')?.parentElement.querySelector('.tt-suggestion')");
        await fill(page, '#search-game-definition', 'Browser QA');
        const receipt = await response();
        check('group-lookup-status', receipt.status, 200);
        check('group-lookup-owned-results', receipt.response.some(row => row.Id === manifest.fixtures.gameDefinitionId));
        await menu();
        await main(page, "document.querySelector('#search-game-definition').parentElement.querySelector('.tt-suggestion').click()");
        check('group-lookup-real-selection', await main(page, "document.querySelector('#record-played-game').__vue__.currentStep"), 3);
    });
    await scenario(page, 'group-game-typeahead-empty', async () => {
        await navigate(page, manifest.httpsOrigin + '/PlayedGame/Create');
        await click(page, '#btn-setDateToday');
        const response = await armAjax(page, '/gamedefinition/SearchGameDefinition');
        const empty = await armDom(page, "()=>!!document.querySelector('.tt-menu .empty-message')");
        await fill(page, '#search-game-definition', 'No owned matching game ' + output.attemptId);
        check('group-empty-results', (await response()).response, []);
        await empty();
        check('group-empty-visible-message', await main(page, "jQuery('.tt-menu .empty-message').is(':visible')"));
    });
    await scenario(page, 'bgg-typeahead', async () => {
        await navigate(page, manifest.httpsOrigin + '/PlayedGame/Create');
        await click(page, '#btn-setDateToday');
        const response = await armAjax(page, '/gamedefinition/SearchBoardGameGeekHttpGet');
        const menu = await armDom(page, "()=>!!document.querySelector('#search-bgg')?.parentElement.querySelector('.tt-suggestion')");
        await fill(page, '#search-bgg', 'Browser QA BGG');
        const receipt = await response();
        check('bgg-typeahead-real-status', receipt.status, 200);
        await menu();
        await main(page, "document.querySelector('#search-bgg').parentElement.querySelector('.tt-suggestion').click()");
        check('bgg-typeahead-real-selection-id', await main(page,
            "document.querySelector('#record-played-game').__vue__.viewModel.Game.BoardGameGeekGameDefinitionId"), 910003);
    });
    await scenario(page, 'bgg-typeahead-empty', async () => {
        await navigate(page, manifest.httpsOrigin + '/PlayedGame/Create');
        await click(page, '#btn-setDateToday');
        const response = await armAjax(page, '/gamedefinition/SearchBoardGameGeekHttpGet');
        const empty = await armDom(page, "()=>!!document.querySelector('#bggsearchnotfound')");
        await fill(page, '#search-bgg', 'Browser QA empty');
        check('bgg-typeahead-empty-response', (await response()).response, []);
        await empty();
        await click(page, '#bggsearchnotfound');
        check('bgg-typeahead-empty-custom-game-name', await main(page,
            "document.querySelector('#record-played-game').__vue__.viewModel.Game.Name"), 'Browser QA empty');
    });
    await scenario(page, 'played-game-create-ranks-ties', async () => {
        await navigate(page, manifest.httpsOrigin + '/PlayedGame/Create');
        await click(page, '#btn-setDateToday');
        const lookup = await armAjax(page, '/gamedefinition/SearchGameDefinition');
        const menu = await armDom(page, "()=>!!document.querySelector('#search-game-definition')?.parentElement.querySelector('.tt-suggestion')");
        await fill(page, '#search-game-definition', 'Browser QA primary game 01');
        const lookupReceipt = await lookup();
        check('played-game-owned-metadata-lookup', lookupReceipt.response.some(game=>game.Id===manifest.fixtures.readmoreGameDefinitionId));
        await menu();
        const selection = await armDom(page, "()=>document.querySelector('#record-played-game').__vue__.currentStep===3");
        await main(page, "document.querySelector('#search-game-definition').parentElement.querySelector('.tt-suggestion').click()");
        await selection();
        await main(page, `document.querySelectorAll('.selectplayers input[type=checkbox]').forEach((e,i)=>{if(i<3&&!e.checked)e.click();})`);
        await click(page, 'button[title="Continue to set the game result"]');
        check('played-game-three-selected', await main(page, "document.querySelector('#record-played-game').__vue__.viewModel.Players.length>=3"));
        await fill(page, '#ranked-game li:nth-child(1) .rank', 1);
        await fill(page, '#ranked-game li:nth-child(2) .rank', 1);
        await fill(page, '#ranked-game li:nth-child(3) .rank', 2);
        await fill(page, '#record-played-game textarea', 'Owned note ' + output.attemptId);
        const response = await armAjax(page, '/playedgame/save', 'POST');
        const summary = await armDom(page, "()=>document.querySelector('#record-played-game').__vue__.gameHasBeenRecorded===true");
        await click(page, 'button[title="Save this game now"]');
        const receipt = await response();
        check('played-game-real-save', receipt.status, 200);
        check('played-game-save-success', receipt.response.success);
        const payload = new URLSearchParams(receipt.data);
        check('played-game-owned-metadata-payload', Number(payload.get('GameDefinitionId')), manifest.fixtures.readmoreGameDefinitionId);
        check('played-game-tied-payload', [payload.get('PlayerRanks[0][GameRank]'),payload.get('PlayerRanks[1][GameRank]')], ['1','1']);
        playedId = receipt.response.playedGameId;
        await summary();
        await navigate(page, manifest.httpsOrigin + '/PlayedGame/Edit/' + playedId);
        const persisted = await main(page, "document.querySelector('#record-played-game').__vue__.viewModel");
        check('played-game-persisted-notes', persisted.GameNotes, 'Owned note ' + output.attemptId);
        check('played-game-persisted-ties', persisted.Players.filter(p=>p.Rank===1).length>=2);
    });
    await scenario(page, 'played-game-edit', async () => {
        assert.ok(playedId, 'Actual played-game save required');
        await navigate(page, manifest.httpsOrigin + '/PlayedGame/Edit/' + playedId);
        await fill(page, '#record-played-game textarea', 'Edited note ' + output.attemptId);
        const response = await armAjax(page, '/playedgame/save', 'POST');
        const summary = await armDom(page, "()=>document.querySelector('#record-played-game').__vue__.gameHasBeenRecorded===true");
        await click(page, 'button[title="Save this game now"]');
        const receipt = await response();
        check('played-game-edit-success', receipt.response.success);
        check('played-game-edit-id', receipt.response.playedGameId, playedId);
        check('played-game-edit-payload', new URLSearchParams(receipt.data).get('EditMode'), 'true');
        await summary();
        await navigate(page, manifest.httpsOrigin + '/PlayedGame/Details/' + playedId);
        check('played-game-edit-persisted-render', await main(page, 'document.body.textContent.includes(' + JSON.stringify('Edited note ' + output.attemptId) + ')'));
    });
    await scenario(page, 'played-game-error', async () => {
        assert.ok(playedId, 'Actual played-game required for failure branch');
        await navigate(page, manifest.httpsOrigin + '/PlayedGame/Edit/' + playedId);
        controlledRequest = { path: '/playedgame/save', method: 'POST', status: 500 };
        const response = await armAjax(page, '/playedgame/save', 'POST');
        const error = await armDom(page, "()=>document.querySelector('#record-played-game').__vue__.alertVisible===true");
        await click(page, 'button[title="Save this game now"]');
        check('played-error-controlled-500', (await response()).status, 500);
        await error();
        check('played-error-not-busy', await main(page, "document.querySelector('#record-played-game').__vue__.serverRequestInProgress"), false);
        check('played-error-not-recorded', await main(page, "document.querySelector('#record-played-game').__vue__.gameHasBeenRecorded"), false);
    });
    await scenario(page, 'date-native-filter', async () => {
        await groupPage(page);
        await fill(page, '#from-date-picker', '2000-01-01');
        await fill(page, '#to-date-picker', '2100-01-01');
        const response = await armAjax(page, '/GamingGroup/GetGamingGroupPlayers/');
        await click(page, '#dateFilterButton');
        const receipt = await response();
        check('native-date-request', receipt.status, 200);
        check('native-date-from-payload', new URLSearchParams(receipt.data).get('Iso8601FromDate'), '2000-01-01');
        check('native-date-history', await main(page, 'new URL(location.href).searchParams.get("Iso8601FromDate")'), '2000-01-01');
    });
    await scenario(page, 'date-fallback-filter', async () => {
        forcedFallback = true;
        try {
            await groupPage(page);
            check('fallback-actual-modernizr-branch', await main(page, 'Modernizr.inputtypes.date'), false);
            check('fallback-calendar-buttons', await main(page, "document.querySelectorAll('.ui-datepicker-trigger').length"), 2);
            await click(page, '.ui-datepicker-trigger');
            check('fallback-calendar-open', await main(page, "jQuery('#ui-datepicker-div').is(':visible')"));
            await click(page, '#ui-datepicker-div td[data-handler="selectDay"] a');
            const response = await armAjax(page, '/GamingGroup/GetGamingGroupPlayers/');
            await click(page, '#dateFilterButton');
            check('fallback-date-real-request', (await response()).status, 200);
        } finally { forcedFallback = false; }
    });
    await scenario(page, 'played-search', async () => {
        await navigate(page, manifest.httpsOrigin + '/PlayedGame/Search');
        await fill(page, '#Filter_DatePlayedStart', '2000-01-01');
        await fill(page, '#Filter_DatePlayedEnd', '2100-01-01');
        const response = await responseSignal(page, '/PlayedGame/Search', 'POST');
        const loaded = await loadSignal(page);
        await click(page, 'input[type="submit"][value="Search"]');
        check('played-search-real-post', (await response.promise).status, 200);
        await loaded.promise;
        check('played-search-persisted-results', await main(page, "document.querySelectorAll('.game-result').length>0"));
    });
    await scenario(page, 'group-plays-tab', async () => {
        await groupPage(page);
        const response = await responseSignal(page, '/GamingGroup/GetGamingGroupPlayedGames/', 'GET');
        const rendered = await armDom(page, "()=>document.querySelectorAll('#playedGamesList .game-result').length>0");
        await click(page, '#playedGamesTab');
        check('group-plays-real-response', (await response.promise).status, 200);
        await rendered();
        check('group-plays-persisted-content', await main(page, "document.querySelectorAll('#playedGamesList .game-result').length>0"));
    });
    await scenario(page, 'charts', async () => {
        await groupPage(page);
        const response = await armAjax(page, '/api/v2/PlayedGames/');
        const chart = await armDom(page, "()=>!!document.querySelector('#statsDivId svg .nv-linesWrap path')");
        await click(page, '#statsTab');
        const receipt = await response();
        check('charts-real-api-response', receipt.status, 200);
        check('charts-persisted-game-data', receipt.response.playedGames.length > 0);
        await chart();
        check('charts-visible-geometry', await main(page, "document.querySelector('#statsDivId svg').getBoundingClientRect().width>0"));
    });
    await scenario(page, 'bootstrap-dropdown-modal', async () => {
        await navigate(page, manifest.httpsOrigin + '/');
        if (args.viewport === 'mobile') {
            await main(page, `void (window.__navShown=new Promise(resolve=>
                jQuery('.navbar-collapse').one('shown.bs.collapse',()=>resolve(true))))`);
            await click(page, '.navbar-toggle');
            check('mobile-navigation-actual-shown', await main(page, 'window.__navShown'));
        }
        await click(page, '.dropdown-toggle');
        check('dropdown-open', await main(page, "!!document.querySelector('.dropdown.open .dropdown-menu')"));
        check('dropdown-actually-visible', await main(page, "jQuery('.dropdown.open .dropdown-menu').first().is(':visible')"));
        await capture(page, 'bootstrap-dropdown-open');
        await click(page, '.dropdown-toggle');
        check('dropdown-closed', await main(page, "!document.querySelector('.dropdown.open')"));
        if (args.viewport === 'mobile') {
            await main(page, `void (window.__navHidden=new Promise(resolve=>
                jQuery('.navbar-collapse').one('hidden.bs.collapse',()=>resolve(true))))`);
            await click(page, '.navbar-toggle');
            check('mobile-navigation-actual-hidden', await main(page, 'window.__navHidden'));
        }
        await main(page, `void (window.__modalShown=new Promise(resolve=>jQuery('#homefeature1').one('shown.bs.modal',()=>resolve(true))))`);
        await click(page, '[data-target="#homefeature1"]');
        check('modal-shown-event', await bounded(main(page, 'window.__modalShown'), 'Actual Bootstrap shown'));
        check('modal-visible', await main(page, "jQuery('#homefeature1').is(':visible')"));
        await capture(page, 'bootstrap-modal-visible');
        await main(page, `void (window.__modalHidden=new Promise(resolve=>jQuery('#homefeature1').one('hidden.bs.modal',()=>resolve(true))))`);
        await click(page, '#homefeature1 [data-dismiss="modal"]');
        check('modal-hidden-event', await bounded(main(page, 'window.__modalHidden'), 'Actual Bootstrap hidden'));
    });
    await scenario(page, 'rank-vote-cookie', async () => {
        await navigate(page, manifest.httpsOrigin + '/');
        const before = await main(page, "Number(document.querySelector('#recentGamesVote .upvote-count').textContent)");
        const response = await armAjax(page, '/api/votablefeatures/', 'POST');
        await click(page, '#recentGamesVote .upvote-link');
        const receipt = await response();
        check('rank-vote-post', receipt.status, 204);
        check('rank-vote-payload-feature', new URLSearchParams(receipt.data).get('VotableFeatureId'), 'RecentGlobalGames');
        check('rank-vote-visible-increment', await main(page, "Number(document.querySelector('#recentGamesVote .upvote-count').textContent)"), before + 1);
        check('rank-cookie-real-write', await main(page, "JSON.parse(jQuery.cookie('rankCookie')).RecentGlobalGames"));
        await navigate(page, manifest.httpsOrigin + '/');
        check('rank-vote-persisted-new-get', await main(page, "Number(document.querySelector('#recentGamesVote .upvote-count').textContent)"), before + 1);
        await main(page, "window.__votePosts=0;void jQuery(document).on('ajaxSend',window.__voteSend=(e,x,o)=>{if(o.type==='POST')window.__votePosts++;})");
        await click(page, '#recentGamesVote .upvote-link');
        check('rank-cookie-prevents-repeat-post', await main(page, 'window.__votePosts'), 0);
        await main(page, "void jQuery(document).off('ajaxSend',window.__voteSend)");
    });
    await scenario(page, 'rank-vote-error', async () => {
        await navigate(page, manifest.httpsOrigin + '/');
        controlledRequest = { path: '/api/votablefeatures/', method: 'POST', status: 400 };
        const response = await armAjax(page, '/api/votablefeatures/', 'POST');
        await click(page, '#topGamesVote .upvote-link');
        check('rank-vote-controlled-failure', (await response()).status, 400);
        check('rank-error-original-dialog', output.dialogs.some(row=>row.scenarioId==='rank-vote-error'&&row.type==='alert'));
        check('rank-vote-baseline-cookie-still-recorded', await main(page, "JSON.parse(jQuery.cookie('rankCookie')).TopGamesEver"));
    });
    await scenario(page, 'bgg-import-success', async () => {
        await loginPage(page, manifest.fixtures.userName);
        await groupPage(page, 'games');
        const batch = manifest.fixtures.importBatches[manifest.qaFixtureCell];
        output.importPersistence = { batch, before: (await persistedGames(page)).map(game=>game.name) };
        check('import-two-known-pending-before-post', batch.gameNames.every(name=>!output.importPersistence.before.includes(name)));
        const spectator = await ownedBrowser.newTab('about:blank');
        await omowright.createAgentTabs(ownedBrowser).repin(spectator, {
            width: args.viewport === 'desktop' ? 1440 : 390,
            height: args.viewport === 'desktop' ? 900 : 844
        });
        await selectPage(spectator);
        await installRoutes(spectator, omowright);
        await spectator.cdp.send('Runtime.enable', {}, pageSession);
        await installPageLedger(spectator);
        const importJoin = await groupPage(spectator, 'games');
        await main(spectator, `(() => {
            const proxy=window.__nemeStatsObserver.proxies.find(p=>p.hubName.toLowerCase()==='longrunningtaskhub').proxy;
            window.__importProgress=new Promise((resolve,reject)=>{
                const events=[];
                const handler=(...args)=>{
                    events.push(args);
                    if(args[0]===2){clearTimeout(timer);proxy.off('BGGImportDetailsProgress',handler);resolve(events);}
                };
                const timer=setTimeout(()=>{proxy.off('BGGImportDetailsProgress',handler);reject(new Error('Actual importer progress timeout'));},30000);
                proxy.on('BGGImportDetailsProgress',handler);
            });
            window.__importProgress.catch(()=>{});
        })()`);
        await selectPage(page);
        const response = await responseSignal(page, '/GameDefinition/ImportFromBGG', 'POST');
        const loaded = await loadSignal(page);
        await click(page, '#gtm-import-games-from-bgg');
        output.importPersistence.response = await response.promise;
        check('import-real-post-outcome', output.importPersistence.response.status, 302);
        await loaded.promise;
        await groupPage(page, 'games');
        output.importPersistence.after = await persistedGames(page);
        check('import-two-persisted-after-response', batch.gameNames.every(name=>output.importPersistence.after.some(game=>game.name===name)));
        check('import-persistence-not-progress', output.importPersistence.after.filter(game=>batch.gameNames.includes(game.name)).length, 2);
        await selectPage(spectator);
        output.importPersistence.originalSpectatorJoin = importJoin;
        output.importPersistence.actualImporterEvents = await main(spectator, 'window.__importProgress');
        check('import-real-progress-exact-payloads', output.importPersistence.actualImporterEvents,
            [[1,2,batch.gameNames[0]],[2,2,batch.gameNames[1]]]);
        await selectPage(page);
    });
    await scenario(page, 'readmore-expand-collapse', async () => {
        const batch = manifest.fixtures.importBatches[manifest.qaFixtureCell];
        const imported = output.importPersistence.after.find(game=>game.name===batch.gameNames[0]);
        assert.ok(imported, 'Persisted actual import metadata required');
        await navigate(page, manifest.httpsOrigin + '/GameDefinition/Details/' + manifest.fixtures.readmoreGameDefinitionId);
        const readmore = await armDom(page, "()=>!!document.querySelector('[data-readmore-toggle]')");
        await readmore();
        const state = () => main(page, `(() => {
            const element=document.querySelector('.bggdescription');
            return {height:element.getBoundingClientRect().height,styleHeight:element.style.height,
                aria:element.getAttribute('aria-expanded'),
                transition:getComputedStyle(element).transitionDuration,
                expandedHeight:jQuery(element).data('expandedHeight'),
                collapsedHeight:jQuery(element).data('collapsedHeight'),
                label:document.querySelector('[data-readmore-toggle]').textContent};
        })()`);
        output.readmore = { loadMeasurement: await main(page, 'window.__qaReadmoreMeasured'), before: await state() };
        for (const mode of ['expand','collapse']) {
            await main(page, `(() => {
                const element=document.querySelector('.bggdescription');
                window.__readmoreTransition=new Promise((resolve,reject)=>{
                    if(getComputedStyle(element).transitionDuration.split(',').every(v=>parseFloat(v)===0))
                        return resolve({transitionApplicable:false});
                    const handler=event=>{
                        if(event.target!==element||event.propertyName!=='height')return;
                        clearTimeout(timer);element.removeEventListener('transitionend',handler);
                        resolve({transitionApplicable:true});
                    };
                    const timer=setTimeout(()=>{element.removeEventListener('transitionend',handler);reject(new Error('Readmore height transition timeout'));},30000);
                    element.addEventListener('transitionend',handler);
                });
                window.__readmoreTransition.catch(()=>{});
            })()`);
            const rendered = await armDom(page, `()=>document.querySelector('[data-readmore-toggle]')?.textContent.toLowerCase().includes(${JSON.stringify(mode === 'expand' ? 'less' : 'more')})`);
            await click(page, '[data-readmore-toggle]');
            await rendered();
            const transition = await main(page, 'window.__readmoreTransition');
            output.readmore[mode] = { ...await state(), ...transition };
            if (mode === 'expand') {
                await main(page, "document.querySelector('.bggdescription').scrollIntoView({block:'start'})");
                await capture(page, 'readmore-expanded');
            }
        }
        if (output.readmore.expand.aria !== 'true') {
            output.knownBaselineDefects ??= [];
            output.knownBaselineDefects.push({
                id:'readmore-generated-css-selector',scenarioId:'readmore-expand-collapse',
                classification:'Observed baseline accessibility/transition defect; not repaired',
                finding:"Readmore changes data-readmore=true to an empty attribute, invalidating the original generated CSS selector. Its transitionend-driven expanded aria state is not updated.",
                states:output.readmore
            });
        }
        check('readmore-real-expanded-geometry', output.readmore.expand.height > output.readmore.before.height);
        check('readmore-real-collapsed-geometry', output.readmore.collapse.height, output.readmore.before.height);
    });
    await scenario(page, 'bootstrap-tooltip-popover', async () => {
        await navigate(page, manifest.httpsOrigin + '/GameDefinition/Details/' + manifest.fixtures.readmoreGameDefinitionId);
        await main(page, `void (window.__tooltipShown=new Promise(resolve=>jQuery('[data-toggle="tooltip"]').first().one('shown.bs.tooltip',()=>resolve(true))))`);
        await main(page, "document.querySelector('[data-toggle=\"tooltip\"]').dispatchEvent(new MouseEvent('mouseover',{bubbles:true}))");
        check('tooltip-real-hover-event', await bounded(main(page, 'window.__tooltipShown'), 'Tooltip shown'));
        check('tooltip-visible', await main(page, "jQuery('.tooltip').is(':visible')"));
        await main(page, `void (window.__popoverShown=new Promise(resolve=>jQuery('[data-toggle="popover"]').first().one('shown.bs.popover',()=>resolve(true))))`);
        await main(page, "document.querySelector('[data-toggle=\"popover\"]').dispatchEvent(new MouseEvent('mouseover',{bubbles:true}))");
        check('popover-real-hover-event', await bounded(main(page, 'window.__popoverShown'), 'Popover shown'));
        check('popover-visible', await main(page, "jQuery('.popover').is(':visible')"));
    });
    await realtimeBaseline(page, ownedBrowser, omowright);
    check('browser-analytics-external-boundary-captured', output.browserAnalyticsCalls.some(call=>
        call.args[0]==='send'&&call.args[1]==='event'&&call.args[2]==='PlayedGames'));
    const executed = output.scenarios.map(row=>row.id);
    const required = coverage.rows.filter(row=>row.status==='required-executed-evidence').map(row=>row.id);
    check('coverage-all-reachable-scenarios-executed', executed.slice().sort(), required.slice().sort());
    output.boundaries = (await pipe('Ready')).result.boundaries;
    output.baselineFindings = output.scenarios.filter(row=>row.status!=='passed');
    output.knownUnexercisedFinding = {
        id: 'empty-game-list-template', classification: 'Pre-existing baseline defect retained from task2 independent receipts',
        source: 'Source/UI/Scripts/GamingGroup/gamingGroup.js:266-268; Scripts/list.js:705',
        repairApplied: false, reason: 'Owned populated fixtures are used; no empty-game pass claimed'
    };
    if (output.baselineFindings.length) {
        process.exitCode = 1;
        output.status = 'failed';
    }
}
async function subscribeRouting(page, hub, eventName, markerId, excluded) {
    await selectPage(page);
    await main(page, `(() => {
        const o=window.__nemeStatsObserver;
        window.__routingStart=o.sequence;
        window.__routingEvent=${excluded ? 'null' : `o.waitForEvent({hubName:${JSON.stringify(hub)},eventName:${JSON.stringify(eventName)}})`};
        window.__routingMarker=${excluded ? `o.waitForEvent({hubName:${JSON.stringify(hub)},eventName:'QaMarker',markerId:${JSON.stringify(markerId)}})` : 'null'};
        window.__routingEvent?.catch(()=>{});window.__routingMarker?.catch(()=>{});
    })()`);
}
function assertRoutingProof(receipt, positive, excluded, markerId) {
    const proof = receipt.routingProof, id = receipt.method;
    check(id + '-original-routing-invariant', proof?.invariant,
        'original-topic-membership-and-callback-completion-v1');
    check(id + '-membership-generation-stable', proof.generationEnd, proof.generationStart);
    check(id + '-membership-not-mutated', proof.membershipChanged, false);
    check(id + '-exact-original-recipients', proof.originalGroupCallbacksCompleted, proof.routedConnectionIds);
    check(id + '-actual-positive-recipients', positive.every(connection=>proof.routedConnectionIds.includes(connection)));
    check(id + '-actual-topic-recipient-keyset', proof.subscriptions.filter(s=>s.eventKeys.includes(proof.groupKey))
        .map(s=>s.connectionId).sort(), proof.routedConnectionIds);
    const excludedSubscriptions = proof.subscriptions.filter(s=>s.connectionId===excluded.connectionId);
    check(id + '-one-live-excluded-subscription', excludedSubscriptions.length, 1);
    check(id + '-actual-excluded-group-key', excludedSubscriptions[0].eventKeys.includes(proof.excludedGroupKey));
    check(id + '-excluded-has-no-target-topic', excludedSubscriptions[0].eventKeys.includes(proof.groupKey), false);
    const groupPublished = proof.trace.filter(row=>row.kind==='group'&&row.phase==='publish');
    check(id + '-one-original-group-publication', groupPublished.length, 1);
    check(id + '-actual-group-payload', JSON.parse(groupPublished[0].payload).A, receipt.arguments);
    const completed = proof.trace.filter(row=>row.kind==='group'&&row.phase==='callback-completed');
    check(id + '-all-original-transport-completions', completed.length, proof.routedConnectionIds.length);
    check(id + '-original-group-completions-before-marker', completed.every(row=>
        row.originalCallbackReturnedTrue && row.sequence>proof.groupPublishedSequence &&
        row.sequence<proof.markerPublishedSequence));
    const marker = proof.trace.filter(row=>row.kind==='marker'&&row.phase==='publish');
    check(id + '-one-original-marker-publication', marker.length, 1);
    check(id + '-actual-marker-payload', JSON.parse(marker[0].payload).A, [markerId]);
    const markerCompleted = proof.trace.filter(row=>row.kind==='marker'&&row.phase==='callback-completed');
    check(id + '-excluded-original-marker-transport-completed', markerCompleted.length===1 &&
        markerCompleted[0].connectionId===excluded.connectionId && markerCompleted[0].originalCallbackReturnedTrue &&
        markerCompleted[0].sequence>proof.markerPublishedSequence && markerCompleted[0].sequence<proof.completedSequence);
}
async function realtimeBaseline(page, ownedBrowser, omowright) {
    let samePage, otherPage;
    let primaryProgress, sameProgress, otherProgress, primaryNotification, sameNotification, otherNotification;
    const originalJoin = async (target, hub) => {
        await selectPage(target);
        return main(target, `(async()=>{const row=await window.__nemeStatsObserver.waitForJoin({hubName:${JSON.stringify(hub)}});return {...row,promise:undefined,transport:window.__nemeStatsObserver.connections.find(c=>c.connection.id===row.connectionId).connection.transport.name};})()`);
    };
    const newOwnedPage = async () => {
        const target = await ownedBrowser.newTab('about:blank');
        await omowright.createAgentTabs(ownedBrowser).repin(target, {
            width: args.viewport === 'desktop' ? 1440 : 390,
            height: args.viewport === 'desktop' ? 900 : 844
        });
        await selectPage(target);
        await installRoutes(target, omowright);
        await target.cdp.send('Runtime.enable', {}, pageSession);
        await installPageLedger(target);
        return target;
    };
    await scenario(page, 'realtime-original-ui-membership', async () => {
        await loginPage(page, manifest.fixtures.userName);
        primaryProgress = await groupPage(page, 'games');
        primaryNotification = await originalJoin(page, 'notificationsHub');
        samePage = await newOwnedPage();
        sameProgress = await groupPage(samePage, 'games');
        sameNotification = await originalJoin(samePage, 'notificationsHub');
        otherPage = await newOwnedPage();
        await loginPage(otherPage, manifest.fixtures.secondUserName);
        otherProgress = await groupPage(otherPage, 'games');
        otherNotification = await originalJoin(otherPage, 'notificationsHub');
        check('primary-ui-owned-progress-key', String(primaryProgress.groupKey), String(manifest.fixtures.gamingGroupId));
        check('same-ui-owned-progress-key', String(sameProgress.groupKey), String(manifest.fixtures.gamingGroupId));
        check('other-ui-different-progress-key', String(otherProgress.groupKey), String(manifest.fixtures.secondGamingGroupId));
        check('primary-ui-owned-user-key', primaryNotification.groupKey, manifest.fixtures.userId);
        check('other-ui-different-user-key', otherNotification.groupKey, manifest.fixtures.secondUserId);
        check('actual-connections-distinct', new Set([primaryProgress.connectionId,sameProgress.connectionId,otherProgress.connectionId]).size, 3);
        output.membership = { primaryProgress, sameProgress, otherProgress,
            primaryNotification, sameNotification, otherNotification };
        if (task === 9 || task === 10) for (const [name, member] of Object.entries(output.membership))
            check(name + '-original-forever-transport', ['webSockets','serverSentEvents'].includes(member.transport));
        await selectPage(page);
    });
    await scenario(page, 'realtime-achievement-routing', async () => {
        assert.ok(otherNotification && sameNotification, 'Three actual UI memberships required');
        const markerId = 'achievement-routing-' + randomUUID();
        const payload = { userId: manifest.fixtures.userId, achievementId: 7,
            name: 'Owned baseline ' + output.attemptId, iconClass: 'fa fa-trophy',
            description: 'Exact same-AppDomain five-argument contract', level: 'Gold',
            excludedConnectionId: otherNotification.connectionId, markerId,
            positiveConnectionIds: [primaryNotification.connectionId, sameNotification.connectionId] };
        const domSignals = [];
        for (const target of [page,samePage]) {
            await subscribeRouting(target, 'notificationsHub', 'NewAchievementUnlocked', markerId, false);
            domSignals.push(await armDom(target, `()=>Array.from(document.querySelectorAll('.ui-pnotify')).find(e=>e.textContent.includes(${JSON.stringify(payload.name + ' unlocked!')})&&getComputedStyle(e).opacity==='1'&&e.getBoundingClientRect().height>0)?.textContent`));
        }
        await subscribeRouting(otherPage, 'notificationsHub', 'NewAchievementUnlocked', markerId, true);
        const receipt = await pipe('SendAchievement', payload);
        (output.routingAttempts ??= []).push({ command: 'SendAchievement', receipt });
        check('achievement-routing-server-send', receipt.success);
        check('achievement-routing-same-domain', receipt.result.appDomainId, output.activation.appDomainId);
        check('achievement-routing-same-worker', receipt.result.pid, output.activation.pid);
        output.sends.push(receipt.result);
        assertRoutingProof(receipt.result, payload.positiveConnectionIds, otherNotification, markerId);
        const received = [];
        for (const [index,target] of [page,samePage].entries()) {
            await selectPage(target);
            const event = await main(target, 'window.__routingEvent');
            received.push(event);
            check('achievement-client-' + index + '-five-arguments', event.args,
                ['7',payload.name,payload.iconClass,payload.description,payload.level]);
            await domSignals[index]();
            check('achievement-client-' + index + '-one-visible-pnotify', await main(target,
                `Array.from(document.querySelectorAll('.ui-pnotify')).filter(e=>e.textContent.includes(${JSON.stringify(payload.name)})&&e.getBoundingClientRect().height>0).length`), 1);
            check('achievement-client-' + index + '-icon-and-level', await main(target,
                `!!Array.from(document.querySelectorAll('.ui-pnotify.gold')).find(e=>e.textContent.includes(${JSON.stringify(payload.name)})&&e.querySelector('.fa-trophy'))`));
        }
        await selectPage(otherPage);
        const marker = await main(otherPage, 'window.__routingMarker');
        check('achievement-exclusion-correlated-marker', marker.args, [markerId]);
        const excluded = await main(otherPage, `window.__nemeStatsObserver.events.filter(e=>e.sequence>window.__routingStart&&e.sequence<=${marker.sequence}&&e.eventName.toLowerCase()==='newachievementunlocked')`);
        check('achievement-other-group-zero-through-marker', excluded, []);
        output.achievementRouting = { receipt: receipt.result, received, excludedMarker: marker, excludedEvents: excluded };
        await selectPage(page);
        await capture(page, 'achievement-routing-visible');
        const loaded = await loadSignal(page);
        await main(page, `Array.from(document.querySelectorAll('.ui-pnotify')).find(e=>e.textContent.includes(${JSON.stringify(payload.name)})).click()`);
        await loaded.promise;
        check('achievement-original-click-local-link', await main(page, 'location.pathname'), '/achievements/7');
        check('achievement-local-origin', await main(page, 'location.origin'), manifest.httpsOrigin);
    });
    await scenario(page, 'realtime-progress-routing', async () => {
        // The achievement link navigated the primary page; reacquire its original UI proxy.
        await loginPage(page, manifest.fixtures.userName);
        primaryProgress = await groupPage(page, 'games');
        output.progressMembership = { primaryProgress, sameProgress, otherProgress };
        const markerId = 'progress-routing-' + randomUUID();
        const domSignals = [];
        for (const target of [page,samePage]) {
            await subscribeRouting(target, 'longRunningTaskHub', 'BGGImportDetailsProgress', markerId, false);
            domSignals.push(await armDom(target, "()=>document.querySelector('.createGameDefinitionPartial .alert-info')?.textContent.includes('Owned routing Alpha')&&document.querySelector('.createGameDefinitionPartial .progress-bar')?.style.width==='50%'"));
        }
        await subscribeRouting(otherPage, 'longRunningTaskHub', 'BGGImportDetailsProgress', markerId, true);
        const payload = { gamingGroupId: manifest.fixtures.gamingGroupId,
            currentGamesImported: 1, gamesToImport: 2, currentGameName: 'Owned routing Alpha',
            excludedConnectionId: otherProgress.connectionId, markerId,
            positiveConnectionIds: [primaryProgress.connectionId, sameProgress.connectionId] };
        const receipt = await pipe('SendProgress', payload);
        (output.routingAttempts ??= []).push({ command: 'SendProgress', receipt });
        check('progress-routing-server-send', receipt.success);
        check('progress-routing-same-domain', receipt.result.appDomainId, output.activation.appDomainId);
        check('progress-routing-same-worker', receipt.result.pid, output.activation.pid);
        output.sends.push(receipt.result);
        assertRoutingProof(receipt.result, payload.positiveConnectionIds, otherProgress, markerId);
        const received = [];
        for (const [index,target] of [page,samePage].entries()) {
            await selectPage(target);
            const event = await main(target, 'window.__routingEvent');
            received.push(event);
            check('progress-client-' + index + '-three-arguments', event.args, [1,2,'Owned routing Alpha']);
            await domSignals[index]();
            check('progress-client-' + index + '-visible-count', await main(target,
                "document.querySelector('.createGameDefinitionPartial .alert-info').getBoundingClientRect().height>0"));
        }
        await selectPage(otherPage);
        const marker = await main(otherPage, 'window.__routingMarker');
        check('progress-exclusion-correlated-marker', marker.args, [markerId]);
        const excluded = await main(otherPage, `window.__nemeStatsObserver.events.filter(e=>e.sequence>window.__routingStart&&e.sequence<=${marker.sequence}&&e.eventName.toLowerCase()==='bggimportdetailsprogress')`);
        check('progress-other-group-zero-through-marker', excluded, []);
        output.progressRouting = { receipt: receipt.result, received, excludedMarker: marker, excludedEvents: excluded,
            orderingAssumption: 'None for positive callbacks versus excluded marker; both positive promises and marker independently awaited' };
        await selectPage(page);
        await main(page, "document.querySelector('.createGameDefinitionPartial .alert-info').scrollIntoView({block:'center'})");
        await capture(page, 'progress-routing-visible');
        await observeSend(page, 'longRunningTaskHub', 'SendProgress', {
            gamingGroupId: manifest.fixtures.gamingGroupId, currentGamesImported: 2,
            gamesToImport: 2, currentGameName: 'Owned routing Beta',
            excludedConnectionId: primaryProgress.connectionId, markerId: 'complete-' + randomUUID()
        }, "()=>document.querySelector('.createGameDefinitionPartial .alert-info')?.getBoundingClientRect().height===0");
    });
    if (args['check-routing']) {
        await selectPage(page);
        return;
    }
    await scenario(page, 'realtime-reload-next-event', async () => {
        await loginPage(page, manifest.fixtures.userName);
        const initial = await groupPage(page, 'games');
        await observeSend(page, 'longRunningTaskHub', 'SendProgress', {
            gamingGroupId: manifest.fixtures.gamingGroupId, currentGamesImported: 1,
            gamesToImport: 2, currentGameName: 'Before actual reload',
            excludedConnectionId: initial.connectionId, markerId: 'before-reload-' + randomUUID()
        }, "()=>document.querySelector('.createGameDefinitionPartial .alert-info')?.textContent.includes('Before actual reload')&&document.querySelector('.createGameDefinitionPartial .progress-bar')?.style.width==='50%'");
        const loaded = await loadSignal(page);
        await page.cdp.send('Page.reload', {}, pageSession);
        await loaded.promise;
        const nextJoin = await originalJoin(page, 'longRunningTaskHub');
        check('recovery-actual-new-ui-join', nextJoin.status, 'succeeded');
        check('recovery-new-actual-connection', nextJoin.connectionId !== initial.connectionId);
        const cursor = await main(page, 'window.__nemeStatsObserver.sequence');
        await observeSend(page, 'longRunningTaskHub', 'SendProgress', {
            gamingGroupId: manifest.fixtures.gamingGroupId, currentGamesImported: 1,
            gamesToImport: 2, currentGameName: 'After actual reload',
            excludedConnectionId: nextJoin.connectionId, markerId: 'after-reload-' + randomUUID()
        }, "()=>document.querySelector('.createGameDefinitionPartial .alert-info')?.textContent.includes('After actual reload')&&document.querySelector('.createGameDefinitionPartial .alert-info')?.textContent.includes('1 of 2')&&document.querySelector('.createGameDefinitionPartial .progress-bar')?.style.width==='50%'&&document.querySelector('.createGameDefinitionPartial .alert-info')?.getBoundingClientRect().height>0");
        await main(page, "document.querySelector('.createGameDefinitionPartial .alert-info').scrollIntoView({block:'center'})");
        await capture(page, 'recovery-restored-visible');
        await observeSend(page, 'longRunningTaskHub', 'SendProgress', {
            gamingGroupId: manifest.fixtures.gamingGroupId, currentGamesImported: 2,
            gamesToImport: 2, currentGameName: 'After reload completed',
            excludedConnectionId: nextJoin.connectionId, markerId: 'after-complete-' + randomUUID()
        }, "()=>document.querySelector('.createGameDefinitionPartial .alert-info')?.getBoundingClientRect().height===0");
        output.recovery = { beforeJoin: initial, afterJoin: nextJoin,
            events: await main(page, `window.__nemeStatsObserver.events.filter(e=>e.sequence>${cursor}&&e.eventName.toLowerCase()==='bggimportdetailsprogress')`) };
        check('recovery-one-callback-per-controlled-event', output.recovery.events.map(e=>e.args), [
            [1,2,'After actual reload'],[2,2,'After reload completed']
        ]);
    });
    await scenario(page, 'bgg-import-boundary-failure', async () => {
        await loginPage(page, manifest.fixtures.secondUserName);
        await groupPage(page, 'games');
        const before = (await persistedGames(page, manifest.fixtures.secondGamingGroupId)).map(game=>game.name).sort();
        const response = await responseSignal(page, '/GameDefinition/ImportFromBGG', 'POST');
        const loaded = await loadSignal(page);
        await click(page, '#gtm-import-games-from-bgg');
        const receipt = await response.promise;
        await loaded.promise;
        check('failed-import-real-controller-response', receipt.status, 500);
        await groupPage(page, 'games');
        const after = (await persistedGames(page, manifest.fixtures.secondGamingGroupId)).map(game=>game.name).sort();
        check('failed-import-no-unintended-persistence', after, before);
        output.failedImport = { response: receipt, before, after, substitutedBoundary: 'IBoardGameGeekApiClient.GetUserGames(second owned BGG username) throws',
            actualControllerAndImporterRetained: true };
        await loginPage(page, manifest.fixtures.userName);
    });
    await selectPage(page);
}
async function negotiationFailure(page) {
    const sendsBefore = (await pipe('Ready')).result.sendInvocations;
    const failureCursor = output.networkFailures.length;
    controlledRequest = {path:'/signalr/negotiate',method:'GET',status:500};
    const failedNegotiation = await responseSignal(page, '/signalr/negotiate', 'GET');
    await navigate(page, manifest.httpsOrigin + '/GamingGroup');
    check('controlled-negotiate-response', (await failedNegotiation.promise).status, 500);
    output.failedNegotiation = await main(page, 'window.__qaNegotiateFailure');
    check('original-start-rejection-observed', output.failedNegotiation.rejected);
    check('failed-start-no-original-join', output.failedNegotiation.joins, 0);
    check('failed-start-not-connected', output.failedNegotiation.connected, false);
    check('failed-start-no-send', (await pipe('Ready')).result.sendInvocations, sendsBefore);
    await navigate(page, manifest.httpsOrigin + '/GamingGroup');
    output.recoveredNotificationJoin = await main(page, `(async()=>{
        const join=await window.__nemeStatsObserver.waitForJoin({hubName:'notificationsHub'});
        return {...join,promise:undefined};
    })()`);
    check('fresh-normal-original-join-after-negotiation-failure',
        output.recoveredNotificationJoin.status, 'succeeded');
    check('actual-negotiate-success', output.signalRResponses.some(row=>
        row.path === '/signalr/negotiate' && row.status === 200));
    check('actual-start-success', output.signalRResponses.some(row=>
        row.path === '/signalr/start' && row.status === 200));
    check('only-controlled-negotiate-network-failure', output.networkFailures.slice(failureCursor).map(row=>({
        path:row.path,status:row.status
    })), [{path:'/signalr/negotiate',status:500}]);
    await capture(page, 'negotiation-recovered');
    check('negotiation-no-browser-console-exceptions', output.consoleExceptions, []);
}
try {
    assert.ok(['baseline', 'migrate', 'upgraded'].includes(args.mode));
    if (args['check-routing']) {
        assert.equal(args.mode, 'upgraded');
        for (const flag of ['check-seam','check-dependencies','diagnostic-comparison','seed-only','negative-control'])
            assert.ok(!args[flag], 'Routing proof cannot be combined with ' + flag);
    }
    if (args['check-dependencies']) assert.equal(args.mode, 'upgraded');
    assert.ok(['app', 'published'].includes(args.stage));
    if (args.acceptance) assert.equal(args.mode, 'upgraded');
    assert.ok(['debug', 'optimized'].includes(args.bundle));
    if (args['check-seam']) assert.equal(args.bundle, 'debug');
    assert.ok(['desktop', 'mobile'].includes(args.viewport));
    const manifestPath = resolve(args['run-manifest']);
    manifest = JSON.parse(await readFile(manifestPath, 'utf8'));
    assert.equal(manifestPath, join(manifest.evidenceRoot, manifest.runId + '-run-manifest.json'));
    assert.equal(resolve(args.evidence), resolve(manifest.evidenceRoot));
    assert.equal(manifest.preparationReady, true);
    assert.equal(manifest.applicationStarted, false);
    assert.equal(manifest.stage, args.stage);
    assert.equal(manifest.qaPipeName, 'NemeStatsQa_' + manifest.runId.replaceAll('-', ''));
    output.runId = manifest.runId;
    manifest.qaEvidenceMode = args.mode === 'migrate' ? 'upgraded' : args.mode;
    if (args.mode === 'migrate') {
        diagnosticMigrate = await readFile(migratePath, 'utf8');
        check('official-migrate-file-hash', createHash('sha256').update(diagnosticMigrate).digest('hex'),
            migrateOfficialHash);
        output.migrateAsset = {version:'3.5.2',sha256:migrateOfficialHash,
            source:'https://code.jquery.com/jquery-migrate-3.5.2.js',shippingDependency:false};
    }
    output.viewport = args.viewport;
    output.stage = args.stage;
    output.bundle = args.bundle;
    output.commit = (await exec('git', ['-C', manifest.repositoryRoot, 'rev-parse', 'HEAD'])).stdout.trim();
    output.worktreeState = (await exec('git', ['-C', manifest.repositoryRoot, 'status', '--short'])).stdout.trim();
    const guard = await exec('pwsh', ['-NoProfile', '-File', join(here, 'seam-stage.ps1'),
        '-Phase', 'guard', '-RunId', manifest.runId, '-EvidenceRoot', manifest.evidenceRoot,
        '-Bundle', args.bundle, '-Stage', args.stage]);
    check('active-owned-guard', guard.stdout.includes('Task2Guard=verified'));
    if (args['check-seam']) await assert.rejects(exec('pwsh', ['-NoProfile', '-File', join(here, 'seam-stage.ps1'),
        '-Phase', 'guard', '-RunId', manifest.runId, '-EvidenceRoot', manifest.evidenceRoot,
        '-Bundle', 'optimized']), error => error.code !== 0);
    if (args['check-seam']) check('mismatched-stage-readiness-rejected-before-start', true);
    await readAuthorizedModernizr();
    ca = await readFile(manifest.qaCertificatePem);
    await rm(join(manifest.evidenceRoot, 'task-2-dispose.json'), { force: true });
    await rm(join(manifest.evidenceRoot, 'task-2-pipe-fault.json'), { force: true });
    await rm(join(manifest.evidenceRoot, 'task-2-dispose-started.json'), { force: true });
    host = spawn(manifest.ownedIisExpress, manifest.applicationHost.launchArguments, { windowsHide: true });
    hostExit = new Promise(resolveExit => host.once('exit', (code, signal) => resolveExit({ code, signal })));
    manifest.applicationStarted = true;
    manifest.applicationProcess = { processId: host.pid, executable: manifest.ownedIisExpress,
        startedUtc: new Date().toISOString() };
    const registered = new Promise((resolveRegistered, reject) => {
        let lines = '';
        const consume = chunk => {
            lines += chunk.toString();
            if (lines.includes('Successfully registered URL')) resolveRegistered(true);
            if (/Cannot|Failed to|error/i.test(lines)) reject(new Error('Owned IIS startup failed: ' + lines));
        };
        host.stdout.on('data', consume);
        host.stderr.on('data', consume);
        host.once('error', reject);
        host.once('exit', () => reject(new Error('Owned IIS exited before HTTPS registration')));
    });
    await writeFile(manifestPath, JSON.stringify(manifest, null, 2));
    await bounded(registered, 'Owned IIS registration');
    const bootstrap = await request(manifest.httpsOrigin + '/Account/Login');
    output.bootstrap = { status: bootstrap.status };
    if (bootstrap.status !== 200) {
        await writeFile(join(manifest.evidenceRoot, 'task-2-bootstrap-failure.html'), bootstrap.body);
        throw new Error('Actual HTTPS login GET returned ' + bootstrap.status);
    }
    check('https-login-get', bootstrap.status, 200);
    const ready = await pipe('Ready');
    check('authenticated-ready', ready.success && ready.result.ready);
    check('actual-pipe-acl-protected', ready.result.pipeAclProtected);
    check('actual-pipe-acl-owner-only', ready.result.pipeAcl.filter(r => r.access === 'Allow')
        .every(r => r.sid === manifest.evidenceOwnerSid));
    check('actual-pipe-acl-denies-network', ready.result.pipeAcl.some(r => r.sid === 'S-1-5-2' && r.access === 'Deny'));
    check('ready-worker-is-owned-host', ready.result.pid, host.pid);
    output.activation = ready.result;
    manifest = JSON.parse(await readFile(manifestPath, 'utf8'));
    check('ready-owned-fixtures', ready.result.fixtures, manifest.fixtures);
    check('owned-game-fixture-recorded', Number.isInteger(manifest.fixtures.gameDefinitionId) &&
        manifest.fixtures.gameDefinitionName === 'Browser QA game ' + manifest.runId);
    const bindingSource = await readFile(join(manifest.repositoryRoot,
        'Source/UI/Scripts/GameDefinition/createGameDefinitionPartial.js'));
    const servedBinding = await request(manifest.httpsOrigin +
        '/Scripts/GameDefinition/createGameDefinitionPartial.js');
    check('served-binding-response', servedBinding.status, 200);
    output.bindingSource = { path: 'Source/UI/Scripts/GameDefinition/createGameDefinitionPartial.js',
        sourceSha256: createHash('sha256').update(bindingSource).digest('hex'),
        servedSha256: createHash('sha256').update(servedBinding.body).digest('hex') };
    check('served-binding-matches-current-product-source',
        output.bindingSource.servedSha256, output.bindingSource.sourceSha256);
    check('nested-scopes-root-boundaries', ready.result.nestedScopesVerified > 0);
    check('root-overrides-precede-tested-nested-scope',
        Date.parse(ready.result.overridesInstalledUtc) < Date.parse(ready.result.firstNestedScopeVerifiedUtc));
    if (args['seed-only']) {
        output.status = 'passed';
        throw Object.assign(new Error('Seed-only complete'), { seedComplete: true });
    }
    check('actual-antiforgery-requires-ssl', ready.result.antiForgeryRequireSsl);
    const missingToken = await request(manifest.httpsOrigin + '/Account/Login', {
        method: 'POST', headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: new URLSearchParams({ UserName: manifest.fixtures.userName,
            Password: manifest.qaFixturePassword }).toString()
    });
    output.protectedFailureProbe = { status: missingToken.status, location: missingToken.headers.location,
        authenticatedCookieIssued: (missingToken.headers['set-cookie'] || [])
            .some(value => value.startsWith('.AspNet.ApplicationCookie=')) };
    check('missing-antiforgery-token-rejected', missingToken.status, 302);
    check('missing-token-redirect-stays-local',
        new URL(missingToken.headers.location, manifest.httpsOrigin).origin, manifest.httpsOrigin);
    check('missing-token-never-authenticates', output.protectedFailureProbe.authenticatedCookieIssued, false);
    for (const [name, secret, command, payload] of [
        ['invalid-secret', 'incorrect', 'Ready', {}],
        ['invalid-secret-stop', 'incorrect', 'Stop', {}],
        ['unknown-command', manifest.qaPipeSecret, 'Sql', {}],
        ['foreign-group', manifest.qaPipeSecret, 'SendProgress', { gamingGroupId: -1 }],
        ['foreign-user', manifest.qaPipeSecret, 'SendAchievement', { userId: 'not-owned' }],
        ['invalid-progress-payload', manifest.qaPipeSecret, 'SendProgress', {
            gamingGroupId: manifest.fixtures.gamingGroupId, currentGamesImported: 0.5,
            gamesToImport: 2, currentGameName: 'not sent' }],
        ['invalid-achievement-payload', manifest.qaPipeSecret, 'SendAchievement', {
            userId: manifest.fixtures.userId, achievementId: 7, name: null,
            iconClass: 'fa fa-trophy', description: 'not sent', level: 'Gold' }]
    ]) check(name + '-rejected', (await pipe(command, payload, secret)).success, false);
    check('rejected-commands-made-no-server-send', (await pipe('Ready')).result.sendInvocations, 0);
    if (process.env.OMO_BROWSER_ENGINE) throw new Error('Owned exact-event QA requires an unset OMO_BROWSER_ENGINE; current engine: ' + process.env.OMO_BROWSER_ENGINE);
    const { omowright } = await loadOmowright();
    const candidateProbe = await exec('pwsh', ['-NoProfile', '-Command',
        `$c=Get-Command chrome-headless-shell.exe -ErrorAction SilentlyContinue;if($c){$c.Source}else {@('$env:ProgramFiles\\Google\\Chrome\\Application\\chrome.exe','$env:ProgramFiles(x86)\\Google\\Chrome\\Application\\chrome.exe','$env:LOCALAPPDATA\\Google\\Chrome\\Application\\chrome.exe','$env:ProgramFiles\\Microsoft\\Edge\\Application\\msedge.exe','$env:ProgramFiles(x86)\\Microsoft\\Edge\\Application\\msedge.exe') | % {[Environment]::ExpandEnvironmentVariables($_)} | ? {Test-Path -LiteralPath $_ -PathType Leaf} | Select-Object -First 1}`.replaceAll("'$env:ProgramFiles\\", "'%ProgramFiles%\\").replaceAll("'$env:ProgramFiles(x86)\\", "'%ProgramFiles(x86)%\\").replaceAll("'$env:LOCALAPPDATA\\", "'%LOCALAPPDATA%\\")]);
    const browserPath = manifest.browserPath || candidateProbe.stdout.trim();
    assert.ok(browserPath, 'Real Chromium binary required');
    await access(browserPath);
    const probeProfilePath = join(manifest.evidenceRoot, 'task-2-version-probe-' + randomUUID());
    await mkdir(probeProfilePath);
    probeProfile = probeProfilePath;
    const version = await exec(browserPath, ['--headless', '--no-first-run', '--no-default-browser-check',
        '--disable-background-networking', '--disable-component-update', '--disable-sync',
        '--user-data-dir=' + probeProfile, '--version'], { timeout: 30000, windowsHide: true });
    output.browser = { path: browserPath, version: version.stdout.trim(),
        sha256: createHash('sha256').update(await readFile(browserPath)).digest('hex') };
    const profilePath = join(manifest.evidenceRoot, 'task-2-browser-' + randomUUID());
    await mkdir(profilePath);
    profile = profilePath;
    browser = await omowright.connectPipe({ browserPath,
        browserArgs: ['--headless', '--no-first-run', '--no-default-browser-check',
            '--disable-background-networking', '--disable-component-update', '--disable-sync',
            '--user-data-dir=' + profile],
        storageRoot: profile, dialogPolicy: { accept: false } });
    // "close" follows exit AND closure of all inherited stdio handles.
    // Subscribe before triggering browser shutdown or any tested navigation.
    browserExit = new Promise(resolveExit => browser.browserProcess.once('close',
        (code, signal) => resolveExit({ code, signal, pid: browser.browserProcess.pid })));
    const page = await browser.newTab('about:blank');
    activePage = page;
    // The owned tab manager repins on navigation. Change its public viewport state,
    // not just temporary CDP metrics, so every actual document stays at this width.
    await omowright.createAgentTabs(browser).repin(page, {
        width: args.viewport === 'desktop' ? 1440 : 390,
        height: args.viewport === 'desktop' ? 900 : 844
    });
    pageSession = await page.resolveSessionId();
    const routes = await installRoutes(page, omowright);
    output.consoleExceptions = [];
    await page.cdp.send('Runtime.enable', {}, pageSession);
    await installPageLedger(page);
    await page.cdp.send('Emulation.setDeviceMetricsOverride', {
        width: args.viewport === 'desktop' ? 1440 : 390,
        height: args.viewport === 'desktop' ? 900 : 844, deviceScaleFactor: 1,
        mobile: args.viewport === 'mobile', screenWidth: args.viewport === 'desktop' ? 1440 : 390,
        screenHeight: args.viewport === 'desktop' ? 900 : 844
    }, pageSession);
    await page.cdp.send('Emulation.setTouchEmulationEnabled', {
        enabled: args.viewport === 'mobile', maxTouchPoints: 1
    }, pageSession);
    await navigate(page, manifest.httpsOrigin + '/Account/Login');
    check('browser-actual-https-evaluate', await main(page, 'location.protocol'), 'https:');
    check('observer-script-installed-before-ui', await main(page, '!!window.__nemeStatsObserver'));
    output.clientVersions = await main(page, '({jquery: jQuery.fn.jquery, signalr: jQuery.signalR.version})');
    if (task === 7 || task === 8 || task === 9 || task === 10) {
        output.startupDiagnostic = await diagnosticSnapshot(page);
        check('diagnostic-migrate-runtime-mode', output.startupDiagnostic.migrateVersion,
            args.mode === 'migrate' ? '3.5.2' : null);
        if (args.mode === 'migrate') {
            check('diagnostic-after-actual-core-before-cookie-and-signalr',
                {jquery:output.startupDiagnostic.injectionOrder.jquery,
                    cookie:output.startupDiagnostic.injectionOrder.cookie,
                    signalr:output.startupDiagnostic.injectionOrder.signalr},
                {jquery:'3.7.1',cookie:'undefined',signalr:'undefined'});
            check('no-legacy-html-prefilter-restoration', output.startupDiagnostic.legacyHtmlPrefilterEnabled, false);
        }
    }
    await capture(page, 'login');
    if (args['negative-control']) {
        assert.equal(manifest.task3NegativeControlStarted, undefined, 'Exactly one negative control per run');
        manifest.task3NegativeControlStarted = { attemptId: output.attemptId, assertionId: 'core-jquery-version',
            intentionallyWrongExpected: '0.0.0', actual: output.clientVersions.jquery };
        await writeFile(manifestPath, JSON.stringify(manifest, null, 2));
        output.negativeControl = manifest.task3NegativeControlStarted;
        check('core-jquery-version', output.clientVersions.jquery, '0.0.0');
    }
    check('core-jquery-version', output.clientVersions.jquery, args.mode === 'baseline' ? '2.2.4' : '3.7.1');
    check('core-signalr-version', output.clientVersions.signalr, args.mode === 'baseline' ? '2.2.0' : '2.4.3');
    if (args['check-dependencies'] || args.acceptance) {
        const scripts = await main(page, 'Array.from(document.scripts,s=>s.src).filter(Boolean)');
        output.loadedScriptUrls = scripts;
        check('one-intended-core-runtime', scripts.filter(url=>
            /^\/Scripts\/jquery-(?:\d+\.){2}\d+(?:\.min)?\.js$|^\/bundles\/jquery$/i.test(new URL(url).pathname)).length, 1);
        check('one-intended-signalr-runtime', scripts.filter(url=>
            /^\/Scripts\/jquery\.signalR-[^/]+\.js$|^\/bundles\/signalr$/i.test(new URL(url).pathname)).length, 1);
        for (const [library, debugPath, bundlePath] of [
            ['jquery', '/Scripts/jquery-3.7.1.js', '/bundles/jquery'],
            ['signalr', '/Scripts/jquery.signalR-2.4.3.js', '/bundles/signalr']
        ]) {
            const path = args.bundle === 'debug' ? debugPath : bundlePath;
            check('one-loaded-' + library, scripts.filter(url=>new URL(url).pathname === path).length, 1);
            const response = output.loadedDependencies.find(row=>new URL(row.url).pathname === path);
            check('loaded-' + library + '-response', response?.status, 200);
            const body = await page.cdp.send('Network.getResponseBody',
                {requestId:response.requestId}, pageSession);
            const bytes = Buffer.from(body.body, body.base64Encoded ? 'base64' : 'utf8');
            response.loadedSha256 = createHash('sha256').update(bytes).digest('hex');
            const asset = debugPath.split('/').pop().replace(/\.js$/,args.bundle === 'debug' ? '.js' : '.min.js');
            const official = await readFile(join(manifest.repositoryRoot, 'Source/UI/Scripts', asset), 'utf8');
            const normalized = text=>text.replace(/\r\n/g,'\n');
            if (args.bundle === 'optimized') {
                // ASP.NET re-minifies selected official .min.js inputs. Compare
                // exact native builder/transform output, not a vendor substring.
                const nativeRoot = join(manifest.repositoryRoot,
                    '.omo/evidence/jquery-3-migration/wave2/task-6');
                const provenance = JSON.parse((await readFile(join(nativeRoot,
                    'native-bundle-provenance.json'), 'utf8')).replace(/^\uFEFF/, ''));
                const variant = provenance.variants.find(row=>row.name === library + '-min');
                check('native-' + library + '-known-inputs', variant.files, library === 'jquery' ?
                    ['jquery-3.7.1.min.js', 'jquery.cookie.js'] : ['jquery.signalR-2.4.3.min.js']);
                check('native-' + library + '-minification-succeeded', variant.nativeMinificationFailed, false);
                for (const input of variant.inputHashes) {
                    check('native-current-input-' + input.file, createHash('sha256').update(
                        await readFile(join(manifest.repositoryRoot, 'Source/UI/Scripts', input.file))).digest('hex'),
                    input.sha256);
                }
                check('native-known-compiler-files', provenance.compiler.map(row=>row.file),
                    ['System.Web.Optimization.dll', 'WebGrease.dll']);
                for (const compiler of provenance.compiler) {
                    check('native-current-compiler-' + compiler.file, createHash('sha256').update(
                        await readFile(join(manifest.repositoryRoot, 'Source/UI/bin', compiler.file))).digest('hex'),
                    compiler.sha256);
                }
                const expected = await readFile(join(nativeRoot, library + '-min-native-bundle.js'));
                check('native-' + library + '-artifact-hash',
                    createHash('sha256').update(expected).digest('hex'), variant.sha256);
                const observerResponse = output.observerResponses.find(row=>row.path === path);
                if (library === 'signalr') check('loaded-signalr-observer-delivery-hash',
                    response.loadedSha256, observerResponse.deliveredSha256);
                const productHash = library === 'signalr' ? observerResponse.productSha256 : response.loadedSha256;
                check('loaded-' + library + '-matches-native-official-bundle', productHash, variant.sha256);
                output.nativeBundleProof ??= [];
                output.nativeBundleProof.push({library,productHash,variant:variant.name,
                    expectedSha256:variant.sha256,inputs:variant.inputHashes,compiler:provenance.compiler});
            } else {
            check('loaded-' + library + '-contains-official-runtime',
                normalized(bytes.toString('utf8')).includes(normalized(official).replace(/^\/\*![\s\S]*?\*\/\s*/, '').trim()));
            }
        }
        check('no-loaded-old-runtime', scripts.some(url=>/jquery-2\.2\.4|signalR-2\.2\.0/i.test(url)), false);
        const map = await request(manifest.httpsOrigin + '/Scripts/jquery-3.7.1.min.map');
        check('current-jquery-map-response', map.status, 200);
        check('current-jquery-map-source', JSON.parse(map.body).sources, ['jquery-3.7.1.js']);
        output.sourceMapSha256 = createHash('sha256').update(map.body).digest('hex');
        check('actual-loaded-server-core', output.activation.assemblies.some(name=>
            name.startsWith('Microsoft.AspNet.SignalR.Core, Version=2.4.3.0,')));
    }
    check('exact-dom-event-capability', await main(page, `new Promise((resolveProbe,reject) => {
        const timer=setTimeout(()=>{watch.disconnect();reject(new Error('Exact-event capability timeout'));},30000);
        const watch=new MutationObserver(rows=>{if(rows.some(row=>row.attributeName==='data-qa-capability')){
            watch.disconnect();clearTimeout(timer);document.documentElement.removeAttribute('data-qa-capability');resolveProbe(true);
        }});
        watch.observe(document.documentElement,{attributes:true});
        document.documentElement.setAttribute('data-qa-capability','observed-before-trigger');
    })`));
    const cookies = await page.cdp.send('Network.getCookies', { urls: [manifest.httpsOrigin] }, pageSession);
    check('antiforgery-cookie-remains-secure', cookies.cookies.some(c => c.name.startsWith('__RequestVerificationToken') && c.secure));
    await main(page, `(() => {
        for (const [selector,value] of ${JSON.stringify([['#UserName', manifest.fixtures.userName], ['#Password', manifest.qaFixturePassword]])}) {
            const input=document.querySelector(selector);
            Object.getOwnPropertyDescriptor(HTMLInputElement.prototype,'value').set.call(input,value);
            input.dispatchEvent(new Event('input',{bubbles:true}));
            input.dispatchEvent(new Event('change',{bubbles:true}));
        }
    })()`);
    const loginResponse = await responseSignal(page, '/Account/Login', 'POST');
    const loginLoad = await loadSignal(page);
    // Subscribe to the actual protected POST before submitting its real form.
    await main(page, "document.querySelector('#Password').form.requestSubmit()");
    const login = await loginResponse.promise;
    await loginLoad.promise;
    output.protectedPost = { status: login.status, method: 'POST' };
    check('protected-login-post-redirect', login.status, 302);
    const authenticatedCookies = await page.cdp.send('Network.getCookies',
        { urls: [manifest.httpsOrigin] }, pageSession);
    check('actual-authentication-cookie-secure', authenticatedCookies.cookies.some(c =>
        c.name === '.AspNet.ApplicationCookie' && c.secure));
    if (args['check-routing']) {
        output.scenarios = [];
        await realtimeBaseline(page, browser, omowright);
        check('bounded-routing-scenarios-executed', output.scenarios.map(row=>row.id), [
            'realtime-original-ui-membership','realtime-achievement-routing','realtime-progress-routing'
        ]);
        check('bounded-routing-all-scenarios-passed', output.scenarios.every(row=>row.status==='passed'));
        check('no-browser-console-exceptions', output.consoleExceptions, []);
        output.boundaries = (await pipe('Ready')).result.boundaries;
    } else if (!args['check-seam'] && !args['check-dependencies']) {
        await runBaseline(page, browser, omowright);
        if (task === 8 || task === 10) check('no-browser-console-exceptions', output.consoleExceptions, []);
        if (args.acceptance) await negotiationFailure(page);
    } else {
    await navigate(page, manifest.httpsOrigin + '/GamingGroup');
    const notification = await main(page, `(async () => {
        const o=window.__nemeStatsObserver;
        const notification=await o.waitForJoin({hubName:'notificationsHub',groupKey:${JSON.stringify(manifest.fixtures.userId)}});
        return { ...notification, promise:undefined };
    })()`);
    output.observer = await main(page, 'window.__nemeStatsObserver.snapshot()');
    check('original-ui-notification-join', notification.status, 'succeeded');
    await observeSend(page, 'notificationsHub', 'SendAchievement', {
        userId: manifest.fixtures.userId, achievementId: 7, name: 'Owned seam',
        iconClass: 'fa fa-trophy', description: 'Test-only same-AppDomain send', level: 'Gold',
        excludedConnectionId: notification.connectionId, markerId: 'task2-achievement-' + randomUUID()
    }, `() => { const e=document.querySelector('.ui-pnotify');return e && getComputedStyle(e).opacity === '1' && e.getBoundingClientRect().width > 0 && e.textContent.includes('Owned seam unlocked!') && !!e.querySelector('.fa-trophy'); }`);
    await capture(page, 'achievement');
    check('original-games-controller-dependency-ready', await main(page,
        "typeof window.Modernizr === 'object' && !!window.Modernizr.inputtypes"));
    // The actual product Games tab creates the progress proxy; no harness setup call.
    const sendsBeforeProgressJoin = (await pipe('Ready')).result.sendInvocations;
    const gamesResponse = await responseSignal(page, '/GamingGroup/GetGamingGroupGameDefinitions/', 'GET');
    await main(page, "document.querySelector('#gamesTab').click()");
    const games = await gamesResponse.promise;
    check('actual-games-tab-response', games.status, 200);
    const progress = await main(page, `(async () => {
        try {
            const join=await window.__nemeStatsObserver.waitForJoin({hubName:'longRunningTaskHub'});
            return {...join,promise:undefined};
        } catch (error) {
            if (!error.join) throw error;
            return {...error.join,promise:undefined};
        }
    })()`);
    output.originalProgressJoin = progress;
    if (progress.status === 'failed') {
        output.actualFailedJoin = { originalJoin: progress,
            sendsBefore: sendsBeforeProgressJoin,
            sendsAfter: (await pipe('Ready')).result.sendInvocations,
            markup: await main(page, `({
                originalJoinElement: document.querySelector('#importGameDefinitionPartial')?.outerHTML,
                dataOnOriginalJoinElement: document.querySelector('#importGameDefinitionPartial')?.getAttribute('data-gaminggroupid'),
                dataOnParent: document.querySelector('.createGameDefinitionPartial')?.getAttribute('data-gaminggroupid')
            })`) };
        check('actual-original-failed-join-observed', progress.status, 'failed');
        check('actual-failed-join-made-no-server-send',
            output.actualFailedJoin.sendsAfter, sendsBeforeProgressJoin);
        check('actual-failed-join-made-no-progress-send',
            output.sends.some(send => send.method === 'BGGImportDetailsProgress'), false);
        output.unmetAcceptance = ['Original progress UI Join success and owned group',
            'Same-AppDomain progress send/event/DOM', ...(output.consoleExceptions.length ?
                ['Exception-free browser console'] : [])];
    }
    check('original-ui-progress-join', progress.status, 'succeeded');
    check('original-ui-progress-owned-group', String(progress.groupKey), String(manifest.fixtures.gamingGroupId));
    const marker = 'task2-progress-' + randomUUID();
    await observeSend(page, 'longRunningTaskHub', 'SendProgress', {
        gamingGroupId: manifest.fixtures.gamingGroupId, currentGamesImported: 1,
        gamesToImport: 2, currentGameName: 'Owned seam game',
        excludedConnectionId: progress.connectionId, markerId: marker
    }, `() => { const e=document.querySelector('.createGameDefinitionPartial');const bar=e?.querySelector('.progress-bar');const text=e?.querySelector('.alert-info');return text && text.getBoundingClientRect().height > 0 && getComputedStyle(text).display !== 'none' && text.textContent.includes('Owned seam game') && text.textContent.includes('1 of 2 games imported') && bar?.style.width === '50%' && bar.getBoundingClientRect().width > 0; }`);
    output.progressDom = await main(page, `(() => {
        const text=document.querySelector('.createGameDefinitionPartial .alert-info');
        const bar=text.querySelector('.progress-bar');
        return {text:text.textContent.trim(),width:bar.style.width,
            visible:text.getBoundingClientRect().height > 0 && bar.getBoundingClientRect().width > 0};
    })()`);
    await main(page, "document.querySelector('.createGameDefinitionPartial .alert-info').scrollIntoView({block:'center'})");
    await capture(page, 'progress');
    output.observerAfterSends = await main(page, 'window.__nemeStatsObserver.snapshot()');
    check('existing-create-form-validation-retained', await main(page,
        "!!jQuery('#js-createGameForm').data('validator')"));
    check('existing-create-game-click-handler-retained', await main(page,
        "!!jQuery._data(document.querySelector('#createGameDefinitionFromPartial'),'events')?.click?.length"));
    check('populated-owned-game-visible', await main(page,
        'document.querySelector("#gameDefinitionsList").textContent.includes(' +
        JSON.stringify(manifest.fixtures.gameDefinitionName) + ')'));
    const sendsBeforeFailedJoin = (await pipe('Ready')).result.sendInvocations;
    if (args['check-dependencies']) {
        await negotiationFailure(page);
    } else {
    await navigate(page, manifest.httpsOrigin + '/GamingGroup');
    failedJoinProbe = true;
    const failedGamesResponse = await responseSignal(page, '/GamingGroup/GetGamingGroupGameDefinitions/', 'GET');
    await main(page, "document.querySelector('#gamesTab').click()");
    check('failed-join-probe-games-response', (await failedGamesResponse.promise).status, 200);
    const failedJoin = await main(page, `(async () => {
        try {
            await window.__nemeStatsObserver.waitForJoin({hubName:'longRunningTaskHub'});
            return {unexpectedSuccess:true};
        } catch(error) {
            if (!error.join) throw error;
            return {...error.join,promise:undefined};
        }
    })()`);
    output.actualFailedJoin = { originalJoin: failedJoin, sendsBefore: sendsBeforeFailedJoin,
        sendsAfter: (await pipe('Ready')).result.sendInvocations };
    check('actual-original-failed-join-observed', failedJoin.status, 'failed');
    check('actual-failed-join-original-null-group-argument', failedJoin.args, ['join', null]);
    check('actual-failed-join-made-no-server-send',
        output.actualFailedJoin.sendsAfter, sendsBeforeFailedJoin);
    output.failedJoinObserver = await main(page, 'window.__nemeStatsObserver.snapshot()');
    check('failed-join-probe-one-original-progress-invocation',
        output.failedJoinObserver.joins.filter(join => join.hubName === 'longrunningtaskhub').length, 1);
    await capture(page, 'failed-join');
    }
    check('no-browser-console-exceptions', output.consoleExceptions, []);
    }
    for (const installed of allRoutes) await installed.dispose();
    output.status = 'passed';
} catch (error) {
    if (error.seedComplete) {
        output.status = 'passed';
    } else {
    output.status = 'failed';
    output.error = error.message;
    if (host) output.failureActivation = await pipe('Ready').catch(() => null);
    if (manifest) {
        output.pipeFault = await readFile(join(manifest.evidenceRoot, 'task-2-pipe-fault.json'), 'utf8')
            .then(JSON.parse).catch(() => null);
        output.disposeStarted = await readFile(join(manifest.evidenceRoot, 'task-2-dispose-started.json'), 'utf8')
            .then(JSON.parse).catch(() => null);
    }
    process.exitCode = 1;
    }
} finally {
    if (activePage) {
        output.finalObserver = await main(activePage, 'window.__nemeStatsObserver?.snapshot()').catch(() => null);
        output.observerAfterSends ??= output.finalObserver;
        await capture(activePage, 'final')
            .catch(error => { output.finalScreenshotError = error.message; });
    }
    if (manifest) await writeFile(join(manifest.evidenceRoot,
        prefix + '-' + output.attemptId + '-before-teardown.json'), JSON.stringify(output, null, 2));
    if (browser) await browser.close().catch(error => { output.browserCloseError = error.message; process.exitCode = 1; });
    if (browserExit) output.browserExit = await bounded(browserExit, 'Owned browser exact process close')
        .catch(error => { process.exitCode = 1; return { error: error.message }; });
    if (host) {
        let watcher;
        try {
            const disposed = new Promise((resolveDispose, reject) => {
                watcher = watch(manifest.evidenceRoot, (event, file) => {
                    if (event === 'rename' && String(file) === 'task-2-dispose.json') {
                        readFile(join(manifest.evidenceRoot, String(file)), 'utf8')
                            .then(text => resolveDispose(JSON.parse(text)), reject);
                    }
                });
                watcher.on('error', reject);
            });
            const stopped = await pipe('Stop');
            output.stop = stopped;
            output.dispose = await bounded(disposed, 'Exact module Dispose receipt').finally(() => watcher.close());
            const closed = await new Promise(resolveClosed => {
                const probe = net.createConnection('\\\\.\\pipe\\' + manifest.qaPipeName);
                probe.once('error', error => resolveClosed(error.code === 'ENOENT'));
                probe.once('connect', () => { probe.destroy(); resolveClosed(false); });
            });
            check('stop-closes-exact-pipe', closed);
        } catch (error) { output.stopError = error.message; process.exitCode = 1; output.status = 'failed'; }
        finally { watcher?.close(); }
        host.kill();
        output.hostExit = await bounded(hostExit, 'Owned IIS exact process exit').catch(error => {
            process.exitCode = 1; return { error: error.message };
        });
        if (!output.hostExit.error) {
            const latest = JSON.parse(await readFile(resolve(args['run-manifest']), 'utf8'));
            latest.applicationStarted = false;
            latest.applicationProcess.stoppedUtc = new Date().toISOString();
            await writeFile(resolve(args['run-manifest']), JSON.stringify(latest, null, 2));
            try {
                const settled = await exec('pwsh', ['-NoProfile', '-File', join(here, 'seam-stage.ps1'),
                    '-Phase', 'settle', '-RunId', manifest.runId, '-EvidenceRoot', manifest.evidenceRoot,
                    '-Bundle', args.bundle, '-Stage', args.stage]);
                output.sqlTeardown = settled.stdout.trim().split('\n').filter(line => line.startsWith('Task2SqlSettled='));
                check('exact-owned-sql-process-stopped', output.sqlTeardown.length, 1);
            } catch (error) { output.sqlTeardownError = error.message; process.exitCode = 1; }
        }
    }
    for (const ownedProfile of [profile, probeProfile].filter(Boolean)) {
        try { await rm(ownedProfile, { recursive: true, force: true }); }
        catch (error) { output.profileCleanupError = { path: ownedProfile, error: error.code }; process.exitCode = 1; }
    }
    if (process.exitCode) output.status = 'failed';
    controlSocket?.destroy();
    if (manifest) {
        const destination = resolve(args.evidence);
        await writeFile(join(destination, prefix + '-results.json'), JSON.stringify(output, null, 2));
        await writeFile(join(destination, prefix + '-' + output.attemptId + '-results.json'), JSON.stringify(output, null, 2));
        if (output.negativeControl) await writeFile(join(destination, 'task-3-negative-control.json'),
            JSON.stringify({ runId: manifest.runId, expectedFailure: output.status === 'failed',
                assertion: output.assertions.find(row => row.id === 'core-jquery-version'),
                ...output.negativeControl, error: output.error }, null, 2));
        for (const [name, value] of [['activation', output.activation], ['server-sends', output.sends],
            ['observer', { before: output.observer, after: output.observerAfterSends }]]) {
            await writeFile(join(destination, 'task-' + task + '-' + name + '.json'), JSON.stringify({
                runId: manifest.runId, status: output.status, evidence: value || null
            }, null, 2));
        }
    }
    console.log(JSON.stringify({ task, status: output.status, assertions: output.assertions.length,
        error: output.error, hostExit: output.hostExit }));
}

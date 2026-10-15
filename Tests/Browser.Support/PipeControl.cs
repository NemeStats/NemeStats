#region LICENSE
// NemeStats is free software under the GNU General Public License version 3 or later.
// Copyright (C) 2015 Jacob Gordon. See <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Messaging;
using NemeStats.Hubs;

namespace NemeStats.Browser.Support
{
    internal sealed class PipeControl : IDisposable
    {
        private readonly string run, name, secret;
        private readonly CancellationTokenSource stop = new CancellationTokenSource();
        private Task listener;
        private int sendInvocations;
        private NamedPipeServerStream current;
        internal PipeControl(string runId, string pipeName, string pipeSecret)
        {
            run = runId; name = pipeName; secret = pipeSecret;
            if (name != "NemeStatsQa_" + run.Replace("-", "") || secret.Length < 40)
                throw new InvalidOperationException("Invalid per-run pipe identity.");
        }
        internal void Start()
        {
            listener = Task.Run(Listen);
            listener.ContinueWith(fault => File.WriteAllText(
                Path.Combine((string)StagedQaModule.Manifest["evidenceRoot"], "task-2-pipe-fault.json"),
                StagedQaModule.Json.Serialize(new { runId = run,
                    pid = Process.GetCurrentProcess().Id, appDomainId = AppDomain.CurrentDomain.Id,
                    exceptionType = fault.Exception.GetBaseException().GetType().FullName,
                    stackTrace = fault.Exception.GetBaseException().StackTrace,
                    lifecycle = StagedQaModule.Lifecycle.ToArray() })),
                TaskContinuationOptions.OnlyOnFaulted);
        }
        private async Task Listen()
        {
            var acl = new PipeSecurity();
            acl.SetAccessRuleProtection(true, false);
            acl.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.NetworkSid, null),
                PipeAccessRights.FullControl, AccessControlType.Deny));
            acl.AddAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().User,
                PipeAccessRights.FullControl, AccessControlType.Allow));
            while (!stop.IsCancellationRequested)
            {
                try
                {
                    using (var pipe = new NamedPipeServerStream(name, PipeDirection.InOut, 1,
                        PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 4096, 4096, acl))
                    {
                        current = pipe;
                        await pipe.WaitForConnectionAsync(stop.Token).ConfigureAwait(false);
                        using (var reader = new StreamReader(pipe, new UTF8Encoding(false), false, 4096, true))
                        using (var writer = new StreamWriter(pipe, new UTF8Encoding(false), 4096, true) { AutoFlush = true })
                        {
                            while (!stop.IsCancellationRequested && pipe.IsConnected)
                            {
                            // An idle authenticated session is a listener, not an in-flight command.
                            // Dispose closes this exact stream and cancels the pending first-character read.
                            var prefix = new char[1];
                            if (await reader.ReadAsync(prefix, 0, 1).ConfigureAwait(false) == 0) break;
                            var read = reader.ReadLineAsync();
                            var timeout = Task.Delay(TimeSpan.FromSeconds(30), stop.Token);
                            if (await Task.WhenAny(read, timeout).ConfigureAwait(false) != read)
                            {
                                stop.Token.ThrowIfCancellationRequested();
                                throw new TimeoutException("Pipe request timed out.");
                            }
                            var line = prefix[0] + await read.ConfigureAwait(false);
                            Dictionary<string, object> request = null;
                            object response;
                            bool stopping = false;
                            try
                            {
                                request = StagedQaModule.Json.Deserialize<Dictionary<string, object>>(line);
                                if (!Equals(request["runId"], run) || !SecretEquals((string)request["secret"]))
                                    throw new InvalidOperationException("Authentication rejected.");
                                var command = (string)request["command"];
                                var payload = (Dictionary<string, object>)request["payload"];
                                response = new { requestId = request["requestId"], success = true,
                                    result = await Dispatch(command, payload).ConfigureAwait(false) };
                                stopping = command == "Stop";
                            }
                            catch (Exception error)
                            {
                                response = new { requestId = request != null && request.ContainsKey("requestId") ? request["requestId"] : null,
                                    success = false, error = error is InvalidOperationException ? error.Message : error.GetType().Name };
                            }
                            await writer.WriteLineAsync(StagedQaModule.Json.Serialize(response)).ConfigureAwait(false);
                            if (stopping)
                            {
                                stop.Cancel();
                                ThreadPool.QueueUserWorkItem(ignored =>
                                {
                                    if (!listener.Wait(TimeSpan.FromSeconds(30)))
                                        throw new TimeoutException("Stopped listener did not complete.");
                                    System.Web.HttpRuntime.UnloadAppDomain();
                                });
                            }
                            }
                        }
                    }
                }
                catch (OperationCanceledException) { if (!stop.IsCancellationRequested) throw; }
                catch (ObjectDisposedException) { if (!stop.IsCancellationRequested) throw; }
                catch (TimeoutException) { /* Close the bounded idle client, not the listener. */ }
                finally { current = null; }
            }
        }
        private bool SecretEquals(string value)
        {
            if (value == null || value.Length != secret.Length) return false;
            int different = 0;
            for (int i = 0; i < value.Length; i++) different |= value[i] ^ secret[i];
            return different == 0;
        }
        private async Task<object> Dispatch(string command, Dictionary<string, object> payload)
        {
            if (command == "Stop") return new { stopped = true };
            if (command == "Ready")
                return new { ready = StagedQaModule.BootstrapCompleted && StagedQaModule.Fixtures.Count > 0,
                    runId = run, pid = Process.GetCurrentProcess().Id, appDomainId = AppDomain.CurrentDomain.Id,
                    appDomainName = AppDomain.CurrentDomain.FriendlyName,
                    catalog = ((Dictionary<string, object>)StagedQaModule.Manifest["database"])["catalog"],
                    fixtures = StagedQaModule.Fixtures, overridesInstalledUtc = StagedQaModule.OverridesInstalledUtc.ToString("o"),
                    nestedScopesVerified = StagedQaModule.NestedScopesVerified,
                    firstNestedScopeVerifiedUtc = StagedQaModule.FirstNestedScopeVerifiedUtc.ToString("o"),
                    lifecycle = StagedQaModule.Lifecycle.ToArray(),
                    pipeAclProtected = current.GetAccessControl().AreAccessRulesProtected,
                    pipeAcl = current.GetAccessControl().GetAccessRules(true, true, typeof(SecurityIdentifier))
                        .Cast<PipeAccessRule>().Select(r => new {
                            sid = r.IdentityReference.Value, access = r.AccessControlType.ToString(),
                            rights = r.PipeAccessRights.ToString() }).ToArray(),
                    sendInvocations = sendInvocations,
                    antiForgeryRequireSsl = (bool)Type.GetType("System.Web.Helpers.AntiForgeryConfig, System.Web.WebPages")
                        .GetProperty("RequireSsl").GetValue(null, null),
                    boundaries = StagedQaModule.External.Calls.ToArray(),
                    assemblies = new[] { typeof(GlobalHost).Assembly, typeof(UI.MvcApplication).Assembly,
                        typeof(BusinessLogic.DataAccess.NemeStatsDbContext).Assembly, typeof(NotificationsHub).Assembly }
                        .Select(a => a.FullName).ToArray() };
            if (!StagedQaModule.BootstrapCompleted) throw new InvalidOperationException("Bootstrap is not ready.");
            IHubContext hub;
            string group, method;
            object[] args;
            if (command == "SendProgress")
            {
                group = Convert.ToString(payload["gamingGroupId"]);
                if (group != Convert.ToString(StagedQaModule.Fixtures["gamingGroupId"]))
                    throw new InvalidOperationException("Foreign fixture rejected.");
                if (!(payload["currentGamesImported"] is int) || !(payload["gamesToImport"] is int))
                    throw new InvalidOperationException("Invalid progress payload.");
                int imported = Convert.ToInt32(payload["currentGamesImported"]), total = Convert.ToInt32(payload["gamesToImport"]);
                if (total < 1 || imported < 0 || imported > total || !(payload["currentGameName"] is string))
                    throw new InvalidOperationException("Invalid progress payload.");
                hub = GlobalHost.ConnectionManager.GetHubContext<LongRunningTaskHub>();
                method = "BGGImportDetailsProgress";
                args = new object[] { imported, total, (string)payload["currentGameName"] };
            }
            else if (command == "SendAchievement")
            {
                group = (string)payload["userId"];
                if (group != (string)StagedQaModule.Fixtures["userId"])
                    throw new InvalidOperationException("Foreign fixture rejected.");
                if (!(payload["achievementId"] is int) || !(payload["name"] is string) ||
                    !(payload["iconClass"] is string) || !(payload["description"] is string) ||
                    !(payload["level"] is string))
                    throw new InvalidOperationException("Invalid achievement payload.");
                hub = GlobalHost.ConnectionManager.GetHubContext<NotificationsHub>();
                method = "NewAchievementUnlocked";
                args = new object[] { Convert.ToInt32(payload["achievementId"]).ToString(), (string)payload["name"],
                    (string)payload["iconClass"], (string)payload["description"], (string)payload["level"] };
            }
            else throw new InvalidOperationException("Unknown command rejected.");
            if (payload.ContainsKey("excludedConnectionId") || payload.ContainsKey("markerId"))
            {
                if (!payload.ContainsKey("excludedConnectionId") || !payload.ContainsKey("markerId") ||
                    String.IsNullOrWhiteSpace((string)payload["excludedConnectionId"]) ||
                    String.IsNullOrWhiteSpace((string)payload["markerId"]))
                    throw new InvalidOperationException("Marker requires actual connection and marker IDs.");
            }
            Interlocked.Increment(ref sendInvocations);
            RoutingObserver.Proof proof = null;
            object routingProof = null;
            try
            {
                if (payload.ContainsKey("positiveConnectionIds"))
                {
                    string hubName = GlobalHost.DependencyResolver.Resolve<IHubManager>().GetHub(
                        command == "SendProgress" ? "longrunningtaskhub" : "notificationshub").Name;
                    string otherGroup = Convert.ToString(StagedQaModule.Fixtures[
                        command == "SendProgress" ? "secondGamingGroupId" : "secondUserId"]);
                    proof = StagedQaModule.Routing.Begin(hubName, group, otherGroup, method, args,
                        ((System.Collections.IEnumerable)payload["positiveConnectionIds"]).Cast<string>().ToArray(),
                        (string)payload["excludedConnectionId"], (string)payload["markerId"]);
                }
                await SendBounded((IClientProxy)hub.Clients.Group(group), method, args).ConfigureAwait(false);
                if (proof != null)
                {
                    await AwaitBounded(proof.GroupDone.Task).ConfigureAwait(false);
                    StagedQaModule.Routing.Validate(proof);
                }
                if (payload.ContainsKey("markerId"))
                {
                    await SendBounded((IClientProxy)hub.Clients.Client((string)payload["excludedConnectionId"]),
                        "QaMarker", new object[] { (string)payload["markerId"] }).ConfigureAwait(false);
                }
                if (proof != null)
                {
                    await AwaitBounded(proof.MarkerDone.Task).ConfigureAwait(false);
                    routingProof = StagedQaModule.Routing.Finish(proof);
                }
            }
            finally { if (proof != null) StagedQaModule.Routing.End(proof); }
            return new { enqueued = true, runId = run, pid = Process.GetCurrentProcess().Id,
                appDomainId = AppDomain.CurrentDomain.Id, method = method, group = group, arguments = args,
                routingProof = routingProof };
        }
        private async Task SendBounded(IClientProxy client, string method, object[] args)
        {
            var send = client.Invoke(method, args);
            await AwaitBounded(send).ConfigureAwait(false);
        }
        private async Task AwaitBounded(Task send)
        {
            var timeout = Task.Delay(TimeSpan.FromSeconds(30), stop.Token);
            if (await Task.WhenAny(send, timeout).ConfigureAwait(false) != send)
            {
                stop.Token.ThrowIfCancellationRequested();
                throw new TimeoutException("Same-AppDomain send timed out.");
            }
            await send.ConfigureAwait(false);
        }
        public void Dispose()
        {
            stop.Cancel();
            var pipe = current;
            if (pipe != null) pipe.Dispose();
            if (listener != null && !listener.Wait(TimeSpan.FromSeconds(30)))
                throw new TimeoutException("Pipe listener failed to stop.");
        }
    }
}

namespace NemeStats.Browser.Support
{
    // Observes the original bus and original callbacks; never routes or drains work itself.
    internal sealed class RoutingObserver : IMessageBus
    {
        private readonly IMessageBus inner;
        private readonly object gate = new object();
        private readonly List<Registration> registrations = new List<Registration>();
        private long generation, sequence;
        private Proof current;

        internal RoutingObserver(IMessageBus original)
        {
            if (original.GetType() != typeof(MessageBus) ||
                original.GetType().Assembly.GetName().Version.ToString() != "2.4.3.0")
                throw new InvalidOperationException("Routing proof requires the original 2.4.3 MessageBus.");
            inner = original;
        }

        private sealed class Registration
        {
            internal ISubscriber Subscriber;
            internal bool Initialized, Live = true;
            internal int Active, Disposals;
            internal string Callback;
        }

        internal sealed class Proof
        {
            internal string GroupKey, ExcludedKey, MarkerKey, Method, Arguments, MarkerId, Failure;
            internal string[] Positive, Routed;
            internal long Generation, Opened, GroupPublished, MarkerPublished;
            internal object[] Snapshot;
            internal Message GroupMessage, MarkerMessage;
            internal readonly List<object> Trace = new List<object>();
            internal readonly HashSet<string> Started = new HashSet<string>();
            internal readonly HashSet<string> Completed = new HashSet<string>();
            internal readonly TaskCompletionSource<bool> GroupDone =
                new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            internal readonly TaskCompletionSource<bool> MarkerDone =
                new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        private void Changed(string reason)
        {
            generation++;
            if (current != null) Fail(current, "Membership changed: " + reason);
        }

        private static void Fail(Proof proof, string reason)
        {
            proof.Failure = proof.Failure ?? reason;
            proof.GroupDone.TrySetResult(false);
            proof.MarkerDone.TrySetResult(false);
        }

        public IDisposable Subscribe(ISubscriber subscriber, string cursor,
            Func<MessageResult, object, Task<bool>> callback, int maxMessages, object state)
        {
            var registration = new Registration { Subscriber = subscriber,
                Callback = callback.Method.DeclaringType.FullName + "." + callback.Method.Name };
            Action<ISubscriber, string> added = (s, key) => { lock (gate) Changed("add:" + key); };
            Action<ISubscriber, string> removed = (s, key) => { lock (gate) Changed("remove:" + key); };
            lock (gate)
            {
                Changed("subscribe:" + subscriber.Identity);
                registrations.Add(registration);
                subscriber.EventKeyAdded += added;
                subscriber.EventKeyRemoved += removed;
            }
            var disposable = inner.Subscribe(subscriber, cursor,
                (result, originalState) => Delivered(registration, result, callback, originalState),
                maxMessages, state);
            lock (gate)
            {
                registration.Initialized = true;
                Changed("subscribed:" + subscriber.Identity);
            }
            return new RegistrationDisposer(() =>
            {
                lock (gate)
                {
                    registration.Disposals++;
                    registration.Live = false;
                    Changed("dispose:" + subscriber.Identity);
                }
                try
                {
                    disposable.Dispose();
                    subscriber.EventKeyAdded -= added;
                    subscriber.EventKeyRemoved -= removed;
                }
                finally
                {
                    lock (gate)
                    {
                        registration.Disposals--;
                        Changed("disposed:" + subscriber.Identity);
                    }
                }
            });
        }

        private async Task<bool> Delivered(Registration registration, MessageResult result,
            Func<MessageResult, object, Task<bool>> callback, object state)
        {
            Proof proof;
            string kind = null;
            lock (gate)
            {
                registration.Active++;
                if (result.Terminal)
                {
                    registration.Live = false;
                    Changed("terminal:" + registration.Subscriber.Identity);
                }
                proof = current;
                if (proof != null)
                {
                    foreach (var segment in result.Messages)
                        for (int i = segment.Offset; i < segment.Offset + segment.Count; i++)
                        {
                            var message = segment.Array[i];
                            if (ReferenceEquals(message, proof.GroupMessage)) kind = "group";
                            if (ReferenceEquals(message, proof.MarkerMessage)) kind = "marker";
                        }
                    if (kind != null)
                    {
                        var id = registration.Subscriber.Identity;
                        if (kind == "group" && (!proof.Routed.Contains(id) || !proof.Started.Add(id)))
                            Fail(proof, "Unexpected or duplicate original group callback: " + id);
                        if (kind == "marker" && proof.MarkerKey != "hc-" +
                            proof.GroupKey.Substring(3, proof.GroupKey.IndexOf('.') - 3) + "." + id)
                            Fail(proof, "Marker reached an unexpected original subscriber.");
                        proof.Trace.Add(new { sequence = ++sequence, kind, phase = "callback-start",
                            connectionId = id, originalCallback = registration.Callback,
                            totalCount = result.TotalCount });
                    }
                }
            }
            bool completed = false;
            try
            {
                // Preserve the exact original arguments, state and outcome.
                completed = await callback(result, state).ConfigureAwait(false);
                return completed;
            }
            finally
            {
                lock (gate)
                {
                    registration.Active--;
                    if (!completed) Changed("callback-ended:" + registration.Subscriber.Identity);
                    if (proof != null && kind != null)
                    {
                        var id = registration.Subscriber.Identity;
                        proof.Trace.Add(new { sequence = ++sequence, kind, phase = "callback-completed",
                            connectionId = id, originalCallbackReturnedTrue = completed });
                        if (!completed) Fail(proof, "Original delivery callback failed or ended.");
                        else if (kind == "group")
                        {
                            proof.Completed.Add(id);
                            if (proof.Completed.Count == proof.Routed.Length)
                                proof.GroupDone.TrySetResult(true);
                        }
                        else proof.MarkerDone.TrySetResult(true);
                    }
                }
            }
        }

        public Task Publish(Message message)
        {
            lock (gate)
            {
                var proof = current;
                if (proof != null && (message.Key == proof.GroupKey || message.Key == proof.MarkerKey))
                {
                    var value = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(message.GetString());
                    bool group = message.Key == proof.GroupKey;
                    if (message.IsCommand || message.IsAck || !String.IsNullOrEmpty(message.Filter) ||
                        !Equals(value["M"], group ? proof.Method : "QaMarker") ||
                        new JavaScriptSerializer().Serialize(value["A"]) !=
                            (group ? proof.Arguments : new JavaScriptSerializer().Serialize(new[] { proof.MarkerId })))
                        Fail(proof, "Actual publication does not match the controlled payload.");
                    if (group)
                    {
                        if (proof.GroupMessage != null) Fail(proof, "Duplicate group publication.");
                        proof.GroupMessage = message;
                        proof.GroupPublished = ++sequence;
                    }
                    else
                    {
                        if (proof.MarkerMessage != null || proof.Completed.Count != proof.Routed.Length)
                            Fail(proof, "Marker published before all original group callbacks completed.");
                        proof.MarkerMessage = message;
                        proof.MarkerPublished = ++sequence;
                    }
                    proof.Trace.Add(new { sequence, kind = group ? "group" : "marker", phase = "publish",
                        messageKey = message.Key, payload = message.GetString(), messageFilter = message.Filter });
                }
            }
            return inner.Publish(message);
        }

        internal Proof Begin(string hub, string group, string excludedGroup, string method,
            object[] arguments, string[] positive, string excluded, string marker)
        {
            lock (gate)
            {
                if (current != null || positive.Length != 2 || positive.Distinct().Count() != 2 ||
                    positive.Contains(excluded))
                    throw new InvalidOperationException("Routing proof requires two positive and one excluded connection.");
                var live = registrations.Where(r => r.Live).ToArray();
                if (registrations.Any(r => !r.Initialized || r.Active != 0 || r.Disposals != 0))
                    throw new InvalidOperationException("Original subscription registration, disposal or callback is still active.");
                var key = "hg-" + hub + "." + group;
                var excludedKey = "hg-" + hub + "." + excludedGroup;
                var keys = live.ToDictionary(r => r, r =>
                {
                    lock (r.Subscriber.EventKeys) return r.Subscriber.EventKeys.ToArray();
                });
                var routed = live.Where(r => keys[r].Contains(key)).Select(r => r.Subscriber.Identity).ToArray();
                if (positive.Any(id => !routed.Contains(id)) || routed.Contains(excluded) ||
                    routed.Distinct().Count() != routed.Length)
                    throw new InvalidOperationException("Actual group subscription keyset is not the two UI connections: " +
                        new JavaScriptSerializer().Serialize(new { groupKey = key, positiveConnectionIds = positive,
                            subscriptions = live.Select(r => new { connectionId = r.Subscriber.Identity,
                                eventKeys = keys[r] }).ToArray() }));
                foreach (var id in routed.Concat(new[] { excluded }))
                {
                    var selected = live.Where(r => r.Subscriber.Identity == id).ToArray();
                    if (selected.Length != 1 ||
                        selected[0].Subscriber.GetType().FullName != "Microsoft.AspNet.SignalR.Infrastructure.Connection" ||
                        selected[0].Subscriber.Subscription.GetType().FullName != "Microsoft.AspNet.SignalR.Messaging.DefaultSubscription" ||
                        !ReferenceEquals(selected[0].Subscriber.EventKeys, selected[0].Subscriber.Subscription.EventKeys) ||
                        !keys[selected[0]].Contains("hc-" + hub + "." + id) ||
                        (id == excluded && (keys[selected[0]].Contains(key) || !keys[selected[0]].Contains(excludedKey))))
                        throw new InvalidOperationException("Actual original connection subscription does not match UI membership.");
                }
                current = new Proof { GroupKey = key, ExcludedKey = excludedKey,
                    MarkerKey = "hc-" + hub + "." + excluded, MarkerId = marker,
                    Method = method, Arguments = new JavaScriptSerializer().Serialize(arguments),
                    Positive = positive, Routed = routed, Generation = generation, Opened = ++sequence,
                    Snapshot = live.Select(r => (object)new { connectionId = r.Subscriber.Identity,
                        subscriberType = r.Subscriber.GetType().FullName,
                        subscriptionType = r.Subscriber.Subscription.GetType().FullName,
                        eventKeys = keys[r], originalCallback = r.Callback,
                        activeCallbacks = r.Active, initialized = r.Initialized }).ToArray() };
                return current;
            }
        }

        internal void Validate(Proof proof)
        {
            lock (gate)
            {
                if (proof != current || generation != proof.Generation || proof.Failure != null)
                    throw new InvalidOperationException(proof.Failure ?? "Routing generation changed.");
            }
        }

        internal object Finish(Proof proof)
        {
            lock (gate)
            {
                Validate(proof);
                if (!proof.GroupDone.Task.Result || !proof.MarkerDone.Task.Result ||
                    proof.GroupMessage == null || proof.MarkerMessage == null)
                    throw new InvalidOperationException("Missing original delivery completion.");
                return new { invariant = "original-topic-membership-and-callback-completion-v1",
                    originalBus = inner.GetType().AssemblyQualifiedName,
                    groupKey = proof.GroupKey, excludedGroupKey = proof.ExcludedKey,
                    markerKey = proof.MarkerKey, positiveConnectionIds = proof.Positive,
                    routedConnectionIds = proof.Routed.OrderBy(id => id).ToArray(),
                    generationStart = proof.Generation, generationEnd = generation,
                    openedSequence = proof.Opened, groupPublishedSequence = proof.GroupPublished,
                    markerPublishedSequence = proof.MarkerPublished, completedSequence = ++sequence,
                    subscriptions = proof.Snapshot, trace = proof.Trace.ToArray(),
                    originalGroupCallbacksCompleted = proof.Completed.OrderBy(id => id).ToArray(),
                    membershipChanged = false };
            }
        }

        internal void End(Proof proof)
        {
            lock (gate) if (ReferenceEquals(current, proof)) current = null;
        }

        private sealed class RegistrationDisposer : IDisposable
        {
            private readonly Action dispose;
            internal RegistrationDisposer(Action action) { dispose = action; }
            public void Dispose() { dispose(); }
        }
    }
}

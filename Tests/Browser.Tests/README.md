# Isolated application browser baseline

This is the shared task2/task3 runner, not a second test application. It uses the
actual HTTPS MVC pages, real cookies, controllers, importer, repositories and
original SignalR connections/proxies/Join promises. Only external BGG, Identity
email and analytics boundaries are replaced before request scopes and seeding.
The exercised browser `ga` API is a captured no-op external boundary as well;
the original `Views.Shared.GoogleAnalytics` wrapper still executes. Its calls
reach a local CDP binding, never an external analytics service.

Run only from the owned migration worktree with its ACL-restricted run manifest.
The manifest supplies the origin, catalog, fixture IDs, synthetic password,
browser path, pipe secret, certificate and resource ownership. There is no
arbitrary URL option. Do not print the protected manifest or copy real settings.

## Preparation and execution

Task1's `prepare.ps1 -Phase prepare` establishes the private owned LocalDB
instance, generated application catalog, empty approved NerdScorekeeper
compatibility catalog, exact migrations, owned HTTPS binding and copied-worker
outbound containment without starting the application. Preserve the original
partial catalog's cleanup ledger. Task2 stages the support module before
bootstrap. These prerequisites must already be accepted; locations alone are not
readiness.

For an existing accepted run, refresh the owned support assembly while its host
is stopped, then configure each cell before a fresh AppDomain. The example
placeholders below describe required caller-supplied owned values, not defaults:

```powershell
pwsh -NoProfile -File Tests/Browser.Tests/seam-stage.ps1 -Phase refresh -RunId <GUID> -EvidenceRoot <owned-root>
pwsh -NoProfile -File Tests/Browser.Tests/seam-stage.ps1 -Phase configure -RunId <GUID> -EvidenceRoot <owned-root> -Bundle debug -FixtureCell debug-desktop
pwsh -NoProfile -File Tests/Browser.Tests/prepare.ps1 -Phase seed -RunId <GUID> -EvidenceRoot <owned-root> -Bundle debug -Stage app
node Tests/Browser.Tests/run.mjs --run-manifest <absolute-owned-manifest> --mode baseline --stage app --bundle debug --viewport desktop --evidence <absolute-owned-root>
```

Repeat configuration and baseline execution for `debug-mobile`,
`optimized-desktop` and `optimized-mobile`, choosing the matching bundle and
viewport. Each execution owns one browser/profile and closes it, its pipe and
its exact IIS/SQL processes. Retained owned infrastructure remains tracked for
the final `prepare.ps1 -Phase cleanup`; it is not silently deleted or adopted.

`--check-seam` retains the bounded original task2 prelude. `--seed-only` performs
guarded bootstrap/fixture verification and teardown without browser scenarios.
`--negative-control` runs exactly once per owned manifest on a disposable actual
login page: `core-jquery-version` intentionally expects `0.0.0`, must fail with
exit1 and an assertion receipt, then ordinary baseline execution restores the
correct `2.2.4` expectation. No other harness sabotage exists.

## Dependency upgrade checks

`--mode upgraded --check-dependencies` uses the same owned manifest, HTTPS host,
fixtures, original proxies and Join observer. It expects full jQuery `3.7.1` and
classic SignalR `2.4.3`, records loaded script URLs and response hashes, checks
one intended core/client source and the current jQuery source map, and records
the actual web AppDomain's SignalR assembly identity. It exercises real
notification and progress UI Join and same-AppDomain callback rendering.

Its failure case pauses only the actual local `/signalr/negotiate` response,
subscribes to the in-flight original start Deferred before fulfilling a 500,
then asserts rejection, no Join, no connected proof and no send. A fresh normal
page then proves the original UI Join succeeds after interception is removed.
It makes no extra start or Join and does not repeat the wrong-version control.
These checks are dependency acceptance, not complete upgraded workflow or
published-output acceptance. Results use `task-6-*` names and screenshots under
`upgraded/`, preserving frozen baseline result and screenshot files.

Before optimized dependency checks, run
`powershell -NoProfile -File .omo/evidence/jquery-3-migration/wave2/task-6/derive-native-bundles.ps1`.
It uses the actual restored `ScriptBundle` builder/transforms and the existing
WebGrease binding redirect in a standalone process, without application startup,
config secrets, database or network. ASP.NET re-minifies the selected official
`.min.js` files, so optimized acceptance compares exact independently derived
bundle hashes, validates current input/compiler hashes, and separates product
bytes from the appended test observer bytes. A vendor substring comparison is
valid for debug delivery but not for this native optimized transformation.
The protected manifest records `qaEvidenceMode` before controlled bootstrap.
Upgraded fixture confirmation writes `upgraded-fixtures.json`, leaving the
frozen `task-3-fixtures.json` receipt intact. Upgraded `--seed-only` checks use
separate `*-seed-only-*` result names instead of replacing browser pass receipts.

## Coverage and evidence

`coverage.json` maps each reachable task8/task9 behavior to its real page,
action, selectors, happy/failure assertion, request/persistence or DOM proof,
pre-trigger event barrier and screenshot. Per the plan's resolved coverage
clarification, the shipped unused `toEditBox` plugin is source-only inventory:
it has no application caller or save/cancel surface. It is not invoked by a
fake fixture and is not counted as a runtime pass.

The fixture module preserves the original account/group/game IDs and verifies
password authentication, confirmation, exact owner/name/group identity and
persisted domain content. A second confirmed account/group, deterministic
players, populated game lists, recorded games and long BGG metadata support the
real workflows. Four two-game BGG batches are selected by `qaFixtureCell`;
configuration advances protected `qaFixtureGeneration` so a repeated cell still
imports two new known IDs/names without deleting earlier results. Seed does not
precreate those import IDs. Import success proves both names absent
before the real UI POST and present in an independent group page request after
its response. The second owned BGG username throws at the external client
boundary; actual error response and unchanged persisted games are asserted.

The observer preserves original objects, arguments, receivers and Join
promises. Every client waits for its original UI Join. Same-AppDomain pipe sends
reach two actual same-group clients; the other group's pre-subscribed marker
bounds its zero product-event assertion. Positive product callbacks and markers
are awaited independently: no cross-message ordering is assumed. The sole
recovery is progress, actual page reload, new original UI Join, next progress
event restoring state, and completion hiding the bar. It asserts no replay.

Subscriptions precede form submits, clicks, requests, reloads and sends.
Synchronization uses exact navigation/response, AJAX completion, original Join,
proxy-event, Bootstrap lifecycle or DOM mutation signals bounded at 30 seconds,
never sleeps or polling. Product achievement delays are not test barriers.

The only external static read is the specifically approved HTTPS GET of
`https://cdnjs.cloudflare.com/ajax/libs/modernizr/2.8.3/modernizr.min.js`, normal
TLS, no redirects. Its bytes fulfill the actual app script request; the date
fallback row changes only its reported date capability before original setup.
All other nonlocal browser requests are blocked. Host containment is unchanged.

Results carry run/attempt IDs, worktree state, runtime/browser versions, stage,
bundle, viewport, fixture IDs, executed scenarios/assertions, requests,
boundaries, server-send/event/marker evidence and teardown. Runtime output is
under the protected evidence root, with screenshots under
`baseline/<bundle>-<viewport>/screenshots/`. Missing rows, required assertions,
timeouts and observed baseline failures exit nonzero. Each scenario's actual
assertions remain visible; baseline findings are classified rather than fixed
or silently counted as migration passes. The known empty-game List.js template
defect remains documented from task2; populated fixtures do not prove it fixed.
The interrupted desktop run also observed an empty-template exception in the
unplayed GameDefinition player summary. That receipt is retained. Shared
readmore/tooltip/popover acceptance uses the owned BGG-linked game after the
actual played-game UI records its players; no empty-summary repair or
replacement UI is claimed.
Frozen mobile baseline exceptions remain failures: gamingGroup.js:300 references
undefined `gamedefinitionsValues` after rendering the real plays response, and
achievementDetails.js:50 references undefined `winnerplayersValuessValues` on
the real achievement link destination. Record both original console exceptions
and the nonzero baseline cell exit. Task8's authorized source corrections use the
already declared local List value arrays; the frozen receipts remain unchanged.
The same-focus select/clear autocomplete edge is explicitly recorded as a
baseline stale-ID defect, alongside the normal committed-selection clear path.
Readmore's generated selector loses its match when its attribute changes; its
zero transition/missing expanded aria update is retained as a baseline finding.
Actual expand/collapse geometry and separate tooltip/popover outcomes still
execute. Observe the original readmore load measurement through its clone
addition/removal DOM writes before toggling; never repair its scheduling or
invoke its renderer. Notification visual readiness waits for original opacity
1 through the pretrigger DOM subscription, never by advancing its animation.

These are Chromium desktop 1440x900 and mobile 390x844 viewport checks, not
physical Safari coverage. Display and inspect actual screenshots. No shipping
support reference, production secrets/data, deployment or commit is part of
this runner.

## Task7 diagnostic and clean comparison

`--mode migrate` executes the same complete coverage map on the actual upgraded
HTTPS application. It uses the official uncompressed Migrate 3.5.2 file pinned
under `.omo/evidence/jquery-3-migration/wave2/task-7/`, never a shipping asset.
The core response is instrumented immediately after the actual jQuery runtime,
before the cookie and other plugins. For the native optimized bundle, the exact
independent cookie UMD boundary is verified before inserting the diagnostic
script; the input core/cookie bytes are not patched. The unsafe legacy HTML
prefilter patch is explicitly disabled.

Task7 records original console warnings/stacks, exceptions, network event
timing, per-scenario Migrate warnings and behavior assertions. The official
property is `jQuery.migrateWarnings`; evidence calls these `migrateMessages`.
Warnings alone are not migration failures. Compare each actual scenario with
its frozen task3 result and the clean comparison, retaining baseline defects.

`--mode upgraded --diagnostic-comparison` executes the full map without loading
Migrate and records byte-identical original core responses and absent Migrate
runtime properties. Task7 results use separate `task-7-<bundle>-<viewport>-<mode>`
names; screenshots are attempt-qualified under `task-7/<mode>/<attempt>/`.
Diagnostic bootstrap uses the upgraded fixture receipt rather than overwriting
frozen baseline fixtures. The original wrong-version control is never repeated.
All guarded host, database, fixture batch and teardown requirements still apply.

## Task8 clean interactive acceptance

`--mode upgraded` without a narrower prelude or diagnostic-comparison flag runs
all 39 mapped workflows and writes `task-8-<bundle>-<viewport>` results with
attempt-qualified screenshots under `task-8/<bundle>-<viewport>/<attempt>/`.
It never replaces frozen task3 results or screenshots. Each scenario asserts
no console exception, absent Migrate and no legacy HTML-prefilter restoration;
the entire cell also rejects startup or other-page exceptions. Baseline defect
classification remains separate from the corrected mobile acceptance results.

## Task10 shipping and published acceptance

Use the same runner with `--mode upgraded --acceptance --stage app|published`
and the guarded manifest belonging to that stage. Execute debug/optimized at
desktop/mobile for both stages. Each cell runs all 39 coverage rows plus the
real negotiation 500/rejected-start probe and a fresh ordinary successful UI
setup. There is no additional negative control or recovery scenario.
Task10 result names include stage, bundle, viewport and unique attempt ID.

The routing observer delegates to the original classic SignalR MessageBus.
For each controlled achievement/progress exclusion proof, the complete live
group-topic subscription keyset must match the actual original callbacks.
Every routed callback must complete true before the excluded marker publish,
and the membership generation must remain unchanged through marker completion.
The excluded connection must have its own different-group and marker topics
and no target-group topic. Zero callbacks through a marker alone is invalid
proof; older marker-only receipts remain historical evidence, not final passes.
Positive browser callbacks and rendered state are independently awaited.

`prepare.ps1 -Phase effective` uses a new .NET Framework AppDomain configured with the
exact staged Web.config and the actual CloudConfigurationManager. It validates
the generated local catalog, bound HTTPS callbacks and every synthetic
OAuth/email/telemetry value before bootstrap. Published staging runs it after
Release transforms and harmless regeneration, before creating the fresh catalog.
An explicitly cross-linked protected manifest permits that fresh published
catalog on the already owned private instance; it never adopts an existing
catalog or changes the empty compatibility database.

`check-exclusion.ps1 -EvidenceRoot <task10-root>` records the untouched normal filesystem publish before
overlay. The separate QA copy may add only its generated private settings and
the exact support DLL, and may change only Web.config for the guarded settings,
module registration and QA bundle/error mode. No application asset may change
or disappear. The normal manifest rejects QA/Migrate/stale runtime artifacts,
parses configuration, compares official package assets and maps, and checks
the actual SignalR assembly identities and bytes.

Close each cell's exact owned browser, profile, pipe, IIS process and private
SQL process in finally. Retain the manifests, stage copies, owned catalogs,
certificate, bindings and containment rules until the independent app/published
verification finishes. A stopped process is not deleted infrastructure.
Final cleanup must validate and remove each separately recorded published
catalog before removing the original infrastructure; preserve the original
partial-catalog cleanup obligation and never drop a catalog by a name pattern.

Rollback is a reviewed reverse patch or an explicitly authorized revert of
only the migration's dependency, template-generated and minimal application
changes. Preserve unrelated user changes and the frozen evidence. Do not use
destructive reset/checkout, force-push, production configuration or production
data. No rollback command is executed by this runner.

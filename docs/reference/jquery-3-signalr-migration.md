# jQuery 3 migration: SignalR ownership and behavior

This note records the SignalR paths relevant to the jQuery 3 upgrade. The
inspected baseline references SignalR 2.2.0; the approved target is the matched
SignalR 2.4.3 client and server with jQuery 3.7.1. The current-source paths
below describe ownership and callbacks, not measured production delivery.

## Current paths

SignalR is configured during OWIN startup: `Source/UI/Startup.cs:24-30` calls
`ConfigureSignalR`, and `Source/UI/App_Start/Startup.SignalR.cs:9-13` loads
the hub assembly and maps SignalR.

**BoardGameGeek import progress.** The importer sends
`BGGImportDetailsProgress(currentGamesImported, gamesToImport, currentGameName)`
to the group keyed by the current gaming group ID
(`Source/BusinessLogic/Logic/BoardGameGeek/BoardGameGeekGamesImporter.cs:61,80`).
`LongRunningTaskHub.Join` adds the connection to the supplied group key
(`Source/NemeStats.Hubs/LongRunningTaskHub.cs:8-11`). The game-definition
client starts a `longRunningTaskHub` proxy and joins with the page's gaming
group ID (`Source/UI/Scripts/GameDefinition/createGameDefinitionPartial.js:89-108`).
Its callback updates the progress view model and hides the bar when imported
count equals total (`.../createGameDefinitionPartial.js:91-103`). There is no
completion reload in the current callback. A client that joins while an import
is underway can respond to a later event; this code does not replay earlier
events or restore state by itself.

**Achievement notifications.** `AchievementProcessor` sends
`NewAchievementUnlocked(achievementId, achievementName, achievementIconClass,
achievementDescription, achievementLevel)` to the group keyed by the player's
application-user ID (`Source/BusinessLogic/Logic/Achievements/AchievementProcessor.cs:122-129`).
`NotificationsHub.Join` adds the connection to the supplied user key
(`Source/NemeStats.Hubs/NotificationsHub.cs:8-11`). The shared layout starts a
`notificationsHub` proxy and joins with the current user ID
(`Source/UI/Scripts/Shared/_Layout.js:55-79`). Its callback shows a PNotify
notification and links to `/achievements/{id}` on click (`.../_Layout.js:64-75`).

Both hubs accept a caller-provided key in `Join` and pass it to `Groups.Add`.
That is a source-level authorization observation only; no exploit or protection
was tested or established here. Current source wiring also does not establish
production traffic or successful delivery.

## Relevant history

- `fa54e5aea7c3bbc4d6487530079795c65b9916c1` (2016-03-15), "SignalR.LongRunningTaskHub used for ImportFromBGG," introduced the BGG progress group send and client callback. Relevant paths: importer, `LongRunningTaskHub`, and game-definition script.
- `02c4bc7275598789ec17c4fc15d5ac6c46b5b9cd` (2016-03-17), "Improved BGGImportDetailsProgress for reconnected users," recalculated progress on each received event and added a completion `window.location.reload(false)`. It also removed the generated `/signalr/hubs` include. This describes historical behavior, not the current callback or event replay.
- `8356bd0295e23b88ab6054d37224e205b008e1a2` (2016-05-08), "ServiceBus ASYNC. Achievement unlocked notification with SignalR," introduced achievement notification delivery and its client callback.
- `1d89e3664cfae4bd1b6c45726dd49e73d9be4838` (2016-05-09), "Commented New Achievement unlocked notification (from now...). Fixed tests," is an intermediate disable context: display code was commented out. It does not show permanent removal.
- `bd6270185b514ebe81dfc94e0c7dfa5a90cc9de3` (2016-05-24), "achievement notifications," restored visible notifications, passing five values and rendering a PNotify link.
- `62ac53aa39288e7f5a912648b9185eba0cbe6a55` (2017-07-11), "Major overhaul to played game processing...," moved achievement processing to `IAchievementProcessor`/`AchievementProcessor` and consolidated post-game work in `PlayedGameEventHandler`. `CommonRegistry` scans the BusinessLogic assembly; the active sender is the current processor, not the former event handler.
- `2f2a331aea7b4a5d9a6fe219631ab1b616fef94c` (2026-06-17), "chore(deps): bump Microsoft.Owin.* from 3.0.1 to 4.2.3," updates OWIN package references in the inspected project files. It does not establish a SignalR upgrade; those references remain SignalR 2.2.0.

Dates in this history list are the short Git author dates (`%ad --date=short`),
as recorded by the task history evidence.

## jQuery 3 compatibility context

The inspected baseline SignalR 2.2.0 client uses the removed
`$(window).load(handler)` shorthand when handling page load
(`Source/UI/Scripts/jquery.signalR-2.2.0.js:250`). jQuery removed event shorthands such as
`.load(handler)` in 3.0.0; see the
[jQuery `.load()` shorthand API notice](https://api.jquery.com/load-shorthand/).
The SignalR 2.4.3 JavaScript client instead checks for `_pageWindow.on` and
uses `_pageWindow.on("load", ...)`, falling back to `.load(...)` only when `.on`
is unavailable. This is the bounded compatibility correction relevant to the
matched target, not evidence that the application migration has passed. See
the [SignalR 2.4.3 client source](https://app.unpkg.com/signalr%402.4.3/files/jquery.signalR.js),
which identifies itself as Microsoft ASP.NET SignalR JavaScript Library 2.4.3.

## Migration implications and evidence limits

Preserve the two existing sender-to-group-to-callback contracts while moving
from the inspected SignalR 2.2.0 baseline to the approved matched SignalR
2.4.3 client/server target with jQuery 3.7.1. Do not retain the old client
plugin as a target requirement. The history shows that the BGG completion
reload once existed, but the current client does not reload; a later event can
update a joined client's progress, while earlier events are not replayed by
these source paths.

The citations and current paths are source/history inspection. They do not
claim production traffic, successful live delivery, replay, or tested
authentication/authorization protection, and they do not claim that the
application migration passes.

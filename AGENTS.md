# PROJECT KNOWLEDGE BASE

**Generated:** 2026-05-20
**Commit:** (working tree)
**Branch:** master

## OVERVIEW
NemeStats is a free ASP.NET MVC website for tracking board game results among groups of players. Core stack: .NET Framework 4.5.2, EF6 code-first, StructureMap IoC, SignalR, Azure WebJobs.

## STRUCTURE
```
NemeStats/
├── Source/
│   ├── UI/                          # ASP.NET MVC 5 + Web API 2 web app
│   ├── BusinessLogic/               # Domain logic, EF6 DbContext, ~268 migrations
│   ├── BoardGameGeekApiClient/      # BGG XML API wrapper
│   ├── NemeStats.IoC/               # StructureMap registries
│   ├── NemeStats.ScheduledJobs/     # Azure WebJobs (sitemap gen, BGG sync)
│   └── NemeStats.Hubs/              # SignalR real-time hubs
├── Tests/
│   ├── BusinessLogic.Tests/         # Unit + integration tests (333 files)
│   ├── UI.Tests/                    # Controller + transformation tests
│   ├── BoardGameGeekApiClient.Tests/
│   └── NemeStats.ScheduledJobs.Tests/
└── NemeStats.TestingHelpers/        # Shared test utilities
```

## WHERE TO LOOK
| Task | Location | Notes |
|------|----------|-------|
| Add/change domain logic | `Source/BusinessLogic/Logic/` | Organized by feature (Players, Games, Achievements, etc.) |
| Add/change API endpoint | `Source/UI/Areas/Api/Controllers/` | Versioned REST API (v2) |
| Add/change MVC view | `Source/UI/Views/` | Razor views matching controller names |
| Add/change route | `Source/UI/App_Start/RouteConfig.cs` | Attribute routes preferred |
| Register new DI mapping | `Source/NemeStats.IoC/CommonRegistry.cs` | Convention scanning + explicit overrides |
| Add EF migration | `Source/BusinessLogic/Migrations/` | Run `update-database -ProjectName BusinessLogic` |
| Add scheduled job | `Source/NemeStats.ScheduledJobs/` | Azure WebJobs with timer triggers |
| Board game metadata | `Source/BoardGameGeekApiClient/` | Thin wrapper over BGG XML API |
| Test a retriever | `Tests/BusinessLogic.Tests/UnitTests/LogicTests/` | Mirror of Logic/ structure |
| Test a controller | `Tests/UI.Tests/UnitTests/ControllerTests/` | Mirror of Controllers/ structure |

## CODE MAP
| Symbol | Type | Location | Role |
|--------|------|----------|------|
| `MvcApplication` | Class | `Source/UI/Global.asax.cs` | ASP.NET app startup (no Program.cs) |
| `Startup` | Class | `Source/UI/Startup.cs` | OWIN auth + SignalR configuration |
| `NemeStatsDbContext` | Class | `Source/BusinessLogic/DataAccess/` | EF6 code-first DbContext |
| `CommonRegistry` | Class | `Source/NemeStats.IoC/CommonRegistry.cs` | StructureMap DI composition root |
| `IBusinessLogicEventBus` | Interface | `Source/BusinessLogic/Events/` | In-process event publishing |
| `PlayerRetriever` | Class | `Source/BusinessLogic/Logic/Players/` | Core player query logic (~439 lines) |
| `AccountController` | Class | `Source/UI/Controllers/` | Largest MVC controller (~645 lines) |

## CONVENTIONS
- **License header**: Every source file starts with GPL v3 `#region LICENSE` block.
- **IoC**: StructureMap with convention scanning (`RegisterConcreteTypesAgainstTheFirstInterface`).
- **Mapping**: AutoMapper 4.2.1 with static `AutomapperConfiguration.Configure()` in `Global.asax`.
- **Routing**: T4MVC generates `Links.*` classes for compile-safe URL generation. `*.generated.cs` files are auto-generated.
- **JavaScript**: Namespace.js pattern — use `Namespace()` function, namespace matches file path.
- **Naming**: Scripts tied to a `.cshtml` view are named after the view; reusable scripts go in `Source/UI/Scripts/`.
- **Configs**: `PrivateAppSettings.config` (gitignored) holds connection strings and API keys. Created by `GetStarted.ps1`.

## ANTI-PATTERNS (THIS PROJECT)
- **Do not edit `*.generated.cs` files** — these are T4MVC outputs. Edit the T4MVC template instead.
- **Do not set EF database initializer** — explicitly set to `null` in `Global.asax`; migrations handle schema.
- **Do not check in `PrivateAppSettings.config`** — contains secrets and environment-specific settings.
- **Do not add logic directly in controllers** — controllers delegate to `BusinessLogic/` retrievers/services.
- **Avoid `System.Web` dependencies in BusinessLogic** — web concerns stay in UI layer.

## UNIQUE STYLES
- **Event bus**: Lightweight in-process event bus (`IBusinessLogicEventBus` / `IBusinessLogicEventHandler<T>`) for decoupled side effects.
- **Achievements engine**: Complex rule-based achievement system in `BusinessLogic/Logic/Achievements/`.
- **BGG integration**: Automatic board game metadata sync from BoardGameGeek API.
- **Sitemap jobs**: Scheduled Azure WebJob generates XML sitemaps for SEO.
- **Custom build configs**: `Jake` configuration exists in some `.csproj` files (legacy CI profile).

## COMMANDS
```powershell
# Setup (creates PrivateAppSettings.config)
.\GetStarted.ps1

# Apply EF migrations
update-database -ProjectName BusinessLogic

# Build
msbuild NemeStats.sln

# Run tests (via Test Explorer or CLI)
# NUnit / xUnit tests in all *.Tests projects
```

## NOTES
- Migrations folder (`BusinessLogic/Migrations/`) has 268 files — do not be alarmed.
- Frontend build uses Grunt + Sass (legacy); `package.json` and `gruntfile.js` in `Source/UI/`.
- CI is AppVeyor (`appveyor.yml`) targeting Windows Server 2012.
- `AzureWebJobsDashboard` and `AzureWebJobsStorage` settings required for scheduled jobs.
- Some controller actions use `T4MVC` strongly-typed action links (`MVC.Player.Details(...)`) instead of magic strings.

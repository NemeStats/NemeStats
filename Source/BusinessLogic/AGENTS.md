# BUSINESSLOGIC MODULE

**Generated:** 2026-05-20
**Scope:** Domain logic, data access, EF migrations, and business rules.

## OVERVIEW
Core domain layer containing EF6 DbContext, migrations (~268 files), retrievers/services per feature domain, and the in-process event bus.

## STRUCTURE
```
BusinessLogic/
├── DataAccess/          # EF DbContext, repositories, security
├── Events/              # IBusinessLogicEventBus, handlers, factory
├── Logic/               # Feature-organized business logic
│   ├── Achievements/    # Rule-based achievement engine
│   ├── Players/         # Player CRUD + stats (PlayerRetriever ~439 lines)
│   ├── PlayedGames/     # Game session recording
│   ├── GameDefinitions/ # Board game definitions
│   ├── GamingGroups/    # Player groups
│   ├── Users/           # User management
│   └── ...
├── Models/              # POCOs + view models
├── Migrations/          # EF6 code-first migrations (268 files)
├── Jobs/                # Sitemap generators, BGG batch updates
├── Facades/             # High-level orchestration
└── Providers/           # External service abstractions
```

## WHERE TO LOOK
| Task | Location | Notes |
|------|----------|-------|
| Add new entity | `Models/` + `DataAccess/` | Add to DbContext, run migration |
| Add business rule | `Logic/<Feature>/` | Follow existing retriever/service pattern |
| Add achievement | `Logic/Achievements/` | Implement `Achievement<T>` with criteria |
| Add event handler | `Events/` | Implement `IBusinessLogicEventHandler<T>` |
| Change DbContext | `DataAccess/NemeStatsDbContext.cs` | Migrations required after changes |
| Add integration test | `Tests/BusinessLogic.Tests/IntegrationTests/` | Inherit `IntegrationTestBase` |

## CONVENTIONS
- **Retrievers** = query-only classes (naming: `*Retriever`).
- **Creators/Updaters/Deleters** = command classes.
- **Facades** = cross-cutting orchestration (e.g., `PlayedGameCreatorFacade`).
- **Security**: `SecuredEntityValidatorBase<T>` guards access to owned entities.
- **EF**: No database initializer set (`null` in Global.asax); migrations only.

## ANTI-PATTERNS
- **No `System.Web` references** — web concerns stay in UI layer.
- **No direct EF queries outside DataAccess** — use repositories/retrievers.
- **Do not modify migration files after creation** — create new migrations instead.

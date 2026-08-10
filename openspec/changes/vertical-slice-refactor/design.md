## Context

Single net8.0 ASP.NET Core web project `HackerRank1/` with mixed namespaces (`HackerRank1.*` / `LibraryService.WebAPI.*`), entities co-located with `LibraryContext`, all logic following a controller → service → EF Core flow, static `TokenGenerator`, and several `NotImplementedException` stubs. Ghost folders `LibraryService.{Api,Application,Domain,Infrastructure}` (only `bin/obj` + a stray `.csproj.user`) mark previously abandoned refactors. Integration tests (xunit, `WebApplicationFactory<Program>` + `UseStartup<Startup>`, SQLite in-memory) define the intended HTTP contract. See proposal.md - Why.

Constraints:
- net8.0; EF Core 8 + Npgsql against Supabase Postgres.
- JwtBearer auth, Swashbuckle, Newtonsoft.Json in use today.
- Postgres schema is already live on Supabase (migrations applied).
- `LibraryService.Integration.Test` is in the solution; `IntegrationTest/` (net6) is an orphan duplicate.
- Architecture choice locked in with the user: **hand-rolled handlers** (no MediatR) and a **single project with `Features/`** folders.

## Goals / Non-Goals

**Goals:**
- One project where every feature is a self-contained vertical slice (`Features/<Feature>/`), co-locating request, handler, validation, and endpoint.
- A minimal hand-rolled command/query dispatch seam so slices stay independent and unit-testable, without any framework coupling.
- A small **Shared kernel** (`Common/`) as the only thing slices depend on: `LibraryContext`, entities, JWT plumbing, DI aggregation.
- Per-slice DI modules so adding a feature touches exactly one self-registration point.
- Stubbed CRUD completed and the routes the tests assert exposed (POST books / DELETE libraries) with correct 404 semantics.
- Everything builds and the integration suite is green.

**Non-Goals:**
- No new features beyond the CRUD paths the existing tests already require.
- No auth hardening (`admin`/`1234` stays; committed Supabase secret rotation is out of scope).
- No database schema change; same EF model, regenerated migration must stay equivalent.
- No repository layer (deliberate departure from the clean branch): slices access `LibraryContext` directly — vertical slice favors thin slices over shared data abstractions.
- No new test framework or test doubling beyond what's needed to adapt existing tests.

## Decisions

### D1 — Single web project organized by `Features/`
```
LibraryService.Api            web sdk, net8.0 (renamed from HackerRank1/)
├── Common/                   shared kernel — the only cross-slice code
│   ├── Auth/                 JwtSettings + TokenGenerator
│   ├── Data/                 LibraryContext + Migrations
│   ├── Entities/             Book, Library
│   └── DependencyInjection.cs  AddShared() (context, auth, option defaults)
├── Features/
│   ├── Authentication/
│   │   └── Login/            LoginController, LoginQuery, LoginHandler
│   ├── Libraries/
│   │   ├── GetLibraries/     GetLibrariesController, GetLibrariesQuery, GetLibrariesHandler
│   │   ├── GetLibraryById/   ...
│   │   ├── CreateLibrary/    ...
│   │   ├── UpdateLibrary/    ...
│   │   └── DeleteLibrary/    ...
│   └── Books/
│       ├── GetBooksForLibrary/  GetBooksController, GetBooksQuery, ...
│       └── AddBookToLibrary/    AddBookCommand, ...
│   └── <Feature>Module.cs    per-slice AddXxxFeatures() extension
├── Program.cs / Startup.cs
```
Rationale: vertical slice organizes by *business capability*, cutting through all technical layers in one folder each. Single project keeps the dependency graph flat (no layer-boundary ceremony) which is the defining trade of vertical slice vs clean.
Alternative considered: keep the above-the-fold shared kernel as separate projects (Domain/Common) — rejected; the user chose single-project, and cross-slice `Common/` satisfies reuse.

### D2 — Hand-rolled command/query handlers
```csharp
// Common/Handlers/ (a tiny, framework-free contract)
public interface IQueryHandler<TQuery, TResult> { Task<TResult> HandleAsync(TQuery query, CancellationToken ct = default); }
public interface ICommandHandler<TCommand, TResult> { Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default); }
```
Each slice implements a handler that owns its complete flow: validate → query/command `LibraryContext` → map to response. Endpoints resolve a concrete handler (or an interface) from DI and call `HandleAsync`.
Rationale: buy vertical-slice semantics (one cohesive flow per feature) without adopting MediatR's pipeline/assembly-scan machinery. Fewer moving parts for a course exercise; the seam still lets us swap MediatR in later.
Alternative: MediatR with `ISender`/`Assembly.GetExecutingAssembly()` scanning — rejected by explicit user choice (keeps dependencies minimal).

### D3 — Slices self-register via per-feature modules
Each feature exposes a module extension, e.g. `LibrariesModule.AddLibrariesFeatures(this IServiceCollection)` registering only that slice's handler(s). `Startup` composes: `AddShared()` for the kernel + each feature module.
Rationale: mirrors the "one slice, one registration point" idiom; `Startup` never lists handler classes directly. Contrast with the clean branch's `AddApplication()`/`AddInfrastructure()` which group by layer, not capability.
Alternative: assembly scanning for handlers — rejected (implicit, and we have no facilities to scan by interface).

### D4 — Entities and DbContext stay central, in the shared kernel
- `Book`, `Library` → `Common/Entities/`.
- `LibraryContext` → `Common/Data/`; migrations → `Common/Data/Migrations/`, **regenerated** via a design-time `LibraryContextFactory` rather than moved. Model is unchanged, so the regenerated `InitialCreate` is equivalent and `Migrate()` stays a no-op on the live DB.
Rationale: slices share the EF context deliberately; per-slice unit-of-work or per-slice DbSet would be over-engineering at this scale.
Alternative: repositories (as in the clean branch) — rejected for this slice design (Non-Goals).

### D5 — Endpoints are co-located controllers per slice
Each feature folder contains its own thin `ApiController`. Controllers bind request DTOs, dispatch to the slice handler, and map results to HTTP status codes (200/201/204/404), including the 404 paths (missing library for books, missing id for library update/delete).
Rationale: keeps the HTTP boundary *inside* the slice (a core vertical-slice property) and preserves the existing controller/test contract; minimal API endpoints would be equally slice-aligned but introduce a second HTTP style.
Alternative: one shared controller per resource — rejected; divorces endpoint from feature, recreating the God-controller problem.

### D6 — Request/response DTOs live inside each slice
`BookForm`, `LibraryForm`, login request/response move into their feature folders as plain classes, dropping Newtonsoft `JsonProperty` attributes (System.Text.Json camelCase binding is case-insensitive; no Newtonsoft config is wired anyway). Entities are the shared kernel's `Book`/`Library`; feature DTOs are the request/response surface.
Rationale: a slice owns its I/O contract; no cross-slice DTO sharing is needed for these features.

### D7 — Auth as a slice with shared kernel plumbing
- `Features/Authentication/Login/`: `LoginController`, `LoginQuery`, `LoginHandler` (preserves hardcoded `admin`/`1234`), mapping to the shared `TokenGenerator`.
- `Common/Auth/`: `JwtSettings` (config-bound) + `TokenGenerator` (shared signing implementation invoked from the Login slice and validated by the JWT bearer middleware).
Rationale: signing is shared infrastructure; the login *use case* is a slice. Keeps the JWT flow byte-compatible with existing behavior.

### D8 — Namespace and solution rename
- `HackerRank1.sln` → `LibraryService.sln`; `HackerRank1/HackerRank1.csproj` → `LibraryService.Api/LibraryService.Api.csproj`.
- Root namespaces unify to `LibraryService.Api.*` — `LibraryService.Api.Common.*`, `LibraryService.Api.Features.*`.
Rationale: one naming world; the single-project shape makes this one clean replacement. **BREAKING** only for in-repo references (tests).

### D9 — DI registration surface
- `Common/DependencyInjection.cs`: `AddShared(this IServiceCollection, IConfiguration)` — pooled `LibraryContext` (Npgsql + `EnableRetryOnFailure`), `JwtSettings` (bound), `TokenGenerator`.
- Each feature module (D3) registers its handler(s).
- `Startup` calls `AddShared`, all feature modules, and keeps existing JwtBearer validation, `AddAuthorization`, CORS, Swagger, controllers wiring.
Note: tests override the DbContext via `services.RemoveAll<LibraryContext>` + `AddSingleton` on the test host — still works as long as handlers resolve `LibraryContext` purely from constructor injection.

### D10 — Tests authenticate before hitting protected endpoints
Books endpoints keep `[Authorize]` (preserves security intent). Integration tests are updated to call `/login` (admin/1234) once per fixture and attach the bearer token, exactly as in the clean branch's green suite.
Alternative: drop `[Authorize]` so tests pass unauthenticated — rejected (weakens security).

### D11 — Test consolidation and package alignment
- Delete orphaned `IntegrationTest/`.
- Keep `LibraryService.Integration.Test` (net8, xunit); retarget references to `LibraryService.Api`; bump `Microsoft.AspNetCore.Mvc.Testing`, `Microsoft.EntityFrameworkCore.InMemory/Sqlite` to 8.x (currently 6.x against net8); restore the FluentAssertions-based spec coverage from the clean branch's test file (POST books → 201/404, GET books → 200/404/401, DELETE library → 204/404).

## Risks / Trade-offs

- **[Regenerated migration drifts from live Supabase]** → Model is unchanged, so the regenerated `InitialCreate` should be equivalent; verify with `dotnet ef migrations list` and a local smoke run before merging. Fallback: move the existing migration files instead.
- **[Single project collapses clean-architecture boundaries]** → This is the intended trade of vertical slice; the `Common/` folder is the explicit convention that keeps cross-slice coupling visible and bounded. Enforce via review, not the compiler.
- **[Handlers grow large (no repository/validation pipeline)]** → Accept for course scale; keep each handler to its single feature flow. If a handler bloats, that is the natural signal to extract a private helper inside the slice.
- **[Tests break on auth / namespace changes]** → Keep `admin`/`1234` identical and reuse the clean branch's login helper; both test projects are handled in the same change and the orphan is deleted.
- **[Package version mismatch in tests]** → Bump all test packages to 8.x during consolidation.
- **[DbContext override no longer applies]** → Handlers must take `LibraryContext` only via constructor injection (D9) so the test's SQLite singleton replaces Postgres.
- **[Committed Supabase secret]** → Flagged for rotation outside this change; not introduced by this refactor.

## Migration Plan

1. Scaffold the `LibraryService.Api` single-project layout around a temporary `Common/` + `Features/` skeleton; rename `HackerRank1` → `LibraryService.Api`; rebuild the solution and rename it.
2. Relocate entities + `LibraryContext` into `Common/`; add design-time factory; regenerate the initial migration and verify equivalence.
3. Add the handler contracts to `Common/`; implement the auth slices shim so `/login` still works.
4. Build each library feature slice (query/command/controller) feeding the `library-catalog` behavior; implement the missing `DELETE /libraries/{id}`.
5. Build each books feature slice; implement missing `POST /books` and the 404-on-missing-library paths.
6. Compose DI (D9); delete ghost `LibraryService.*` folders and stray `.csproj.user`.
7. Update `LibraryService.Integration.Test` (namespaces, references, package versions, auth helper, spec coverage); delete orphaned `IntegrationTest/`.
8. `dotnet build` clean; `dotnet test` green (SQLite in-memory, no live DB needed).
9. Rollback = `git revert`; no schema/data migration risk since the model is unchanged.

## Open Questions

- Expose book `PUT`/`DELETE` routes now that handlers exist per slice, or leave them non-routed? Deferrable — no test or spec requires them (clean branch kept them unexposed).
- Keep `POST /api/libraries` returning `200` (current behavior) or standardize on `201`? Deferrable — no test asserts it.
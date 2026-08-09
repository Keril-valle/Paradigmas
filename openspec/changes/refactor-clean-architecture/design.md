## Context

Single net8.0 ASP.NET Core web project `HackerRank1/` with mixed namespaces (`HackerRank1.*` / `LibraryService.WebAPI.*`), entities co-located with `LibraryContext`, services coupled directly to EF Core, static `TokenGenerator`, and several `NotImplementedException` stubs. Ghost folders `LibraryService.{Api,Application,Domain,Infrastructure}` (only `bin/obj` + a stray `.csproj.user`) mark a previously abandoned layering attempt. Integration tests (xunit, `WebApplicationFactory<Program>` + `UseStartup<Startup>`, SQLite in-memory) define the intended HTTP contract but reference the old namespaces and currently assume some endpoints exist that don't. See proposal.md - Why.

Constraints:
- net8.0 across the board; EF Core 8 + Npgsql against Supabase Postgres.
- JwtBearer auth, Swashbuckle, Newtonsoft.Json in use today.
- Postgres schema is already live on Supabase (migrations applied).
- `LibraryService.Integration.Test` is in the solution; `IntegrationTest/` (net6) is an orphan duplicate.

## Goals / Non-Goals

**Goals:**
- Four-project clean architecture with a strict, one-direction dependency rule.
- Application layer decoupled from EF Core via repository interfaces.
- Single consistent namespace family `LibraryService.<Layer>`.
- Stubbed CRUD completed and the routes the tests assert exposed (POST/DELETE) with correct 404 semantics.
- Everything builds and the integration suite is green.

**Non-Goals:**
- No new features beyond the CRUD paths the existing tests already require.
- No auth hardening (`admin`/`1234` stays; committed Supabase secret rotation is out of scope).
- No database schema change; same EF model, regenerated migration must stay equivalent.
- No new test framework or test doubling beyond what's needed to adapt existing tests.

## Decisions

### D1 — Project topology and dependency rule
```
LibraryService.Domain          classlib, no package refs
LibraryService.Application     classlib, refs Domain
LibraryService.Infrastructure  classlib, refs Application + Domain
LibraryService.Api             web sdk, refs Application + Infrastructure
LibraryService.Integration.Test refs Api
```
Rationale: the canonical .NET clean-layout (Ardalis/jasontaylor); also matches the ghost folders' intent.
Alternative considered: fold Application into Api — rejected (couples use cases to the host).

### D2 — Repository pattern
Application defines `IBookRepository` / `ILibraryRepository`; Infrastructure implements `BookRepository` / `LibraryRepository` over EF Core. Application services depend only on repository interfaces and domain entities.
Alternative: keep services on `LibraryContext` — works, but leaks EF into Application and hurts unit-testability. Rejected.

### D3 — DbContext and migrations relocate to Infrastructure
- `Book`, `Library` → `Domain/Entities/`.
- `LibraryContext` → `Infrastructure/Data/`.
- Migrations → `Infrastructure/Migrations/`, **regenerated** via a design-time `LibraryContextFactory` rather than moved. Because the model is unchanged, the regenerated `InitialCreate` is equivalent to the already-applied migration, so `Migrate()` at startup is a no-op on the live Supabase DB.
Alternative: move existing migration files and fix namespaces — risk of snapshot drift. Rejected.

### D4 — Use cases in Application, thin controllers in Api
`LibrariesService` / `BooksService` (implementing `ILibrariesService` / `IBooksService`) live in `Application/Services`, fully implementing `Get/Add/Update/Delete` through repositories. Controllers bind request DTOs, call services, map to domain, and produce HTTP results (200/201/204/404). `BooksService` gains library-existence checks so the 404 paths in the spec hold.

### D5 — Request DTOs live in Api (presentation boundary)
`BookForm`, `LibraryForm`, `TokenResponse` move to `Api/DTOs` as plain classes, dropping Newtonsoft `JsonProperty` attributes (System.Text.Json camelCase binding is case-insensitive; no Newtonsoft config is wired anyway).
Alternative: Application-owned DTOs — rejected; DTOs are a presentation concern and would drag serialization concerns inward.

### D6 — Auth split across layers
- `Application/Authentication`: `IAuthenticationService` + `AuthenticationService` (preserves hardcoded `admin`/`1234`), `ITokenService` contract, and the `User` auth model (auth DTO, not a domain entity — nothing persists it).
- `Infrastructure/Auth`: `TokenGenerator` implements `ITokenService`; `JwtSettings` also lives here.
- Api binds the `JwtSettings` config section and registers the token service via `AddInfrastructure`.
Rationale: JWT signing is an infrastructure concern; Application depends only on the `ITokenService` contract.
Alternative: keep the static `TokenGenerator` — rejected (untestable seam).

### D7 — Namespace and project rename
- `HackerRank1.sln` → `LibraryService.sln`; `HackerRank1/HackerRank1.csproj` → `LibraryService.Api/LibraryService.Api.csproj`.
- Root namespaces: `LibraryService.Domain.Entities`, `LibraryService.Application.*`, `LibraryService.Infrastructure.*`, `LibraryService.Api.*`.
Rationale: one naming world; aligns with target structure. **BREAKING** only for in-repo references (tests).

### D8 — DI registration via per-layer extension methods
- `Application/DependencyInjection.cs`: `AddApplication(this IServiceCollection)` — registers application services.
- `Infrastructure/DependencyInjection.cs`: `AddInfrastructure(this IServiceCollection, IConfiguration)` — registers pooled `LibraryContext` (Npgsql + retry), repositories, `JwtSettings` (bound), `ITokenService`/`TokenGenerator`.
- `Startup` calls both and keeps its existing auth/CORS/Swagger/controllers wiring.
Alternative: minimal `Program` — rejected to minimize churn and because tests rely on `UseStartup<Startup>`.
Note: tests override the DbContext via `services.RemoveAll<LibraryContext>` + `AddSingleton` on the test host's `ConfigureServices` — this still works as long as repositories resolve `LibraryContext` purely from constructor injection.

### D9 — Tests authenticate before hitting protected endpoints
Books endpoints keep `[Authorize]` (preserves the security intent). Integration tests are updated to call `/login` (admin/1234) once per fixture and attach the bearer token to requests.
Alternative: drop `[Authorize]` so unauthenticated tests pass — rejected (weakens security).

### D10 — Test consolidation and package alignment
- Delete orphaned `IntegrationTest/`.
- Keep `LibraryService.Integration.Test` (net8, xunit); retarget references to `LibraryService.Api`; bump `Microsoft.AspNetCore.Mvc.Testing`, `Microsoft.EntityFrameworkCore.InMemory/Sqlite` to 8.x (currently 6.x against net8).

## Risks / Trade-offs

- **[Regenerated migration drifts from live Supabase]** → Model is unchanged, so the regenerated `InitialCreate` should be equivalent; verify with `dotnet ef migrations list` and a local smoke run before merging. Fallback: move the existing migration files instead.
- **[Tests break on auth changes]** → Keep `admin`/`1234` behavior identical and add a shared login helper in the test fixture.
- **[Namespace rename breaks the second test project]** → Only in-repo consumers exist; both test projects are handled in the same change and the orphan is deleted.
- **[Package version mismatch in tests]** → Bump all test packages to 8.x during consolidation.
- **[DbContext override no longer applies]** → Repositories must take `LibraryContext` only via constructor injection (D8) so the test's SQLite singleton replaces Postgres.
- **[Committed Supabase secret]** → Flagged for rotation outside this change; not introduced by this refactor.

## Migration Plan

1. Scaffold `Domain`, `Application`, `Infrastructure` projects; rename `HackerRank1` → `LibraryService.Api`; rebuild the solution.
2. Relocate entities, DbContext, migrations; add design-time factory; regenerate the initial migration and verify equivalence.
3. Implement repository interfaces + EF implementations; rewrite services; split auth (ITokenService/TokenGenerator); add DI extension methods.
4. Rewrite controllers and `Startup`; complete POST/DELETE routes and 404 semantics.
5. Update `LibraryService.Integration.Test` (namespaces, auth helper, package versions); delete orphaned `IntegrationTest/`.
6. `dotnet build` clean; `dotnet test` green (SQLite in-memory, no live DB needed).
7. Rollback = `git revert`; no schema/data migration risk since the model is unchanged.

## Open Questions

- Expose book `PUT`/`DELETE` routes now that `BooksService.Update/Delete` are implemented, or leave them service-only? Deferrable — no test or spec requires them.
- Keep `POST /api/libraries` returning `200` (current behavior) or standardize on `201`? Deferrable — no test asserts it.

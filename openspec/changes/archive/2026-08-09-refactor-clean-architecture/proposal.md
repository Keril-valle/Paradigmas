## Why

The solution is a single monolithic ASP.NET Core project (`HackerRank1`) that mixes presentation, application logic, persistence, and domain entities in one assembly. Namespaces are split between `HackerRank1.*` and `LibraryService.WebAPI.*`, entities live inside `LibraryContext.cs`, services are coupled directly to EF Core, several CRUD methods throw `NotImplementedException`, and the earlier layering attempt left behind ghost `LibraryService.*` folders with only build artifacts. This makes the system hard to test, hard to evolve, and misaligned with the course's layered-architecture goals.

## What Changes

- Restructure into four projects following .NET Clean Architecture best practices:
  `LibraryService.Domain`, `LibraryService.Application`, `LibraryService.Infrastructure`, `LibraryService.Api` (renamed from `HackerRank1`).
- Enforce the dependency rule: **Domain** (no dependencies) ← **Application** (depends on Domain) ← **Infrastructure** (depends on Application + Domain) ← **Api** (depends on all).
- Move entities (`Book`, `Library`) to Domain; `DbContext` + migrations to Infrastructure; use-case services, repository contracts, and DTOs to Application; controllers and host wiring (`Startup`/`Program`) to Api.
- Introduce the repository pattern: Application defines `IBookRepository`/`ILibraryRepository`; Infrastructure implements them with EF Core. Application services no longer touch `DbContext` directly.
- Replace the static `TokenGenerator` with an `ITokenService` contract in Application, implemented in Infrastructure. JWT flow preserved.
- Unify namespaces under `LibraryService.*` (drop `HackerRank1`). **BREAKING** for anything referencing the old namespaces (only the in-repo test project).
- Complete the stubbed CRUD paths: `BooksService.Add/Update/Delete`, `LibrariesService.Delete`; add `POST /books` and `DELETE /libraries/{id}` actions with 404 handling so the integration tests (which already assert this contract) pass.
- Consolidate to a single integration test project (`LibraryService.Integration.Test`); remove the orphaned `IntegrationTest` project.
- Clean up the ghost `LibraryService.*` folders (stray `.csproj.user`, leftover `bin/obj`) as they become real projects.
- Rename the solution `HackerRank1.sln` → `LibraryService.sln`.

## Capabilities

### New Capabilities
- `library-catalog`: management of libraries and books via the public HTTP API — list/get/create/update/delete for libraries, nested books routes, and the 404 semantics for missing libraries.
- `authentication`: JWT-based login via `POST /login`, credential validation, and token issuance used to secure the books endpoints.

### Modified Capabilities
- None — there are no existing specs yet.

## Impact

- **Projects**: `HackerRank1/` becomes `LibraryService.Api/`; new `LibraryService.Domain`, `LibraryService.Application`, `LibraryService.Infrastructure` projects are created; orphaned `IntegrationTest/` is removed; solution renamed.
- **Namespaces/assemblies**: all code moves to `LibraryService.*` namespaces; project references updated accordingly.
- **External API surface**: existing routes, auth, CORS, and Swagger behavior preserved; `POST /api/libraries/{id}/books` and `DELETE /api/libraries/{id}` become functional (previously missing/stubbed) to match the integration tests.
- **Persistence**: EF Core `DbContext` and migrations move to Infrastructure (migrations regenerated or retargeted); Postgres/Supabase connection unchanged.
- **Dependencies**: Domain has no packages; Application uses Domain only; Infrastructure brings EF Core + Npgsql + JWT; Api brings host + Swagger + Newtonsoft.
- **Known issue (not in scope)**: the Supabase password is committed in `appsettings.Development.json`; flagged in risks, no credential rotation in this change.

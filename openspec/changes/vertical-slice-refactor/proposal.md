## Why

The solution is a single monolithic ASP.NET Core project (`HackerRank1`) where presentation, use-case logic, persistence, and domain entities are mixed together in one assembly with inconsistent namespaces (`HackerRank1.*` vs `LibraryService.WebAPI.*`), `Book`/`Library` entities trapped inside `LibraryContext.cs`, services coupled directly to EF Core, and several CRUD methods still throwing `NotImplementedException`. The course has already demonstrated layered and clean architectures (sibling branches); this change restructures the same app into a **vertical slice** so feature code is organized end-to-end instead of by technical layer.

## What Changes

- Restructure into a **single web project** reorganized around `Features/`: each feature slice is self-contained and independent — it owns its request DTO, its handler, and its data access.
- Introduce a **hand-rolled command/query handler pattern** (`IQueryHandler<...>` / `ICommandHandler<...>`), implemented per feature, without adding MediatR.
- Move shared, cross-cutting concerns into a **Shared kernel** (`Common`/`Shared`): `LibraryContext`, entities (`Book`, `Library`), `JwtSettings`, `TokenGenerator`, DI registration — the only code all slices depend on.
- Each slice self-registers its own handlers via a per-slice module, so `Startup.cs` calls `AddFeatures()` / each `FeatureModule` instead of enumerating every service.
- Unify namespaces under `LibraryService.*` (drop `HackerRank1`). **BREAKING** for anything referencing the old namespaces (only the in-repo test project).
- Complete the stubbed CRUD paths within their slices: books `Add`, library `Delete`; expose `POST /books` and `DELETE /libraries/{id}` with 404 handling so the integration tests (which already assert this contract) pass.
- Update `LibraryService.Integration.Test` (namespaces, auth helper, package versions) and remove the orphaned `IntegrationTest/` project.
- Rename the solution `HackerRank1.sln` → `LibraryService.sln`.
- Clean up the ghost `LibraryService.*` folders (stray `.csproj.user`, leftover `bin/obj`).

## Capabilities

### New Capabilities
- `library-catalog`: management of libraries and books via the public HTTP API — list/get/create/update/delete for libraries, nested books routes, and the 404 semantics for missing libraries.
- `authentication`: JWT-based login via `POST /login`, credential validation, and token issuance used to secure the books endpoints.

### Modified Capabilities
- None — there are no existing specs yet.

## Impact

- **Projects**: `HackerRank1/` becomes a single reorganized API project (`LibraryService.Api`, web SDK); orphaned `IntegrationTest/` removed; solution renamed.
- **Namespaces/assemblies**: all code moves to `LibraryService.*` namespaces; project references updated accordingly.
- **External API surface**: existing routes, auth, CORS, and Swagger behavior preserved; `POST /api/libraries/{id}/books` and `DELETE /api/libraries/{id}` become functional (previously missing/stubbed) to match the integration tests.
- **Persistence**: EF Core `DbContext` and migrations stay in the shared kernel; Postgres/Supabase connection unchanged.
- **Dependencies**: no new packages for the handler pattern (hand-rolled); existing EF Core + Npgsql + JWT + Swagger + Newtonsoft stay.
- **Known issue (not in scope)**: the Supabase password is committed in `appsettings.Development.json`; flagged in risks, no credential rotation in this change.
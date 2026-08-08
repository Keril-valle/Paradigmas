## Why

The current solution is a single monolithic `HackerRank1` project where controllers, EF data access, services, and JWT auth all live together, several service methods are `NotImplementedException` stubs, and `BooksController` is missing its POST endpoint. This violates .NET N-layer separation-of-concerns best practices and leaves the API functionally incomplete.

## What Changes

- **BREAKING** Restructure the solution into separate .NET projects per layer following classic N-layer architecture:
  - `LibraryService.Domain` — entities, DTOs, interfaces
  - `LibraryService.Application` — business/service logic and use cases
  - `LibraryService.Infrastructure` — EF `DbContext` and repository/persistence
  - `LibraryService.Api` — controllers and startup (presentation)
- Add project references between layers (Api → Application → Domain; Infrastructure → Domain).
- Register the new layers with Dependency Injection and wire up EF (Npgsql Postgres) in the Api/Infrastructure boundary.
- **Finish the endpoints** that are currently stubbed or missing:
  - Complete `BooksService.Add` → supports `POST api/libraries/{libraryId}/books` returning `201`, and `404` when the library does not exist.
  - Complete `LibrariesService.Delete` → supports `DELETE api/libraries/{libraryId}` returning `204` on success and `404` when the library does not exist.
  - Add the missing `DELETE` action to the libraries controller and the missing `POST` action to the books controller.
- Keep the existing JWT auth flow and the `GET api/libraries/{libraryId}/books` behavior (200 with the book list, 404 on missing library).
- **BREAKING** Replace the current `Library`-entity binding in controllers with DTO form models.
- Remove the single-project layer-by-folder mixing so each layer owns its responsibility.

## Capabilities

### New Capabilities
- `library-management`: The HTTP contract for the library API — listing libraries, retrieving a library, adding/updating/deleting a library, and listing/adding books within a library, with the required status codes (200, 201, 204, 404) and JWT authorization.

### Modified Capabilities
<!-- None. No existing specs exist in this repo. -->

## Impact

- Solution file: add new project entries; result is multiple projects instead of one.
- `HackerRank1.csproj`: split/replaced by the layer projects; namespace changes from `HackerRank1.*` / `LibraryService.WebAPI.*` to per-layer namespaces.
- Files: entities, DTOs, `LibraryContext`, services, controllers, `Startup`/`Program`, `JwtSettings`, `TokenGenerator`, `AuthenticationService` are redistributed into the new layer projects.
- Migration files: EF migrations move to `Infrastructure`.
- Integration test project: updated project reference (references `HackerRank1` today) and updated to hit the non-todo new Api.
- No new external dependencies beyond those for the chosen projects (AutoMapper optional for DTO mapping).
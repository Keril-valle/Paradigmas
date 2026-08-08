## Context

Today the solution is a single web project (`HackerRank1` / `LibraryService.WebAPI`) mixing controllers, services, EF data access and entity classes, JWT helpers, DTOs, and migrations in one assembly (see proposal.md - Why, Impact). The codebase is incomplete: `BooksService.Add/Update/Delete` and `LibrariesService.Delete` throw `NotImplementedException`, and the books POST and library DELETE actions are absent. The decision was made to adopt classic .NET N-layer architecture as separate projects. Motivations are in proposal.md - Why.

## Goals / Non-Goals

**Goals:**
- Separate the solution into distinct projects per architectural layer (Domain, Application, Infrastructure, Api) with unidirectional references.
- Keep the externally observable HTTP contract unchanged except for completing the missing endpoints and standardizing on DTO-form input binding.
- Enable unit testing of application logic independent of the database.
- Remove the missing/stubbed endpoint behavior (DELETE library, POST book).

**Non-Goals:**
- Changing the authentication mechanism (hardcoded `admin`/`1234` credentials and JWT flow stay as-is).
- Introducing a new framework or ORM; EF Core + Npgsql stays.
- Introducing AutoMapper as a hard dependency (manual mapping is used to keep the change focused).
- Changing the HTTP status code contract beyond what is currently specified.

## Decisions

### 1. Four projects, one per layer
- `LibraryService.Domain` (class library): entities (`Library`, `Book`), DTOs (`LibraryForm`, `BookForm`, `User`), configuration type (`JwtSettings`), and the service/repository interfaces. No EF or ASP.NET Core package references.
- `LibraryService.Application` (class library, refs Domain): business/services implementations (`LibrariesService`, `BooksService`, `AuthenticationService`) conversing through interfaces to the Domain. Holds no database context.
- `LibraryService.Infrastructure` (class library, refs Domain): EF `LibraryContext` (the single `DbContext`), EF migrations, and repository implementations for Domain repository interfaces.
- `LibraryService.Api` (web project, refs Application + Infrastructure + Domain): controllers, `Program`/`Startup`, and composition root (DI wiring, auth, CORS, Swagger, app settings).

**Rationale:** classic N-layer enforces that the presentation layer depends on application and infrastructure, application depends on domain, and infrastructure depends on domain; the core (Domain) stays independent. Alternatives considered: a single project with folders (rejected: does not enforce compile-time boundaries) and a Clean/Hexagonal split exporting ports/adapters (heavier than needed for this scope).

### 2. Repository interfaces in Domain, EF implementations in Infrastructure
Adding `ILibraryRepository` and `IBookRepository` in Domain and concretes in Infrastructure gives tests a seam to fake persistence and keeps `DbContext` out of `Application`. `LibrariesService`/`BooksService` depend on these interfaces.

### 3. Controllers bind DTO forms; services operate on entities
Controllers accept `LibraryForm`/`BookForm` (and `User` for login) instead of binding the entity. Services accept DTO input, map to `Library`/`Book`, persist, and return entities (or the created id) back up; the controller maps to the response. This standardizes the API boundary per the spec while leaving `GET` book/library list shapes effectively unchanged so existing clients keep working.

### 4. DI wiring in the Api composition root
`Startup` registers `AddTransient`/`AddScoped` for services and repositories, `AddDbContextPool(&lt;LibraryContext&gt;)` pointing at the configured Npgsql connection string, JWT bearer auth, CORS (frontend origin `http://localhost:5173`), Swagger, and `controllers`. `DbContext` lives in `Infrastructure`, so `Startup` references `Infrastructure` only through composition.

### 5. Auth pairing
`AuthenticationService` (Application) retains the hardcoded credential check. `TokenGenerator` moves to `Infrastructure` (or `Api` helper) and is registered as a service/DTO. `AuthController` issues the JWT. Book/library endpoints remain public (no `[Authorize]`), per the decision to keep the provided unauthenticated integration tests passing.

## Risks / Trade-offs

- **Split-brain namespaces**: `HackerRank1.*` and `LibraryService.WebAPI.*` are currently mixed. New projects use consistent per-layer namespaces (`LibraryService.Domain`, `LibraryService.Application`, `LibraryService.Infrastructure`, `LibraryService.Api`). → Mitigation: update all `using` directives during the move; compiler will surface stragglers.
- **Test project compatibility**: the integration test targets `HackerRank1` today and mixes 6.0/8.0 test packages. → Mitigation: point the test's project reference at the new `Api` project and align test packages (Mvc.Testing / EF InMemory / Sqlite) to 8.0. `WebApplicationFactory&lt;Program&gt;` needs `Program` public (`LibraryService.Api`).
- **Missed behavior during migration**: renaming namespaces / moving files risks typos. → Mitigation: run `dotnet build` and the integration suite after each layer moves.
- **Manual DTO mapping** adds boilerplate but avoids a new dependency. → Acceptable for the scope; map via small extension/private methods.

## Migration Plan

1. Create the four project scaffolding / solution entries and references.
2. Move entities, DTOs, `JwtSettings` and interfaces into `Domain`; services into `Application`; `LibraryContext`, repositories and migrations into `Infrastructure`; controllers, `Program`/`Startup`, and app settings into `Api`. Edit namespaces.
3. Rewire `Startup` DI for the new services, repositories and the Infrastructure `DbContext`.
4. Complete `BooksService.Add/Update/Delete` and `LibrariesService.Delete` (formerly `NotImplementedException`) so the finishing endpoints return the specified codes.
5. Update the integration test project references and package versions; run `dotnet test`.
6. `dotnet build` and `dotnet test` to verify green.

Rollback: since this is a structural refactor, the previous single-project state is recoverable via the existing git history (branch `lab1/layers`).

## Open Questions

- Whether the `LibraryController` (singular) vs `LibrariesController` (plural) route/class naming should be kept as-is. Deferred; default to preserving current names.
- Exact location for `TokenGenerator` (Infrastructure vs Api). Chosen: Infrastructure.
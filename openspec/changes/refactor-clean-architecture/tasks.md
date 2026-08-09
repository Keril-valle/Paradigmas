## 1. Project Scaffolding and Solution Restructure

- [x] 1.1 Create classlib projects `LibraryService.Domain`, `LibraryService.Application`, `LibraryService.Infrastructure` (net8.0)
- [x] 1.2 Rename `HackerRank1/` → `LibraryService.Api/` (`HackerRank1.csproj` → `LibraryService.Api.csproj`, web SDK)
- [x] 1.3 Rename `HackerRank1.sln` → `LibraryService.sln` and update project references in the solution file
- [x] 1.4 Delete ghost leftovers in the `LibraryService.*` folders (stray `LibraryService.Api.csproj.user`, stale `bin/obj`)
- [x] 1.5 Wire project references per the dependency rule (D1): Api → Application + Infrastructure; Infrastructure → Application + Domain; Application → Domain
- [x] 1.6 `dotnet build` the skeleton solution successfully

## 2. Domain Layer

- [x] 2.1 Move `Book` and `Library` entities from `LibraryContext.cs` into `LibraryService.Domain/Entities/` (namespace `LibraryService.Domain.Entities`)
- [x] 2.2 Confirm `LibraryService.Domain` has no package references and no dependencies

## 3. Infrastructure Layer

- [x] 3.1 Move `LibraryContext` to `LibraryService.Infrastructure/Data/` (namespace `LibraryService.Infrastructure.Data`)
- [x] 3.2 Add `LibraryContextFactory` design-time factory (derives `IDesignTimeDbContextFactory<LibraryContext>`)
- [x] 3.3 Regenerate the `InitialCreate` migration under `Infrastructure/Migrations/` and verify equivalence against the applied one (`dotnet ef migrations list`)
- [x] 3.4 Add `BookRepository` and `LibraryRepository` to `Infrastructure/Persistence/` implementing the Application repository interfaces over EF Core (constructor-injected `LibraryContext` only)
- [x] 3.5 Move `JwtSettings` and `TokenGenerator` to `Infrastructure/Auth/`; `TokenGenerator` implements `ITokenService`
- [x] 3.6 Add `DependencyInjection.cs` with `AddInfrastructure(this IServiceCollection, IConfiguration)`: pooled `LibraryContext` (Npgsql + `EnableRetryOnFailure`), repositories, `JwtSettings` binding, `ITokenService`/`TokenGenerator`
- [x] 3.7 Add EF Core, Npgsql, and JWT package references to `LibraryService.Infrastructure`

## 4. Application Layer

- [x] 4.1 Add `IBookRepository` and `ILibraryRepository` interfaces to `Application/Interfaces/`
- [x] 4.2 Add `IAuthenticationService`, `ITokenService`, and the `User` auth model to `Application/Authentication/`
- [x] 4.3 Rewrite `LibrariesService` to depend on `ILibraryRepository`; fully implement `Get/Add/Update/Delete` (delete → 204 on success, 404 when absent)
- [x] 4.4 Rewrite `BooksService` to depend on `IBookRepository`; fully implement `Get/Add/Update/Delete` with library-existence checks for the 404 paths
- [x] 4.5 Add `DependencyInjection.cs` with `AddApplication(this IServiceCollection)` registering application services
- [x] 4.6 Confirm `LibraryService.Application` references only `LibraryService.Domain`

## 5. API Layer (Presentation)

- [x] 5.1 Update `LibraryService.Api` root namespace/assembly to `LibraryService.Api`
- [x] 5.2 Move `BookForm`, `LibraryForm`, `TokenResponse` to `Api/DTOs/` as plain classes (drop Newtonsoft `JsonProperty` attributes)
- [x] 5.3 Rewrite `LibrariesController`: GET list (200), GET by id (200/404), POST (200), PUT (204/404), DELETE (204/404) per the `library-catalog` spec
- [x] 5.4 Rewrite `BooksController`: GET books (auth, 200/404), POST books (auth, 201/404) per the `library-catalog` spec
- [x] 5.5 Rewrite `AuthController`: `POST /login` (200 with token / 401) via `IAuthenticationService` + `ITokenService` per the `authentication` spec
- [x] 5.6 Rewrite `Startup.cs`/`Program.cs`: call `AddApplication` + `AddInfrastructure(configuration)`, keep JwtBearer validation, `AddAuthorization`, CORS, Swagger, controllers; `Program` stays public for `WebApplicationFactory`
- [x] 5.7 Move `appsettings*.json` into `LibraryService.Api` and point them at the same Supabase connection

## 6. Test Consolidation

- [ ] 6.1 Delete the orphaned `IntegrationTest/` project
- [ ] 6.2 Update `LibraryService.Integration.Test`: reference `LibraryService.Api`, retarget namespaces to `LibraryService.*`, bump `Microsoft.AspNetCore.Mvc.Testing` and EF InMemory/Sqlite packages to 8.x
- [ ] 6.3 Add a login helper to the test fixture that obtains a token via `/login` (admin/1234) and attaches the bearer header (D9)
- [ ] 6.4 Update integration tests to cover the `library-catalog` scenarios: POST books → 201/404, GET books → 200/404/401, DELETE library → 204/404

## 7. Verification

- [ ] 7.1 `dotnet build` clean across the solution
- [ ] 7.2 `dotnet test` green (SQLite in-memory, no live DB required)
- [ ] 7.3 Grep confirms no remaining `HackerRank1` namespace/assembly references
- [ ] 7.4 `openspec validate --change refactor-clean-architecture` passes

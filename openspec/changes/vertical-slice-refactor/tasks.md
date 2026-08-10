## 1. Project Scaffolding and Shared Kernel

- [x] 1.1 Rename `HackerRank1/` → `LibraryService.Api/` (`HackerRank1.csproj` → `LibraryService.Api.csproj`, web SDK) and update the project entry in `HackerRank1.sln` (D8)
- [x] 1.2 Rename `HackerRank1.sln` → `LibraryService.sln`
- [x] 1.3 Delete ghost leftovers in the `LibraryService.*` folders (stray `LibraryService.Api.csproj.user`, stale `bin/obj`)
- [x] 1.4 Move `Book` and `Library` entities from `LibraryContext.cs` into `Common/Entities/` (namespace `LibraryService.Api.Common.Entities`) (D4)
- [x] 1.5 Move `LibraryContext` to `Common/Data/` (namespace `LibraryService.Api.Common.Data`); update `DbSet`/entity visibility
- [x] 1.6 Add `LibraryContextFactory` design-time factory (derives `IDesignTimeDbContextFactory<LibraryContext>`) and move migrations under `Common/Data/Migrations/` (D4)
- [x] 1.7 Regenerate the `InitialCreate` migration and verify equivalence against the applied one (`dotnet ef migrations list`)
- [x] 1.8 Add handler contracts to `Common/Handlers/` (namespace `LibraryService.Api.Common.Handlers`): `IQueryHandler<TQuery, TResult>` and `ICommandHandler<TCommand, TResult>` (D2)

## 2. Shared Auth Plumbing

- [x] 2.1 Move `JwtSettings` and `TokenGenerator` into `Common/Auth/` (namespaces `LibraryService.Api.Common.Auth`; keep `TokenGenerator` signatures so the JWT bearer middleware validation stays compatible) (D7)
- [x] 2.2 Add `Common/DependencyInjection.cs` with `AddShared(this IServiceCollection, IConfiguration)`: pooled `LibraryContext` (Npgsql + `EnableRetryOnFailure`), `JwtSettings` binding, `TokenGenerator` (D9)
- [x] 2.3 Confirm the API project has all required package references (EF Core, Npgsql, JwtBearer, Swashbuckle, Newtonsoft) under `LibraryService.Api.csproj`

## 3. Authentication Feature Slice

- [x] 3.1 Create `Features/Authentication/Login/`: `LoginController` (POST `/login`), `LoginQuery`, `LoginHandler` (validates `admin`/`1234`, returns a `LoginResponse`) (D7)
- [x] 3.2 Have `LoginHandler` generate the token via the shared `TokenGenerator`; respond `200 OK` with `{ token }` or `401 Unauthorized` per the `authentication` spec
- [x] 3.3 Add `AuthenticationModule.AddAuthenticationFeatures(this IServiceCollection)` registering the login handler (D3)

## 4. Library Feature Slices

- [x] 4.1 Create `Features/Libraries/GetLibraries/`: `GetLibrariesController` (GET `/api/libraries`, 200 with array) + `GetLibrariesQuery`/handler
- [x] 4.2 Create `Features/Libraries/GetLibraryById/`: handler + controller (GET `/api/libraries/{id}`, 200 / 404)
- [x] 4.3 Create `Features/Libraries/CreateLibrary/`: handler + controller (POST `/api/libraries`, 200 with created library)
- [x] 4.4 Create `Features/Libraries/UpdateLibrary/`: handler + controller (PUT `/api/libraries/{id}`, 204 / 404)
- [x] 4.5 Create `Features/Libraries/DeleteLibrary/`: handler + controller (DELETE `/api/libraries/{id}`, 204 / 404); complete the previously stubbed delete, removing the library only when it exists (library-catalog spec)
- [x] 4.6 Add `LibrariesModule.AddLibrariesFeatures(this IServiceCollection)` registering the library slice handlers (D3)

## 5. Books Feature Slices

- [x] 5.1 Create `Features/Books/GetBooksForLibrary/`: handler + `GetBooksController` (GET `/api/libraries/{libraryId}/books`, `[Authorize]`, 200 with array or 404 when the library is missing) (library-catalog spec)
- [x] 5.2 Create `Features/Books/AddBookToLibrary/`: handler + controller (POST `/api/libraries/{libraryId}/books`, `[Authorize]`, 201 with created book or 404 when the library is missing); complete the previously stubbed book add (library-catalog spec)
- [x] 5.3 Add `BooksModule.AddBooksFeatures(this IServiceCollection)` registering the books slice handlers (D3)
- [x] 5.4 Confirm a shared `LibraryExists` check is used consistently for the 404 paths (via `LibraryContext` lookup inside a handler or exported context helper)

## 6. Composition and Host Wiring

- [x] 6.1 Rewrite `Startup.cs`/`Program.cs`: call `AddShared(configuration)` then each feature module; keep JwtBearer validation, `AddAuthorization`, CORS, Swagger, controllers (D9)
- [x] 6.2 Keep `Program` public for `WebApplicationFactory` and keep `UseStartup<Startup>` so the integration tests host correctly (D9)
- [x] 6.3 Move `appsettings*.json` into `LibraryService.Api` and point them at the same Supabase connection
- [x] 6.4 `dotnet build` the solution clean

## 7. Test Consolidation

- [x] 7.1 Delete the orphaned `IntegrationTest/` project
- [x] 7.2 Update `LibraryService.Integration.Test`: reference `LibraryService.Api`, retarget namespaces to `LibraryService.Api.*`, bump `Microsoft.AspNetCore.Mvc.Testing` and EF InMemory/Sqlite packages to 8.x (D11)
- [x] 7.3 Add a login helper to the test fixture that obtains a token via `/login` (admin/1234) and attaches the bearer header (D10)
- [x] 7.4 Cover the library-catalog scenarios: POST books → 201/404, GET books → 200/404/401, DELETE library → 204/404 (matching the prior green suite)

## 8. Verification

- [x] 8.1 `dotnet build` clean across the solution
- [x] 8.2 `dotnet test` green (SQLite in-memory, no live DB required)
- [x] 8.3 Grep confirms no remaining `HackerRank1` namespace/assembly references
- [x] 8.4 `openspec validate --change vertical-slice-refactor` passes
## 1. Scaffold the N-layer solution

- [x] 1.1 Create `LibraryService.Domain` class library project and add to `HackerRank1.sln`
- [x] 1.2 Create `LibraryService.Application` class library project (references Domain) and add to solution
- [x] 1.3 Create `LibraryService.Infrastructure` class library project (references Domain) and add to solution
- [x] 1.4 Create `LibraryService.Api` web project (references Application + Infrastructure + Domain) and add to solution
- [x] 1.5 Add EF Core / Npgsql package references to `LibraryService.Infrastructure`
- [x] 1.6 Add JWT Bearer + Swashbuckle package references to `LibraryService.Api`
- [x] 1.7 Remove the old `HackerRank1` project from the solution (after its contents are relocated)

## 2. Move code into Domain

- [x] 2.1 Move entities `Library` and `Book` into `LibraryService.Domain` with namespace `LibraryService.Domain`
- [x] 2.2 Move DTOs `LibraryForm`, `BookForm`, `User` into `LibraryService.Domain` with consistent namespace
- [x] 2.3 Move `JwtSettings` into `LibraryService.Domain`
- [x] 2.4 Define service interfaces `ILibrariesService`, `IBooksService`, `IAuthenticationService` in `LibraryService.Domain`
- [x] 2.5 Define repository interfaces `ILibraryRepository`, `IBookRepository` in `LibraryService.Domain`
- [x] 2.6 Verify Domain compiles with no EF / ASP.NET Core package references

## 3. Move code into Infrastructure

- [x] 3.1 Move `LibraryContext` (with `DbSet<Library>`, `DbSet<Book>`) into `LibraryService.Infrastructure`
- [x] 3.2 Move EF migrations and the model snapshot into `LibraryService.Infrastructure`
- [x] 3.3 Implement `LibraryRepository` and `BookRepository` in `LibraryService.Infrastructure` against the repository interfaces
- [x] 3.4 Move `TokenGenerator` into `LibraryService.Infrastructure`
- [x] 3.5 Verify Infrastructure compiles

## 4. Move code into Application

- [x] 4.1 Move `LibrariesService` into `LibraryService.Application` implementing `ILibrariesService` against `ILibraryRepository`
- [x] 4.2 Move `BooksService` into `LibraryService.Application` implementing `IBooksService` against `IBookRepository`
- [x] 4.3 Move `AuthenticationService` into `LibraryService.Application` (hardcoded credentials preserved)
- [x] 4.4 Complete `LibrariesService.Delete` (remove the `NotImplementedException`)
- [x] 4.5 Complete `BooksService.Add`, `BooksService.Update`, `BooksService.Delete` (remove the `NotImplementedException` stubs)
- [x] 4.6 Verify Application compiles with no database context

## 5. Build the Api project

- [x] 5.1 Move controllers `LibrariesController`, `BooksController`, `AuthController` into `LibraryService.Api`
- [x] 5.2 Move `Program`/`Startup` and appsettings files into `LibraryService.Api`
- [x] 5.3 Add the missing `DELETE api/libraries/{libraryId}` action to `LibrariesController` (204 on success, 404 when missing)
- [x] 5.4 Add the missing `POST api/libraries/{libraryId}/books` action to `BooksController` (201 on success, 404 when library missing)
- [x] 5.5 Change controllers to bind DTO forms (`LibraryForm`, `BookForm`, `User`) instead of entities
- [x] 5.6 Wire DI in `Startup`: register repositories and services, `AddDbContextPool(LibraryContext)`, keep JWT auth, CORS, and Swagger
- [x] 5.7 Confirm books endpoints have no `[Authorize]` attribute (public), matching the decision
- [x] 5.8 Ensure `Program` is public for the test host and build the whole solution green

## 6. Update the integration test project

- [x] 6.1 Point `LibraryService.Integration.Test` project reference at `LibraryService.Api` instead of the old project
- [x] 6.2 Align test package versions (Mvc.Testing, EF InMemory/Sqlite) with net8.0
- [x] 6.3 Update test usings/namespaces to the new per-layer namespaces
- [x] 6.4 Run `dotnet build` and `dotnet test`; fix any failures until the suite is green

## 7. Verify

- [x] 7.1 `dotnet build` the full solution with no errors
- [x] 7.2 `dotnet test` passes the integration suite (GET/POST books, DELETE library, 404 paths)
- [x] 7.3 Manually smoke-test `/login`, `GET /api/libraries`, and `GET /api/libraries/{id}` via Swagger








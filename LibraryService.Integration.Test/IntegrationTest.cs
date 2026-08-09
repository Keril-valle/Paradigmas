using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using LibraryService.Api;
using LibraryService.Api.DTOs;
using LibraryService.Domain.Entities;
using LibraryService.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Xunit;

namespace LibraryService.Tests
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _configuredFactory;
        private readonly LibraryContext context;
        private HttpClient? _authClient;

        public HttpClient Client { get; private set; }

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            context = new LibraryContext(new DbContextOptionsBuilder<LibraryContext>()
                        .UseSqlite("DataSource=:memory:")
                        .EnableSensitiveDataLogging()
                        .Options);

            _configuredFactory = factory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll(typeof(LibraryContext));
                    services.AddSingleton(context);

                    context.Database.OpenConnection();
                })
            );

            Client = _configuredFactory.CreateClient();
        }

        private async Task ResetDatabase()
        {
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Books");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Libraries");
            context.ChangeTracker.Clear();
        }

        private async Task SeedLibrary()
        {
            var libraries = new List<Library>
            {
                new Library { Name = "Library Name 1", Location = "Location 1" },
                new Library { Name = "Library Name 2", Location = "Location 2" },
                new Library { Name = "Library Name 3", Location = "Location 3" },
                new Library { Name = "Library Name 4", Location = "Location 4" }
            };

            await context.Libraries.AddRangeAsync(libraries);
            await context.SaveChangesAsync();
        }

        private async Task SeedBook(HttpClient client, string bookName, int libraryId)
        {
            var bookForm = new BookForm
            {
                Name = bookName
            };

            await client.PostAsync($"/api/libraries/{libraryId}/books",
                new StringContent(JsonConvert.SerializeObject(bookForm), Encoding.UTF8, "application/json"));
        }

        private async Task<string> GetTokenAsync()
        {
            var response = await Client.PostAsync("/login",
                new StringContent(JsonConvert.SerializeObject(new { email = "admin", password = "1234" }), Encoding.UTF8, "application/json"));

            response.StatusCode.Should().BeEquivalentTo(StatusCodes.Status200OK);

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LoginResponse>(body)!.token;
        }

        private async Task<HttpClient> GetAuthenticatedClientAsync()
        {
            if (_authClient is null)
            {
                var token = await GetTokenAsync();
                _authClient = _configuredFactory.CreateClient();
                _authClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return _authClient;
        }

        // TEST NAME - addBookToLibrary
        // TEST DESCRIPTION - It adds a book to a library (201) and returns 404 for a missing library
        [Fact]
        public async Task TestAddBook_Ok_GetBook_NotFound()
        {
            await ResetDatabase();
            await SeedLibrary();
            var authClient = await GetAuthenticatedClientAsync();

            var bookForm = new BookForm
            {
                Name = "Test book 1"
            };

            var response1 = await authClient.PostAsync("/api/libraries/1/books",
                new StringContent(JsonConvert.SerializeObject(bookForm), Encoding.UTF8, "application/json"));

            response1.StatusCode.Should().BeEquivalentTo(StatusCodes.Status201Created);

            bookForm = new BookForm
            {
                Name = "Test book 2"
            };

            var response2 = await authClient.PostAsync("/api/libraries/100/books",
                new StringContent(JsonConvert.SerializeObject(bookForm), Encoding.UTF8, "application/json"));

            response2.StatusCode.Should().BeEquivalentTo(StatusCodes.Status404NotFound);
        }

        // TEST NAME - getBooksInALibrary
        // TEST DESCRIPTION - It finds all books in a library by ID
        [Fact]
        public async Task TestGetBooks_Ok_NotFound()
        {
            await ResetDatabase();
            await SeedLibrary();
            var authClient = await GetAuthenticatedClientAsync();

            await SeedBook(authClient, "test book 1", 1);
            await SeedBook(authClient, "test book 2", 1);

            var response1 = await authClient.GetAsync($"/api/libraries/2/books");
            response1.StatusCode.Should().BeEquivalentTo(StatusCodes.Status200OK);
            var books = JsonConvert.DeserializeObject<IEnumerable<Book>>(await response1.Content.ReadAsStringAsync()).ToList();
            books.Count.Should().Be(0);

            var response2 = await authClient.GetAsync($"/api/libraries/1/books");
            response2.StatusCode.Should().BeEquivalentTo(StatusCodes.Status200OK);
            var books2 = JsonConvert.DeserializeObject<IEnumerable<Book>>(await response2.Content.ReadAsStringAsync()).ToList();
            books2.Count.Should().Be(2);

            var response3 = await authClient.GetAsync($"/api/libraries/31232/books");
            response3.StatusCode.Should().BeEquivalentTo(StatusCodes.Status404NotFound);
        }

        // TEST NAME - getBooksWithoutToken
        // TEST DESCRIPTION - Protected book endpoints reject unauthenticated requests
        [Fact]
        public async Task TestGetBooks_Unauthorized()
        {
            await ResetDatabase();
            await SeedLibrary();

            var response = await Client.GetAsync($"/api/libraries/1/books");
            response.StatusCode.Should().BeEquivalentTo(StatusCodes.Status401Unauthorized);
        }

        // TEST NAME - deleteLibraryById
        // TEST DESCRIPTION - Check delete library web api end point
        [Fact]
        public async Task TestDeleteLibrary()
        {
            await ResetDatabase();
            await SeedLibrary();
            var authClient = await GetAuthenticatedClientAsync();

            var bookForm = new BookForm
            {
                Name = "test book 1"
            };

            // add book to library
            var response0 = await authClient.PostAsync("/api/libraries/1/books",
                new StringContent(JsonConvert.SerializeObject(bookForm), Encoding.UTF8, "application/json"));
            response0.StatusCode.Should().BeEquivalentTo(StatusCodes.Status201Created);

            // delete library
            var response1 = await authClient.DeleteAsync("/api/libraries/1");
            response1.StatusCode.Should().BeEquivalentTo(StatusCodes.Status204NoContent);

            // verify that delete is successful
            var response2 = await authClient.GetAsync("/api/libraries/1/books");
            response2.StatusCode.Should().BeEquivalentTo(StatusCodes.Status404NotFound);

            var response3 = await authClient.DeleteAsync("/api/libraries/1");
            response3.StatusCode.Should().BeEquivalentTo(StatusCodes.Status404NotFound);
        }

        private class LoginResponse
        {
            public string token { get; set; }
        }
    }
}

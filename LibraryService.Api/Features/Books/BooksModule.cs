using LibraryService.Api.Common.Entities;
using LibraryService.Api.Common.Handlers;
using LibraryService.Api.Features.Books.AddBookToLibrary;
using LibraryService.Api.Features.Books.GetBooksForLibrary;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryService.Api.Features.Books;

public static class BooksModule
{
    public static IServiceCollection AddBooksFeatures(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetBooksForLibraryQuery, IEnumerable<Book>?>, GetBooksForLibraryHandler>();
        services.AddScoped<ICommandHandler<AddBookToLibraryCommand, Book?>, AddBookToLibraryHandler>();
        return services;
    }
}
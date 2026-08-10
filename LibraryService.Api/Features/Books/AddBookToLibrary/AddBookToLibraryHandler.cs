using LibraryService.Api.Common.Data;
using LibraryService.Api.Common.Entities;
using LibraryService.Api.Common.Handlers;

namespace LibraryService.Api.Features.Books.AddBookToLibrary;

public class AddBookToLibraryHandler : ICommandHandler<AddBookToLibraryCommand, Book?>
{
    private readonly LibraryContext _context;

    public AddBookToLibraryHandler(LibraryContext context)
    {
        _context = context;
    }

    public async Task<Book?> HandleAsync(AddBookToLibraryCommand command, CancellationToken ct = default)
    {
        var libraryExists = await _context.LibraryExistsAsync(command.LibraryId, ct);
        if (!libraryExists)
            return null;

        var book = new Book
        {
            Name = command.Name ?? string.Empty,
            Category = command.Category ?? string.Empty,
            LibraryId = command.LibraryId
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync(ct);
        return book;
    }
}
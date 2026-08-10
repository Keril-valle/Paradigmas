using LibraryService.Api.Common.Data;
using LibraryService.Api.Common.Entities;
using LibraryService.Api.Common.Handlers;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Api.Features.Books.GetBooksForLibrary;

public class GetBooksForLibraryHandler : IQueryHandler<GetBooksForLibraryQuery, IEnumerable<Book>?>
{
    private readonly LibraryContext _context;

    public GetBooksForLibraryHandler(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>?> HandleAsync(GetBooksForLibraryQuery query, CancellationToken ct = default)
    {
        var libraryExists = await _context.LibraryExistsAsync(query.LibraryId, ct);
        if (!libraryExists)
            return null;

        return await _context.Books
            .AsNoTracking()
            .Where(b => b.LibraryId == query.LibraryId)
            .ToListAsync(ct);
    }
}
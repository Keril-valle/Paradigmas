using LibraryService.Api.Common.Data;
using LibraryService.Api.Common.Entities;
using LibraryService.Api.Common.Handlers;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Api.Features.Libraries.GetLibraryById;

public class GetLibraryByIdHandler : IQueryHandler<GetLibraryByIdQuery, Library?>
{
    private readonly LibraryContext _context;

    public GetLibraryByIdHandler(LibraryContext context)
    {
        _context = context;
    }

    public async Task<Library?> HandleAsync(GetLibraryByIdQuery query, CancellationToken ct = default)
    {
        return await _context.Libraries
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == query.LibraryId, ct);
    }
}
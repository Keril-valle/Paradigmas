using LibraryService.Api.Common.Data;
using LibraryService.Api.Common.Entities;
using LibraryService.Api.Common.Handlers;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Api.Features.Libraries.GetLibraries;

public class GetLibrariesHandler : IQueryHandler<GetLibrariesQuery, IEnumerable<Library>>
{
    private readonly LibraryContext _context;

    public GetLibrariesHandler(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Library>> HandleAsync(GetLibrariesQuery query, CancellationToken ct = default)
    {
        return await _context.Libraries.AsNoTracking().ToListAsync(ct);
    }
}
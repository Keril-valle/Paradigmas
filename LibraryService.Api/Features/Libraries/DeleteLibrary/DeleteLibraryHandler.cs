using LibraryService.Api.Common.Data;
using LibraryService.Api.Common.Handlers;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Api.Features.Libraries.DeleteLibrary;

public class DeleteLibraryHandler : ICommandHandler<DeleteLibraryCommand, bool>
{
    private readonly LibraryContext _context;

    public DeleteLibraryHandler(LibraryContext context)
    {
        _context = context;
    }

    public async Task<bool> HandleAsync(DeleteLibraryCommand command, CancellationToken ct = default)
    {
        var library = await _context.Libraries.SingleOrDefaultAsync(l => l.Id == command.LibraryId, ct);
        if (library is null)
            return false;

        _context.Libraries.Remove(library);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
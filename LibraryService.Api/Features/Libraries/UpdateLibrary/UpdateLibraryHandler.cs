using LibraryService.Api.Common.Data;
using LibraryService.Api.Common.Handlers;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Api.Features.Libraries.UpdateLibrary;

public class UpdateLibraryHandler : ICommandHandler<UpdateLibraryCommand, bool>
{
    private readonly LibraryContext _context;

    public UpdateLibraryHandler(LibraryContext context)
    {
        _context = context;
    }

    public async Task<bool> HandleAsync(UpdateLibraryCommand command, CancellationToken ct = default)
    {
        var library = await _context.Libraries.SingleOrDefaultAsync(l => l.Id == command.LibraryId, ct);
        if (library is null)
            return false;

        library.Name = command.Name ?? string.Empty;
        library.Location = command.Location ?? string.Empty;

        _context.Libraries.Update(library);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
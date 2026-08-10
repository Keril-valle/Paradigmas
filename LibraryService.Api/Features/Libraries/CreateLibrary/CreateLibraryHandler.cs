using LibraryService.Api.Common.Data;
using LibraryService.Api.Common.Entities;
using LibraryService.Api.Common.Handlers;

namespace LibraryService.Api.Features.Libraries.CreateLibrary;

public class CreateLibraryHandler : ICommandHandler<CreateLibraryCommand, Library>
{
    private readonly LibraryContext _context;

    public CreateLibraryHandler(LibraryContext context)
    {
        _context = context;
    }

    public async Task<Library> HandleAsync(CreateLibraryCommand command, CancellationToken ct = default)
    {
        var library = new Library
        {
            Name = command.Name ?? string.Empty,
            Location = command.Location ?? string.Empty
        };

        _context.Libraries.Add(library);
        await _context.SaveChangesAsync(ct);
        return library;
    }
}
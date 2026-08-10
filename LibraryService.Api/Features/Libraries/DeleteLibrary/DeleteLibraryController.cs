using LibraryService.Api.Common.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Features.Libraries.DeleteLibrary;

[ApiController]
[Route("api/libraries/{libraryId}")]
public class DeleteLibraryController : ControllerBase
{
    private readonly ICommandHandler<DeleteLibraryCommand, bool> _handler;

    public DeleteLibraryController(ICommandHandler<DeleteLibraryCommand, bool> handler)
    {
        _handler = handler;
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int libraryId)
    {
        var deleted = await _handler.HandleAsync(new DeleteLibraryCommand(libraryId));
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
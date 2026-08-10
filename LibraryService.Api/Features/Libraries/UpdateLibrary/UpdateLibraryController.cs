using LibraryService.Api.Common.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Features.Libraries.UpdateLibrary;

[ApiController]
[Route("api/libraries/{libraryId}")]
public class UpdateLibraryController : ControllerBase
{
    private readonly ICommandHandler<UpdateLibraryCommand, bool> _handler;

    public UpdateLibraryController(ICommandHandler<UpdateLibraryCommand, bool> handler)
    {
        _handler = handler;
    }

    [HttpPut]
    public async Task<IActionResult> Update(int libraryId, [FromBody] UpdateLibraryCommand command)
    {
        var updated = await _handler.HandleAsync(command with { LibraryId = libraryId });
        if (!updated)
            return NotFound();

        return NoContent();
    }
}
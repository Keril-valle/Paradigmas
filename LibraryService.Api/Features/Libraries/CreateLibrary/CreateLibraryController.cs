using LibraryService.Api.Common.Entities;
using LibraryService.Api.Common.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Features.Libraries.CreateLibrary;

[ApiController]
[Route("api/libraries")]
public class CreateLibraryController : ControllerBase
{
    private readonly ICommandHandler<CreateLibraryCommand, Library> _handler;

    public CreateLibraryController(ICommandHandler<CreateLibraryCommand, Library> handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLibraryCommand command)
    {
        var library = await _handler.HandleAsync(command);
        return Ok(library);
    }
}
using LibraryService.Api.Common.Entities;
using LibraryService.Api.Common.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Features.Libraries.GetLibraryById;

[ApiController]
[Route("api/libraries/{libraryId}")]
public class GetLibraryByIdController : ControllerBase
{
    private readonly IQueryHandler<GetLibraryByIdQuery, Library?> _handler;

    public GetLibraryByIdController(IQueryHandler<GetLibraryByIdQuery, Library?> handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int libraryId)
    {
        var library = await _handler.HandleAsync(new GetLibraryByIdQuery(libraryId));
        if (library is null)
            return NotFound();

        return Ok(library);
    }
}
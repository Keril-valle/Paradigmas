using LibraryService.Api.Common.Entities;
using LibraryService.Api.Common.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Features.Libraries.GetLibraries;

[ApiController]
[Route("api/libraries")]
public class GetLibrariesController : ControllerBase
{
    private readonly IQueryHandler<GetLibrariesQuery, IEnumerable<Library>> _handler;

    public GetLibrariesController(IQueryHandler<GetLibrariesQuery, IEnumerable<Library>> handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var libraries = await _handler.HandleAsync(new GetLibrariesQuery());
        return Ok(libraries);
    }
}
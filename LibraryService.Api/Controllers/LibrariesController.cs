using LibraryService.Domain.DTO;
using LibraryService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LibrariesController : ControllerBase
{
    private readonly ILibrariesService _librariesService;

    public LibrariesController(ILibrariesService librariesService)
    {
        _librariesService = librariesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var libraries = await _librariesService.Get(null);
        return Ok(libraries);
    }

    [HttpGet("{libraryId}")]
    public async Task<IActionResult> Get(int libraryId)
    {
        var library = (await _librariesService.Get(new[] { libraryId })).FirstOrDefault();
        if (library == null)
            return NotFound();
        return Ok(library);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] LibraryForm form)
    {
        var library = await _librariesService.Add(form);
        return Ok(library);
    }

    [HttpPut("{libraryId}")]
    public async Task<IActionResult> Update(int libraryId, [FromBody] LibraryForm form)
    {
        form.Id = libraryId;
        var existingLibrary = (await _librariesService.Get(new[] { libraryId })).FirstOrDefault();
        if (existingLibrary == null)
            return NotFound();

        await _librariesService.Update(form);
        return NoContent();
    }

    [HttpDelete("{libraryId}")]
    public async Task<IActionResult> Delete(int libraryId)
    {
        var deleted = await _librariesService.Delete(libraryId);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
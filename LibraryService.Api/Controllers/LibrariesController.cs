using LibraryService.Api.DTOs;
using LibraryService.Application.Services;
using LibraryService.Domain.Entities;
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
        var libraries = await _librariesService.GetAsync();
        return Ok(libraries);
    }

    [HttpGet("{libraryId}")]
    public async Task<IActionResult> Get(int libraryId)
    {
        var library = await _librariesService.GetByIdAsync(libraryId);
        if (library is null)
            return NotFound();

        return Ok(library);
    }

    [HttpPost]
    public async Task<IActionResult> Add(LibraryForm form)
    {
        var library = new Library
        {
            Name = form.Name ?? string.Empty,
            Location = form.Location ?? string.Empty
        };

        await _librariesService.AddAsync(library);
        return Ok(library);
    }

    [HttpPut("{libraryId}")]
    public async Task<IActionResult> Update(int libraryId, LibraryForm form)
    {
        var existing = await _librariesService.GetByIdAsync(libraryId);
        if (existing is null)
            return NotFound();

        var library = new Library
        {
            Id = libraryId,
            Name = form.Name ?? string.Empty,
            Location = form.Location ?? string.Empty
        };

        await _librariesService.UpdateAsync(library);
        return NoContent();
    }

    [HttpDelete("{libraryId}")]
    public async Task<IActionResult> Delete(int libraryId)
    {
        var deleted = await _librariesService.DeleteAsync(libraryId);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}

using LibraryService.Domain.DTO;
using LibraryService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Controllers;

[ApiController]
[Route("api/libraries/{libraryId}/[controller]")]
public class BooksController : ControllerBase
{
    private readonly ILibrariesService _librariesService;
    private readonly IBooksService _booksService;

    public BooksController(IBooksService booksService, ILibrariesService librariesService)
    {
        _librariesService = librariesService;
        _booksService = booksService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int libraryId)
    {
        var library = (await _librariesService.Get(new[] { libraryId })).FirstOrDefault();
        if (library == null)
            return NotFound();

        var books = await _booksService.Get(libraryId, null);
        return Ok(books);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int libraryId, [FromBody] BookForm form)
    {
        var library = (await _librariesService.Get(new[] { libraryId })).FirstOrDefault();
        if (library == null)
            return NotFound();

        var book = await _booksService.Add(libraryId, form);
        return StatusCode(StatusCodes.Status201Created, book);
    }
}
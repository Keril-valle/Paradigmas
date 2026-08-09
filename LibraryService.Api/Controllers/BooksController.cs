using LibraryService.Api.DTOs;
using LibraryService.Application.Services;
using LibraryService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Controllers;

[ApiController]
[Route("api/libraries/{libraryId}/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBooksService _booksService;

    public BooksController(IBooksService booksService)
    {
        _booksService = booksService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(int libraryId)
    {
        if (!await _booksService.LibraryExistsAsync(libraryId))
            return NotFound();

        var books = await _booksService.GetAsync(libraryId);
        return Ok(books);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Add(int libraryId, BookForm form)
    {
        if (!await _booksService.LibraryExistsAsync(libraryId))
            return NotFound();

        var book = new Book
        {
            Name = form.Name ?? string.Empty,
            Category = form.Category ?? string.Empty,
            LibraryId = libraryId
        };

        await _booksService.AddAsync(book);
        return CreatedAtAction(nameof(GetAll), new { libraryId }, book);
    }
}

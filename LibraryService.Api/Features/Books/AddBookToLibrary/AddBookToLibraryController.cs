using LibraryService.Api.Common.Entities;
using LibraryService.Api.Common.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Features.Books.AddBookToLibrary;

[ApiController]
[Route("api/libraries/{libraryId}/books")]
public class AddBookToLibraryController : ControllerBase
{
    private readonly ICommandHandler<AddBookToLibraryCommand, Book?> _handler;

    public AddBookToLibraryController(ICommandHandler<AddBookToLibraryCommand, Book?> handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Add(int libraryId, [FromBody] AddBookToLibraryCommand command)
    {
        var book = await _handler.HandleAsync(command with { LibraryId = libraryId });
        if (book is null)
            return NotFound();

        return Created($"/api/libraries/{libraryId}/books", book);
    }
}
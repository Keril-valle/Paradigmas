using LibraryService.Api.Common.Entities;
using LibraryService.Api.Common.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Features.Books.GetBooksForLibrary;

[ApiController]
[Route("api/libraries/{libraryId}/books")]
public class GetBooksForLibraryController : ControllerBase
{
    private readonly IQueryHandler<GetBooksForLibraryQuery, IEnumerable<Book>?> _handler;

    public GetBooksForLibraryController(IQueryHandler<GetBooksForLibraryQuery, IEnumerable<Book>?> handler)
    {
        _handler = handler;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll(int libraryId)
    {
        var books = await _handler.HandleAsync(new GetBooksForLibraryQuery(libraryId));
        if (books is null)
            return NotFound();

        return Ok(books);
    }
}
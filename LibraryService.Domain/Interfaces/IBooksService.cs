using LibraryService.Domain.DTO;
using LibraryService.Domain.Entities;

namespace LibraryService.Domain.Interfaces;

public interface IBooksService
{
    Task<IEnumerable<Book>> Get(int libraryId, int[]? ids);
    Task<Book> Add(int libraryId, BookForm book);
    Task<Book> Update(int libraryId, BookForm book);
    Task<bool> Delete(int id);
}
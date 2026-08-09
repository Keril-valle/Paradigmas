using LibraryService.Domain.Entities;

namespace LibraryService.Application.Interfaces;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAsync(int libraryId, int[]? ids = null);

    Task<Book?> GetByIdAsync(int id);

    Task<Book> AddAsync(Book book);

    Task UpdateAsync(Book book);

    Task<bool> DeleteAsync(int bookId);
}

namespace LibraryService.Domain.Interfaces;

using LibraryService.Domain.Entities;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetByLibraryAsync(int libraryId, int[]? ids);
    Task<Book> AddAsync(int libraryId, Book book);
    Task<Book> UpdateAsync(Book book);
    Task<bool> DeleteAsync(int id);
}
using LibraryService.Application.Interfaces;
using LibraryService.Domain.Entities;

namespace LibraryService.Application.Services;

public interface IBooksService
{
    Task<IEnumerable<Book>> GetAsync(int libraryId);

    Task<Book> AddAsync(Book book);

    Task UpdateAsync(Book book);

    Task<bool> DeleteAsync(int bookId);

    Task<bool> LibraryExistsAsync(int libraryId);
}

public class BooksService : IBooksService
{
    private readonly IBookRepository _bookRepository;
    private readonly ILibraryRepository _libraryRepository;

    public BooksService(IBookRepository bookRepository, ILibraryRepository libraryRepository)
    {
        _bookRepository = bookRepository;
        _libraryRepository = libraryRepository;
    }

    public Task<IEnumerable<Book>> GetAsync(int libraryId)
    {
        return _bookRepository.GetAsync(libraryId);
    }

    public Task<Book> AddAsync(Book book)
    {
        return _bookRepository.AddAsync(book);
    }

    public Task UpdateAsync(Book book)
    {
        return _bookRepository.UpdateAsync(book);
    }

    public Task<bool> DeleteAsync(int bookId)
    {
        return _bookRepository.DeleteAsync(bookId);
    }

    public Task<bool> LibraryExistsAsync(int libraryId)
    {
        return _libraryRepository.ExistsAsync(libraryId);
    }
}

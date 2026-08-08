using LibraryService.Domain.DTO;
using LibraryService.Domain.Entities;
using LibraryService.Domain.Interfaces;

namespace LibraryService.Application.Services;

public class BooksService : IBooksService
{
    private readonly IBookRepository _bookRepository;
    private readonly ILibraryRepository _libraryRepository;

    public BooksService(IBookRepository bookRepository, ILibraryRepository libraryRepository)
    {
        _bookRepository = bookRepository;
        _libraryRepository = libraryRepository;
    }

    public Task<IEnumerable<Book>> Get(int libraryId, int[]? ids)
    {
        return _bookRepository.GetByLibraryAsync(libraryId, ids);
    }

    public async Task<Book> Add(int libraryId, BookForm form)
    {
        var library = await _libraryRepository.GetByIdAsync(libraryId);
        if (library == null)
            return null!;

        var book = new Book
        {
            Name = form.Name,
            Category = form.Category
        };

        return await _bookRepository.AddAsync(libraryId, book);
    }

    public async Task<Book> Update(int libraryId, BookForm form)
    {
        var book = new Book
        {
            Id = form.Id,
            Name = form.Name,
            Category = form.Category,
            LibraryId = libraryId
        };

        return await _bookRepository.UpdateAsync(book);
    }

    public Task<bool> Delete(int id)
    {
        return _bookRepository.DeleteAsync(id);
    }
}
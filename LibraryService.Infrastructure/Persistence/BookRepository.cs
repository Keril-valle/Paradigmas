using LibraryService.Application.Interfaces;
using LibraryService.Domain.Entities;
using LibraryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Infrastructure.Persistence;

public class BookRepository : IBookRepository
{
    private readonly LibraryContext _context;

    public BookRepository(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetAsync(int libraryId, int[]? ids = null)
    {
        var query = _context.Books.AsQueryable().Where(b => b.LibraryId == libraryId);

        if (ids is { Length: > 0 })
            query = query.Where(b => ids.Contains(b.Id));

        return await query.ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _context.Books.SingleOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Book> AddAsync(Book book)
    {
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task UpdateAsync(Book book)
    {
        var existing = await _context.Books.SingleAsync(b => b.Id == book.Id);
        existing.Name = book.Name;
        existing.Category = book.Category;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int bookId)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book is null)
            return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }
}

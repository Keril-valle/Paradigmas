using LibraryService.Domain.Entities;
using LibraryService.Domain.Interfaces;
using LibraryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly LibraryContext _context;

    public BookRepository(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetByLibraryAsync(int libraryId, int[]? ids)
    {
        var query = _context.Books.AsQueryable().Where(b => b.LibraryId == libraryId);

        if (ids != null && ids.Any())
            query = query.Where(b => ids.Contains(b.Id));

        return await query.ToListAsync();
    }

    public async Task<Book> AddAsync(int libraryId, Book book)
    {
        book.LibraryId = libraryId;
        var entry = await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<Book> UpdateAsync(Book book)
    {
        var existing = await _context.Books.SingleAsync(b => b.Id == book.Id);
        existing.Name = book.Name;
        existing.Category = book.Category;

        _context.Books.Update(existing);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }
}
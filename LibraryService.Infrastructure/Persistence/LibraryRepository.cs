using LibraryService.Application.Interfaces;
using LibraryService.Domain.Entities;
using LibraryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Infrastructure.Persistence;

public class LibraryRepository : ILibraryRepository
{
    private readonly LibraryContext _context;

    public LibraryRepository(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Library>> GetAsync(int[]? ids = null)
    {
        var query = _context.Libraries.AsQueryable();

        if (ids is { Length: > 0 })
            query = query.Where(l => ids.Contains(l.Id));

        return await query.ToListAsync();
    }

    public async Task<Library?> GetByIdAsync(int id)
    {
        return await _context.Libraries.SingleOrDefaultAsync(l => l.Id == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Libraries.AnyAsync(l => l.Id == id);
    }

    public async Task<Library> AddAsync(Library library)
    {
        await _context.Libraries.AddAsync(library);
        await _context.SaveChangesAsync();
        return library;
    }

    public async Task UpdateAsync(Library library)
    {
        var existing = await _context.Libraries.SingleAsync(l => l.Id == library.Id);
        existing.Name = library.Name;
        existing.Location = library.Location;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int libraryId)
    {
        var library = await _context.Libraries.FindAsync(libraryId);
        if (library is null)
            return false;

        _context.Libraries.Remove(library);
        await _context.SaveChangesAsync();
        return true;
    }
}

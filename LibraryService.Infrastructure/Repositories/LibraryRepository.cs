using LibraryService.Domain.Entities;
using LibraryService.Domain.Interfaces;
using LibraryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Infrastructure.Repositories;

public class LibraryRepository : ILibraryRepository
{
    private readonly LibraryContext _context;

    public LibraryRepository(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Library>> GetAsync(int[]? ids)
    {
        var query = _context.Libraries.AsQueryable();

        if (ids != null && ids.Any())
            query = query.Where(x => ids.Contains(x.Id));

        return await query.ToListAsync();
    }

    public async Task<Library?> GetByIdAsync(int id)
    {
        return await _context.Libraries.FindAsync(id);
    }

    public async Task<Library> AddAsync(Library library)
    {
        var entry = await _context.Libraries.AddAsync(library);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<Library> UpdateAsync(Library library)
    {
        var existing = await _context.Libraries.SingleAsync(x => x.Id == library.Id);
        existing.Name = library.Name;
        existing.Location = library.Location;

        _context.Libraries.Update(existing);
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var library = await _context.Libraries.FindAsync(id);
        if (library == null)
            return false;

        _context.Libraries.Remove(library);
        await _context.SaveChangesAsync();
        return true;
    }
}
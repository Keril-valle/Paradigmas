using LibraryService.Api.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Api.Common.Data;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options)
    { }

    public DbSet<Library> Libraries { get; set; }

    public DbSet<Book> Books { get; set; }

    public Task<bool> LibraryExistsAsync(int libraryId, CancellationToken ct = default)
        => Libraries.AsNoTracking().AnyAsync(l => l.Id == libraryId, ct);
}
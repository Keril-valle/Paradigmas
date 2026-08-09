using LibraryService.Domain.Entities;

namespace LibraryService.Application.Interfaces;

public interface ILibraryRepository
{
    Task<IEnumerable<Library>> GetAsync(int[]? ids = null);

    Task<Library?> GetByIdAsync(int id);

    Task<bool> ExistsAsync(int id);

    Task<Library> AddAsync(Library library);

    Task UpdateAsync(Library library);

    Task<bool> DeleteAsync(int libraryId);
}

namespace LibraryService.Domain.Interfaces;

using LibraryService.Domain.Entities;

public interface ILibraryRepository
{
    Task<IEnumerable<Library>> GetAsync(int[]? ids);
    Task<Library?> GetByIdAsync(int id);
    Task<Library> AddAsync(Library library);
    Task<Library> UpdateAsync(Library library);
    Task<bool> DeleteAsync(int id);
}
using LibraryService.Application.Interfaces;
using LibraryService.Domain.Entities;

namespace LibraryService.Application.Services;

public interface ILibrariesService
{
    Task<IEnumerable<Library>> GetAsync(int[]? ids = null);

    Task<Library?> GetByIdAsync(int id);

    Task<Library> AddAsync(Library library);

    Task UpdateAsync(Library library);

    Task<bool> DeleteAsync(int libraryId);
}

public class LibrariesService : ILibrariesService
{
    private readonly ILibraryRepository _libraryRepository;

    public LibrariesService(ILibraryRepository libraryRepository)
    {
        _libraryRepository = libraryRepository;
    }

    public Task<IEnumerable<Library>> GetAsync(int[]? ids = null)
    {
        return _libraryRepository.GetAsync(ids);
    }

    public Task<Library?> GetByIdAsync(int id)
    {
        return _libraryRepository.GetByIdAsync(id);
    }

    public Task<Library> AddAsync(Library library)
    {
        return _libraryRepository.AddAsync(library);
    }

    public Task UpdateAsync(Library library)
    {
        return _libraryRepository.UpdateAsync(library);
    }

    public Task<bool> DeleteAsync(int libraryId)
    {
        return _libraryRepository.DeleteAsync(libraryId);
    }
}

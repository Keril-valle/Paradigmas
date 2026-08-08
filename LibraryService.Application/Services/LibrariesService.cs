using LibraryService.Domain.DTO;
using LibraryService.Domain.Entities;
using LibraryService.Domain.Interfaces;

namespace LibraryService.Application.Services;

public class LibrariesService : ILibrariesService
{
    private readonly ILibraryRepository _libraryRepository;

    public LibrariesService(ILibraryRepository libraryRepository)
    {
        _libraryRepository = libraryRepository;
    }

    public Task<IEnumerable<Library>> Get(int[]? ids)
    {
        return _libraryRepository.GetAsync(ids);
    }

    public async Task<Library> Add(LibraryForm form)
    {
        var library = new Library
        {
            Name = form.Name,
            Location = form.Location
        };

        return await _libraryRepository.AddAsync(library);
    }

    public async Task<Library> Update(LibraryForm form)
    {
        var library = new Library
        {
            Id = form.Id,
            Name = form.Name,
            Location = form.Location
        };

        return await _libraryRepository.UpdateAsync(library);
    }

    public Task<bool> Delete(int id)
    {
        return _libraryRepository.DeleteAsync(id);
    }
}
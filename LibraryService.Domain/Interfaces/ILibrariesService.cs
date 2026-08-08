using LibraryService.Domain.DTO;
using LibraryService.Domain.Entities;

namespace LibraryService.Domain.Interfaces;

public interface ILibrariesService
{
    Task<IEnumerable<Library>> Get(int[]? ids);
    Task<Library> Add(LibraryForm library);
    Task<Library> Update(LibraryForm library);
    Task<bool> Delete(int id);
}
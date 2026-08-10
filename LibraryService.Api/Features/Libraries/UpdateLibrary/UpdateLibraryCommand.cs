namespace LibraryService.Api.Features.Libraries.UpdateLibrary;

public record UpdateLibraryCommand(int LibraryId, string Name, string Location);
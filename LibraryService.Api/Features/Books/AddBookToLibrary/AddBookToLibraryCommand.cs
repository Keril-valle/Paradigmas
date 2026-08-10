namespace LibraryService.Api.Features.Books.AddBookToLibrary;

public record AddBookToLibraryCommand(int LibraryId, string Name, string Category);
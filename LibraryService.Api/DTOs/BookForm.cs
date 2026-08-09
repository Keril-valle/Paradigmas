namespace LibraryService.Api.DTOs;

public class BookForm
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Category { get; set; }

    public int LibraryId { get; set; }
}

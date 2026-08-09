namespace LibraryService.Application.Authentication;

public interface ITokenService
{
    string GenerateToken(User user);
}

namespace LibraryService.Application.Authentication;

public interface IAuthenticationService
{
    Task<User?> AuthenticateAsync(string email, string password);
}

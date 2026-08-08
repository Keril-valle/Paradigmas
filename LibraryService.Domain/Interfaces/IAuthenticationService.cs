using LibraryService.Domain.DTO;

namespace LibraryService.Domain.Interfaces;

public interface IAuthenticationService
{
    Task<User?> AuthenticateAsync(string email, string password);
}
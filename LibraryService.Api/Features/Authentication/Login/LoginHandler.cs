using LibraryService.Api.Common.Auth;
using LibraryService.Api.Common.Handlers;

namespace LibraryService.Api.Features.Authentication.Login;

public class LoginHandler : IQueryHandler<LoginQuery, LoginResponse?>
{
    private readonly JwtSettings _jwtSettings;

    public LoginHandler(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    public Task<LoginResponse?> HandleAsync(LoginQuery query, CancellationToken ct = default)
    {
        if (query.Email != "admin" || query.Password != "1234")
            return Task.FromResult<LoginResponse?>(null);

        var token = TokenGenerator.GenerateToken(1, query.Email, "admin", _jwtSettings);
        return Task.FromResult<LoginResponse?>(new LoginResponse(token));
    }
}
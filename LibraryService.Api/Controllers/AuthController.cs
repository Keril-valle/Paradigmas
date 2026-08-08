using LibraryService.Domain;
using LibraryService.Domain.DTO;
using LibraryService.Domain.Interfaces;
using LibraryService.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Controllers;

public record TokenResponse(string token);

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly JwtSettings _jwtSettings;
    private readonly TokenGenerator _tokenGenerator;

    public AuthController(
        IAuthenticationService authenticationService,
        JwtSettings jwtSettings,
        TokenGenerator tokenGenerator)
    {
        _authenticationService = authenticationService;
        _jwtSettings = jwtSettings;
        _tokenGenerator = tokenGenerator;
    }

    [HttpPost("/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(User user)
    {
        var validUser = await _authenticationService.AuthenticateAsync(user.Email, user.Password);
        if (validUser is null)
            return Unauthorized();

        var token = _tokenGenerator.GenerateToken(validUser, _jwtSettings);

        return Ok(new TokenResponse(token));
    }
}
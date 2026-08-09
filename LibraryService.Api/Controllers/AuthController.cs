using LibraryService.Api.DTOs;
using LibraryService.Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ITokenService _tokenService;

    public AuthController(IAuthenticationService authenticationService, ITokenService tokenService)
    {
        _authenticationService = authenticationService;
        _tokenService = tokenService;
    }

    [HttpPost("/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginForm form)
    {
        var user = await _authenticationService.AuthenticateAsync(form.Email, form.Password);
        if (user is null)
            return Unauthorized();

        var token = _tokenService.GenerateToken(user);
        return Ok(new TokenResponse(token));
    }
}

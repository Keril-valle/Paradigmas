using LibraryService.Api.Common.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Features.Authentication.Login;

[ApiController]
public class LoginController : ControllerBase
{
    private readonly IQueryHandler<LoginQuery, LoginResponse?> _loginHandler;

    public LoginController(IQueryHandler<LoginQuery, LoginResponse?> loginHandler)
    {
        _loginHandler = loginHandler;
    }

    [HttpPost("/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginQuery query)
    {
        var response = await _loginHandler.HandleAsync(query);
        if (response is null)
            return Unauthorized();

        return Ok(response);
    }
}
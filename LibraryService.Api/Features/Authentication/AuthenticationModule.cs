using LibraryService.Api.Common.Handlers;
using LibraryService.Api.Features.Authentication.Login;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryService.Api.Features.Authentication;

public static class AuthenticationModule
{
    public static IServiceCollection AddAuthenticationFeatures(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<LoginQuery, LoginResponse?>, LoginHandler>();
        return services;
    }
}
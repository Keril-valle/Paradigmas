using LibraryService.Application.Authentication;
using LibraryService.Application.Interfaces;
using LibraryService.Infrastructure.Auth;
using LibraryService.Infrastructure.Data;
using LibraryService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
            ?? throw new InvalidOperationException("Invalid JWT Settings");
        services.AddSingleton(jwtSettings);

        services.AddScoped<ILibraryRepository, LibraryRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<ITokenService, TokenGenerator>();

        services.AddDbContextPool<LibraryContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 1,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                }),
            poolSize: 20);

        return services;
    }
}

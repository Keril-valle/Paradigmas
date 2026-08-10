using LibraryService.Api.Common.Auth;
using LibraryService.Api.Common.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryService.Api.Common;

public static class DependencyInjection
{
    public static IServiceCollection AddShared(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
            ?? throw new InvalidOperationException("Invalid JWT Settings");

        services.AddSingleton(jwtSettings);

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
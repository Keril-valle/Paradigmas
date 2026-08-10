using System.Text;
using LibraryService.Api.Common;
using LibraryService.Api.Common.Auth;
using LibraryService.Api.Common.Data;
using LibraryService.Api.Features.Authentication;
using LibraryService.Api.Features.Books;
using LibraryService.Api.Features.Libraries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace LibraryService.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // 1. Shared kernel: DbContext pool + JwtSettings
        services.AddShared(Configuration);

        // 2. Feature slices self-register their handlers
        services.AddAuthenticationFeatures();
        services.AddLibrariesFeatures();
        services.AddBooksFeatures();

        // 3. JWT authentication
        var jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>()
            ?? throw new InvalidOperationException("Invalid JWT Settings");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        // 4. CORS for the FE (Vite dev server)
        services.AddCors(o => o.AddPolicy("Frontend", p => p
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()));

        services.AddControllers();

        // 5. Swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "LibraryService API",
                Version = "v1",
                Description = "A simple example ASP.NET Core Web API for LibraryService"
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LibraryService API v1");
            });
        }

        using (var scope = app.ApplicationServices.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
            db.Database.Migrate();
        }

        app.UseRouting();

        app.UseCors("Frontend");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
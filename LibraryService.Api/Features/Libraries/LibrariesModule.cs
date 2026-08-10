using LibraryService.Api.Common.Entities;
using LibraryService.Api.Common.Handlers;
using LibraryService.Api.Features.Libraries.CreateLibrary;
using LibraryService.Api.Features.Libraries.DeleteLibrary;
using LibraryService.Api.Features.Libraries.GetLibraries;
using LibraryService.Api.Features.Libraries.GetLibraryById;
using LibraryService.Api.Features.Libraries.UpdateLibrary;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryService.Api.Features.Libraries;

public static class LibrariesModule
{
    public static IServiceCollection AddLibrariesFeatures(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetLibrariesQuery, IEnumerable<Library>>, GetLibrariesHandler>();
        services.AddScoped<IQueryHandler<GetLibraryByIdQuery, Library?>, GetLibraryByIdHandler>();
        services.AddScoped<ICommandHandler<CreateLibraryCommand, Library>, CreateLibraryHandler>();
        services.AddScoped<ICommandHandler<UpdateLibraryCommand, bool>, UpdateLibraryHandler>();
        services.AddScoped<ICommandHandler<DeleteLibraryCommand, bool>, DeleteLibraryHandler>();
        return services;
    }
}
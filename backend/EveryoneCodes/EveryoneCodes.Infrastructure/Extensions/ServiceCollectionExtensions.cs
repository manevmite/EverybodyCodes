using EveryoneCodes.Core.Configuration;
using EveryoneCodes.Core.Interfaces;
using EveryoneCodes.Core.Models;
using EveryoneCodes.Infrastructure.Parsers;
using EveryoneCodes.Infrastructure.ResourceReaders;
using EveryoneCodes.Shared;
using EveryoneCodes.Shared.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EveryoneCodes.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCameraInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuration - Fixed: Use .Bind to convert IConfigurationSection to Action<T>
            services.Configure<CameraStoreSettings>(configuration.GetSection(CameraStoreSettings.SectionName).Bind);

            // Infrastructure services
            services.AddSingleton<IResourceReader, EmbeddedResourceReader>();
            services.AddScoped<ICsvParser<Camera>, CameraCsvParser>();

            // Store
            services.AddScoped<ICameraStore, EmbeddedCsvCameraStore>();

            return services;
        }
    }
}

using EveryoneCodes.Core.Configuration;
using EveryoneCodes.Core.Interfaces;
using EveryoneCodes.Core.Models;
using EveryoneCodes.Infrastructure.Parsers;
using EveryoneCodes.Infrastructure.Repositories;
using EveryoneCodes.Infrastructure.ResourceReaders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EveryoneCodes.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCameraInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CameraStoreSettings>(configuration.GetSection(CameraStoreSettings.SectionName).Bind);

            // Infrastructure services
            services.AddSingleton<IResourceReader, EmbeddedResourceReader>();
            services.AddScoped<ICsvParser<Camera>, CameraCsvParser>();

            // Store
            services.AddScoped<ICameraStore, EmbeddedCsvCameraStore>();

            return services;
        }

        public static IServiceCollection AddCameraRepository(this IServiceCollection services, string csvFilePath)
        {
            // Infrastructure services
            services.AddScoped<ICsvParser<Camera>, CameraCsvParser>();

            // Repository with file path
            services.AddScoped<ICameraRepository>(provider =>
            {
                var csvParser = provider.GetRequiredService<ICsvParser<Camera>>();
                var logger = provider.GetRequiredService<ILogger<CameraRepository>>();
                return new CameraRepository(csvParser, csvFilePath, logger);
            });

            return services;
        }
    }
}

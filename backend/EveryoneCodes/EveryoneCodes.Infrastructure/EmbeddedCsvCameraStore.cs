using EveryoneCodes.Core.Configuration;
using EveryoneCodes.Core.Interfaces;
using EveryoneCodes.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EveryoneCodes.Infrastructure
{
    public class EmbeddedCsvCameraStore : ICameraStore
    {
        private readonly IResourceReader _resourceReader;
        private readonly ICsvParser<Camera> _csvParser;
        private readonly CameraStoreSettings _settings;
        private readonly ILogger<EmbeddedCsvCameraStore> _logger;

        private IReadOnlyList<Camera>? _cachedCameras;
        private DateTime _lastCacheTime = DateTime.MinValue;

        public EmbeddedCsvCameraStore(
            IResourceReader resourceReader,
            ICsvParser<Camera> csvParser,
            IOptions<CameraStoreSettings> settings,
            ILogger<EmbeddedCsvCameraStore> logger)
        {
            _resourceReader = resourceReader ?? throw new ArgumentNullException(nameof(resourceReader));
            _csvParser = csvParser ?? throw new ArgumentNullException(nameof(csvParser));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyList<Camera>> GetAllAsync()
        {
            if (ShouldUseCache())
            {
                _logger.LogDebug("Returning cached cameras");
                return _cachedCameras!;
            }

            try
            {
                _logger.LogInformation("Loading cameras from resource: {ResourcePath}", _settings.ResourcePath);

                using var stream = await _resourceReader.ReadResourceAsync(_settings.ResourcePath);
                var cameras = await _csvParser.ParseAsync(stream);
                var cameraList = cameras.ToList().AsReadOnly();

                if (_settings.EnableCaching)
                {
                    _cachedCameras = cameraList;
                    _lastCacheTime = DateTime.UtcNow;
                    _logger.LogInformation("Cached {Count} cameras", cameraList.Count);
                }

                return cameraList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load cameras from resource: {ResourcePath}", _settings.ResourcePath);
                throw;
            }
        }

        private bool ShouldUseCache()
        {
            return _settings.EnableCaching &&
                   _cachedCameras is not null &&
                   DateTime.UtcNow - _lastCacheTime < _settings.CacheExpiration;
        }
    }
}

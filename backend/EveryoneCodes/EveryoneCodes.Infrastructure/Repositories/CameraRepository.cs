using EveryoneCodes.Core.Interfaces;
using EveryoneCodes.Core.Models;
using Microsoft.Extensions.Logging;

namespace EveryoneCodes.Infrastructure.Repositories
{
    public class CameraRepository : ICameraRepository
    {
        private readonly ICsvParser<Camera> _csvParser;
        private readonly string _csvFilePath;
        private readonly ILogger<CameraRepository> _logger;

        public CameraRepository(
            ICsvParser<Camera> csvParser,
            string csvFilePath,
            ILogger<CameraRepository> logger)
        {
            _csvParser = csvParser ?? throw new ArgumentNullException(nameof(csvParser));
            _csvFilePath = csvFilePath ?? throw new ArgumentNullException(nameof(csvFilePath));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyList<Camera>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Loading cameras from file: {FilePath}", _csvFilePath);

                if (!File.Exists(_csvFilePath))
                {
                    throw new FileNotFoundException($"CSV file not found: {_csvFilePath}");
                }

                using var fileStream = new FileStream(_csvFilePath, FileMode.Open, FileAccess.Read);
                var cameras = await _csvParser.ParseAsync(fileStream);
                var cameraList = cameras.ToList().AsReadOnly();

                _logger.LogInformation("Loaded {Count} cameras from file", cameraList.Count);

                return cameraList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load cameras from file: {FilePath}", _csvFilePath);
                throw;
            }
        }
    }
}

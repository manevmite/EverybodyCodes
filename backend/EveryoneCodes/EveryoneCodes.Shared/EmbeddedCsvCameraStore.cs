using CsvHelper;
using CsvHelper.Configuration;
using EveryoneCodes.Core.Configuration;
using EveryoneCodes.Core.Interfaces;
using EveryoneCodes.Core.Models;
using EveryoneCodes.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EveryoneCodes.Shared
{
    //public sealed class EmbeddedCsvCameraStore : ICameraStore
    //{
    //    private readonly string _resourceSuffix;
    //    private readonly ILogger<EmbeddedCsvCameraStore> _logger;

    //    static readonly Regex CodeAndNameRegex =
    //        new(@"^(?<code>[A-Za-z]+(?:-[A-Za-z]+)*-\d+)(?:\s+|-\s*)(?<name>.*)$", RegexOptions.Compiled);

    //    static readonly Regex NumberRegex = new(@"\d+", RegexOptions.Compiled);

    //    public EmbeddedCsvCameraStore(
    //        string resourceSuffix = "Data.cameras-defb.csv",
    //        ILogger<EmbeddedCsvCameraStore>? logger = null)
    //    {
    //        _resourceSuffix = resourceSuffix;
    //        _logger = logger ?? NullLogger<EmbeddedCsvCameraStore>.Instance;
    //    }

    //    public async Task<IReadOnlyList<Camera>> GetAllAsync()
    //    {
    //        var asm = Assembly.GetExecutingAssembly();
    //        var resName = asm.GetManifestResourceNames()
    //                         .FirstOrDefault(n => n.EndsWith(_resourceSuffix, StringComparison.OrdinalIgnoreCase));

    //        if (resName is null)
    //        {
    //            throw new FileNotFoundException($"Embedded resource '{_resourceSuffix}' not found.");
    //        }

    //        await using var stream = asm.GetManifestResourceStream(resName)
    //            ?? throw new FileNotFoundException($"Resource stream '{resName}' not found.");
    //        using var reader = new StreamReader(stream);

    //        try
    //        {
    //            var items = ParseCsv(reader);
    //            _logger.LogInformation("Parsed {Count} cameras from {ResourceName}", items.Count, resName);
    //            return items;
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError(ex, "Failed parsing cameras from {ResourceName}", resName);
    //            throw;
    //        }
    //    }

    //    private IReadOnlyList<Camera> ParseCsv(TextReader reader)
    //    {
    //        var config = new CsvConfiguration(CultureInfo.InvariantCulture) 
    //        { 
    //            HasHeaderRecord = true, 
    //            MissingFieldFound = null,
    //            HeaderValidated = null,
    //            Delimiter = ";"
    //        };

    //        using var csv = new CsvReader(reader, config);

    //        var rows = csv.GetRecords<CameraCsvRow>()
    //                      .Where(r => !string.IsNullOrWhiteSpace(r.Camera) &&
    //                                  !r.Camera.StartsWith("ERROR", StringComparison.OrdinalIgnoreCase))
    //                      .ToList();

    //        var result = new List<Camera>(rows.Count);

    //        foreach (var r in rows)
    //        {
    //            var raw = r.Camera?.Trim() ?? string.Empty;

    //            string code, name;
    //            var m2 = CodeAndNameRegex.Match(raw);
    //            if (m2.Success)
    //            {
    //                code = m2.Groups["code"].Value.Trim();
    //                name = m2.Groups["name"].Value.Trim();
    //            }
    //            else
    //            {
    //                var firstSpace = raw.IndexOf(' ');
    //                code = firstSpace > 0 ? raw[..firstSpace] : raw;
    //                name = firstSpace > 0 ? raw[(firstSpace + 1)..] : string.Empty;
    //                _logger.LogDebug("Fallback split used for raw '{Raw}'", raw);
    //            }

    //            var mNum = NumberRegex.Match(code);
    //            var number = mNum.Success && int.TryParse(mNum.Value, out var n) ? n : 0;
    //            if (number == 0)
    //                _logger.LogWarning("No numeric camera number extracted from code '{Code}'", code);

    //            result.Add(new Camera
    //            {
    //                Number = number,
    //                Code = code,
    //                Name = name,
    //                Latitude = r.Latitude,
    //                Longitude = r.Longitude
    //            });
    //        }

    //        return result;
    //    }
    //}

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

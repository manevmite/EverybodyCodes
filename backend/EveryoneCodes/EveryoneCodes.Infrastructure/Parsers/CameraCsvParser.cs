using CsvHelper;
using CsvHelper.Configuration;
using EveryoneCodes.Core.Interfaces;
using EveryoneCodes.Core.Models;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EveryoneCodes.Infrastructure.Parsers
{
    public class CameraCsvParser : ICsvParser<Camera>
    {
        private readonly ILogger<CameraCsvParser> _logger;

        private static readonly Regex CodeAndNameRegex =
            new(@"^(?<code>[A-Za-z]+(?:-[A-Za-z]+)*-\d+)(?:\s+|-\s*)(?<name>.*)$", RegexOptions.Compiled);

        private static readonly Regex NumberRegex = new(@"\d+", RegexOptions.Compiled);

        public CameraCsvParser(ILogger<CameraCsvParser> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Camera>> ParseAsync(Stream stream)
        {
            using var reader = new StreamReader(stream);
            return Parse(reader);
        }

        public IEnumerable<Camera> Parse(TextReader reader)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                Delimiter = ";"
            };

            using var csv = new CsvReader(reader, config);

            var rows = csv.GetRecords<CameraCsvRow>()
                         .Where(r => IsValidRow(r))
                         .ToList();

            _logger.LogInformation("Parsing {RowCount} valid CSV rows", rows.Count);

            var cameras = new List<Camera>(rows.Count);

            foreach (var row in rows)
            {
                var camera = ParseCameraFromRow(row);
                if (camera is not null)
                {
                    cameras.Add(camera);
                }
            }

            return cameras;
        }

        private static bool IsValidRow(CameraCsvRow row)
        {
            return !string.IsNullOrWhiteSpace(row.Camera) &&
                   !row.Camera.StartsWith("ERROR", StringComparison.OrdinalIgnoreCase);
        }

        private Camera? ParseCameraFromRow(CameraCsvRow row)
        {
            try
            {
                var raw = row.Camera?.Trim() ?? string.Empty;
                var (code, name) = ParseCodeAndName(raw);
                var number = ExtractCameraNumber(code);

                if (number == 0)
                {
                    _logger.LogWarning("No numeric camera number extracted from code '{Code}'", code);
                }

                return new Camera
                {
                    Number = number,
                    Code = code,
                    Name = name,
                    Latitude = row.Latitude,
                    Longitude = row.Longitude
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse camera from row: {Row}", row.Camera);
                return null;
            }
        }

        private (string code, string name) ParseCodeAndName(string raw)
        {
            var match = CodeAndNameRegex.Match(raw);
            if (match.Success)
            {
                return (
                    match.Groups["code"].Value.Trim(),
                    match.Groups["name"].Value.Trim()
                );
            }

            // Fallback parsing
            var firstSpace = raw.IndexOf(' ');
            var code = firstSpace > 0 ? raw[..firstSpace] : raw;
            var name = firstSpace > 0 ? raw[(firstSpace + 1)..] : string.Empty;

            _logger.LogDebug("Fallback split used for raw '{Raw}'", raw);
            return (code, name);
        }

        private int ExtractCameraNumber(string code)
        {
            var match = NumberRegex.Match(code);
            return match.Success && int.TryParse(match.Value, out var number) ? number : 0;
        }
    }
}

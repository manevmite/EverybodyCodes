using EveryoneCodes.Core.Interfaces;
using System.Globalization;

namespace EveryoneCodes.Cli
{
    public sealed class SearchRunner(ICameraService service)
    {
        public async Task<int> RunAsync(string term)
        {
            var cameras = await service.SearchAsync(term ?? string.Empty);

            foreach (var c in cameras.OrderBy(c => c.Number))
            {
                Console.WriteLine(
                    $"{c.Number} | {c.Code} {c.Name} | " +
                    $"{c.Latitude.ToString(CultureInfo.InvariantCulture)} | " +
                    $"{c.Longitude.ToString(CultureInfo.InvariantCulture)}"
                );
            }

            return cameras.Any() ? 1 : 0;
        }
    }
}

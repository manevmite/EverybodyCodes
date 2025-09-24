using Microsoft.Extensions.Logging;
using System.Reflection;

namespace EveryoneCodes.Infrastructure.ResourceReaders
{
    public class EmbeddedResourceReader : Core.Interfaces.IResourceReader
    {
        private readonly ILogger<EmbeddedResourceReader> _logger;

        public EmbeddedResourceReader(ILogger<EmbeddedResourceReader> logger)
        {
            _logger = logger;
        }

        public async Task<Stream> ReadResourceAsync(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            var resName = FindResourceName(asm, resourceName);

            if (resName is null)
            {
                throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
            }

            var stream = asm.GetManifestResourceStream(resName);
            if (stream is null)
            {
                throw new FileNotFoundException($"Resource stream '{resName}' not found.");
            }

            _logger.LogDebug("Successfully opened resource stream for '{ResourceName}'", resName);
            return stream;
        }

        public async Task<bool> ResourceExistsAsync(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            return FindResourceName(asm, resourceName) is not null;
        }

        private static string? FindResourceName(Assembly assembly, string resourceSuffix)
        {
            return assembly.GetManifestResourceNames()
                          .FirstOrDefault(n => n.EndsWith(resourceSuffix, StringComparison.OrdinalIgnoreCase));
        }
    }
}

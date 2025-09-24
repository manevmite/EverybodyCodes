namespace EveryoneCodes.Core.Interfaces
{
    public interface IResourceReader
    {
        Task<Stream> ReadResourceAsync(string resourceName);
        Task<bool> ResourceExistsAsync(string resourceName);
    }
}

using EveryoneCodes.Core.Models;

namespace EveryoneCodes.Core.Interfaces
{
    public interface ICameraService
    {
        Task<IEnumerable<Camera>> GetAllAsync();
        Task<IEnumerable<Camera>> SearchAsync(string name);
    }
}

using EveryoneCodes.Core.Models;

namespace EveryoneCodes.Core.Interfaces
{
    public interface ICameraStore
    {
        Task<IReadOnlyList<Camera>> GetAllAsync();
    }
}

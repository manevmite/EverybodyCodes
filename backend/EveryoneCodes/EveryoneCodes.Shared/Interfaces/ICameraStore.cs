using EveryoneCodes.Core.Models;

namespace EveryoneCodes.Shared.Interfaces
{
    public interface ICameraStore
    {
        Task<IReadOnlyList<Camera>> GetAllAsync();
    }
}

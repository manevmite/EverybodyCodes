using EveryoneCodes.Core.Models;

namespace EveryoneCodes.Core.Interfaces
{
    public interface ICameraRepository
    {
        Task<IReadOnlyList<Camera>> GetAllAsync();
    }
}

using EveryoneCodes.Core.Interfaces;
using EveryoneCodes.Core.Models;
using EveryoneCodes.Shared.Interfaces;

namespace EveryoneCodes.Application
{
    public class CameraService : ICameraService
    {
        private readonly ICameraStore _store;
        public CameraService(ICameraStore store)
        {
            _store = store;
        }
        public async Task<IEnumerable<Camera>> GetAllAsync()
        {
            return await _store.GetAllAsync();
        }

        public async Task<IEnumerable<Camera>> SearchAsync(string name)
        {
            var all = await _store.GetAllAsync();
            return all.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}

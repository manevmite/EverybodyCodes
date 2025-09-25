using EveryoneCodes.Core.Interfaces;
using EveryoneCodes.Core.Models;

namespace EveryoneCodes.Application
{
    public class CameraService : ICameraService
    {
        private readonly ICameraStore? _store;
        private readonly ICameraRepository? _repository;

        public CameraService(ICameraStore store)
        {
            _store = store;
        }

        public CameraService(ICameraRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Camera>> GetAllAsync()
        {
            if (_store != null)
            {
                return await _store.GetAllAsync();
            }
            else if (_repository != null)
            {
                return await _repository.GetAllAsync();
            }

            throw new InvalidOperationException("Neither store nor repository is configured");
        }

        public async Task<IEnumerable<Camera>> SearchAsync(string name)
        {
            var all = await GetAllAsync();
            return all.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}

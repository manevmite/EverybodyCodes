using EveryoneCodes.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EveryoneCodes.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CamerasController : ControllerBase
    {
        private readonly ICameraService _cameraService;
        public CamerasController(ICameraService cameraService)
        {
            _cameraService = cameraService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllCameras()
        {
            var cameras = await _cameraService.GetAllAsync();
            return Ok(cameras);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCameras([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Query parameter 'name' is required.");
            }

            var cameras = await _cameraService.SearchAsync(name);
            return Ok(cameras);
        }
    }
}

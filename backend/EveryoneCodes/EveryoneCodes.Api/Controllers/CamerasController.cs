using EveryoneCodes.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EveryoneCodes.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CamerasController : ControllerBase
    {
        private readonly ICameraService _cameraService;
        private readonly ILogger<CamerasController> _logger;
        public CamerasController(ICameraService cameraService, ILogger<CamerasController> logger)
        {
            _cameraService = cameraService;
            _logger = logger;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllCameras()
        {
            try
            {
                var cameras = await _cameraService.GetAllAsync();
                return Ok(cameras);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all cameras");
                return Problem(detail: "Unexpected error while fetching cameras.");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCameras([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Query parameter 'name' is required.");
            }

            try
            {
                var cameras = await _cameraService.SearchAsync(name);
                return Ok(cameras);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search failed for query '{Name}'", name);
                return Problem(detail: "Unexpected error while searching cameras.");
            }
        }
    }
}

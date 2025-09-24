using EveryoneCodes.Api.Controllers;
using EveryoneCodes.Core.Interfaces;
using EveryoneCodes.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace EveryoneCodes.Api.Tests
{
    public class CamerasControllerTests
    {
        [Fact]
        public async Task GetAllCameras_WhenOk_ReturnsOkWithPayload()
        {
            // Arrange
            var data = new[]
            {
                new Camera { Code = "UTR-CM-552", Name = "Neude rijbaan", Latitude = "52.09", Longitude = "5.12" },
                new Camera { Code = "UTR-CM-501", Name = "Somewhere", Latitude = "52.10", Longitude = "5.13" },
            };

            var svc = new Mock<ICameraService>();
            svc.Setup(s => s.GetAllAsync()).ReturnsAsync(data);

            var ctrl = new CamerasController(svc.Object, NullLogger<CamerasController>.Instance);

            // Act
            var result = await ctrl.GetAllCameras();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsAssignableFrom<IEnumerable<Camera>>(ok.Value);
            Assert.Equal(2, payload.Count());
            svc.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllCameras_WhenEmpty_ReturnsOkWithEmptyArray()
        {
            // Arrange
            var svc = new Mock<ICameraService>();
            svc.Setup(s => s.GetAllAsync()).ReturnsAsync(Array.Empty<Camera>());

            var ctrl = new CamerasController(svc.Object, NullLogger<CamerasController>.Instance);

            // Act
            var result = await ctrl.GetAllCameras();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsAssignableFrom<IEnumerable<Camera>>(ok.Value);
            Assert.Empty(payload);
            svc.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllCameras_WhenServiceThrows_ReturnsProblem500()
        {
            // Arrange
            var svc = new Mock<ICameraService>();
            svc.Setup(s => s.GetAllAsync()).ThrowsAsync(new Exception("boom"));

            var ctrl = new CamerasController(svc.Object, NullLogger<CamerasController>.Instance);

            // Act
            var result = await ctrl.GetAllCameras();

            // Assert
            var problem = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problem.StatusCode);
            Assert.IsType<ProblemDetails>(problem.Value);
            svc.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task SearchCameras_EmptyQuery_ReturnsBadRequest()
        {
            var svc = new Mock<ICameraService>();

            var ctrl = new CamerasController(svc.Object, NullLogger<CamerasController>.Instance);
            var result = await ctrl.SearchCameras(string.Empty);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task SearchCameras_ValidQuery_ReturnsOkWithResults()
        {
            var svc = new Mock<ICameraService>();
            svc.Setup(s => s.SearchAsync("neude"))
               .ReturnsAsync(new[] { new Camera { Name = "Neude rijbaan" } });

            var ctrl = new CamerasController(svc.Object, NullLogger<CamerasController>.Instance);

            var result = await ctrl.SearchCameras("neude");
            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsAssignableFrom<IEnumerable<Camera>>(ok.Value);
            Assert.Single(payload);
        }
    }
}

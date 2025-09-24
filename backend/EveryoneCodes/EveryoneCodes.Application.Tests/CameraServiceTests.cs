using EveryoneCodes.Core.Interfaces;
using EveryoneCodes.Core.Models;
using FluentAssertions;
using Moq;

namespace EveryoneCodes.Application.Tests
{
    public class CameraServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsItemsFromStore()
        {
            // Arrange
            var data = new[]
            {
                new Camera { Code = "UTR-CM-552", Name = "Neude rijbaan" },
                new Camera { Code = "UTR-CM-501", Name = "Utrecht Central" }
            };

            var store = new Mock<ICameraStore>();
            store.Setup(s => s.GetAllAsync()).ReturnsAsync(data);

            var svc = new CameraService(store.Object);

            // Act
            var result = await svc.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(data);
            store.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WhenEmpty_ReturnsEmpty()
        {
            var store = new Mock<ICameraStore>();
            store.Setup(s => s.GetAllAsync()).ReturnsAsync(Array.Empty<Camera>());

            var svc = new CameraService(store.Object);

            var result = await svc.GetAllAsync();

            result.Should().BeEmpty();
            store.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_FiltersByName_CaseInsensitive()
        {
            // Arrange
            var data = new[]
            {
                new Camera { Name = "Neude rijbaan" },
                new Camera { Name = "Utrecht Central" },
                new Camera { Name = "NEUDE Bridge" }
            };

            var store = new Mock<ICameraStore>();
            store.Setup(s => s.GetAllAsync()).ReturnsAsync(data);

            var svc = new CameraService(store.Object);

            // Act
            var result = await svc.SearchAsync("neude");

            // Assert
            result.Select(c => c.Name)
                  .Should().BeEquivalentTo(new[] { "Neude rijbaan", "NEUDE Bridge" }, options => options.WithoutStrictOrdering());
            store.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_NoMatches_ReturnsEmpty()
        {
            var data = new[]
            {
                new Camera { Name = "Neude rijbaan" },
                new Camera { Name = "Utrecht Central" }
            };

            var store = new Mock<ICameraStore>();
            store.Setup(s => s.GetAllAsync()).ReturnsAsync(data);

            var svc = new CameraService(store.Object);

            var result = await svc.SearchAsync("amsterdam");

            result.Should().BeEmpty();
            store.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_UsesSingleReadFromStore()
        {
            // Ensures we don't read more than once per call
            var store = new Mock<ICameraStore>();
            store.Setup(s => s.GetAllAsync())
                 .ReturnsAsync(new[] { new Camera { Name = "Neude" } });

            var svc = new CameraService(store.Object);

            _ = await svc.SearchAsync("neude");

            store.Verify(s => s.GetAllAsync(), Times.Once);
        }
    }
}

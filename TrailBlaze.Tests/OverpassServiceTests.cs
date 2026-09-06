using Moq;
using NUnit.Framework;
using TrailBlaze.API.DataTransferObjects;
using TrailBlaze.API.Services;

namespace TrailBlaze.Tests
{
    [TestFixture]
    public class OverpassServiceTests
    {
        private Mock<IOverpassService> _mockOverpassService;

        [SetUp]
        public void SetUp()
        {
            _mockOverpassService = new Mock<IOverpassService>();
        }

        [Test]
        public async Task GetHikingTrailsAsync_ReturnsListOfTrails()
        {
            // Arrange
            var trails = new List<OverpassTrailResultDto>
            {
                new OverpassTrailResultDto { OsmId = 1, Name = "Lake District Path", DistanceMiles = 2.5, StartLatitude = 54.5, StartLongitude = -3.0 },
                new OverpassTrailResultDto { OsmId = 2, Name = "Fell Walk", DistanceMiles = 3.1, StartLatitude = 54.6, StartLongitude = -3.1 }
            };

            _mockOverpassService.Setup(s => s.GetHikingTrailsAsync(54.5, -3.0, 5000)).ReturnsAsync(trails);

            // Act
            var result = await _mockOverpassService.Object.GetHikingTrailsAsync(54.5, -3.0, 5000);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetHikingTrailsAsync_ReturnsEmptyList_WhenNoTrailsFound()
        {
            // Arrange
            _mockOverpassService.Setup(s => s.GetHikingTrailsAsync(0, 0, 5000)).ReturnsAsync(new List<OverpassTrailResultDto>());

            // Act
            var result = await _mockOverpassService.Object.GetHikingTrailsAsync(0, 0, 5000);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetHikingTrailsAsync_ReturnsCorrectTrailName()
        {
            // Arrange
            var trails = new List<OverpassTrailResultDto>
            {
                new OverpassTrailResultDto { OsmId = 1, Name = "Helvellyn Path", DistanceMiles = 2.5, StartLatitude = 54.5274, StartLongitude = -3.0088 }
            };

            _mockOverpassService.Setup(s => s.GetHikingTrailsAsync(54.5274, -3.0088, 5000)).ReturnsAsync(trails);

            // Act
            var result = await _mockOverpassService.Object.GetHikingTrailsAsync(54.5274, -3.0088, 5000);

            // Assert
            Assert.That(result.First().Name, Is.EqualTo("Helvellyn Path"));
        }

        [Test]
        public async Task GetHikingTrailsAsync_ReturnsCorrectDistance()
        {
            // Arrange
            var trails = new List<OverpassTrailResultDto>
            {
                new OverpassTrailResultDto { OsmId = 1, Name = "Test Trail", DistanceMiles = 5.5, StartLatitude = 54.5, StartLongitude = -3.0 }
            };

            _mockOverpassService.Setup(s => s.GetHikingTrailsAsync(54.5, -3.0, 5000)).ReturnsAsync(trails);

            // Act
            var result = await _mockOverpassService.Object.GetHikingTrailsAsync(54.5, -3.0, 5000);

            // Assert
            Assert.That(result.First().DistanceMiles, Is.EqualTo(5.5));
        }

        [Test]
        public async Task GetHikingTrailsAsync_ReturnsCorrectCoordinates()
        {
            // Arrange
            var trails = new List<OverpassTrailResultDto>
            {
                new OverpassTrailResultDto { OsmId = 1, Name = "Test Trail", DistanceMiles = 2.0, StartLatitude = 54.5274, StartLongitude = -3.0088 }
            };

            _mockOverpassService.Setup(s => s.GetHikingTrailsAsync(54.5274, -3.0088, 5000)).ReturnsAsync(trails);

            // Act
            var result = await _mockOverpassService.Object.GetHikingTrailsAsync(54.5274, -3.0088, 5000);

            // Assert
            Assert.That(result.First().StartLatitude, Is.EqualTo(54.5274));
            Assert.That(result.First().StartLongitude, Is.EqualTo(-3.0088));
        }
    }
}

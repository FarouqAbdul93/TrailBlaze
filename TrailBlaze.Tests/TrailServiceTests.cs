using Moq;
using NUnit.Framework;
using TrailBlaze.API.Controllers;
using TrailBlaze.API.DataTransferObjects;
using TrailBlaze.API.Models;
using TrailBlaze.API.Repositories;
using TrailBlaze.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace TrailBlaze.Tests
{
    [TestFixture]
    public class TrailServiceTests
    {
        private Mock<ITrailRepository> _mockTrailRepository;
        private Mock<IReviewRepository> _mockReviewRepository;
        private Mock<IOverpassService> _mockOverpassService;
        private Mock<IGeocodingService> _mockGeocodingService;
        private TrailsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockTrailRepository = new Mock<ITrailRepository>();
            _mockReviewRepository = new Mock<IReviewRepository>();
            _mockOverpassService = new Mock<IOverpassService>();
            _mockGeocodingService = new Mock<IGeocodingService>();

            _controller = new TrailsController(
                _mockTrailRepository.Object,
                _mockReviewRepository.Object,
                _mockOverpassService.Object,
                _mockGeocodingService.Object);
        }

        [Test]
        public async Task GetAllTrails_ReturnsOkResult_WithListOfTrails()
        {
            // Arrange
            var trails = new List<Trail>
            {
                new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5, Latitude = 54.5274, Longitude = -3.0088, Reviews = new List<Review>() },
                new Trail { TrailId = 2, Name = "Snowdon", Location = "Snowdonia", Difficulty = Difficulty.Moderate, DistanceMiles = 9.0, Latitude = 53.0685, Longitude = -4.0764, Reviews = new List<Review>() }
            };

            _mockTrailRepository.Setup(r => r.GetAllTrailsAsync(1, 20)).ReturnsAsync(trails);
            _mockTrailRepository.Setup(r => r.GetTotalTrailCountAsync()).ReturnsAsync(2);

            // Act
            var result = await _controller.GetAllTrails();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var value = okResult!.Value;
            var trailsProp = value!.GetType().GetProperty("trails")!.GetValue(value) as List<TrailDto>;
            Assert.That(trailsProp, Is.Not.Null);
            Assert.That(trailsProp!.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetTrailById_ReturnsOkResult_WhenTrailExists()
        {
            // Arrange
            var trail = new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5, Latitude = 54.5274, Longitude = -3.0088, Reviews = new List<Review>() };

            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(1)).ReturnsAsync(trail);

            // Act
            var result = await _controller.GetTrailById(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var returnedTrail = okResult!.Value as TrailDto;
            Assert.That(returnedTrail, Is.Not.Null);
            Assert.That(returnedTrail!.Name, Is.EqualTo("Helvellyn Summit"));
        }

        [Test]
        public async Task GetTrailById_ReturnsNotFound_WhenTrailDoesNotExist()
        {
            // Arrange
            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(999)).ReturnsAsync((Trail?)null);

            // Act
            var result = await _controller.GetTrailById(999);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateTrail_ReturnsCreatedResult_WithNewTrail()
        {
            // Arrange
            var createDto = new CreateTrailDto
            {
                Name = "New Trail",
                Description = "A lovely walk",
                Difficulty = "Easy",
                DistanceMiles = 3.0,
                Location = "Peak District",
                Latitude = 53.3872,
                Longitude = -1.8726
            };

            var createdTrail = new Trail
            {
                TrailId = 11,
                Name = "New Trail",
                Description = "A lovely walk",
                Difficulty = Difficulty.Easy,
                DistanceMiles = 3.0,
                Location = "Peak District",
                Latitude = 53.3872,
                Longitude = -1.8726,
                Reviews = new List<Review>()
            };

            _mockTrailRepository.Setup(r => r.CreateTrailAsync(It.IsAny<Trail>())).ReturnsAsync(createdTrail);

            // Act
            var result = await _controller.CreateTrail(createDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedTrail = createdResult!.Value as TrailDto;
            Assert.That(returnedTrail, Is.Not.Null);
            Assert.That(returnedTrail!.Name, Is.EqualTo("New Trail"));
        }

        [Test]
        public async Task GetAllTrails_ReturnsAverageRating_WhenReviewsExist()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review { ReviewId = 1, TrailId = 1, Rating = 4 },
                new Review { ReviewId = 2, TrailId = 1, Rating = 5 }
            };

            var trails = new List<Trail>
            {
                new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5, Latitude = 54.5274, Longitude = -3.0088, Reviews = reviews }
            };

            _mockTrailRepository.Setup(r => r.GetAllTrailsAsync(1, 20)).ReturnsAsync(trails);
            _mockTrailRepository.Setup(r => r.GetTotalTrailCountAsync()).ReturnsAsync(1);

            // Act
            var result = await _controller.GetAllTrails();

            // Assert
            var okResult = result as OkObjectResult;
            var value = okResult!.Value;
            var trailsProp = value!.GetType().GetProperty("trails")!.GetValue(value) as List<TrailDto>;
            Assert.That(trailsProp![0].AverageRating, Is.EqualTo(4.5));
        }
    }
}
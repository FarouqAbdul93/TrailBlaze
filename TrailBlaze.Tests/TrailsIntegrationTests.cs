using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TrailBlaze.API.Controllers;
using TrailBlaze.API.DataTransferObjects;
using TrailBlaze.API.Models;
using TrailBlaze.API.Repositories;
using TrailBlaze.API.Services;

namespace TrailBlaze.Tests
{
    [TestFixture]
    public class TrailsIntegrationTests
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
        public async Task GetAllTrails_ReturnsOk_WithCorrectNumberOfTrails()
        {
            // Arrange
            var trails = new List<Trail>
            {
                new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5, Latitude = 54.5274, Longitude = -3.0088, Reviews = new List<Review>() },
                new Trail { TrailId = 2, Name = "Snowdon", Location = "Snowdonia", Difficulty = Difficulty.Moderate, DistanceMiles = 9.0, Latitude = 53.0685, Longitude = -4.0764, Reviews = new List<Review>() },
                new Trail { TrailId = 3, Name = "Malham Cove", Location = "Yorkshire Dales", Difficulty = Difficulty.Easy, DistanceMiles = 4.5, Latitude = 54.0714, Longitude = -2.1568, Reviews = new List<Review>() }
            };

            _mockTrailRepository.Setup(r => r.GetAllTrailsAsync(1, 20)).ReturnsAsync(trails);
            _mockTrailRepository.Setup(r => r.GetTotalTrailCountAsync()).ReturnsAsync(3);

            // Act
            var result = await _controller.GetAllTrails();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var value = okResult!.Value;
            var trailsProp = value!.GetType().GetProperty("trails")!.GetValue(value) as List<TrailDto>;
            Assert.That(trailsProp!.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task GetAllTrails_ReturnsDifficultyAsString()
        {
            // Arrange
            var trails = new List<Trail>
            {
                new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5, Latitude = 54.5274, Longitude = -3.0088, Reviews = new List<Review>() }
            };

            _mockTrailRepository.Setup(r => r.GetAllTrailsAsync(1, 20)).ReturnsAsync(trails);
            _mockTrailRepository.Setup(r => r.GetTotalTrailCountAsync()).ReturnsAsync(1);

            // Act
            var result = await _controller.GetAllTrails();

            // Assert
            var okResult = result as OkObjectResult;
            var value = okResult!.Value;
            var trailsProp = value!.GetType().GetProperty("trails")!.GetValue(value) as List<TrailDto>;
            Assert.That(trailsProp![0].Difficulty, Is.EqualTo("Hard"));
        }

        [Test]
        public async Task GetAllTrails_ReturnsZeroAverageRating_WhenNoReviews()
        {
            // Arrange
            var trails = new List<Trail>
            {
                new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5, Latitude = 54.5274, Longitude = -3.0088, Reviews = new List<Review>() }
            };

            _mockTrailRepository.Setup(r => r.GetAllTrailsAsync(1, 20)).ReturnsAsync(trails);
            _mockTrailRepository.Setup(r => r.GetTotalTrailCountAsync()).ReturnsAsync(1);

            // Act
            var result = await _controller.GetAllTrails();

            // Assert
            var okResult = result as OkObjectResult;
            var value = okResult!.Value;
            var trailsProp = value!.GetType().GetProperty("trails")!.GetValue(value) as List<TrailDto>;
            Assert.That(trailsProp![0].AverageRating, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllTrails_ReturnsEmptyList_WhenNoTrailsExist()
        {
            // Arrange
            _mockTrailRepository.Setup(r => r.GetAllTrailsAsync(1, 20)).ReturnsAsync(new List<Trail>());
            _mockTrailRepository.Setup(r => r.GetTotalTrailCountAsync()).ReturnsAsync(0);

            // Act
            var result = await _controller.GetAllTrails();

            // Assert
            var okResult = result as OkObjectResult;
            var value = okResult!.Value;
            var trailsProp = value!.GetType().GetProperty("trails")!.GetValue(value) as List<TrailDto>;
            Assert.That(trailsProp!.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetTrailById_ReturnsCorrectLatitudeAndLongitude()
        {
            // Arrange
            var trail = new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5, Latitude = 54.5274, Longitude = -3.0088, Reviews = new List<Review>() };

            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(1)).ReturnsAsync(trail);

            // Act
            var result = await _controller.GetTrailById(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnedTrail = okResult!.Value as TrailDto;
            Assert.That(returnedTrail!.Latitude, Is.EqualTo(54.5274));
            Assert.That(returnedTrail.Longitude, Is.EqualTo(-3.0088));
        }

        [Test]
        public async Task GetTrailsByLocation_ReturnsOk_WithMatchingTrails()
        {
            // Arrange
            var trails = new List<Trail>
            {
                new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5, Latitude = 54.5274, Longitude = -3.0088, Reviews = new List<Review>() },
                new Trail { TrailId = 5, Name = "Scafell Pike", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.0, Latitude = 54.4542, Longitude = -3.2116, Reviews = new List<Review>() }
            };

            _mockTrailRepository.Setup(r => r.GetTrailsByLocationAsync("Lake District")).ReturnsAsync(trails);

            // Act
            var result = await _controller.GetTrailsByLocation("Lake District");

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var returnedTrails = okResult!.Value as List<TrailDto>;
            Assert.That(returnedTrails!.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetTrailsByDifficulty_ReturnsOk_WithMatchingTrails()
        {
            // Arrange
            var trails = new List<Trail>
            {
                new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5, Latitude = 54.5274, Longitude = -3.0088, Reviews = new List<Review>() },
                new Trail { TrailId = 7, Name = "Kinder Scout", Location = "Peak District", Difficulty = Difficulty.Hard, DistanceMiles = 9.0, Latitude = 53.3872, Longitude = -1.8726, Reviews = new List<Review>() }
            };

            _mockTrailRepository.Setup(r => r.GetTrailsByDifficultyAsync(Difficulty.Hard)).ReturnsAsync(trails);

            // Act
            var result = await _controller.GetTrailsByDifficulty("Hard");

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var returnedTrails = okResult!.Value as List<TrailDto>;
            Assert.That(returnedTrails!.Count, Is.EqualTo(2));
        }
    }
}
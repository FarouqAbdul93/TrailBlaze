using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TrailBlaze.API.Controllers;
using TrailBlaze.API.DataTransferObjects;
using TrailBlaze.API.Models;
using TrailBlaze.API.Repositories;

namespace TrailBlaze.Tests
{
    [TestFixture]
    public class ReviewsIntegrationTests
    {
        private Mock<IReviewRepository> _mockReviewRepository;
        private Mock<ITrailRepository> _mockTrailRepository;
        private ReviewsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockReviewRepository = new Mock<IReviewRepository>();
            _mockTrailRepository = new Mock<ITrailRepository>();
            _controller = new ReviewsController(_mockReviewRepository.Object, _mockTrailRepository.Object);
        }

        [Test]
        public async Task CreateReview_ReturnsCreated_WithValidData()
        {
            // Arrange
            var trail = new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5 };
            var createDto = new CreateReviewDto { TrailId = 1, Rating = 5, Comment = "Amazing trail!", ImageUrl = "" };
            var createdReview = new Review { ReviewId = 1, TrailId = 1, Rating = 5, Comment = "Amazing trail!", DatePosted = DateTime.Now };

            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(1)).ReturnsAsync(trail);
            _mockReviewRepository.Setup(r => r.CreateReviewAsync(It.IsAny<Review>())).ReturnsAsync(createdReview);

            // Act
            var result = await _controller.CreateReview(createDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
        }

        [Test]
        public async Task CreateReview_ReturnsNotFound_WhenTrailDoesNotExist()
        {
            // Arrange
            var createDto = new CreateReviewDto { TrailId = 999, Rating = 5, Comment = "Amazing!", ImageUrl = "" };
            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(999)).ReturnsAsync((Trail?)null);

            // Act
            var result = await _controller.CreateReview(createDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task CreateReview_ReturnsBadRequest_WhenRatingTooHigh()
        {
            // Arrange
            var trail = new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5 };
            var createDto = new CreateReviewDto { TrailId = 1, Rating = 6, Comment = "Amazing!", ImageUrl = "" };

            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(1)).ReturnsAsync(trail);

            // Act
            var result = await _controller.CreateReview(createDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CreateReview_ReturnsBadRequest_WhenRatingTooLow()
        {
            // Arrange
            var trail = new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5 };
            var createDto = new CreateReviewDto { TrailId = 1, Rating = 0, Comment = "Amazing!", ImageUrl = "" };

            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(1)).ReturnsAsync(trail);

            // Act
            var result = await _controller.CreateReview(createDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CreateReview_ReturnsCorrectTrailId()
        {
            // Arrange
            var trail = new Trail { TrailId = 2, Name = "Snowdon", Location = "Snowdonia", Difficulty = Difficulty.Moderate, DistanceMiles = 9.0 };
            var createDto = new CreateReviewDto { TrailId = 2, Rating = 4, Comment = "Great views!", ImageUrl = "" };
            var createdReview = new Review { ReviewId = 5, TrailId = 2, Rating = 4, Comment = "Great views!", DatePosted = DateTime.Now };

            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(2)).ReturnsAsync(trail);
            _mockReviewRepository.Setup(r => r.CreateReviewAsync(It.IsAny<Review>())).ReturnsAsync(createdReview);

            // Act
            var result = await _controller.CreateReview(createDto);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedReview = createdResult!.Value as ReviewDto;
            Assert.That(returnedReview!.TrailId, Is.EqualTo(2));
        }

        [Test]
        public async Task GetReviewsByTrailId_ReturnsNotFound_WhenTrailDoesNotExist()
        {
            // Arrange
            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(999)).ReturnsAsync((Trail?)null);

            // Act
            var result = await _controller.GetReviewsByTrailId(999);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task CreateReview_SavesImageUrl()
        {
            // Arrange
            var trail = new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5 };
            var createDto = new CreateReviewDto { TrailId = 1, Rating = 5, Comment = "Great!", ImageUrl = "https://res.cloudinary.com/test/image.jpg" };
            var createdReview = new Review { ReviewId = 6, TrailId = 1, Rating = 5, Comment = "Great!", ImageUrl = "https://res.cloudinary.com/test/image.jpg", DatePosted = DateTime.Now };

            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(1)).ReturnsAsync(trail);
            _mockReviewRepository.Setup(r => r.CreateReviewAsync(It.IsAny<Review>())).ReturnsAsync(createdReview);

            // Act
            var result = await _controller.CreateReview(createDto);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedReview = createdResult!.Value as ReviewDto;
            Assert.That(returnedReview!.ImageUrl, Is.EqualTo("https://res.cloudinary.com/test/image.jpg"));
        }
    }
}

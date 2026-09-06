using Moq;
using NUnit.Framework;
using TrailBlaze.API.Controllers;
using TrailBlaze.API.DataTransferObjects;
using TrailBlaze.API.Models;
using TrailBlaze.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace TrailBlaze.Tests
{
    [TestFixture]
    public class ReviewServiceTests
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
        public async Task GetReviewsByTrailId_ReturnsOkResult_WithReviews()
        {
            // Arrange
            var trail = new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5 };
            var reviews = new List<Review>
            {
                new Review { ReviewId = 1, TrailId = 1, Rating = 5, Comment = "Amazing!", DatePosted = DateTime.Now },
                new Review { ReviewId = 2, TrailId = 1, Rating = 4, Comment = "Great views!", DatePosted = DateTime.Now }
            };

            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(1)).ReturnsAsync(trail);
            _mockReviewRepository.Setup(r => r.GetReviewsByTrailIdAsync(1)).ReturnsAsync(reviews);

            // Act
            var result = await _controller.GetReviewsByTrailId(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var returnedReviews = okResult!.Value as IEnumerable<ReviewDto>;
            Assert.That(returnedReviews, Is.Not.Null);
            Assert.That(returnedReviews!.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetReviewsByTrailId_ReturnsEmptyList_WhenNoReviews()
        {
            // Arrange
            var trail = new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5 };

            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(1)).ReturnsAsync(trail);
            _mockReviewRepository.Setup(r => r.GetReviewsByTrailIdAsync(1)).ReturnsAsync(new List<Review>());

            // Act
            var result = await _controller.GetReviewsByTrailId(1);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            var returnedReviews = okResult!.Value as IEnumerable<ReviewDto>;
            Assert.That(returnedReviews!.Count(), Is.EqualTo(0));
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
        public async Task CreateReview_ReturnsCreatedResult_WithNewReview()
        {
            // Arrange
            var trail = new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5 };
            var createDto = new CreateReviewDto
            {
                TrailId = 1,
                Rating = 5,
                Comment = "Brilliant walk!",
                ImageUrl = ""
            };

            var createdReview = new Review
            {
                ReviewId = 3,
                TrailId = 1,
                Rating = 5,
                Comment = "Brilliant walk!",
                DatePosted = DateTime.Now
            };

            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(1)).ReturnsAsync(trail);
            _mockReviewRepository.Setup(r => r.CreateReviewAsync(It.IsAny<Review>())).ReturnsAsync(createdReview);

            // Act
            var result = await _controller.CreateReview(createDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedReview = createdResult!.Value as ReviewDto;
            Assert.That(returnedReview, Is.Not.Null);
            Assert.That(returnedReview!.Rating, Is.EqualTo(5));
        }

        [Test]
        public async Task CreateReview_ReturnsCorrectComment()
        {
            // Arrange
            var trail = new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5 };
            var createDto = new CreateReviewDto
            {
                TrailId = 1,
                Rating = 3,
                Comment = "Decent walk",
                ImageUrl = ""
            };

            var createdReview = new Review
            {
                ReviewId = 4,
                TrailId = 1,
                Rating = 3,
                Comment = "Decent walk",
                DatePosted = DateTime.Now
            };

            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(1)).ReturnsAsync(trail);
            _mockReviewRepository.Setup(r => r.CreateReviewAsync(It.IsAny<Review>())).ReturnsAsync(createdReview);

            // Act
            var result = await _controller.CreateReview(createDto);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedReview = createdResult!.Value as ReviewDto;
            Assert.That(returnedReview!.Comment, Is.EqualTo("Decent walk"));
        }

        [Test]
        public async Task CreateReview_ReturnsBadRequest_WhenRatingIsInvalid()
        {
            // Arrange
            var trail = new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5 };
            var createDto = new CreateReviewDto
            {
                TrailId = 1,
                Rating = 6,
                Comment = "Invalid rating",
                ImageUrl = ""
            };

            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(1)).ReturnsAsync(trail);

            // Act
            var result = await _controller.CreateReview(createDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
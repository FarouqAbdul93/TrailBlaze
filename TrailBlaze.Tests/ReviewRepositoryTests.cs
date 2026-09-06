using Moq;
using NUnit.Framework;
using TrailBlaze.API.Models;
using TrailBlaze.API.Repositories;

namespace TrailBlaze.Tests
{
    [TestFixture]
    public class ReviewRepositoryTests
    {
        private Mock<IReviewRepository> _mockReviewRepository;

        [SetUp]
        public void SetUp()
        {
            _mockReviewRepository = new Mock<IReviewRepository>();
        }

        [Test]
        public async Task GetReviewsByTrailIdAsync_ReturnsListOfReviews()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review { ReviewId = 1, TrailId = 1, Rating = 5, Comment = "Amazing trail!", DatePosted = DateTime.Now },
                new Review { ReviewId = 2, TrailId = 1, Rating = 4, Comment = "Great views!", DatePosted = DateTime.Now }
            };

            _mockReviewRepository.Setup(r => r.GetReviewsByTrailIdAsync(1)).ReturnsAsync(reviews);

            // Act
            var result = await _mockReviewRepository.Object.GetReviewsByTrailIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetReviewsByTrailIdAsync_ReturnsEmptyList_WhenNoReviews()
        {
            // Arrange
            _mockReviewRepository.Setup(r => r.GetReviewsByTrailIdAsync(999)).ReturnsAsync(new List<Review>());

            // Act
            var result = await _mockReviewRepository.Object.GetReviewsByTrailIdAsync(999);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task CreateReviewAsync_ReturnsCreatedReview()
        {
            // Arrange
            var newReview = new Review { TrailId = 1, Rating = 5, Comment = "Brilliant walk!", DatePosted = DateTime.Now };
            var createdReview = new Review { ReviewId = 3, TrailId = 1, Rating = 5, Comment = "Brilliant walk!", DatePosted = DateTime.Now };

            _mockReviewRepository.Setup(r => r.CreateReviewAsync(newReview)).ReturnsAsync(createdReview);

            // Act
            var result = await _mockReviewRepository.Object.CreateReviewAsync(newReview);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ReviewId, Is.EqualTo(3));
            Assert.That(result.Rating, Is.EqualTo(5));
            Assert.That(result.Comment, Is.EqualTo("Brilliant walk!"));
        }

        [Test]
        public async Task GetReviewsByTrailIdAsync_ReturnsCorrectTrailId()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review { ReviewId = 1, TrailId = 2, Rating = 3, Comment = "Decent walk", DatePosted = DateTime.Now }
            };

            _mockReviewRepository.Setup(r => r.GetReviewsByTrailIdAsync(2)).ReturnsAsync(reviews);

            // Act
            var result = await _mockReviewRepository.Object.GetReviewsByTrailIdAsync(2);

            // Assert
            Assert.That(result.First().TrailId, Is.EqualTo(2));
        }

        [Test]
        public async Task CreateReviewAsync_SetsCorrectRating()
        {
            // Arrange
            var newReview = new Review { TrailId = 1, Rating = 1, Comment = "Very difficult", DatePosted = DateTime.Now };
            var createdReview = new Review { ReviewId = 4, TrailId = 1, Rating = 1, Comment = "Very difficult", DatePosted = DateTime.Now };

            _mockReviewRepository.Setup(r => r.CreateReviewAsync(newReview)).ReturnsAsync(createdReview);

            // Act
            var result = await _mockReviewRepository.Object.CreateReviewAsync(newReview);

            // Assert
            Assert.That(result.Rating, Is.EqualTo(1));
        }
    }
}

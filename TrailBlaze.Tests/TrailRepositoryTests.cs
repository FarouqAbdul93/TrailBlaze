using Moq;
using NUnit.Framework;
using TrailBlaze.API.Models;
using TrailBlaze.API.Repositories;

namespace TrailBlaze.Tests
{
    [TestFixture]
    public class TrailRepositoryTests
    {
        private Mock<ITrailRepository> _mockTrailRepository;

        [SetUp]
        public void SetUp()
        {
            _mockTrailRepository = new Mock<ITrailRepository>();
        }

        [Test]
        public async Task GetAllTrailsAsync_ReturnsListOfTrails()
        {
            // Arrange
            var trails = new List<Trail>
            {
                new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5 },
                new Trail { TrailId = 2, Name = "Snowdon", Location = "Snowdonia", Difficulty = Difficulty.Moderate, DistanceMiles = 9.0 }
            };

            _mockTrailRepository.Setup(r => r.GetAllTrailsAsync()).ReturnsAsync(trails);

            // Act
            var result = await _mockTrailRepository.Object.GetAllTrailsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetTrailByIdAsync_ReturnsCorrectTrail()
        {
            // Arrange
            var trail = new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5 };

            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(1)).ReturnsAsync(trail);

            // Act
            var result = await _mockTrailRepository.Object.GetTrailByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Helvellyn Summit"));
            Assert.That(result.Location, Is.EqualTo("Lake District"));
        }

        [Test]
        public async Task GetTrailByIdAsync_ReturnsNull_WhenTrailNotFound()
        {
            // Arrange
            _mockTrailRepository.Setup(r => r.GetTrailByIdAsync(999)).ReturnsAsync((Trail?)null);

            // Act
            var result = await _mockTrailRepository.Object.GetTrailByIdAsync(999);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateTrailAsync_ReturnsCreatedTrail()
        {
            // Arrange
            var newTrail = new Trail { Name = "New Trail", Location = "Peak District", Difficulty = Difficulty.Easy, DistanceMiles = 3.0 };
            var createdTrail = new Trail { TrailId = 4, Name = "New Trail", Location = "Peak District", Difficulty = Difficulty.Easy, DistanceMiles = 3.0 };

            _mockTrailRepository.Setup(r => r.CreateTrailAsync(newTrail)).ReturnsAsync(createdTrail);

            // Act
            var result = await _mockTrailRepository.Object.CreateTrailAsync(newTrail);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TrailId, Is.EqualTo(4));
            Assert.That(result.Name, Is.EqualTo("New Trail"));
        }

        [Test]
        public async Task GetTrailsByLocationAsync_ReturnsMatchingTrails()
        {
            // Arrange
            var trails = new List<Trail>
            {
                new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5 },
                new Trail { TrailId = 5, Name = "Scafell Pike", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.0 }
            };

            _mockTrailRepository.Setup(r => r.GetTrailsByLocationAsync("Lake District")).ReturnsAsync(trails);

            // Act
            var result = await _mockTrailRepository.Object.GetTrailsByLocationAsync("Lake District");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(t => t.Location == "Lake District"), Is.True);
        }

        [Test]
        public async Task GetTrailsByDifficultyAsync_ReturnsMatchingTrails()
        {
            // Arrange
            var trails = new List<Trail>
            {
                new Trail { TrailId = 1, Name = "Helvellyn Summit", Location = "Lake District", Difficulty = Difficulty.Hard, DistanceMiles = 8.5 },
                new Trail { TrailId = 7, Name = "Kinder Scout", Location = "Peak District", Difficulty = Difficulty.Hard, DistanceMiles = 9.0 }
            };

            _mockTrailRepository.Setup(r => r.GetTrailsByDifficultyAsync(Difficulty.Hard)).ReturnsAsync(trails);

            // Act
            var result = await _mockTrailRepository.Object.GetTrailsByDifficultyAsync(Difficulty.Hard);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.All(t => t.Difficulty == Difficulty.Hard), Is.True);
        }
    }
}

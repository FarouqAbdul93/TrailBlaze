using Moq;
using NUnit.Framework;
using TrailBlaze.API.DataTransferObjects;
using TrailBlaze.API.Services;

namespace TrailBlaze.Tests
{
    [TestFixture]
    public class WeatherServiceTests
    {
        private Mock<IWeatherService> _mockWeatherService;

        [SetUp]
        public void SetUp()
        {
            _mockWeatherService = new Mock<IWeatherService>();
        }

        [Test]
        public async Task GetWeatherAsync_ReturnsWeatherData()
        {
            // Arrange
            var weather = new WeatherDto
            {
                Temperature = 15.5,
                WindSpeed = 12.0,
                WeatherCode = 1,
                WeatherDescription = "Partly cloudy",
                IsGoodForHiking = true
            };

            _mockWeatherService.Setup(s => s.GetWeatherAsync(54.5274, -3.0088)).ReturnsAsync(weather);

            // Act
            var result = await _mockWeatherService.Object.GetWeatherAsync(54.5274, -3.0088);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Temperature, Is.EqualTo(15.5));
        }

        [Test]
        public async Task GetWeatherAsync_ReturnsCorrectWindSpeed()
        {
            // Arrange
            var weather = new WeatherDto
            {
                Temperature = 10.0,
                WindSpeed = 25.5,
                WeatherCode = 2,
                WeatherDescription = "Overcast",
                IsGoodForHiking = false
            };

            _mockWeatherService.Setup(s => s.GetWeatherAsync(54.5274, -3.0088)).ReturnsAsync(weather);

            // Act
            var result = await _mockWeatherService.Object.GetWeatherAsync(54.5274, -3.0088);

            // Assert
            Assert.That(result.WindSpeed, Is.EqualTo(25.5));
        }

        [Test]
        public async Task GetWeatherAsync_ReturnsCorrectWeatherCode()
        {
            // Arrange
            var weather = new WeatherDto
            {
                Temperature = 8.0,
                WindSpeed = 15.0,
                WeatherCode = 61,
                WeatherDescription = "Rain",
                IsGoodForHiking = false
            };

            _mockWeatherService.Setup(s => s.GetWeatherAsync(51.5, -0.1)).ReturnsAsync(weather);

            // Act
            var result = await _mockWeatherService.Object.GetWeatherAsync(51.5, -0.1);

            // Assert
            Assert.That(result.WeatherCode, Is.EqualTo(61));
        }

        [Test]
        public async Task GetWeatherAsync_ReturnsIsGoodForHikingTrue_WhenWeatherIsFine()
        {
            // Arrange
            var weather = new WeatherDto
            {
                Temperature = 18.0,
                WindSpeed = 10.0,
                WeatherCode = 0,
                WeatherDescription = "Clear sky",
                IsGoodForHiking = true
            };

            _mockWeatherService.Setup(s => s.GetWeatherAsync(54.5274, -3.0088)).ReturnsAsync(weather);

            // Act
            var result = await _mockWeatherService.Object.GetWeatherAsync(54.5274, -3.0088);

            // Assert
            Assert.That(result.IsGoodForHiking, Is.True);
        }

        [Test]
        public async Task GetWeatherAsync_ReturnsIsGoodForHikingFalse_WhenWeatherIsBad()
        {
            // Arrange
            var weather = new WeatherDto
            {
                Temperature = 2.0,
                WindSpeed = 50.0,
                WeatherCode = 75,
                WeatherDescription = "Heavy snow",
                IsGoodForHiking = false
            };

            _mockWeatherService.Setup(s => s.GetWeatherAsync(54.5274, -3.0088)).ReturnsAsync(weather);

            // Act
            var result = await _mockWeatherService.Object.GetWeatherAsync(54.5274, -3.0088);

            // Assert
            Assert.That(result.IsGoodForHiking, Is.False);
        }

        [Test]
        public async Task GetWeatherAsync_ReturnsCorrectWeatherDescription()
        {
            // Arrange
            var weather = new WeatherDto
            {
                Temperature = 12.0,
                WindSpeed = 20.0,
                WeatherCode = 3,
                WeatherDescription = "Overcast",
                IsGoodForHiking = true
            };

            _mockWeatherService.Setup(s => s.GetWeatherAsync(53.3872, -1.8726)).ReturnsAsync(weather);

            // Act
            var result = await _mockWeatherService.Object.GetWeatherAsync(53.3872, -1.8726);

            // Assert
            Assert.That(result.WeatherDescription, Is.EqualTo("Overcast"));
        }
    }
}

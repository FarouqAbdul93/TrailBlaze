using System.Text.Json;
using TrailBlaze.API.DataTransferObjects;

namespace TrailBlaze.API.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherDto> GetWeatherAsync(double latitude, double longitude)
        {
            var url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            var currentWeather = data.GetProperty("current_weather");

            var temperature = currentWeather.GetProperty("temperature").GetDouble();
            var windSpeed = currentWeather.GetProperty("windspeed").GetDouble();
            var weatherCode = currentWeather.GetProperty("weathercode").GetInt32();

            var description = GetWeatherDescription(weatherCode);
            var isGoodForHiking = IsGoodForHiking(temperature, windSpeed, weatherCode);

            return new WeatherDto
            {
                Temperature = temperature,
                WindSpeed = windSpeed,
                WeatherCode = weatherCode,
                WeatherDescription = description,
                IsGoodForHiking = isGoodForHiking
            };
        }

        private static string GetWeatherDescription(int code) => code switch
        {
            0 => "Clear sky",
            1 or 2 or 3 => "Partly cloudy",
            45 or 48 => "Foggy",
            51 or 53 or 55 => "Drizzle",
            61 or 63 or 65 => "Rain",
            71 or 73 or 75 => "Snow",
            80 or 81 or 82 => "Rain showers",
            95 => "Thunderstorm",
            96 or 99 => "Thunderstorm with hail",
            _ => "Unknown"
        };

        private static bool IsGoodForHiking(double temperature, double windSpeed, int weatherCode)
        {
            if (weatherCode >= 61) return false;
            if (temperature < 0 || temperature > 35) return false;
            if (windSpeed > 50) return false;
            return true;
        }
    }
}
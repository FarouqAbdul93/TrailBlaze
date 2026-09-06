using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrailBlaze.API.Services
{
    public interface IGeocodingService
    {
        Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(string locationName);
    }

    public class GeocodingService : IGeocodingService
    {
        private readonly HttpClient _httpClient;

        public GeocodingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TrailBlazeApp/1.0");
        }

        public async Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(string locationName)
        {
            try
            {
                var encodedLocation = Uri.EscapeDataString(locationName + ", UK");
                var url = $"https://nominatim.openstreetmap.org/search?q={encodedLocation}&format=json&limit=1";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var results = JsonSerializer.Deserialize<List<NominatimResult>>(json);

                if (results == null || !results.Any())
                    return null;

                return (double.Parse(results[0].Lat), double.Parse(results[0].Lon));
            }
            catch
            {
                return null;
            }
        }
    }

    public class NominatimResult
    {
        [JsonPropertyName("lat")]
        public string Lat { get; set; } = string.Empty;

        [JsonPropertyName("lon")]
        public string Lon { get; set; } = string.Empty;

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = string.Empty;
    }
}
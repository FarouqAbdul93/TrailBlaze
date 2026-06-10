using System.Net.Http.Json;

namespace TrailBlaze.UI.Services
{
    public class TrailService
    {
        private readonly HttpClient _httpClient;

        public TrailService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TrailDto>?> GetTrailsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<TrailDto>>("api/Trails");
        }

        public async Task<TrailDto?> GetTrailByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<TrailDto>($"api/Trails/{id}");
        }

        public async Task<List<TrailDto>?> SearchTrailsAsync(string location)
        {
            return await _httpClient.GetFromJsonAsync<List<TrailDto>>($"api/Trails/search?location={location}");
        }

        public async Task<List<TrailDto>?> GetTrailsByDifficultyAsync(string difficulty)
        {
            return await _httpClient.GetFromJsonAsync<List<TrailDto>>($"api/Trails/difficulty?difficulty={difficulty}");
        }
    }

    public class TrailDto
    {
        public int TrailId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double DistanceMiles { get; set; }
        public string Difficulty { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}

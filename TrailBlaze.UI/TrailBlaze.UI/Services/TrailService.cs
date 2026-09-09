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

        public async Task<PagedTrailsDto?> GetTrailsPagedAsync(int pageNumber = 1, int pageSize = 20)
        {
            return await _httpClient.GetFromJsonAsync<PagedTrailsDto>($"api/Trails?pageNumber={pageNumber}&pageSize={pageSize}");
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

        public async Task<List<LiveTrailDto>?> LiveSearchTrailsAsync(string location)
        {
            return await _httpClient.GetFromJsonAsync<List<LiveTrailDto>>($"api/Trails/live-search?location={location}");
        }
    }

    public class PagedTrailsDto
    {
        public List<TrailDto> Trails { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool HasMore { get; set; }
    }

    public class TrailDto
    {
        public int TrailId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double DistanceMiles { get; set; }
        public string Difficulty { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double AverageRating { get; set; }
        public string RouteData { get; set; } = string.Empty;
    }

    public class LiveTrailDto
    {
        public long OsmId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double DistanceMiles { get; set; }
        public double StartLatitude { get; set; }
        public double StartLongitude { get; set; }
        public double EndLatitude { get; set; }
        public double EndLongitude { get; set; }
        public string RouteData { get; set; } = string.Empty;
    }
}
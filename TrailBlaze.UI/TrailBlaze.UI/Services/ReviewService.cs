using System.Net.Http.Json;

namespace TrailBlaze.UI.Services
{
    public class ReviewService
    {
        private readonly HttpClient _httpClient;

        public ReviewService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CreateReviewAsync(CreateReviewDto review)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Reviews", review);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<ReviewDto>?> GetReviewsByTrailIdAsync(int trailId)
        {
            return await _httpClient.GetFromJsonAsync<List<ReviewDto>>($"api/Reviews/{trailId}");
        }
    }

    public class CreateReviewDto
    {
        public int TrailId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public int TrailId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime DatePosted { get; set; }
    }
}

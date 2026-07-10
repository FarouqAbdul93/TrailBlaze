using System.Text.Json;
using TrailBlaze.API.DataTransferObjects;

namespace TrailBlaze.API.Services
{
    public class OverpassService : IOverpassService
    {
        private readonly HttpClient _httpClient;
        private const string OverpassUrl = "https://overpass.kumi.systems/api/interpreter";

        public OverpassService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestVersion = System.Net.HttpVersion.Version11;

            if (!_httpClient.DefaultRequestHeaders.UserAgent.Any())
            {
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TrailBlazeApp/1.0 (contact: ebrahim@example.com)");
            }
        }

        public async Task<IEnumerable<OverpassTrailResultDto>> GetHikingTrailsAsync(double latitude, double longitude, int radiusMeters)
        {
            var query = $@"[out:json][timeout:25];way[highway=path](around:{radiusMeters},{latitude},{longitude});out geom;";

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("data", query)
            });

            var response = await _httpClient.PostAsync(OverpassUrl, content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var overpassResponse = JsonSerializer.Deserialize<OverpassResponseDto>(json);

            var results = new List<OverpassTrailResultDto>();

            if (overpassResponse?.Elements == null)
            {
                return results;
            }

            foreach (var element in overpassResponse.Elements)
            {
                if (element.Geometry == null || element.Geometry.Count < 2)
                {
                    continue;
                }

                var name = element.Tags != null && element.Tags.TryGetValue("name", out var n)
                    ? n
                    : "Unnamed Trail";

                var distanceMiles = CalculateDistanceMiles(element.Geometry);

                var start = element.Geometry.First();
                var end = element.Geometry.Last();

                results.Add(new OverpassTrailResultDto
                {
                    OsmId = element.Id,
                    Name = name,
                    DistanceMiles = distanceMiles,
                    StartLatitude = start.Lat,
                    StartLongitude = start.Lon,
                    EndLatitude = end.Lat,
                    EndLongitude = end.Lon,
                    RouteData = JsonSerializer.Serialize(element.Geometry)
                });
            }

            return results;
        }

        private static double CalculateDistanceMiles(List<OverpassGeometryDto> points)
        {
            double totalMeters = 0;

            for (int i = 0; i < points.Count - 1; i++)
            {
                totalMeters += HaversineDistance(points[i].Lat, points[i].Lon, points[i + 1].Lat, points[i + 1].Lon);
            }

            return totalMeters / 1609.34; // convert meters to miles
        }

        private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadiusMeters = 6371000;

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadiusMeters * c;
        }

        private static double ToRadians(double degrees) => degrees * Math.PI / 180;
    }
}
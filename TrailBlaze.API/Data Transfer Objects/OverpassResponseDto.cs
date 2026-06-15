using System.Text.Json.Serialization;

namespace TrailBlaze.API.DataTransferObjects
{
    public class OverpassResponseDto
    {
        [JsonPropertyName("elements")]
        public List<OverpassElementDto> Elements { get; set; } = new();
    }

    public class OverpassElementDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("tags")]
        public Dictionary<string, string>? Tags { get; set; }

        [JsonPropertyName("lat")]
        public double? Lat { get; set; }

        [JsonPropertyName("lon")]
        public double? Lon { get; set; }

        [JsonPropertyName("geometry")]
        public List<OverpassGeometryDto>? Geometry { get; set; }
    }

    public class OverpassGeometryDto
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }
    }
}
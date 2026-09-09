namespace TrailBlaze.API.Models
{
    public class TrailRoute
    {
        public int TrailRouteId { get; set; }
        public int TrailId { get; set; }
        public string RouteData { get; set; } = string.Empty;
        public Trail Trail { get; set; } = null!;
    }
}

namespace TrailBlaze.API.Models
{
    public class Trail
    {
        public int TrailId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Difficulty Difficulty { get; set; }
        public double DistanceMiles { get; set; }
        public string Location { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string RouteData { get; set; } = string.Empty;
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }

    public enum Difficulty
    {
        Easy,
        Moderate,
        Hard
    }
}
namespace TrailBlaze.API.DataTransferObjects
{
    public class TrailDto
    {
        public int TrailId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public double DistanceMiles { get; set; }
        public string Location { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
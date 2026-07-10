namespace TrailBlaze.API.DataTransferObjects
{
    public class OverpassTrailResultDto
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
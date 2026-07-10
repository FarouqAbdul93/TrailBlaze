namespace TrailBlaze.API.DataTransferObjects
{
    public class WeatherDto
    {
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public int WeatherCode { get; set; }
        public string WeatherDescription { get; set; } = string.Empty;
        public bool IsGoodForHiking { get; set; }
    }
}
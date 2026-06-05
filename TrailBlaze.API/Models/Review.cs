namespace TrailBlaze.API.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int TrailId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime DatePosted { get; set; }
        public Trail Trail { get; set; } = null!;
    }
}
namespace TrailBlaze.API.DataTransferObjects
{
    public class CreateReviewDto
    {
        public int TrailId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
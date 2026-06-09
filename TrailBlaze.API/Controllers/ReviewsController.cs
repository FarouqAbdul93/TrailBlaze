using Microsoft.AspNetCore.Mvc;
using TrailBlaze.API.DataTransferObjects;
using TrailBlaze.API.Models;
using TrailBlaze.API.Repositories;

namespace TrailBlaze.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ITrailRepository _trailRepository;

        public ReviewsController(IReviewRepository reviewRepository, ITrailRepository trailRepository)
        {
            _reviewRepository = reviewRepository;
            _trailRepository = trailRepository;
        }

        // POST: api/reviews
        [HttpPost]
        public async Task<ActionResult<ReviewDto>> CreateReview(CreateReviewDto createReviewDto)
        {
            var trail = await _trailRepository.GetTrailByIdAsync(createReviewDto.TrailId);
            if (trail == null)
            {
                return NotFound("Trail not found.");
            }

            if (createReviewDto.Rating < 1 || createReviewDto.Rating > 5)
            {
                return BadRequest("Rating must be between 1 and 5.");
            }

            var review = new Review
            {
                TrailId = createReviewDto.TrailId,
                Comment = createReviewDto.Comment,
                Rating = createReviewDto.Rating,
                ImageUrl = createReviewDto.ImageUrl,
                DatePosted = DateTime.UtcNow
            };

            var createdReview = await _reviewRepository.CreateReviewAsync(review);

            var reviewDto = new ReviewDto
            {
                ReviewId = createdReview.ReviewId,
                TrailId = createdReview.TrailId,
                Comment = createdReview.Comment,
                Rating = createdReview.Rating,
                ImageUrl = createdReview.ImageUrl,
                DatePosted = createdReview.DatePosted
            };

            return CreatedAtAction(nameof(GetReviewsByTrailId), new { trailId = createdReview.TrailId }, reviewDto);
        }

        // GET: api/reviews/{trailId}
        [HttpGet("{trailId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByTrailId(int trailId)
        {
            var trail = await _trailRepository.GetTrailByIdAsync(trailId);
            if (trail == null)
            {
                return NotFound("Trail not found.");
            }

            var reviews = await _reviewRepository.GetReviewsByTrailIdAsync(trailId);

            var reviewDtos = reviews.Select(r => new ReviewDto
            {
                ReviewId = r.ReviewId,
                TrailId = r.TrailId,
                Comment = r.Comment,
                Rating = r.Rating,
                ImageUrl = r.ImageUrl,
                DatePosted = r.DatePosted
            });

            return Ok(reviewDtos);
        }
    }
}
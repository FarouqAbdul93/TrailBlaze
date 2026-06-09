using Microsoft.AspNetCore.Mvc;
using TrailBlaze.API.DataTransferObjects;
using TrailBlaze.API.Models;
using TrailBlaze.API.Repositories;

namespace TrailBlaze.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrailsController : ControllerBase
    {
        private readonly ITrailRepository _trailRepository;
        private readonly IReviewRepository _reviewRepository;

        public TrailsController(ITrailRepository trailRepository, IReviewRepository reviewRepository)
        {
            _trailRepository = trailRepository;
            _reviewRepository = reviewRepository;
        }

        // GET: api/trails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrailDto>>> GetAllTrails()
        {
            var trails = await _trailRepository.GetAllTrailsAsync();

            var trailDtos = trails.Select(t => new TrailDto
            {
                TrailId = t.TrailId,
                Name = t.Name,
                Description = t.Description,
                Difficulty = t.Difficulty.ToString(),
                DistanceMiles = t.DistanceMiles,
                Location = t.Location,
                Latitude = t.Latitude,
                Longitude = t.Longitude
            });

            return Ok(trailDtos);
        }
       
        // GET: api/trails/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TrailDto>> GetTrailById(int id)
        {
            var trail = await _trailRepository.GetTrailByIdAsync(id);

            if (trail == null)
            {
                return NotFound();
            }

            var trailDto = new TrailDto
            {
                TrailId = trail.TrailId,
                Name = trail.Name,
                Description = trail.Description,
                Difficulty = trail.Difficulty.ToString(),
                DistanceMiles = trail.DistanceMiles,
                Location = trail.Location,
                Latitude = trail.Latitude,
                Longitude = trail.Longitude
            };

            return Ok(trailDto);
        }

        // POST: api/trails
        [HttpPost]
        public async Task<ActionResult<TrailDto>> CreateTrail(CreateTrailDto createTrailDto)
        {
            if (!Enum.TryParse<Difficulty>(createTrailDto.Difficulty, true, out var difficulty))
            {
                return BadRequest("Invalid difficulty. Must be Easy, Moderate or Hard.");
            }

            var trail = new Trail
            {
                Name = createTrailDto.Name,
                Description = createTrailDto.Description,
                Difficulty = difficulty,
                DistanceMiles = createTrailDto.DistanceMiles,
                Location = createTrailDto.Location,
                Latitude = createTrailDto.Latitude,
                Longitude = createTrailDto.Longitude
            };

            var createdTrail = await _trailRepository.CreateTrailAsync(trail);

            var trailDto = new TrailDto
            {
                TrailId = createdTrail.TrailId,
                Name = createdTrail.Name,
                Description = createdTrail.Description,
                Difficulty = createdTrail.Difficulty.ToString(),
                DistanceMiles = createdTrail.DistanceMiles,
                Location = createdTrail.Location,
                Latitude = createdTrail.Latitude,
                Longitude = createdTrail.Longitude
            };

            return CreatedAtAction(nameof(GetTrailById), new { id = createdTrail.TrailId }, trailDto);
        }
        // GET: api/trails?location=
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TrailDto>>> GetTrailsByLocation([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return BadRequest("Location parameter is required.");
            }

            var trails = await _trailRepository.GetTrailsByLocationAsync(location);

            var trailDtos = trails.Select(t => new TrailDto
            {
                TrailId = t.TrailId,
                Name = t.Name,
                Description = t.Description,
                Difficulty = t.Difficulty.ToString(),
                DistanceMiles = t.DistanceMiles,
                Location = t.Location,
                Latitude = t.Latitude,
                Longitude = t.Longitude
            });

            return Ok(trailDtos);
        }
        // GET: api/trails/difficulty?difficulty=
        [HttpGet("difficulty")]
        public async Task<ActionResult<IEnumerable<TrailDto>>> GetTrailsByDifficulty([FromQuery] string difficulty)
        {
            if (string.IsNullOrWhiteSpace(difficulty))
            {
                return BadRequest("Difficulty parameter is required.");
            }

            if (!Enum.TryParse<Difficulty>(difficulty, true, out var difficultyEnum))
            {
                return BadRequest("Invalid difficulty. Must be Easy, Moderate or Hard.");
            }

            var trails = await _trailRepository.GetTrailsByDifficultyAsync(difficultyEnum);

            var trailDtos = trails.Select(t => new TrailDto
            {
                TrailId = t.TrailId,
                Name = t.Name,
                Description = t.Description,
                Difficulty = t.Difficulty.ToString(),
                DistanceMiles = t.DistanceMiles,
                Location = t.Location,
                Latitude = t.Latitude,
                Longitude = t.Longitude
            });

            return Ok(trailDtos);
        }

        // GET: api/trails/{id}/reviews
        [HttpGet("{id}/reviews")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsForTrail(int id)
        {
            var trail = await _trailRepository.GetTrailByIdAsync(id);
            if (trail == null)
            {
                return NotFound("Trail not found.");
            }

            var reviews = await _reviewRepository.GetReviewsByTrailIdAsync(id);

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
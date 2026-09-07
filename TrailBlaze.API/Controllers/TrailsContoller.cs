using Microsoft.AspNetCore.Mvc;
using TrailBlaze.API.DataTransferObjects;
using TrailBlaze.API.Models;
using TrailBlaze.API.Repositories;
using TrailBlaze.API.Services;

namespace TrailBlaze.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrailsController : ControllerBase
    {
        private readonly ITrailRepository _trailRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IOverpassService _overpassService;
        private readonly IGeocodingService _geocodingService;

        public TrailsController(ITrailRepository trailRepository, IReviewRepository reviewRepository, IOverpassService overpassService, IGeocodingService geocodingService)
        {
            _trailRepository = trailRepository;
            _reviewRepository = reviewRepository;
            _overpassService = overpassService;
            _geocodingService = geocodingService;
        }

        // GET: api/trails?pageNumber=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult> GetAllTrails([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var trails = await _trailRepository.GetAllTrailsAsync(pageNumber, pageSize);
            var totalCount = await _trailRepository.GetTotalTrailCountAsync();

            var trailDtos = new List<TrailDto>();
            foreach (var t in trails)
            {
                var averageRating = t.Reviews.Any() ? t.Reviews.Average(r => r.Rating) : 0;
                trailDtos.Add(new TrailDto
                {
                    TrailId = t.TrailId,
                    Name = t.Name,
                    Description = t.Description,
                    Difficulty = t.Difficulty.ToString(),
                    DistanceMiles = t.DistanceMiles,
                    Location = t.Location,
                    Latitude = t.Latitude,
                    Longitude = t.Longitude,
                    AverageRating = averageRating
                });
            }

            return Ok(new
            {
                trails = trailDtos,
                totalCount = totalCount,
                pageNumber = pageNumber,
                pageSize = pageSize,
                hasMore = (pageNumber * pageSize) < totalCount
            });
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
            var averageRating = trail.Reviews.Any() ? trail.Reviews.Average(r => r.Rating) : 0;
            var trailDto = new TrailDto
            {
                TrailId = trail.TrailId,
                Name = trail.Name,
                Description = trail.Description,
                Difficulty = trail.Difficulty.ToString(),
                DistanceMiles = trail.DistanceMiles,
                Location = trail.Location,
                Latitude = trail.Latitude,
                Longitude = trail.Longitude,
                AverageRating = averageRating,
                RouteData = trail.RouteData
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
                Longitude = createdTrail.Longitude,
                AverageRating = 0
            };
            return CreatedAtAction(nameof(GetTrailById), new { id = createdTrail.TrailId }, trailDto);
        }

        // PATCH: api/trails/{id}/routedata
        [HttpPatch("{id}/routedata")]
        public async Task<ActionResult> UpdateRouteData(int id, [FromBody] UpdateRouteDataDto updateDto)
        {
            var trail = await _trailRepository.GetTrailByIdAsync(id);
            if (trail == null)
            {
                return NotFound();
            }
            trail.RouteData = updateDto.RouteData;
            await _trailRepository.UpdateTrailAsync(id, trail);
            return NoContent();
        }

        // GET: api/trails/search?location=
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TrailDto>>> GetTrailsByLocation([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return BadRequest("Location parameter is required.");
            }
            var trails = await _trailRepository.GetTrailsByLocationAsync(location);
            var trailDtos = new List<TrailDto>();
            foreach (var t in trails)
            {
                var averageRating = t.Reviews.Any() ? t.Reviews.Average(r => r.Rating) : 0;
                trailDtos.Add(new TrailDto
                {
                    TrailId = t.TrailId,
                    Name = t.Name,
                    Description = t.Description,
                    Difficulty = t.Difficulty.ToString(),
                    DistanceMiles = t.DistanceMiles,
                    Location = t.Location,
                    Latitude = t.Latitude,
                    Longitude = t.Longitude,
                    AverageRating = averageRating
                });
            }
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
            var trailDtos = new List<TrailDto>();
            foreach (var t in trails)
            {
                var averageRating = t.Reviews.Any() ? t.Reviews.Average(r => r.Rating) : 0;
                trailDtos.Add(new TrailDto
                {
                    TrailId = t.TrailId,
                    Name = t.Name,
                    Description = t.Description,
                    Difficulty = t.Difficulty.ToString(),
                    DistanceMiles = t.DistanceMiles,
                    Location = t.Location,
                    Latitude = t.Latitude,
                    Longitude = t.Longitude,
                    AverageRating = averageRating
                });
            }
            return Ok(trailDtos);
        }

        // GET: api/trails/live-search?location=
        [HttpGet("live-search")]
        public async Task<ActionResult<IEnumerable<OverpassTrailResultDto>>> GetLiveTrailsByLocation([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return BadRequest("Location parameter is required.");
            }
            var coordinates = await _geocodingService.GetCoordinatesAsync(location);
            if (coordinates == null)
            {
                return NotFound($"Could not find coordinates for location: {location}");
            }
            var trails = await _overpassService.GetHikingTrailsAsync(
                coordinates.Value.Latitude,
                coordinates.Value.Longitude,
                5000);
            return Ok(trails);
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
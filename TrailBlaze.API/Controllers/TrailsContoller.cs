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

        public TrailsController(ITrailRepository trailRepository)
        {
            _trailRepository = trailRepository;
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
    }
}
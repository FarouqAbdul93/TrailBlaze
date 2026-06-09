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
    }
}
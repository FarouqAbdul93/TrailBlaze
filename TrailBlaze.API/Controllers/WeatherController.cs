using Microsoft.AspNetCore.Mvc;
using TrailBlaze.API.Services;

namespace TrailBlaze.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        // GET: api/weather?lat=&lon=
        [HttpGet]
        public async Task<ActionResult> GetWeather(double lat, double lon)
        {
            var weather = await _weatherService.GetWeatherAsync(lat, lon);
            return Ok(weather);
        }
    }
}
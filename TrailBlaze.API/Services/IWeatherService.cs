using TrailBlaze.API.DataTransferObjects;

namespace TrailBlaze.API.Services
{
    public interface IWeatherService
    {
        Task<WeatherDto> GetWeatherAsync(double latitude, double longitude);
    }
}
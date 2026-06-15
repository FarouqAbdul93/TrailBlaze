using TrailBlaze.API.DataTransferObjects;

namespace TrailBlaze.API.Services
{
    public interface IOverpassService
    {
        Task<IEnumerable<OverpassTrailResultDto>> GetHikingTrailsAsync(double latitude, double longitude, int radiusMeters);
    }
}
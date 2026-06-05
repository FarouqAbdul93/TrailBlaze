using TrailBlaze.API.Models;

namespace TrailBlaze.API.Repositories
{
    public interface ITrailRepository
    {
        Task<IEnumerable<Trail>> GetAllTrailsAsync();
        Task<Trail?> GetTrailByIdAsync(int id);
        Task<Trail> CreateTrailAsync(Trail trail);
        Task<Trail?> UpdateTrailAsync(int id, Trail trail);
        Task<bool> DeleteTrailAsync(int id);
    }
}
using TrailBlaze.API.Models;

namespace TrailBlaze.API.Repositories
{
    public interface IReviewRepository
    {
        Task<Review> CreateReviewAsync(Review review);
        Task<IEnumerable<Review>> GetReviewsByTrailIdAsync(int trailId);
    }
}
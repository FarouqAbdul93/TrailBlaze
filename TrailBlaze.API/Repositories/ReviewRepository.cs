using Microsoft.EntityFrameworkCore;
using TrailBlaze.API.Data_DbContext;
using TrailBlaze.API.Models;

namespace TrailBlaze.API.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly TrailBlazeDbContext _context;

        public ReviewRepository(TrailBlazeDbContext context)
        {
            _context = context;
        }

        public async Task<Review> CreateReviewAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<IEnumerable<Review>> GetReviewsByTrailIdAsync(int trailId)
        {
            return await _context.Reviews
                .Where(r => r.TrailId == trailId)
                .ToListAsync();
        }
    }
}
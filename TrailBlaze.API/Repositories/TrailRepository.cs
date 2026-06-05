using Microsoft.EntityFrameworkCore;
using TrailBlaze.API.Data_DbContext;
using TrailBlaze.API.Models;

namespace TrailBlaze.API.Repositories
{
    public class TrailRepository : ITrailRepository
    {
        private readonly TrailBlazeDbContext _context;

        public TrailRepository(TrailBlazeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Trail>> GetAllTrailsAsync()
        {
            return await _context.Trails.Include(t => t.Reviews).ToListAsync();
        }

        public async Task<Trail?> GetTrailByIdAsync(int id)
        {
            return await _context.Trails.Include(t => t.Reviews).FirstOrDefaultAsync(t => t.TrailId == id);
        }

        public async Task<Trail> CreateTrailAsync(Trail trail)
        {
            _context.Trails.Add(trail);
            await _context.SaveChangesAsync();
            return trail;
        }

        public async Task<Trail?> UpdateTrailAsync(int id, Trail trail)
        {
            var existing = await _context.Trails.FindAsync(id);
            if (existing == null) return null;

            existing.Name = trail.Name;
            existing.Description = trail.Description;
            existing.Difficulty = trail.Difficulty;
            existing.DistanceMiles = trail.DistanceMiles;
            existing.Location = trail.Location;
            existing.Latitude = trail.Latitude;
            existing.Longitude = trail.Longitude;
            existing.RouteData = trail.RouteData;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteTrailAsync(int id)
        {
            var trail = await _context.Trails.FindAsync(id);
            if (trail == null) return false;

            _context.Trails.Remove(trail);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
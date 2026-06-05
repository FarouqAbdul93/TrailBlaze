using Microsoft.EntityFrameworkCore;
using TrailBlaze.API.Models;

namespace TrailBlaze.API.Data_DbContext
{
    public class TrailBlazeDbContext : DbContext
    {
        public TrailBlazeDbContext(DbContextOptions<TrailBlazeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Trail> Trails { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
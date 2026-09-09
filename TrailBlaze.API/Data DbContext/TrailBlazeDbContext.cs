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
        public DbSet<TrailRoute> TrailRoutes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrailRoute>()
                .HasOne(tr => tr.Trail)
                .WithOne(t => t.TrailRoute)
                .HasForeignKey<TrailRoute>(tr => tr.TrailId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
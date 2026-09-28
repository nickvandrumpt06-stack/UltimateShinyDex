using Microsoft.EntityFrameworkCore;
using UltimateShinyDex.Api.Models;

namespace UltimateShinyDex.Api.Data
{
    public class ShinyDbContext : DbContext
    {
        public ShinyDbContext(DbContextOptions<ShinyDbContext> options)
            : base(options)
        {
        }

        public DbSet<Shiny> Shinies { get; set; }

        public DbSet<ShinyMark> ShinyMarks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShinyMark>()
                .HasOne(mark => mark.Shiny)
                .WithMany(shiny => shiny.Marks)
                .HasForeignKey(mark => mark.ShinyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
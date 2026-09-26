using Microsoft.EntityFrameworkCore;
using UltimateShinyDex.Api.Models;  

namespace UltimateShinyDex.Api.Data
{
    public class ShinyDbContext : DbContext
    {
        public ShinyDbContext(DbContextOptions<ShinyDbContext> options) : base(options)
        {
        }

        public DbSet<Shiny> Shinies { get; set; }
    }
}
using Microsoft.EntityFrameworkCore;
using UltimateShinyDex.Api.Models;

namespace UltimateShinyDex.Api.Data
{
    public class ShinyDbContext : DbContext
    {
        public ShinyDbContext(
            DbContextOptions<ShinyDbContext> options
        )
            : base(options)
        {
        }

        public DbSet<Shiny> Shinies { get; set; }

        public DbSet<ShinyMark> ShinyMarks { get; set; }

        public DbSet<DisabledDexSlot> DisabledDexSlots { get; set; }

        public DbSet<Hunt> Hunts { get; set; }

        public DbSet<HuntGame> HuntGames { get; set; }

        public DbSet<HuntEncounter> HuntEncounters { get; set; }

        public DbSet<HuntTarget> HuntTargets { get; set; }

        public DbSet<HuntPhase> HuntPhases { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder
        )
        {
            base.OnModelCreating(modelBuilder);

            // Shiny -> Marks
            modelBuilder.Entity<ShinyMark>()
                .HasOne(mark => mark.Shiny)
                .WithMany(shiny => shiny.Marks)
                .HasForeignKey(mark => mark.ShinyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Disabled Living Dex slots
            modelBuilder.Entity<DisabledDexSlot>()
                .HasIndex(d => new
                {
                    d.DexNumber,
                    d.Pokemon,
                    d.Form,
                    d.Gender
                })
                .IsUnique();

            // Hunt -> Games
            modelBuilder.Entity<HuntGame>()
                .HasOne(game => game.Hunt)
                .WithMany(hunt => hunt.Games)
                .HasForeignKey(game => game.HuntId)
                .OnDelete(DeleteBehavior.Cascade);

            // HuntGame -> Encounter Pool
            modelBuilder.Entity<HuntEncounter>()
                .HasOne(encounter => encounter.HuntGame)
                .WithMany(game => game.Encounters)
                .HasForeignKey(encounter => encounter.HuntGameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HuntEncounter>()
                .Property(encounter => encounter.EncounterPercentage)
                .HasPrecision(5, 2);

            // Hunt -> Targets
            modelBuilder.Entity<HuntTarget>()
                .HasOne(target => target.Hunt)
                .WithMany(hunt => hunt.Targets)
                .HasForeignKey(target => target.HuntId)
                .OnDelete(DeleteBehavior.Cascade);

            // Hunt -> Phases
            modelBuilder.Entity<HuntPhase>()
                .HasOne(phase => phase.Hunt)
                .WithMany(hunt => hunt.Phases)
                .HasForeignKey(phase => phase.HuntId)
                .OnDelete(DeleteBehavior.Cascade);

            // A completed phase can point to the shiny
            // that was found during that phase.
            modelBuilder.Entity<HuntPhase>()
                .HasOne(phase => phase.FoundShiny)
                .WithOne()
                .HasForeignKey<HuntPhase>(
                    phase => phase.FoundShinyId
                )
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
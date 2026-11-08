namespace UltimateShinyDex.Api.Models
{
    public class Hunt
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Status { get; set; } = "Active";

        // Some hunts, such as Scarlet/Violet overworld hunts,
        // do not use an encounter counter.
        public bool UsesCounter { get; set; } = true;

        // How much the + and - buttons change the counter by.
        public int CounterStep { get; set; } = 1;

        public int CurrentEncounters { get; set; } = 0;

        public int TotalEncounters { get; set; } = 0;

        public int CurrentPhaseNumber { get; set; } = 1;

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public string? Notes { get; set; }

        public ICollection<HuntGame> Games { get; set; }
            = new List<HuntGame>();

        public ICollection<HuntTarget> Targets { get; set; }
            = new List<HuntTarget>();

        public ICollection<HuntPhase> Phases { get; set; }
            = new List<HuntPhase>();
    }
}
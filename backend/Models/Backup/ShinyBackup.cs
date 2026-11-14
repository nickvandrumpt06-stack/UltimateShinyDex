namespace UltimateShinyDex.Api.Models.Backup
{
    public class ShinyBackup
    {
        public int Version { get; set; } = 3;

        public DateTime ExportedAt { get; set; }
            = DateTime.UtcNow;

        public List<ShinyBackupItem> Shinies { get; set; }
            = new();

        public List<DisabledDexSlotBackupItem> DisabledDexSlots { get; set; }
            = new();

        public List<HuntBackupItem> Hunts { get; set; }
            = new();
    }

    public class ShinyBackupItem
    {
        // Used by version 3 backups so HuntPhases can
        // reconnect to the correct shiny after import.
        public int SourceId { get; set; }

        public string Pokemon { get; set; }
            = string.Empty;

        public string? Nickname { get; set; }

        public string Nature { get; set; }
            = string.Empty;

        public string Game { get; set; }
            = string.Empty;

        public string Ball { get; set; }
            = string.Empty;

        public string Method { get; set; }
            = string.Empty;

        public int? Encounters { get; set; }

        public bool IsAlpha { get; set; }

        public string? Gender { get; set; }

        public string? Form { get; set; }

        public List<string> Marks { get; set; }
            = new();
    }

    public class DisabledDexSlotBackupItem
    {
        public int DexNumber { get; set; }

        public string Pokemon { get; set; }
            = string.Empty;

        public string? Form { get; set; }

        public string? Gender { get; set; }
    }

    public class HuntBackupItem
    {
        public string Name { get; set; }
            = string.Empty;

        public string Status { get; set; }
            = "Active";

        public bool UsesCounter { get; set; }

        public int CounterStep { get; set; } = 1;

        public int CurrentEncounters { get; set; }

        public int TotalEncounters { get; set; }

        public int CurrentPhaseNumber { get; set; } = 1;

        public DateTime StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string? Notes { get; set; }

        public List<HuntGameBackupItem> Games { get; set; }
            = new();

        public List<HuntTargetBackupItem> Targets { get; set; }
            = new();

        public List<HuntPhaseBackupItem> Phases { get; set; }
            = new();
    }

    public class HuntGameBackupItem
    {
        public string Game { get; set; }
            = string.Empty;

        public string? Label { get; set; }

        public int? ShinyOdds { get; set; }

        public List<HuntEncounterBackupItem> Encounters { get; set; }
            = new();
    }

    public class HuntEncounterBackupItem
    {
        public string Pokemon { get; set; }
            = string.Empty;

        public string? Form { get; set; }

        public decimal? EncounterPercentage { get; set; }
    }

    public class HuntTargetBackupItem
    {
        public string Pokemon { get; set; }
            = string.Empty;

        public string? Form { get; set; }

        public string? Gender { get; set; }

        public string? Nature { get; set; }

        public bool? IsAlpha { get; set; }

        public string? Mark { get; set; }
    }

    public class HuntPhaseBackupItem
    {
        public int PhaseNumber { get; set; }

        public int? Encounters { get; set; }

        // Refers to ShinyBackupItem.SourceId.
        public int? FoundShinySourceId { get; set; }

        public bool WasTarget { get; set; }

        public DateTime CompletedAt { get; set; }
    }
}
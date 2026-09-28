namespace UltimateShinyDex.Api.Models.Backup
{
    public class ShinyBackup
    {
        public int Version { get; set; } = 1;
        public DateTime ExportedAt { get; set; } = DateTime.UtcNow;
        public List<ShinyBackupItem> Shinies { get; set; } = new();
    }

    public class ShinyBackupItem
    {
        public string Pokemon { get; set; } = string.Empty;
        public string? Nickname { get; set; }
        public string Nature { get; set; } = string.Empty;
        public string Game { get; set; } = string.Empty;
        public string Ball { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public int? Encounters { get; set; }
        public bool IsAlpha { get; set; }
        public string? Gender { get; set; }
        public string? Form { get; set; }

        public List<string> Marks { get; set; } = new();
    }
}
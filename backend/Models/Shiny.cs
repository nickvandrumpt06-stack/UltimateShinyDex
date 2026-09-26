namespace UltimateShinyDex.Api.Models
{
    public class Shiny
    {
        public int Id { get; set; }
        public string Pokemon { get; set; } = string.Empty;
        public string? Nickname { get; set; }
        public string Nature { get; set; } = string.Empty;
        public string Game { get; set; } = string.Empty;
        public string Ball { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public int? Encounters { get; set; }
        public bool IsAlpha { get; set; }
        public string? Mark { get; set; }
        public string? Gender { get; set; }
    }
}
namespace UltimateShinyDex.Api.Models
{
    public class DisabledDexSlot
    {
        public int Id { get; set; }

        public int DexNumber { get; set; }

        public string Pokemon { get; set; } = string.Empty;

        public string? Form { get; set; }

        public string? Gender { get; set; }
    }
}
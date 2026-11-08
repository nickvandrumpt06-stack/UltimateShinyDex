using System.Text.Json.Serialization;

namespace UltimateShinyDex.Api.Models
{
    public class HuntGame
    {
        public int Id { get; set; }

        public int HuntId { get; set; }

        [JsonIgnore]
public Hunt? Hunt { get; set; }

        public string Game { get; set; } = string.Empty;

        // Optional label for multiple copies of the same game.
        // Example: "HG #1", "HG #2", "3DS", etc.
        public string? Label { get; set; }

        // 4096 means 1 / 4096.
        // Nullable because some hunts may not care about odds.
        public int? ShinyOdds { get; set; }

        public ICollection<HuntEncounter> Encounters { get; set; }
            = new List<HuntEncounter>();
    }
}
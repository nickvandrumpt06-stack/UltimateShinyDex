using System.Text.Json.Serialization;

namespace UltimateShinyDex.Api.Models
{
    public class HuntPhase
    {
        public int Id { get; set; }

        public int HuntId { get; set; }

        [JsonIgnore]
public Hunt? Hunt { get; set; }

        public int PhaseNumber { get; set; }

        // Null when the hunt does not use a counter.
        public int? Encounters { get; set; }

        public int? FoundShinyId { get; set; }

        public Shiny? FoundShiny { get; set; }

        // True when the user classified the found shiny
        // as one of the hunt's targets.
        public bool WasTarget { get; set; }

        public DateTime CompletedAt { get; set; }
            = DateTime.UtcNow;
    }
}
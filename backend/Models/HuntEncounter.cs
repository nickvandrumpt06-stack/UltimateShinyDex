using System.Text.Json.Serialization;

namespace UltimateShinyDex.Api.Models
{
    public class HuntEncounter
    {
        public int Id { get; set; }

        public int HuntGameId { get; set; }

        [JsonIgnore]
public HuntGame? HuntGame { get; set; }

        public string Pokemon { get; set; } = string.Empty;

        public string? Form { get; set; }

        public decimal? EncounterPercentage { get; set; }
    }
}
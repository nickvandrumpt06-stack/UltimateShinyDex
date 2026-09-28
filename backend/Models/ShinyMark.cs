using System.Text.Json.Serialization;

namespace UltimateShinyDex.Api.Models
{
    public class ShinyMark
    {
        public int Id { get; set; }

        public string MarkName { get; set; } = string.Empty;

        public int ShinyId { get; set; }

        [JsonIgnore]
        public Shiny? Shiny { get; set; }
    }
}
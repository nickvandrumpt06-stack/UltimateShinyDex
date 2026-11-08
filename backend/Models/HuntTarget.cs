using System.Text.Json.Serialization;

namespace UltimateShinyDex.Api.Models
{
    public class HuntTarget
    {
        public int Id { get; set; }

        public int HuntId { get; set; }

        [JsonIgnore]
public Hunt? Hunt { get; set; }

        public string Pokemon { get; set; } = string.Empty;

        public string? Form { get; set; }

        public string? Gender { get; set; }

        public string? Nature { get; set; }

        public bool? IsAlpha { get; set; }

        public string? Mark { get; set; }
    }
}
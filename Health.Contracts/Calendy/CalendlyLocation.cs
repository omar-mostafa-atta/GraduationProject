using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health.Contracts.Calendy
{
    public class CalendlyLocation
    {
        [JsonPropertyName("join_url")] public string? JoinUrl { get; set; }
        [JsonPropertyName("type")] public string Type { get; set; } = "";
    }
}

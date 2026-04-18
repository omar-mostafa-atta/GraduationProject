using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health.Contracts.Calendy
{
    public class CalendlyEventType
    {
        [JsonPropertyName("uri")] public string Uri { get; set; } = "";
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("duration")] public int Duration { get; set; }
        [JsonPropertyName("scheduling_url")] public string SchedulingUrl { get; set; } = "";
        [JsonPropertyName("description_plain")] public string? Description { get; set; }
        [JsonPropertyName("active")] public bool Active { get; set; }
    }
}

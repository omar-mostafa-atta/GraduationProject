using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health.Contracts.Calendy
{
    public class CalendlyUserResource
    {
        [JsonPropertyName("uri")] public string Uri { get; set; } = "";
        [JsonPropertyName("scheduling_url")] public string SchedulingUrl { get; set; } = "";
        [JsonPropertyName("current_organization")] public string CurrentOrganization { get; set; } = "";
        [JsonPropertyName("email")] public string Email { get; set; } = "";
        [JsonPropertyName("name")] public string Name { get; set; } = "";
    }
}

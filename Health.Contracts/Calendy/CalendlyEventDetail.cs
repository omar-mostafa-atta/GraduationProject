using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health.Contracts.Calendy
{
    public class CalendlyEventDetail
    {
        [JsonPropertyName("uri")] public string Uri { get; set; } = "";
        [JsonPropertyName("start_time")] public DateTime StartTime { get; set; }
        [JsonPropertyName("end_time")] public DateTime EndTime { get; set; }
        [JsonPropertyName("location")] public CalendlyLocation? Location { get; set; }
        [JsonPropertyName("event_memberships")]
        public List<EventMembership> EventMemberships { get; set; } = new();
    }
}

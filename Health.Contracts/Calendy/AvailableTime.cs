using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health.Contracts.Calendy
{
    public class AvailableTime
    {
        [JsonPropertyName("start_time")] public DateTime StartTime { get; set; }
        [JsonPropertyName("status")] public string Status { get; set; } = "";
        [JsonPropertyName("invitees_remaining")] public int InviteesRemaining { get; set; }
    }
}

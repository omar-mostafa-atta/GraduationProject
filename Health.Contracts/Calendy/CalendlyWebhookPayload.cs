using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health.Contracts.Calendy
{
    public class CalendlyWebhookPayload
    {
        [JsonPropertyName("event")] public string Event { get; set; } = "";
        [JsonPropertyName("payload")] public CalendlyInviteePayload Payload { get; set; } = new();
    }
}

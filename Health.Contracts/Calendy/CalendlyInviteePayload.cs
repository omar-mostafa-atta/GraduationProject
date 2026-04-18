using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health.Contracts.Calendy
{
    public class CalendlyInviteePayload
    {
        [JsonPropertyName("email")] public string Email { get; set; } = "";
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("event")] public CalendlyEventDetail Event { get; set; } = new();
        [JsonPropertyName("questions_and_answers")]
        public List<QnA> QuestionsAndAnswers { get; set; } = new();
    }
}

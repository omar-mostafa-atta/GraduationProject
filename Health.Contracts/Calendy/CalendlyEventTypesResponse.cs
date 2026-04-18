using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health.Contracts.Calendy
{
    public class CalendlyEventTypesResponse
    {
        [JsonPropertyName("collection")] public List<CalendlyEventType> Collection { get; set; } = new();
    }
}

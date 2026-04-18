using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health.Contracts.Calendy
{
    public class CalendlyUserResponse
    {
        [JsonPropertyName("resource")] public CalendlyUserResource Resource { get; set; } = new();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health.Contracts.Calendy
{
    public class EventMembership
    {
        [JsonPropertyName("user")] public string UserUri { get; set; } = "";
    }
}

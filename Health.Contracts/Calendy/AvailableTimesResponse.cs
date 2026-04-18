using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health.Contracts.Calendy
{
    public class AvailableTimesResponse
    {
        [JsonPropertyName("collection")] public List<AvailableTime> Collection { get; set; } = new();
    }
}

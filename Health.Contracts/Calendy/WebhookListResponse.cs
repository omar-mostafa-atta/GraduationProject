using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Health.Contracts.Calendy
{
    public class WebhookListResponse
    {
        [JsonPropertyName("collection")]
        public List<WebhookItem> Collection { get; set; } = new();
    }
    public class WebhookItem
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; } = "";

        [JsonPropertyName("callback_url")]
        public string CallbackUrl { get; set; } = "";
    }
}

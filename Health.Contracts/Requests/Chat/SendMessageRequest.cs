using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Contracts.Requests.Chat
{
    public class SendMessageRequest
    {
        public Guid ReceiverId { get; set; }
        public string MessageContent { get; set; }
    }
}

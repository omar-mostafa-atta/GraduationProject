using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Contracts.Responses.Chat
{
    public class ChatResponse
    {

        public Guid Id { get; set; }
        public Guid OtherUserId { get; set; }
        public string OtherUserName { get; set; }
        public string OtherUserProfilePicture { get; set; }
        public MessageResponse LastMessage { get; set; }
        public int UnreadCount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Models
{
    public class Chat
    {
        [Key]
        public Guid Id { get; set; }
        public Guid FirstUserId { get; set; }
        public User FirstUser { get; set; }
        public Guid SecondUserId { get; set; }
        public User SecondUser { get; set; }
        public List<Message> Messages { get; set; } = new();
    }
}

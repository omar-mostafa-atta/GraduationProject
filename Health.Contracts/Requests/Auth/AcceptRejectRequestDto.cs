using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Contracts.Requests.Auth
{
    public class AcceptRejectRequestDto
    {
        public string UserId { get; set; }
        public bool IsAccepted { get; set; }
    }
}

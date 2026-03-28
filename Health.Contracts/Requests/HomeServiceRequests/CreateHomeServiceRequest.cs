using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Contracts.Requests.HomeServiceRequests
{
    public class CreateHomeServiceRequest
    {
        [Required]
        public string ServiceDescription { get; set; }

        [Required]
        public DateTime RequestedTime { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public Guid? NurseId { get; set; }
    }
}

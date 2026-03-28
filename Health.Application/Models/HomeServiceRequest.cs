
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Health.Contracts.Enums;

namespace Health.Application.Models
{
    public class HomeServiceRequest
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }

        public Guid? NurseId { get; set; }
        public Nurse Nurse { get; set; }

        public string ServiceDescription { get; set; }
        public DateTime RequestedTime { get; set; }
        public string Address { get; set; }
        public HomeServiceStatus Status { get; set; }// momkn tkon pending | accepted | completed | Rejected (ظبطها) 
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Models
{
    public class Nurse : UserBase
    {
        [Key]
        public Guid Id { get; set; }
        public User User { get; set; }

        public string LicenseNumber { get; set; }
        public string Specialization { get; set; }
        public int ExperienceYears { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public int CompletedRequests { get; set; } = 0;

        public string? Government { get; set; }=null;

        public NurseStatus Status { get; set; }= NurseStatus.Pending;

        public ICollection<HomeServiceRequest> HomeServiceRequests { get; set; }
    }
    public enum NurseStatus
    {
        Pending,
        Approved,
        Rejected
    }

}

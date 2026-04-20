using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Contracts.Responses.Admin
{
    public class PendingDoctorDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string LicenseNumber { get; set; }
        public string WorkPlace { get; set; }
        public int ExperienceYears { get; set; }

        public DoctorStatus Status { get; set; }
    }
    public enum DoctorStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
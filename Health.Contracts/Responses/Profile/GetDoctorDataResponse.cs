using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Contracts.Responses.Profile
{
    public class GetDoctorDataResponse
    {
         public Guid Id { get; set; }

        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }
        public string? Bio { get; set; } 
        public string PhoneNumber { get; set; }
        public string? AvailabilitySchedule { get; set; }
        public int ExperienceYears { get; set; }

        public string? WorkPlace { get; set; }
    }
}

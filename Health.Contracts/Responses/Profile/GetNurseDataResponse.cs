using Health.Contracts.Responses.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Contracts.Responses.Profile
{
    public class GetNurseDataResponse
    {
        public Guid Id { get; set; }
        public string LicenseNumber { get; set; }
        public string Specialization { get; set; }
        public int ExperienceYears { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public int CompletedRequests { get; set; }

        public string? Government { get; set; }
    }

    
}

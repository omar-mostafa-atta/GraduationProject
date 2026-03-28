using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Contracts.Requests.Auth
{
    public class RegisterDoctorRequestDto : RegisterBaseDto
    {

        [Required]
        public string Specialization { get; set; }

        [Required]
        public string LicenseNumber { get; set; }

        public string? Bio { get; set; } // Nullable

        [Required]
        public string DoctorPhoneNumber { get; set; } 

        [Required]
        public string AvailabilitySchedule { get; set; }
    }
}

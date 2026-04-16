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

        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string WorkPlace { get; set; }
        [Required]
        public int ExperienceYears { get; set; }
    }
}

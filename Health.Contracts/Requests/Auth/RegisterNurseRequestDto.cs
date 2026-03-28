using System.ComponentModel.DataAnnotations;

namespace Health.Contracts.Requests.Auth
{
    public class RegisterNurseRequestDto: RegisterBaseDto
    {
        [Required]
        public string LicenseNumber { get; set; }
        
   
        public string Specialization { get; set; }

        [Required]
        public int ExperienceYears { get; set; }
        [Required]
        public string NursePhoneNumber { get; set; }
        [Required]
        public bool IsActive { get; set; }=true;

    }
}

using System.ComponentModel.DataAnnotations;

namespace Health.Contracts.Requests.Auth
{
    public class RegisterRequestDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

       
        [Required]
        [RegularExpression("^(Patient|Doctor|Nurse)$", ErrorMessage = "Role must be 'Patient', 'Doctor', or 'Nurse'.")]
        public string Role { get; set; }
    }
}


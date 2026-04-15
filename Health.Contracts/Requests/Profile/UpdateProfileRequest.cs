using System.ComponentModel.DataAnnotations;

namespace Health.Contracts.Requests.Profile
{
    public class UpdateProfileRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? ProfilePictureUrl { get; set; }
    }

    public class UpdatePatientProfileRequest : UpdateProfileRequest
    {
        [RegularExpression("^(Male|Female|male|female)$", ErrorMessage = "Gender must be 'Male' or 'Female'.")]
        //public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public int? SystolicPressure { get; set; }
        public int? DiastolicPressure { get; set; }
        public int? HeartRate { get; set; }
        public int? Sugar { get; set; }
    }
}
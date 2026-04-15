using System.ComponentModel.DataAnnotations;

namespace Health.Contracts.Requests.Auth
{
    public class RegisterPatientRequestDto: RegisterBaseDto
    {
       

        [RegularExpression("^(Male|Female|male|female)$", ErrorMessage = "Gender must be 'Male' or 'Female'.")]
        public string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string PhoneNumber {  get; set; }


    }
}


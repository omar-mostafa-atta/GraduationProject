using System.ComponentModel.DataAnnotations;

namespace Health.Contracts.Requests.Appointments
{
    public class CreateAppointmentRequest
    {
        [Required]
        public Guid DoctorId { get; set; }

        [Required]
        public DateTime AppointmentTime { get; set; }

        [Required]
        [RegularExpression("^(video|in_person|message)$")]
        public string Type { get; set; }

        public string? Notes { get; set; }
    }
}
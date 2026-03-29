using System.ComponentModel.DataAnnotations;

namespace Health.Contracts.Requests.Appointments
{
    public class RescheduleAppointmentRequest
    {
        [Required]
        public DateTime NewAppointmentTime { get; set; }

        [Required]
        public string RescheduleReason { get; set; }
    }
}
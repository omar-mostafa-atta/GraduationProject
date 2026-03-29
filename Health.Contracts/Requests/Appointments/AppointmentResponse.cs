using Health.Contracts.Enums;

namespace Health.Contracts.Requests.Appointments
{
    public class AppointmentResponse
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialization { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string Type { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }
        public string? VideoCallLink { get; set; }
        public string? RescheduleReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
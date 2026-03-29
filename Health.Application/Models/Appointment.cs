using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Health.Contracts.Enums;

namespace Health.Application.Models
{
    public class Appointment
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }

        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public DateTime AppointmentTime { get; set; }
        public string Type { get; set; }   // video | in_person | message
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public string? Notes { get; set; }
        public string? VideoCallLink { get; set; }
        public string? RescheduleReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;// وقت مايحجز يتحفظ
    }

}

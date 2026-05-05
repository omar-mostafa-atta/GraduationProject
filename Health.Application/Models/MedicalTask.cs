using System.ComponentModel.DataAnnotations;

namespace Health.Application.Models
{
    public class MedicalTask
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }

        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }    // High / Medium / Low
        public string Category { get; set; }    // Test / Appointment / Medication / Other
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Medication> Medications { get; set; }
    }
}
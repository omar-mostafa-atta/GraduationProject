using System.ComponentModel.DataAnnotations;

namespace Health.Application.Models
{
    public class Medication
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }

        public Guid? DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public Guid? MedicalTaskId { get; set; }
        public MedicalTask MedicalTask { get; set; }

        public string Name { get; set; }
        public string Dosage { get; set; }
        public int Frequency { get; set; }
        public DateTime? NextReminderTime { get; set; }
        public string Duration { get; set; }
        public string Instructions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
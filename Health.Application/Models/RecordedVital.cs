using System.ComponentModel.DataAnnotations;

namespace Health.Application.Models
{
    public class RecordedVital
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }

        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; }

        public string? BloodPressure { get; set; }    // e.g. "120/80"
        public float? BloodSugarLevel { get; set; }
        public int? HeartRate { get; set; }
        public float? Temperature { get; set; }
        public float? Weight { get; set; }
        public int? OxygenLevel { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
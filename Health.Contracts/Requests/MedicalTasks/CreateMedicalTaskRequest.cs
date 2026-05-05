using System.ComponentModel.DataAnnotations;

namespace Health.Contracts.Requests.MedicalTasks
{
    public class CreateMedicalTaskRequest
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public string TaskTitle { get; set; }

        [Required]
        public string TaskDescription { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        [RegularExpression("^(High|Medium|Low)$")]
        public string Priority { get; set; }

        [Required]
        [RegularExpression("^(Test|Appointment|Medication|Other)$")]
        public string Category { get; set; }
    }
}
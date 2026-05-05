using System.ComponentModel.DataAnnotations;

namespace Health.Contracts.Requests.Medications
{
    public class CreateMedicationRequest
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Dosage { get; set; }

        [Required]
        public string Frequency { get; set; }

        [Required]
        public string Duration { get; set; }

        [Required]
        public string Instructions { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
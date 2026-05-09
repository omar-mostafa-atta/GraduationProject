using System.ComponentModel.DataAnnotations;

namespace Health.Contracts.Requests.MedicalRecords
{
    public class CreateMedicalRecordRequest
    {
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        [RegularExpression("^(Lab Result|Doctor Note|Medical History|Imaging)$")]
        public string RecordType { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string? FileUrl { get; set; }

        [Required]
        public DateTime RecordDate { get; set; }
    }
}
namespace Health.Contracts.Requests.MedicalRecords
{
    public class MedicalRecordResponse
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public string RecordType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? FileUrl { get; set; }
        public DateTime RecordDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
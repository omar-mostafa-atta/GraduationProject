namespace Health.Contracts.Requests.MedicalTasks
{
    public class MedicalTaskResponse
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid? DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public string Category { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
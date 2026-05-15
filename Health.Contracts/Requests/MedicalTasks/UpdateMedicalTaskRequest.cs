namespace Health.Contracts.Requests.MedicalTasks
{
    public class UpdateMedicalTaskRequest
    {
        public string? TaskTitle { get; set; }
        public string? TaskDescription { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Priority { get; set; }    // High / Medium / Low
        public string? Category { get; set; }    // Test / Appointment / Medication / Other
    }
}
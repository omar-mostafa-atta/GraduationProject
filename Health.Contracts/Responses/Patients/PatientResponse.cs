namespace Health.Contracts.Responses.Patients
{
    public class PatientResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
namespace Health.Contracts.Requests.Nurses
{
    public class NurseResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string? Specialization { get; set; }
        public int ExperienceYears { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
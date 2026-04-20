namespace Health.Contracts.Responses
{
    public class DoctorResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
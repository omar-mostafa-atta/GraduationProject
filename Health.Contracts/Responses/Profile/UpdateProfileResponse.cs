namespace Health.Contracts.Responses.Users
{
    public class UpdateProfileResponse
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
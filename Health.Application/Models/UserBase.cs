
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Health.Application.Models
{
    [NotMapped]
    public class UserBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePictureUrl { get; set; } = "TEST.pNG";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [EmailAddress]
       public string Email { get; set; }

    }
}

using System;
using Microsoft.AspNetCore.Identity;

namespace Health.Application.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePictureUrl { get; set; } = "TEST.pNG";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

      
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public Nurse Nurse { get; set; }


    
        public ICollection<Notification> Notifications { get; set; }

    }
}

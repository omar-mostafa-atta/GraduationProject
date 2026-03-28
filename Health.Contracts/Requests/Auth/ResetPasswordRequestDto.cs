using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Contracts.Requests.Auth
{
    public class ResetPasswordRequestDto
    {
        [Required]
        public string UserId { get; set; } // Can be string or Guid based on your route

        [Required]
        public string Token { get; set; } // The code sent to the user

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; }
    }
}

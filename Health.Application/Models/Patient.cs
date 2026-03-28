using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Health.Application.Models
{
    public class Patient : UserBase
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [RegularExpression("^(Male|Female|male|female)$", ErrorMessage = "Gender must be 'Male' or 'Female'.")]
        public string Gender { get; set; }

        public string? Address { get; set; }

        public User User { get; set; }
    }
}

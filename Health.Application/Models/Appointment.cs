using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Health.Contracts.Enums;

namespace Health.Application.Models
{
    public class Appointment
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }

        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public DateTime AppointmentTime { get; set; }
        public AppointmentStatus Status { get; set; } // bdalt el string 
        public string Notes { get; set; }
        public string VideoCallLink { get; set; }
    }

}

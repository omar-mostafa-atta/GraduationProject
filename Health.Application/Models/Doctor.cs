using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Models
{
    public class Doctor : UserBase
    {
        [Key]
        public Guid Id { get; set; }

        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }
        public string? Bio { get; set; } 
        public string PhoneNumber { get; set; }
        public string AvailabilitySchedule { get; set; }
        public int ExperienceYears { get; set; }

        public DoctorStatus Status { get; set; }= DoctorStatus.Pending;
        public User User { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<MedicalTask> MedicalTasks { get; set; }
    }
    public enum DoctorStatus
    {
        Pending,
        Approved,
        Rejected
    }

}

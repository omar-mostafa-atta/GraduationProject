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
        public string? AvailabilitySchedule { get; set; }
        public int ExperienceYears { get; set; }

        public string? WorkPlace { get; set; }
        public DoctorStatus Status { get; set; }= DoctorStatus.Pending;

        //34an calendy integration 
        public string? CalendlyAccessToken { get; set; }
        public string? CalendlyRefreshToken { get; set; }
        public string? CalendlyUri { get; set; }          
        public string? CalendlySchedulingUrl { get; set; }
        public string? CalendlyOrganizationUri { get; set; }
        public bool IsCalendlyConnected => !string.IsNullOrEmpty(CalendlyUri);

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

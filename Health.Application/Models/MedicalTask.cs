using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Models
{
    public class MedicalTask
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }

        public string TaskDescription { get; set; }
       // public string Frequency { get; set; }

        public ICollection<Medication> Medications { get; set; }
    }

}

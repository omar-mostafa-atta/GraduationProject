using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Models
{
    public class MedicalRecord
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }

        public string RecordType { get; set; } // momkn ykon (Lab Report, Prescription, Imaging, a4e3a, t7alel, e.g.)
        public string Title { get; set; }
        public string Description { get; set; }
        //public string FileUrl { get; set; }
        public DateTime RecordDate { get; set; }
    }

}

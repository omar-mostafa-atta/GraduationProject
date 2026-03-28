using Health.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Contracts.Responses.HomeService
{
    public class HomeServiceResponse
    {
        public Guid Id { get; set; }
        public string ServiceDescription { get; set; }
        public DateTime RequestedTime { get; set; }
        public string Address { get; set; }
        public HomeServiceStatus Status { get; set; }
        public Guid PatientId { get; set; }
        public Guid? NurseId { get; set; }
        public string? NurseName { get; set; }
        public string? PatientName { get; set; }
    }
}

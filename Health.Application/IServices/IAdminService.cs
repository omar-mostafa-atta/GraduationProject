using Health.Contracts.Responses.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.IServices
{
    public interface IAdminService
    {
        Task<IEnumerable<PendingDoctorDto>> GetPendingDoctorsAsync();
        Task<IEnumerable<PendingNurseDto>> GetPendingNursesAsync();
    }
}

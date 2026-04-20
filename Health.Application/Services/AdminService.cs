using Health.Application.IServices;
using Health.Contracts.Responses.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Services
{
    public class AdminService : IAdminService
    {
        private  readonly WateenDbContext _dbContext;
        public AdminService(WateenDbContext dbContext) { 
            _dbContext = dbContext;
        }
        public Task<IEnumerable<PendingDoctorDto>> GetPendingDoctorsAsync()
        {
            var pendingDoctors = _dbContext.Doctors.Where(d => d.Status == 0)
                .Select(d => new PendingDoctorDto
                {
                    Id = d.Id,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Email = d.Email,
                    Phone = d.PhoneNumber,
                    LicenseNumber = d.LicenseNumber,
                    ExperienceYears = d.ExperienceYears,
                    WorkPlace = d.WorkPlace,
                    Status =0
                }).ToList();


            return Task.FromResult<IEnumerable<PendingDoctorDto>>(pendingDoctors);
        }

        public Task<IEnumerable<PendingNurseDto>> GetPendingNursesAsync()
        {
            var pendingNurses = _dbContext.Nurses.Where(n => n.Status == 0)
                .Select(n => new PendingNurseDto
                {
                    Id = n.Id,
                    FirstName = n.FirstName,
                    LastName = n.LastName,
                    Email = n.Email,
                    Phone = n.PhoneNumber,
                    LicenseNumber = n.LicenseNumber,
                    ExperienceYears = n.ExperienceYears,
                    Government = n.Government,
                    Status = 0
                }).ToList();

            return Task.FromResult<IEnumerable<PendingNurseDto>>(pendingNurses);
        }


    }
}

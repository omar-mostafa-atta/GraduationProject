using Health.Application.IServices;
using Health.Contracts.Responses.Admin;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<PendingNurseDto>> GetPendingNursesAsync()
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
                }).ToListAsync();

            return await Task.FromResult<IEnumerable<PendingNurseDto>>(await pendingNurses);
        }

        public async Task<int> GetUsersCountAsync()
        {
            var count= await _dbContext.Users.CountAsync();
            var countWithoutAdmin=count-1;
            return countWithoutAdmin;
        }
        public async Task<int> GetDoctorsCountAsync()
        {
            var count = await _dbContext.Doctors.CountAsync();
            return count;
        }
        public async Task<int> GetPatientsCountAsync()
        {
            var count = await _dbContext.Patients.CountAsync();
            return count;
        }
        public async Task<int> GetNursesCountAsync()
        {
            var count = await _dbContext.Nurses.CountAsync();
            return count;
        }
    }
}

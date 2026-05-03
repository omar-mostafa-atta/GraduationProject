using Health.Application.IServices;
using Health.Application.Models;
using Health.Contracts.Common;
using Health.Contracts.Enums;
using Health.Contracts.Requests.HomeServiceRequests;
using Health.Contracts.Requests.Nurses;
using Health.Contracts.Responses.HomeService;
using Health.Contracts.Responses.Patients;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Services
{
    public class HomeService : IHomeService
    {
        private readonly WateenDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        public HomeService(UserManager<User> userManager, WateenDbContext dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<HomeServiceResponse> CreateRequestAsync(string userId, CreateHomeServiceRequest request)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new Exception("Invalid User ID format.");
            }


            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.User.Id == userGuid);

            if (patient == null)
            {
                throw new Exception("Patient record not found for this user.");
            }


            var serviceRequest = new HomeServiceRequest
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                ServiceDescription = request.ServiceDescription,
                RequestedTime = request.RequestedTime,
                Address = request.Address,
                NurseId = request.NurseId,
                Status = HomeServiceStatus.Pending
            };

            _dbContext.HomeServiceRequests.Add(serviceRequest);
            await _dbContext.SaveChangesAsync();

            // 3. Return Response DTO
            return new HomeServiceResponse
            {
                Id = serviceRequest.Id,
                ServiceDescription = serviceRequest.ServiceDescription,
                RequestedTime = serviceRequest.RequestedTime,
                Address = serviceRequest.Address,
                Status = serviceRequest.Status,
                PatientId = serviceRequest.PatientId,
                NurseId = serviceRequest.NurseId,
                NurseName = serviceRequest.NurseId.HasValue
                    ? (await _dbContext.Nurses.Include(n => n.User)
                        .FirstOrDefaultAsync(n => n.Id == serviceRequest.NurseId.Value))?.User.FirstName
                    : null
            };
        }

        public async Task<PaginatedResponse<HomeServiceResponse>> GetPatientRequestsAsync(string userId, int pageNumber, int pageSize)
        {
            if (!Guid.TryParse(userId, out var userGuid))
                throw new Exception("Invalid User ID format.");

            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.User.Id == userGuid);
            if (patient == null)
                throw new Exception("Patient record not found for this user.");

            var totalCount = await _dbContext.HomeServiceRequests
                .Where(r => r.PatientId == patient.Id)
                .CountAsync();

            var requests = await _dbContext.HomeServiceRequests
                .Where(r => r.PatientId == patient.Id)
                .Include(r => r.Nurse).ThenInclude(n => n.User)
                .OrderByDescending(r => r.RequestedTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<HomeServiceResponse>
            {
                Data = requests.Select(r => new HomeServiceResponse
                {
                    Id = r.Id,
                    ServiceDescription = r.ServiceDescription,
                    RequestedTime = r.RequestedTime,
                    Address = r.Address,
                    Status = r.Status,
                    PatientId = r.PatientId,
                    NurseId = r.NurseId,
                    NurseName = r.NurseId.HasValue ? r.Nurse.User.FirstName : null
                }).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PaginatedResponse<HomeServiceResponse>> GetNurseRequestsAsync(string userId, int pageNumber, int pageSize)
        {
            if (!Guid.TryParse(userId, out var userGuid))
                throw new Exception("Invalid User ID format.");

            var nurse = await _dbContext.Nurses.FirstOrDefaultAsync(n => n.User.Id == userGuid);
            if (nurse == null)
                throw new Exception("Nurse record not found for this user.");

            var totalCount = await _dbContext.HomeServiceRequests
                .Where(r => r.NurseId == nurse.Id)
                .CountAsync();

            var requests = await _dbContext.HomeServiceRequests
                .Where(r => r.NurseId == nurse.Id)
                .Include(r => r.Patient).ThenInclude(p => p.User)
                .OrderByDescending(r => r.RequestedTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<HomeServiceResponse>
            {
                Data = requests.Select(r => new HomeServiceResponse
                {
                    Id = r.Id,
                    ServiceDescription = r.ServiceDescription,
                    RequestedTime = r.RequestedTime,
                    Address = r.Address,
                    Status = r.Status,
                    PatientId = r.PatientId,
                    NurseId = r.NurseId,
                    PatientName = r.Patient.User.FirstName
                }).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<HomeServiceResponse> UpdateRequestStatusAsync(string userId, string requestId, bool newStatus)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new Exception("Invalid User ID format.");
            }
            var nurse = await _dbContext.Nurses.FirstOrDefaultAsync(n => n.User.Id == userGuid);
            if (nurse == null)
            {
                throw new Exception("Nurse record not found for this user.");
            }
            if (!Guid.TryParse(requestId, out var requestIdGuid))
            {
                throw new Exception("Invalid request ID format.");
            }
            var request = await _dbContext.HomeServiceRequests.FirstOrDefaultAsync(r => r.Id == requestIdGuid);
            if (request == null)
            {
                throw new Exception("Home service request not found.");
            }
            if (request.NurseId != nurse.Id)
            {
                throw new Exception("You are not authorized to update this request.");
            }

            if (newStatus)
            {
                request.Status = HomeServiceStatus.Accepted;
            }
            else
            {
                request.Status = HomeServiceStatus.Rejected;
            }
            await _dbContext.SaveChangesAsync();
            return new HomeServiceResponse
            {
                Id = request.Id,
                ServiceDescription = request.ServiceDescription,
                RequestedTime = request.RequestedTime,
                Address = request.Address,
                Status = request.Status,
                PatientId = request.PatientId,
                NurseId = request.NurseId,
                NurseName = request.NurseId.HasValue
                    ? (await _dbContext.Nurses.Include(n => n.User)
                        .FirstOrDefaultAsync(n => n.Id == request.NurseId.Value))?.User.FirstName
                    : null,
                PatientName = request.PatientId != Guid.Empty
                    ? (await _dbContext.Patients.Include(p => p.User)
                        .FirstOrDefaultAsync(p => p.Id == request.PatientId))?.User.FirstName
                    : null

            };
        }


        public async Task<HomeServiceResponse> CompleteRequestAsync(string userId, string requestId, bool newStatus)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                throw new Exception("Invalid User ID format.");
            }
            var nurse = await _dbContext.Nurses.FirstOrDefaultAsync(n => n.User.Id == userGuid);
            if (nurse == null)
            {
                throw new Exception("Nurse record not found for this user.");
            }
            if (!Guid.TryParse(requestId, out var requestIdGuid))
            {
                throw new Exception("Invalid request ID format.");
            }
            var request = await _dbContext.HomeServiceRequests.FirstOrDefaultAsync(r => r.Id == requestIdGuid);
            if (request == null)
            {
                throw new Exception("Home service request not found.");
            }
            if (request.NurseId != nurse.Id)
            {
                throw new Exception("You are not authorized to update this request.");
            }

            if (newStatus)
            {
                request.Status = HomeServiceStatus.Completed;
                nurse.CompletedRequests += 1;
            }
            else
            {
                request.Status = HomeServiceStatus.Canceled;
            }

            await _dbContext.SaveChangesAsync();
            return new HomeServiceResponse
            {
                Id = request.Id,
                ServiceDescription = request.ServiceDescription,
                RequestedTime = request.RequestedTime,
                Address = request.Address,
                Status = request.Status,
                PatientId = request.PatientId,
                NurseId = request.NurseId,
                NurseName = request.NurseId.HasValue
                    ? (await _dbContext.Nurses.Include(n => n.User)
                        .FirstOrDefaultAsync(n => n.Id == request.NurseId.Value))?.User.FirstName
                    : null,
                PatientName = request.PatientId != Guid.Empty
                    ? (await _dbContext.Patients.Include(p => p.User)
                        .FirstOrDefaultAsync(p => p.Id == request.PatientId))?.User.FirstName
                    : null
                

            };


        }

        //public async Task<List<Nurse>> GetNursesAsync()
        //{
        //    return await _dbContext.Nurses./*Include(n => n.User).*/ToListAsync();
        //}
        public async Task<PaginatedResponse<NurseResponse>> GetNursesAsync(int pageNumber, int pageSize, string? government)
        {
            var query = _dbContext.Nurses
                .Include(n => n.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(government))
            {
                query = query.Where(n => n.Government.ToLower() == government.ToLower());
            }

            var totalCount = await query.CountAsync(); 

            var nurses = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = nurses.Select(n => new NurseResponse
            {
                Id = n.Id,
                FullName = n.User.FirstName + " " + n.User.LastName,
                Specialization = n.Specialization,
                ExperienceYears = n.ExperienceYears,
                ProfilePictureUrl = n.User.ProfilePictureUrl,
                PhoneNumber = n.PhoneNumber
            }).ToList();

            return new PaginatedResponse<NurseResponse>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PaginatedResponse<PatientResponse>> GetMyPatientsAsync(string nurseUserId, int pageNumber, int pageSize)
        {
            if (!Guid.TryParse(nurseUserId, out var userGuid))
                throw new Exception("Invalid User ID format.");

            var nurse = await _dbContext.Nurses.FirstOrDefaultAsync(n => n.User.Id == userGuid);
            if (nurse == null)
                throw new Exception("Nurse not found.");

            // جيب كل المرضى اللي عندهم (ريكويست كومبليت) مع النيرس دي
            var patientIds = await _dbContext.HomeServiceRequests
                .Where(r => r.NurseId == nurse.Id && r.Status == HomeServiceStatus.Completed)
                .Select(r => r.PatientId)
                .Distinct()
                .ToListAsync();

            var totalCount = patientIds.Count;

            var patients = await _dbContext.Patients
                .Include(p => p.User)
                .Where(p => patientIds.Contains(p.Id))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<PatientResponse>
            {
                Data = patients.Select(p => new PatientResponse
                {
                    Id = p.Id,
                    FullName = p.User.FirstName + " " + p.User.LastName,
                    ProfilePictureUrl = p.User.ProfilePictureUrl,
                    Gender = p.Gender,
                    Address = p.Address,
                    DateOfBirth = p.DateOfBirth
                }).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}

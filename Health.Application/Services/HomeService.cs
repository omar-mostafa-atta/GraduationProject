using Health.Application.IServices;
using Health.Application.Models;
using Health.Contracts.Enums;
using Health.Contracts.Requests.HomeServiceRequests;
using Health.Contracts.Responses.HomeService;
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

        public async Task<List<HomeServiceResponse>> GetPatientRequestsAsync(string userId)
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
            var requests = await _dbContext.HomeServiceRequests
                .Where(r => r.PatientId == patient.Id)
                .Include(r => r.Nurse)
                    .ThenInclude(n => n.User)
                .ToListAsync();
            return requests.Select(r => new HomeServiceResponse
            {
                Id = r.Id,
                ServiceDescription = r.ServiceDescription,
                RequestedTime = r.RequestedTime,
                Address = r.Address,
                Status = r.Status,
                PatientId = r.PatientId,
                NurseId = r.NurseId,
                NurseName = r.NurseId.HasValue ? r.Nurse.User.FirstName : null
            }).ToList();
        }

        public async Task<List<HomeServiceResponse>> GetNurseRequestsAsync(string userId)
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
            var requests = await _dbContext.HomeServiceRequests
                .Where(r => r.NurseId == nurse.Id)
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .ToListAsync();
            return requests.Select(r => new HomeServiceResponse
            {
                Id = r.Id,
                ServiceDescription = r.ServiceDescription,
                RequestedTime = r.RequestedTime,
                Address = r.Address,
                Status = r.Status,
                PatientId = r.PatientId,
                NurseId = r.NurseId,
                PatientName = r.Patient.User.FirstName
            }).ToList();
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
    }
}

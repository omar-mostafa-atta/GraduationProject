using Health.Application.Models;
using Health.Contracts.Requests.HomeServiceRequests;
using Health.Contracts.Responses.HomeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.IServices
{
    public interface IHomeService
    {
        Task<HomeServiceResponse> CreateRequestAsync(string userId, CreateHomeServiceRequest request);
        Task<List<HomeServiceResponse>> GetPatientRequestsAsync(string userId);
        Task<List<HomeServiceResponse>> GetNurseRequestsAsync(string userId);
        Task<HomeServiceResponse> UpdateRequestStatusAsync(string userId, string requestId, bool newStatus);
        Task<HomeServiceResponse> CompleteRequestAsync(string userId, string requestId, bool newStatus);
        Task<List<Nurse>> GetNursesAsync();
    }
}

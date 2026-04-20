using Health.Application.Models;
using Health.Contracts.Common;
using Health.Contracts.Requests.HomeServiceRequests;
using Health.Contracts.Requests.Nurses;
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
        Task<PaginatedResponse<HomeServiceResponse>> GetPatientRequestsAsync(string userId, int pageNumber, int pageSize);
        Task<PaginatedResponse<HomeServiceResponse>> GetNurseRequestsAsync(string userId, int pageNumber, int pageSize);
        Task<HomeServiceResponse> UpdateRequestStatusAsync(string userId, string requestId, bool newStatus);
        Task<HomeServiceResponse> CompleteRequestAsync(string userId, string requestId, bool newStatus);
        Task<PaginatedResponse<NurseResponse>> GetNursesAsync(int pageNumber, int pageSize);
    }
}

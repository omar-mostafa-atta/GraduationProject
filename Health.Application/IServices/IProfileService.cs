using Health.Contracts.Requests.Profile;
using Health.Contracts.Responses.Users;

namespace Health.Application.IServices
{
    public interface IProfileService
    {
        Task<UpdateProfileResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
        Task<UpdateProfileResponse> UpdatePatientProfileAsync(Guid userId, UpdatePatientProfileRequest request);
    }
}
using Health.Application.Models;
using Health.Contracts.Requests.Profile;
using Health.Contracts.Responses.Users;

namespace Health.Application.IServices
{
    public interface IProfileService
    {
        Task<Patient> GetPatientDataAsync(Guid userId);
        Task<UpdateProfileResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);

        Task<UpdateProfileResponse> UpdatePatientProfileAsync(Guid userId, UpdatePatientProfileRequest request);

        Task<User> GetProfileAsync(Guid userId);
        Task<Nurse> GetNurseDataAsync(Guid userId);
        Task<Doctor> GetDoctorDataAsync(Guid userId);
    }
}
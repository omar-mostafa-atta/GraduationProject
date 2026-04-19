using Health.Contracts.Requests.Auth;
using Health.Contracts.Requests.Users;
using Health.Contracts.Responses.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.IServices
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterPatientAsync(RegisterPatientRequestDto request);
        Task<AuthResponseDto> RegisterDoctorAsync(RegisterDoctorRequestDto request);
        Task<AuthResponseDto> RegisterNurseAsync(RegisterNurseRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task <string> DeleteUserAsync(DeleteUser request);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequestDto request);

        Task<bool> AcceptRejectAsync(string userId, bool isAccepted);
    }
}

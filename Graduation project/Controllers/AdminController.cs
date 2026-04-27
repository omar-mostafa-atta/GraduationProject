using Health.Application.IServices;
using Health.Application.Services;
using Health.Contracts.Requests.Auth;
using Health.Contracts.Requests.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Graduation_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IAuthService _authService;

        public AdminController(IAdminService adminService, IAuthService authService)
        {
            _adminService = adminService;
            _authService = authService;
        }

        [HttpGet("pending/nurses")]

        public async Task<IActionResult> GetPendingNurse()
        {
            var nurses= await _adminService.GetPendingNursesAsync();

            return Ok(nurses);
        }

        [HttpGet("pending/doctors")]
        public async Task<IActionResult> GetPendingDoctors()
        {
            var doctors= await _adminService.GetPendingDoctorsAsync();

            return Ok(doctors);
        }
        [HttpGet("users/count")]
        public async Task<IActionResult> GetUsersCount()
        {
            var usersCount = await _adminService.GetUsersCountAsync();
            return Ok(usersCount);
        }
        [HttpGet("doctors/count")]
        public async Task<IActionResult> GetDoctorsCount()
        {
            var doctorsCount =await _adminService.GetDoctorsCountAsync();
            return Ok(doctorsCount);
        }
        [HttpGet("nurses/count")]
        public async Task<IActionResult> GetNursesCount()
        {
            var nursesCount = await _adminService.GetNursesCountAsync();
            return Ok(nursesCount);
        }
        [HttpGet("patients/count")]
        public async Task<IActionResult> GetPatientsCount()
        {
            var patientsCount = await _adminService.GetPatientsCountAsync();
            return Ok(patientsCount);
        }
        [HttpPost("accept-reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AcceptReject([FromBody] AcceptRejectRequestDto model)
        {
            var response = await _authService.AcceptRejectAsync(model.UserId, model.IsAccepted);
            if (response)
            {
                return Ok(new { Message = "User status has been successfully updated." });
            }
            return BadRequest(new { Message = "Failed to update user status." });
        }

        [HttpDelete("delete-account")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteUser model)
        {
            var response = await _authService.DeleteUserAsync(model);

            return Ok(response);

        }
    }
}

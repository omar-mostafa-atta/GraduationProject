using Health.Application.IServices;
using Health.Contracts.Requests.Auth;
using Health.Contracts.Requests.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/register
        [HttpPost("register/Patient")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterPatient([FromBody] RegisterPatientRequestDto model)
        {
            var response = await _authService.RegisterPatientAsync(model);

            if (response.IsSuccess)
            {

                response.Errors = null;
                return Ok(response);
            }

            return BadRequest(new { Errors = response.Errors });
        }

        // POST: api/Auth/register/doctor
        [HttpPost("register/doctor")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterDoctor([FromBody] RegisterDoctorRequestDto model)
        {
            var response = await _authService.RegisterDoctorAsync(model);

            if (response.IsSuccess)
            {
                response.Errors = null;
                return Ok(response);
            }

            return BadRequest(new { Errors = response.Errors });
        }

        // POST: api/Auth/register/nurse
        [HttpPost("register/nurse")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterNurse([FromBody] RegisterNurseRequestDto model)
        {
            var response = await _authService.RegisterNurseAsync(model);

            if (response.IsSuccess)
            {
                response.Errors = null;
                return Ok(response);
            }

            return BadRequest(new { Errors = response.Errors });
        }


        // POST: api/Auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var response = await _authService.LoginAsync(model);

            if (response.IsSuccess)
            {
                response.Errors = null;
                return Ok(response);
            }

            return Unauthorized(new { Errors = response.Errors });
        }

        // POST: api/Auth/forgot-password
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto model)
        {
            await _authService.ForgotPasswordAsync(model);


            return Ok(new { Message = "If a user with that email exists, a password reset link has been sent." });
        }

        // POST: api/Auth/reset-password
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto model)
        {
            var isSuccess = await _authService.ResetPasswordAsync(model);

            if (isSuccess)
            {
                return Ok(new { Message = "Password has been successfully reset." });
            }

            return BadRequest(new { Message = "Invalid user ID or token." });
        }



        [HttpPost("change/password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isSuccess = await _authService.ChangePasswordAsync(userId, model);
            if (isSuccess)
            {
                return Ok(new { Message = "Password has been successfully changed." });
            }
            return BadRequest(new { Message = "Current password is incorrect." });
        }


    }
}

using Health.Application.IServices;
using Health.Contracts.Requests.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPut("update/Doctor-Nurse")]
        [Authorize(Roles = "Doctor,Nurse")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var response = await _profileService.UpdateProfileAsync(Guid.Parse(userId), request);

            if (response.IsSuccess) return Ok(response);
            return BadRequest(new { response.Message });
        }

        [HttpPut("update/patient")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> UpdatePatientProfile([FromBody] UpdatePatientProfileRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var response = await _profileService.UpdatePatientProfileAsync(Guid.Parse(userId), request);

            if (response.IsSuccess) return Ok(response);
            return BadRequest(new { response.Message });
        }
    }
}
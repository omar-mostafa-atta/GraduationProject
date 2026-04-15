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

        [HttpPut("doctorNurse")]
        [Authorize(Roles = "Doctor,Nurse,Admin")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var response = await _profileService.UpdateProfileAsync(Guid.Parse(userId), request);

            if (response.IsSuccess) return Ok(response);
            return BadRequest(new { response.Message });
        }

        [HttpPut("patient")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> UpdatePatientProfile([FromBody] UpdatePatientProfileRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var response = await _profileService.UpdatePatientProfileAsync(Guid.Parse(userId), request);

            if (response.IsSuccess) return Ok(response);
            return BadRequest(new { response.Message });
        }

        [HttpGet("patientData")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetPatientData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("User Not Found");
            var patientData = await _profileService.GetPatientDataAsync(Guid.Parse(userId));
            if (patientData == null) return NotFound("No Data was found for this user in the database");
            return Ok(patientData);
        }
    }
}
using Health.Application.IServices;
using Health.Application.Models;
using Health.Contracts.Requests.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly IPhotoService _photoService;
        private readonly UserManager<User> _userManager;
        public ProfileController(IProfileService profileService, IPhotoService photoService, UserManager<User> userManager)
        {
            _profileService = profileService;
            _photoService = photoService;
            _userManager = userManager;
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

        [HttpGet("nurseData")]
        [Authorize(Roles = "Nurse")]
        public async Task<IActionResult> GetNurseData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("User Not Found");
            var nurseData = await _profileService.GetNurseDataAsync(Guid.Parse(userId));
            if (nurseData == null) return NotFound("No Data was found for this user in the database");
            return Ok(nurseData);
        }


        [HttpGet("doctorData")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetDoctorData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("User Not Found");
            var doctorData = await _profileService.GetDoctorDataAsync(Guid.Parse(userId));
            if (doctorData == null) return NotFound("No Data was found for this user in the database");
            return Ok(doctorData);
        }

        [HttpPut("profile-picture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
           
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            try
            {
                var imageUrl = await _photoService.UploadProfilePictureAsync(file, userId);

                user.ProfilePictureUrl = imageUrl;
                await _userManager.UpdateAsync(user);

                return Ok(new { profilePictureUrl = imageUrl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var profile = await _profileService.GetProfileAsync(Guid.Parse(userId));
            if (profile == null) return NotFound("Profile not found.");
            return Ok(profile);
        }
    }
}
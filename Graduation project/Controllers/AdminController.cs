using Health.Application.IServices;
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

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("pending/nurses")]

        public IActionResult GetPendingNurse()
        {
            var nurses= _adminService.GetPendingNursesAsync();

            return Ok(nurses);
        }

        [HttpGet("pending/doctors")]
        public IActionResult GetPendingDoctors()
        {
            var doctors= _adminService.GetPendingDoctorsAsync();

            return Ok(doctors);
        }


    }
}

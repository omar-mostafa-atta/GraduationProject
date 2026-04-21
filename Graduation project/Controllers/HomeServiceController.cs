using Health.Application.IServices;
using Health.Contracts.Requests.HomeServiceRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class HomeServiceController : ControllerBase
    {
        private readonly IHomeService _homeService;

        public HomeServiceController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        [HttpPost("book")]
        [Authorize]
        public async Task<IActionResult> BookService([FromBody] CreateHomeServiceRequest model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                var response = await _homeService.CreateRequestAsync(userId, model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("PatientRequests")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetPatientRequests([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            try
            {
                var response = await _homeService.GetPatientRequestsAsync(userId, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }


        [HttpGet("NurseRequests")]
        [Authorize(Roles = "Nurse")]
        public async Task<IActionResult> GetNurseRequests([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            try
            {
                var response = await _homeService.GetNurseRequestsAsync(userId, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        [HttpPut("UpdateStatus/{requestId}")]
        [Authorize(Roles = "Nurse")]
        public async Task<IActionResult> UpdateRequestStatus(string requestId, [FromQuery] bool accept)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            try
            {
                var response = await _homeService.UpdateRequestStatusAsync(userId, requestId, accept);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("CompleteRequest/{requestId}")]
        public async Task<IActionResult> CompleteRequest(string requestId, [FromQuery] bool complete)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            try
            {
                var response = await _homeService.CompleteRequestAsync(userId, requestId, complete);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        //[HttpGet("Nurses")]
        //[Authorize]
        //public async Task<IActionResult> GetNurses()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (string.IsNullOrEmpty(userId)) return Unauthorized();
        //    try
        //    {
        //        var response = await _homeService.GetNursesAsync();
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Message = ex.Message });
        //    }
        //}
        [HttpGet("Nurses")]
        [Authorize]
        public async Task<IActionResult> GetNurses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            try
            {
                var response = await _homeService.GetNursesAsync(pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("MyPatients")]
        [Authorize(Roles = "Nurse")]
        public async Task<IActionResult> GetMyPatients([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            try
            {
                var response = await _homeService.GetMyPatientsAsync(userId, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }
    }
}
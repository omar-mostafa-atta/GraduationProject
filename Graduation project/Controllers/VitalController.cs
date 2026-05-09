using Health.Application.IServices;
using Health.Contracts.Requests.Vitals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VitalController : ControllerBase
    {
        private readonly IVitalService _vitalService;

        public VitalController(IVitalService vitalService)
        {
            _vitalService = vitalService;
        }

        // المريض يسجل Vitals بنفسه
        // POST: api/Vital/my
        [HttpPost("my")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> AddMyVitals([FromBody] RecordVitalRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _vitalService.AddMyVitalsAsync(userId, request);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // الدكتور يسجل Vitals لمريض
        // POST: api/Vital/patient/{patientId}
        [HttpPost("patient/{patientId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AddPatientVitals(Guid patientId, [FromBody] RecordVitalRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _vitalService.AddPatientVitalsAsync(userId, patientId, request);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // المريض يشوف Vitals بتاعته
        // GET: api/Vital/my
        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyVitals([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _vitalService.GetMyVitalsAsync(userId, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // الدكتور يشوف Vitals مريض معين
        // GET: api/Vital/patient/{patientId}
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetPatientVitals(Guid patientId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _vitalService.GetPatientVitalsAsync(userId, patientId, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // جيب آخر 7 قراءات Blood Pressure للـ Trend
        // GET: api/Vital/trend/{patientId}
        [HttpGet("trend/{patientId}")]
        [Authorize(Roles = "Doctor,Patient")]
        public async Task<IActionResult> GetBloodPressureTrend(Guid patientId)
        {
            try
            {
                var response = await _vitalService.GetBloodPressureTrendAsync(patientId);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }
    }
}
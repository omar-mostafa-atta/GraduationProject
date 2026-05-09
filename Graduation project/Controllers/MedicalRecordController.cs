using Health.Application.IServices;
using Health.Contracts.Requests.MedicalRecords;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        // الدكتور يضيف Record للمريض
        // POST: api/MedicalRecord/add
        [HttpPost("add")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AddRecord([FromBody] CreateMedicalRecordRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _medicalRecordService.AddRecordAsync(userId, request);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // المريض يضيف Medical History بنفسه
        // POST: api/MedicalRecord/my-history
        [HttpPost("my-history")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> AddMyHistory([FromBody] CreateMedicalRecordRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _medicalRecordService.AddMyHistoryAsync(userId, request);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // المريض يشوف Records بتاعته
        // GET: api/MedicalRecord/my?recordType=Lab Result
        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyRecords([FromQuery] string? recordType, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _medicalRecordService.GetMyRecordsAsync(userId, recordType, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // الدكتور يشوف Records مريض معين
        // GET: api/MedicalRecord/patient/{patientId}?recordType=Doctor Note
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetPatientRecords(Guid patientId, [FromQuery] string? recordType, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _medicalRecordService.GetPatientRecordsAsync(userId, patientId, recordType, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // حذف Record
        // DELETE: api/MedicalRecord/delete/{recordId}
        [HttpDelete("delete/{recordId}")]
        [Authorize(Roles = "Doctor,Patient")]
        public async Task<IActionResult> DeleteRecord(Guid recordId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                await _medicalRecordService.DeleteRecordAsync(userId, recordId);
                return Ok(new { Message = "Record deleted successfully." });
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }
    }
}
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
        // POST: api/MedicalRecord/upload-file
        [HttpPost("upload-file")]
        [Authorize]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "No file uploaded." });
            // تحويل الفايل لـ base64
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            var base64 = Convert.ToBase64String(stream.ToArray());
            var fileType = file.ContentType; // image/jpeg or application/pdf
                                             // بعت على Cloudinary
            using var httpClient = new HttpClient();
            var cloudName = "dushdtpdb";
            var uploadPreset = "sykdvle5";
            var formData = new MultipartFormDataContent
    {
        { new StringContent($"data:{fileType};base64,{base64}"), "file" },
        { new StringContent(uploadPreset), "upload_preset" }
    };
            var response = await httpClient.PostAsync(
                $"https://api.cloudinary.com/v1_1/{cloudName}/auto/upload",
                formData
            );
            if (!response.IsSuccessStatusCode)
                return BadRequest(new { Message = "File upload failed." });
            var result = await response.Content.ReadAsStringAsync();
            var json = System.Text.Json.JsonDocument.Parse(result);
            var fileUrl = json.RootElement.GetProperty("secure_url").GetString();
            return Ok(new { FileUrl = fileUrl });
        }

    }
}
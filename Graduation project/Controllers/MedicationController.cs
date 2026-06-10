using Graduation_project.Hubs;
using Health.Application.IServices;
using Health.Contracts.Requests.Medications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Graduation_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicationController : ControllerBase
    {
        private readonly IMedicationService _medicationService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;
        public MedicationController(IMedicationService medicationService, INotificationService notificationService, IHubContext<NotificationHub> hubContext)
        {
            _medicationService = medicationService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        // الدكتور يضيف Medication لمريض
        // POST: api/Medication/add
        [HttpPost("add")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AddMedication([FromBody] CreateMedicationRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _medicationService.AddMedicationAsync(userId, request);
                var title = "New Prescription Added";
                var message = $"Dr. {response.DoctorName} prescribed you {response.Name}.";
                var notification = await _notificationService.CreateNotificationAsync(
                response.PatientId,
                title,
                message,
                "system");
                if (NotificationHub.OnlineUsers.TryGetValue(response.PatientId.ToString(), out var connectionId))
                {
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", notification);
                }
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // الدكتور يعدل Medication
        // PUT: api/Medication/update/{medicationId}
        [HttpPut("update/{medicationId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateMedication(Guid medicationId, [FromBody] CreateMedicationRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _medicationService.UpdateMedicationAsync(userId, medicationId, request);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // المريض يشوف Medications بتاعته
        // GET: api/Medication/my?isActive=true
        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyMedications([FromQuery] bool? isActive, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _medicationService.GetMyMedicationsAsync(userId, isActive, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // الدكتور يشوف Medications مريض معين
        // GET: api/Medication/patient/{patientId}
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetPatientMedications(Guid patientId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _medicationService.GetPatientMedicationsByDoctorAsync(userId, patientId, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // الدكتور يحذف Medication
        // DELETE: api/Medication/delete/{medicationId}
        [HttpDelete("delete/{medicationId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeleteMedication(Guid medicationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                await _medicationService.DeleteMedicationAsync(userId, medicationId);
                return Ok(new { Message = "Medication deleted successfully." });
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }
    }
}
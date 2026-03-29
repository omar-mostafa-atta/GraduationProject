using Health.Application.IServices;
using Health.Contracts.Requests.Appointments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // المريض يحجز ميعاد
        // POST: api/Appointment/book
        [HttpPost("book")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> BookAppointment([FromBody] CreateAppointmentRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _appointmentService.BookAppointmentAsync(userId, request);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // المريض يشوف مواعيده
        // GET: api/Appointment/my
        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _appointmentService.GetPatientAppointmentsAsync(userId);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // الدكتور يشوف مواعيده
        // GET: api/Appointment/doctor
        [HttpGet("doctor")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetDoctorAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _appointmentService.GetDoctorAppointmentsAsync(userId);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // الدكتور يوافق أو يرفض
        // PUT: api/Appointment/respond/{appointmentId}?accept=true
        [HttpPut("respond/{appointmentId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> RespondToAppointment(Guid appointmentId, [FromQuery] bool accept)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _appointmentService.RespondToAppointmentAsync(userId, appointmentId, accept);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // المريض يلغي
        // PUT: api/Appointment/cancel/{appointmentId}
        [HttpPut("cancel/{appointmentId}")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CancelByPatient(Guid appointmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _appointmentService.CancelByPatientAsync(userId, appointmentId);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // الدكتور يلغي
        // PUT: api/Appointment/doctor/cancel/{appointmentId}
        [HttpPut("doctor/cancel/{appointmentId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CancelByDoctor(Guid appointmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _appointmentService.CancelByDoctorAsync(userId, appointmentId);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // المريض يعيد جدولة
        // PUT: api/Appointment/reschedule/{appointmentId}
        [HttpPut("reschedule/{appointmentId}")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Reschedule(Guid appointmentId, [FromBody] RescheduleAppointmentRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _appointmentService.RescheduleByPatientAsync(userId, appointmentId, request);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // الدكتور يكمل الميعاد
        // PUT: api/Appointment/complete/{appointmentId}
        [HttpPut("complete/{appointmentId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CompleteAppointment(Guid appointmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _appointmentService.CompleteAppointmentAsync(userId, appointmentId);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }
    }
}

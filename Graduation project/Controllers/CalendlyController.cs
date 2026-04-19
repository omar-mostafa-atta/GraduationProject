using Health.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalendlyController : ControllerBase
    {
        private readonly ICalendlyService _calendlyService;

        public CalendlyController(ICalendlyService calendlyService)
        {
            _calendlyService = calendlyService;
        }

        // DOCTOR: Step 1  pass userId into the URL 
        [HttpGet("connect")]
        [Authorize(Roles = "Doctor")]
        public IActionResult GetConnectUrl()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var url = _calendlyService.GetAuthorizationUrl(userId); 
            return Ok(new { AuthorizationUrl = url });
        }

        // DOCTOR: Step 2  remove [Authorize], read userId from state
        [HttpGet("callback")]
        [AllowAnonymous] 
        public async Task<IActionResult> CalendlyCallback(
            [FromQuery] string code,
            [FromQuery] string state) 
        {
            try
            {
                // Decode the doctorUserId from state
                var doctorUserId = System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(state));

                await _calendlyService.ConnectDoctorAsync(doctorUserId, code);

                return Ok(new { Message = "Calendly connected successfully! Your availability is now live." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        //  DOCTOR: See their own event types
        // GET: api/Calendly/doctor/event-types
        [HttpGet("doctor/event-types")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetMyEventTypes()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                // Resolve doctor from userId
                // (You could inject a DoctorRepo or use the service)
                // For simplicity, we pass userId and resolve inside the service
                // Adjust based on your repo structure
                return Ok(); // wire to service
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        //PATIENT: See doctor's available slots
        // GET: api/Calendly/slots/{doctorId}?eventTypeUri=...&from=2025-01-01&to=2025-01-07
        [HttpGet("slots/{doctorId}")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetAvailableSlots(
            Guid doctorId,
            [FromQuery] string eventTypeUri,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            try
            {
                var slots = await _calendlyService.GetAvailableSlotsAsync(
                    doctorId, eventTypeUri, from, to);
                return Ok(slots);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        //PATIENT: Get doctor's event types (consultation types/durations) 
        // GET: api/Calendly/doctor/{doctorId}/event-types
        [HttpGet("doctor/{doctorId}/event-types")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetDoctorEventTypes(Guid doctorId)
        {
            try
            {
                var eventTypes = await _calendlyService.GetDoctorEventTypesAsync(doctorId);
                return Ok(eventTypes);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        //WEBHOOK: Calendly calls this when patient books/cancels
        // POST: api/Calendly/webhook
        [HttpPost("webhook")]
        [AllowAnonymous] // Calendly doesn't send JWT we verify via signature instead
        public async Task<IActionResult> CalendlyWebhook()
        {
            Request.EnableBuffering();
            var signature = Request.Headers["Calendly-Webhook-Signature"].ToString();
            var body = await new StreamReader(Request.Body).ReadToEndAsync();

            Console.WriteLine($"=== WEBHOOK RECEIVED ===");
            Console.WriteLine($"Signature: {signature}");
            Console.WriteLine($"Body length: {body?.Length}");
            Console.WriteLine($"Body: {body}");
            try
            {
                await _calendlyService.ProcessWebhookAsync(body, signature);
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== WEBHOOK ERROR: {ex.Message}");
                Console.WriteLine($"=== STACK: {ex.StackTrace}");
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
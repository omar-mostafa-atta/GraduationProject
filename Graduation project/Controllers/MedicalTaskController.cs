using Health.Application.IServices;
using Health.Contracts.Requests.MedicalTasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Graduation_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicalTaskController : ControllerBase
    {
        private readonly IMedicalTaskService _medicalTaskService;

        public MedicalTaskController(IMedicalTaskService medicalTaskService)
        {
            _medicalTaskService = medicalTaskService;
        }

        // الدكتور يضيف Task لمريض
        // POST: api/MedicalTask/add
        [HttpPost("add")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AddTask([FromBody] CreateMedicalTaskRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _medicalTaskService.AddTaskAsync(userId, request);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // الدكتور يشوف Tasks مريض معين
        // GET: api/MedicalTask/patient/{patientId}
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetPatientTasks(Guid patientId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _medicalTaskService.GetPatientTasksByDoctorAsync(userId, patientId, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // المريض يشوف Tasks بتاعته
        // GET: api/MedicalTask/my?isCompleted=false
        [HttpGet("my")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyTasks([FromQuery] bool? isCompleted, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _medicalTaskService.GetMyTasksAsync(userId, isCompleted, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // المريض يعلم Task كـ Completed
        // PUT: api/MedicalTask/complete/{taskId}
        [HttpPut("complete/{taskId}")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CompleteTask(Guid taskId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _medicalTaskService.CompleteTaskAsync(userId, taskId);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        // الدكتور يحذف Task
        // DELETE: api/MedicalTask/delete/{taskId}
        [HttpDelete("delete/{taskId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeleteTask(Guid taskId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                await _medicalTaskService.DeleteTaskAsync(userId, taskId);
                return Ok(new { Message = "Task deleted successfully." });
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }
        // PUT: api/MedicalTask/update/{taskId}
        [HttpPut("update/{taskId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] UpdateMedicalTaskRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            try
            {
                var response = await _medicalTaskService.UpdateTaskAsync(userId, taskId, request);
                return Ok(response);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }
    }
}
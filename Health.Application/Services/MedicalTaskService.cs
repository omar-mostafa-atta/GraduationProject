using Health.Application.IServices;
using Health.Application.Models;
using Health.Contracts.Common;
using Health.Contracts.Requests.MedicalTasks;
using Microsoft.EntityFrameworkCore;

namespace Health.Application.Services
{
    public class MedicalTaskService : IMedicalTaskService
    {
        private readonly WateenDbContext _dbContext;

        public MedicalTaskService(WateenDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MedicalTaskResponse> AddTaskAsync(string doctorUserId, CreateMedicalTaskRequest request)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var patient = await _dbContext.Patients
                .FirstOrDefaultAsync(p => p.Id == request.PatientId);
            if (patient == null)
                throw new Exception("Patient not found.");

            var task = new MedicalTask
            {
                Id = Guid.NewGuid(),
                DoctorId = doctor.Id,
                PatientId = request.PatientId,
                TaskTitle = request.TaskTitle,
                TaskDescription = request.TaskDescription,
                DueDate = request.DueDate,
                Priority = request.Priority,
                Category = request.Category,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.MedicalTasks.Add(task);
            await _dbContext.SaveChangesAsync();

            return MapToResponse(task, doctor.User.FirstName + " " + doctor.User.LastName);
        }

        public async Task<PaginatedResponse<MedicalTaskResponse>> GetPatientTasksByDoctorAsync(string doctorUserId, Guid patientId, int pageNumber, int pageSize)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var query = _dbContext.MedicalTasks
                .Include(t => t.Doctor).ThenInclude(d => d.User)
                .Where(t => t.PatientId == patientId && t.DoctorId == doctor.Id);

            var totalCount = await query.CountAsync();

            var tasks = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<MedicalTaskResponse>
            {
                Data = tasks.Select(t => MapToResponse(t, t.Doctor.User.FirstName + " " + t.Doctor.User.LastName)).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PaginatedResponse<MedicalTaskResponse>> GetMyTasksAsync(string patientUserId, bool? isCompleted, int pageNumber, int pageSize)
        {
            if (!Guid.TryParse(patientUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.User.Id == userGuid);
            if (patient == null)
                throw new Exception("Patient not found.");

            var query = _dbContext.MedicalTasks
                .Include(t => t.Doctor).ThenInclude(d => d.User)
                .Where(t => t.PatientId == patient.Id);

            if (isCompleted.HasValue)
                query = query.Where(t => t.IsCompleted == isCompleted.Value);

            var totalCount = await query.CountAsync();

            var tasks = await query
                .OrderBy(t => t.DueDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<MedicalTaskResponse>
            {
                Data = tasks.Select(t => MapToResponse(t, t.Doctor != null ? t.Doctor.User.FirstName + " " + t.Doctor.User.LastName : null)).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<MedicalTaskResponse> CompleteTaskAsync(string patientUserId, Guid taskId)
        {
            if (!Guid.TryParse(patientUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.User.Id == userGuid);
            if (patient == null)
                throw new Exception("Patient not found.");

            var task = await _dbContext.MedicalTasks
                .Include(t => t.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
                throw new Exception("Task not found.");

            if (task.PatientId != patient.Id)
                throw new Exception("Not authorized.");

            task.IsCompleted = true;
            await _dbContext.SaveChangesAsync();

            return MapToResponse(task, task.Doctor != null ? task.Doctor.User.FirstName + " " + task.Doctor.User.LastName : null);
        }

        public async Task<bool> DeleteTaskAsync(string doctorUserId, Guid taskId)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var task = await _dbContext.MedicalTasks.FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null)
                throw new Exception("Task not found.");

            if (task.DoctorId != doctor.Id)
                throw new Exception("Not authorized.");

            _dbContext.MedicalTasks.Remove(task);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private MedicalTaskResponse MapToResponse(MedicalTask t, string doctorName)
        {
            return new MedicalTaskResponse
            {
                Id = t.Id,
                PatientId = t.PatientId,
                DoctorId = t.DoctorId,
                DoctorName = doctorName,
                TaskTitle = t.TaskTitle,
                TaskDescription = t.TaskDescription,
                DueDate = t.DueDate,
                Priority = t.Priority,
                Category = t.Category,
                IsCompleted = t.IsCompleted,
                CreatedAt = t.CreatedAt
            };
        }
        public async Task<MedicalTaskResponse> UpdateTaskAsync(string doctorUserId, Guid taskId, UpdateMedicalTaskRequest request)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var task = await _dbContext.MedicalTasks
                .Include(t => t.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
                throw new Exception("Task not found.");

            if (task.DoctorId != doctor.Id)
                throw new Exception("Not authorized.");

            if (!string.IsNullOrWhiteSpace(request.TaskTitle))
                task.TaskTitle = request.TaskTitle;

            if (!string.IsNullOrWhiteSpace(request.TaskDescription))
                task.TaskDescription = request.TaskDescription;

            if (request.DueDate.HasValue)
                task.DueDate = request.DueDate.Value;

            if (!string.IsNullOrWhiteSpace(request.Priority))
                task.Priority = request.Priority;

            if (!string.IsNullOrWhiteSpace(request.Category))
                task.Category = request.Category;

            await _dbContext.SaveChangesAsync();

            return MapToResponse(task, doctor.User.FirstName + " " + doctor.User.LastName);
        }
    }
}
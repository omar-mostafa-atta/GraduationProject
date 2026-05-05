using Health.Contracts.Common;
using Health.Contracts.Requests.MedicalTasks;

namespace Health.Application.IServices
{
    public interface IMedicalTaskService
    {
        // الدكتور يضيف Task لمريض
        Task<MedicalTaskResponse> AddTaskAsync(string doctorUserId, CreateMedicalTaskRequest request);

        // الدكتور يشوف Tasks مريض معين
        Task<PaginatedResponse<MedicalTaskResponse>> GetPatientTasksByDoctorAsync(string doctorUserId, Guid patientId, int pageNumber, int pageSize);

        // المريض يشوف Tasks بتاعته
        Task<PaginatedResponse<MedicalTaskResponse>> GetMyTasksAsync(string patientUserId, bool? isCompleted, int pageNumber, int pageSize);

        // المريض يعلم Task كـ Completed
        Task<MedicalTaskResponse> CompleteTaskAsync(string patientUserId, Guid taskId);

        // الدكتور يحذف Task
        Task<bool> DeleteTaskAsync(string doctorUserId, Guid taskId);
    }
}
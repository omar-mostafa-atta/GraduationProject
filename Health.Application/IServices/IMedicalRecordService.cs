using Health.Contracts.Common;
using Health.Contracts.Requests.MedicalRecords;

namespace Health.Application.IServices
{
    public interface IMedicalRecordService
    {
        // الدكتور يضيف Record للمريض
        Task<MedicalRecordResponse> AddRecordAsync(string doctorUserId, CreateMedicalRecordRequest request);

        // المريض يضيف Medical History بنفسه
        Task<MedicalRecordResponse> AddMyHistoryAsync(string patientUserId, CreateMedicalRecordRequest request);

        // المريض يشوف Records بتاعته
        Task<PaginatedResponse<MedicalRecordResponse>> GetMyRecordsAsync(string patientUserId, string? recordType, int pageNumber, int pageSize);

        // الدكتور يشوف Records مريض معين
        Task<PaginatedResponse<MedicalRecordResponse>> GetPatientRecordsAsync(string doctorUserId, Guid patientId, string? recordType, int pageNumber, int pageSize);

        // حذف Record
        Task<bool> DeleteRecordAsync(string userId, Guid recordId);
    }
}
using Health.Contracts.Common;
using Health.Contracts.Requests.Medications;

namespace Health.Application.IServices
{
    public interface IMedicationService
    {
        Task<MedicationResponse> AddMedicationAsync(string doctorUserId, CreateMedicationRequest request);
        Task<MedicationResponse> UpdateMedicationAsync(string doctorUserId, Guid medicationId, CreateMedicationRequest request);
        Task<PaginatedResponse<MedicationResponse>> GetMyMedicationsAsync(string patientUserId, bool? isActive, int pageNumber, int pageSize);
        Task<PaginatedResponse<MedicationResponse>> GetPatientMedicationsByDoctorAsync(string doctorUserId, Guid patientId, int pageNumber, int pageSize);
        Task<bool> DeleteMedicationAsync(string doctorUserId, Guid medicationId);
    }
}
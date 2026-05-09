using Health.Contracts.Common;
using Health.Contracts.Requests.Vitals;
using Health.Contracts.Responses.Vitals;

namespace Health.Application.IServices
{
    public interface IVitalService
    {
        // المريض يسجل Vitals بنفسه
        Task<VitalResponse> AddMyVitalsAsync(string patientUserId, RecordVitalRequest request);

        // الدكتور يسجل Vitals لمريض
        Task<VitalResponse> AddPatientVitalsAsync(string doctorUserId, Guid patientId, RecordVitalRequest request);

        // المريض يشوف Vitals بتاعته
        Task<PaginatedResponse<VitalResponse>> GetMyVitalsAsync(string patientUserId, int pageNumber, int pageSize);

        // الدكتور يشوف Vitals مريض معين
        Task<PaginatedResponse<VitalResponse>> GetPatientVitalsAsync(string doctorUserId, Guid patientId, int pageNumber, int pageSize);

        // جيب آخر 7 قراءات Blood Pressure للـ Trend
        Task<List<VitalResponse>> GetBloodPressureTrendAsync(Guid patientId);
    }
}
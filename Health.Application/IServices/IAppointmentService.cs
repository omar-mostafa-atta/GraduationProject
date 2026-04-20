using Health.Application.Models;
using Health.Contracts.Common;
using Health.Contracts.Requests.Appointments;
using Health.Contracts.Responses;

namespace Health.Application.IServices
{
    public interface IAppointmentService
    {
        Task<AppointmentResponse> BookAppointmentAsync(string patientUserId, CreateAppointmentRequest request);
        Task<PaginatedResponse<AppointmentResponse>> GetPatientAppointmentsAsync(string patientUserId, int pageNumber, int pageSize);
        Task<PaginatedResponse<AppointmentResponse>> GetDoctorAppointmentsAsync(string doctorUserId, int pageNumber, int pageSize);
        Task<AppointmentResponse> RespondToAppointmentAsync(string doctorUserId, Guid appointmentId, bool accept);
        Task<AppointmentResponse> CancelByPatientAsync(string patientUserId, Guid appointmentId);
        Task<AppointmentResponse> CancelByDoctorAsync(string doctorUserId, Guid appointmentId);
        Task<AppointmentResponse> RescheduleByPatientAsync(string patientUserId, Guid appointmentId, RescheduleAppointmentRequest request);
        Task<AppointmentResponse> CompleteAppointmentAsync(string doctorUserId, Guid appointmentId);
        Task<PaginatedResponse<DoctorResponse>> GetDoctorAsync(int pageNumber, int pageSize);
    }
}
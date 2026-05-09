using Health.Application.Models;
using Health.Contracts.Common;
using Health.Contracts.Requests.Appointments;
using Health.Contracts.Responses;
using Health.Contracts.Responses.Patients;

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

        Task<PaginatedResponse<AppointmentResponse>> GetTodaysAppointmentForDoctorAsync(string doctorUserId, int pageNumber, int pageSize);
        Task<PaginatedResponse<AppointmentResponse>> GetUpcomingAppointmentsForDoctorAsync(string doctorUserId, int pageNumber, int pageSize);

        Task<PaginatedResponse<PatientForDoctorDto>> GetPatientsForDoctorAsync(string doctorUserId, int pageNumber, int pageSize);
        Task<AppointmentResponse> GetAppointmentByIdAsync(string userId, Guid appointmentId);
        Task<PatientDetailsResponse> GetPatientDetailsAsync(string doctorUserId, Guid patientId);
    }
}
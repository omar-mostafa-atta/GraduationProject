using Health.Application.Models;
using Health.Contracts.Requests.Appointments;

namespace Health.Application.IServices
{
    public interface IAppointmentService
    {
        Task<AppointmentResponse> BookAppointmentAsync(string patientUserId, CreateAppointmentRequest request);
        Task<List<AppointmentResponse>> GetPatientAppointmentsAsync(string patientUserId);
        Task<List<AppointmentResponse>> GetDoctorAppointmentsAsync(string doctorUserId);
        Task<AppointmentResponse> RespondToAppointmentAsync(string doctorUserId, Guid appointmentId, bool accept);
        Task<AppointmentResponse> CancelByPatientAsync(string patientUserId, Guid appointmentId);
        Task<AppointmentResponse> CancelByDoctorAsync(string doctorUserId, Guid appointmentId);
        Task<AppointmentResponse> RescheduleByPatientAsync(string patientUserId, Guid appointmentId, RescheduleAppointmentRequest request);
        Task<AppointmentResponse> CompleteAppointmentAsync(string doctorUserId, Guid appointmentId);
        Task<List<Doctor>> GetDoctorAsync();
    }
}
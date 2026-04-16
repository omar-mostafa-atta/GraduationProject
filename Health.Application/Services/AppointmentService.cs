using Health.Application.IServices;
using Health.Application.Models;
using Health.Contracts.Enums;
using Health.Contracts.Requests.Appointments;
using Microsoft.EntityFrameworkCore;

namespace Health.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly WateenDbContext _dbContext;

        public AppointmentService(WateenDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // المريض يحجز ميعاد
        public async Task<AppointmentResponse> BookAppointmentAsync(string patientUserId, CreateAppointmentRequest request)
        {
            if (!Guid.TryParse(patientUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.User.Id == userGuid);
            if (patient == null)
                throw new Exception("Patient not found.");

            var doctor = await _dbContext.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == request.DoctorId);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            // تأكد إن الدكتور مش عنده حجز في نفس الوقت
            var conflict = await _dbContext.Appointments.AnyAsync(a =>
                a.DoctorId == request.DoctorId &&
                a.AppointmentTime == request.AppointmentTime &&
                a.Status != AppointmentStatus.CancelledPatient &&
                a.Status != AppointmentStatus.CancelledDoctor &&
                a.Status != AppointmentStatus.Rejected);

            if (conflict)
                throw new Exception("Doctor already has an appointment at this time.");

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                AppointmentTime = request.AppointmentTime,
                Type = request.Type,
                Status = AppointmentStatus.Pending,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Appointments.Add(appointment);
            await _dbContext.SaveChangesAsync();

            return MapToResponse(appointment, patient, doctor);
        }

        // المريض يشوف مواعيده
        public async Task<List<AppointmentResponse>> GetPatientAppointmentsAsync(string patientUserId)
        {
            if (!Guid.TryParse(patientUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.User.Id == userGuid);
            if (patient == null)
                throw new Exception("Patient not found.");

            var appointments = await _dbContext.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Where(a => a.PatientId == patient.Id)
                .OrderByDescending(a => a.AppointmentTime)
                .ToListAsync();

            return appointments.Select(a => MapToResponse(a, a.Patient, a.Doctor)).ToList();
        }

        // الدكتور يشوف مواعيده
        public async Task<List<AppointmentResponse>> GetDoctorAppointmentsAsync(string doctorUserId)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var appointments = await _dbContext.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Where(a => a.DoctorId == doctor.Id)
                .OrderByDescending(a => a.AppointmentTime)
                .ToListAsync();

            return appointments.Select(a => MapToResponse(a, a.Patient, a.Doctor)).ToList();
        }

        // الدكتور يوافق أو يرفض
        public async Task<AppointmentResponse> RespondToAppointmentAsync(string doctorUserId, Guid appointmentId, bool accept)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors.Include(d => d.User)
                .FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var appointment = await _dbContext.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found.");

            if (appointment.DoctorId != doctor.Id)
                throw new Exception("Not authorized.");

            if (appointment.Status != AppointmentStatus.Pending)
                throw new Exception("Appointment is no longer pending.");

            appointment.Status = accept ? AppointmentStatus.Confirmed : AppointmentStatus.Rejected;
            await _dbContext.SaveChangesAsync();

            return MapToResponse(appointment, appointment.Patient, appointment.Doctor);
        }

        // المريض يلغي
        public async Task<AppointmentResponse> CancelByPatientAsync(string patientUserId, Guid appointmentId)
        {
            if (!Guid.TryParse(patientUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.User.Id == userGuid);
            if (patient == null)
                throw new Exception("Patient not found.");

            var appointment = await _dbContext.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found.");

            if (appointment.PatientId != patient.Id)
                throw new Exception("Not authorized.");

            if (appointment.Status == AppointmentStatus.Completed)
                throw new Exception("Cannot cancel a completed appointment.");

            appointment.Status = AppointmentStatus.CancelledPatient;
            await _dbContext.SaveChangesAsync();

            return MapToResponse(appointment, appointment.Patient, appointment.Doctor);
        }

        // الدكتور يلغي
        public async Task<AppointmentResponse> CancelByDoctorAsync(string doctorUserId, Guid appointmentId)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors.Include(d => d.User)
                .FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var appointment = await _dbContext.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found.");

            if (appointment.DoctorId != doctor.Id)
                throw new Exception("Not authorized.");

            if (appointment.Status == AppointmentStatus.Completed)
                throw new Exception("Cannot cancel a completed appointment.");

            appointment.Status = AppointmentStatus.CancelledDoctor;
            await _dbContext.SaveChangesAsync();

            return MapToResponse(appointment, appointment.Patient, appointment.Doctor);
        }

        // المريض يعيد جدولة
        public async Task<AppointmentResponse> RescheduleByPatientAsync(string patientUserId, Guid appointmentId, RescheduleAppointmentRequest request)
        {
            if (!Guid.TryParse(patientUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.User.Id == userGuid);
            if (patient == null)
                throw new Exception("Patient not found.");

            var appointment = await _dbContext.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found.");

            if (appointment.PatientId != patient.Id)
                throw new Exception("Not authorized.");

            if (appointment.Status == AppointmentStatus.Completed ||
                appointment.Status == AppointmentStatus.CancelledPatient ||
                appointment.Status == AppointmentStatus.CancelledDoctor)
                throw new Exception("Cannot reschedule this appointment.");

            // تأكد مفيش conflict في الوقت الجديد
            var conflict = await _dbContext.Appointments.AnyAsync(a =>
                a.DoctorId == appointment.DoctorId &&
                a.AppointmentTime == request.NewAppointmentTime &&
                a.Id != appointmentId &&
                a.Status != AppointmentStatus.CancelledPatient &&
                a.Status != AppointmentStatus.CancelledDoctor &&
                a.Status != AppointmentStatus.Rejected);

            if (conflict)
                throw new Exception("Doctor already has an appointment at this time.");

            appointment.AppointmentTime = request.NewAppointmentTime;
            appointment.RescheduleReason = request.RescheduleReason;
            appointment.Status = AppointmentStatus.Rescheduled;
            await _dbContext.SaveChangesAsync();

            return MapToResponse(appointment, appointment.Patient, appointment.Doctor);
        }

        // الدكتور يكمل الميعاد
        public async Task<AppointmentResponse> CompleteAppointmentAsync(string doctorUserId, Guid appointmentId)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors.Include(d => d.User)
                .FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var appointment = await _dbContext.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found.");

            if (appointment.DoctorId != doctor.Id)
                throw new Exception("Not authorized.");

            if (appointment.Status != AppointmentStatus.Confirmed)
                throw new Exception("Appointment must be confirmed before completing.");

            appointment.Status = AppointmentStatus.Completed;
            await _dbContext.SaveChangesAsync();

            return MapToResponse(appointment, appointment.Patient, appointment.Doctor);
        }

        // Helper
        private AppointmentResponse MapToResponse(Appointment a, Patient patient, Doctor doctor)
        {
            return new AppointmentResponse
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = patient.User.FirstName + " " + patient.User.LastName,
                DoctorId = a.DoctorId,
                DoctorName = doctor.User.FirstName + " " + doctor.User.LastName,
                DoctorSpecialization = doctor.Specialization,
                AppointmentTime = a.AppointmentTime,
                Type = a.Type,
                Status = a.Status,
                Notes = a.Notes,
                VideoCallLink = a.VideoCallLink,
                RescheduleReason = a.RescheduleReason,
                CreatedAt = a.CreatedAt
            };
        }

        public async Task<List<Doctor>> GetDoctorAsync()
        {
            var doctors = await _dbContext.Doctors.ToListAsync();
            return doctors;
        }
    }
}

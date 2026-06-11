using Health.Application.IServices;
using Health.Application.Models;
using Health.Contracts.Common;
using Health.Contracts.Enums;
using Health.Contracts.Requests.Appointments;
using Health.Contracts.Requests.MedicalRecords;
using Health.Contracts.Responses;
using Health.Contracts.Responses.Patients;
using Health.Contracts.Responses.Vitals;
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
                PatientProblem = request.PatientProblem,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Appointments.Add(appointment);
            await _dbContext.SaveChangesAsync();

            return MapToResponse(appointment, patient, doctor);
        }

        // المريض يشوف مواعيده
        public async Task<PaginatedResponse<AppointmentResponse>> GetPatientAppointmentsAsync(string patientUserId, int pageNumber, int pageSize)
        {
            if (!Guid.TryParse(patientUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.User.Id == userGuid);
            if (patient == null)
                throw new Exception("Patient not found.");

            var totalCount = await _dbContext.Appointments
                .Where(a => a.PatientId == patient.Id)
                .CountAsync();

            var appointments = await _dbContext.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Where(a => a.PatientId == patient.Id)
                .OrderByDescending(a => a.AppointmentTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<AppointmentResponse>
            {
                Data = appointments.Select(a => MapToResponse(a, a.Patient, a.Doctor)).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        // الدكتور يشوف مواعيده
        public async Task<PaginatedResponse<AppointmentResponse>> GetDoctorAppointmentsAsync(string doctorUserId, int pageNumber, int pageSize)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var totalCount = await _dbContext.Appointments
                .Where(a => a.DoctorId == doctor.Id)
                .CountAsync();

            var appointments = await _dbContext.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Where(a => a.DoctorId == doctor.Id)
                .OrderByDescending(a => a.AppointmentTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<AppointmentResponse>
            {
                Data = appointments.Select(a => MapToResponse(a, a.Patient, a.Doctor)).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
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


            appointment.Status = AppointmentStatus.Completed;
            await _dbContext.SaveChangesAsync();

            return MapToResponse(appointment, appointment.Patient, appointment.Doctor);
        }

        // Helper
        private AppointmentResponse MapToResponse(Appointment a, Patient patient, Doctor doctor)
        {
            var age = patient.DateOfBirth.HasValue
        ? DateTime.UtcNow.Year - patient.DateOfBirth.Value.Year
        : (int?)null;

            return new AppointmentResponse
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = patient.User.FirstName + " " + patient.User.LastName,
                PatientGender = patient.Gender,
                PatientAge = age,
                PatientProblem = a.PatientProblem,
                DoctorId = a.DoctorId,
                DoctorName = doctor.User.FirstName + " " + doctor.User.LastName,
                DoctorSpecialization = doctor.Specialization,
                DoctorLocation = doctor.Location,
                DoctorProfilePicture = doctor.User.ProfilePictureUrl,
                AppointmentTime = a.AppointmentTime,
                Type = a.Type,
                Status = a.Status,
                Notes = a.Notes,
                VideoCallLink = a.VideoCallLink,
                RescheduleReason = a.RescheduleReason,
                CreatedAt = a.CreatedAt
            };
        }
        public async Task<AppointmentResponse> GetAppointmentByIdAsync(string userId, Guid appointmentId)
        {
            if (!Guid.TryParse(userId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var appointment = await _dbContext.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found.");

            // تأكد إن اليوزر ده صاحب الميعاد ده
            var isPatient = appointment.Patient.User.Id == userGuid;
            var isDoctor = appointment.Doctor.User.Id == userGuid;

            if (!isPatient && !isDoctor)
                throw new Exception("Not authorized.");

            return MapToResponse(appointment, appointment.Patient, appointment.Doctor);
        }

        //public async Task<List<Doctor>> GetDoctorAsync()
        //{
        //    var doctors = await _dbContext.Doctors.ToListAsync();
        //    return doctors;
        //}
        public async Task<PaginatedResponse<DoctorResponse>> GetDoctorAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _dbContext.Doctors.CountAsync();

            var doctors = await _dbContext.Doctors
                .Include(d => d.User)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = doctors.Select(d => new DoctorResponse
            {
                Id = d.Id,
                FullName = d.User.FirstName + " " + d.User.LastName,
                Specialization = d.Specialization,
                Bio = d.Bio,
                ProfilePictureUrl = d.User.ProfilePictureUrl,
                PhoneNumber = d.PhoneNumber,
                ExperienceYears = d.ExperienceYears,
                Education = d.Education,        
                Certification = d.Certification

            }).ToList();

            return new PaginatedResponse<DoctorResponse>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }



       public async Task<PaginatedResponse<AppointmentResponse>> GetTodaysAppointmentForDoctorAsync(string doctorUserId, int pageNumber, int pageSize) 
        {

            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");


            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var totalCount = await _dbContext.Appointments
                .Where(a => a.DoctorId == doctor.Id &&
                                          a.AppointmentTime >= today &&
                                          a.AppointmentTime < tomorrow
                                         ).CountAsync();


            var appointment = await _dbContext.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Where(a => a.DoctorId == doctor.Id &&
                                          a.AppointmentTime >= today &&
                                          a.AppointmentTime < tomorrow 
                                         )
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PaginatedResponse<AppointmentResponse>
            {
                Data = appointment.Select(a => MapToResponse(a, a.Patient, a.Doctor)).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }



        public async Task<PaginatedResponse<AppointmentResponse>> GetUpcomingAppointmentsForDoctorAsync(string doctorUserId, int pageNumber, int pageSize)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");


            var today = DateTime.UtcNow.Date;
            var tomorrow = DateTime.UtcNow.Date.AddDays(1);


            var totalCount = await _dbContext.Appointments
                .Where(a => a.DoctorId == doctor.Id &&
                                        a.AppointmentTime >= tomorrow
                                         ).CountAsync();


            var appointment = await _dbContext.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Where(a => a.DoctorId == doctor.Id &&
                                          a.AppointmentTime >=tomorrow
                                         )
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PaginatedResponse<AppointmentResponse>
            {
                Data = appointment.Select(a => MapToResponse(a, a.Patient, a.Doctor)).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };



        }



        public async Task<PaginatedResponse<PatientForDoctorDto>> GetPatientsForDoctorAsync(string doctorUserId, int pageNumber, int pageSize)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");
            var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");
            var totalCount = await _dbContext.Appointments
                .Where(a => a.DoctorId == doctor.Id)
                .Select(a => a.PatientId)
                .Distinct()
                .CountAsync();
            var patients = await _dbContext.Patients
                 .Include(p => p.User)
                 .Where(p => _dbContext.Appointments
                     .Where(a => a.DoctorId == doctor.Id)
                     .Select(a => a.PatientId)
                     .Contains(p.Id))
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .Select(p => new PatientForDoctorDto
                 {
                     Id = p.Id,
                     FirstName = p.User.FirstName,
                     LastName = p.User.LastName,
                     Email = p.User.Email,
                     Gender = p.Gender,
                     PhoneNumber = p.PhoneNumber
                 })
                 .ToListAsync();
            return new PaginatedResponse<PatientForDoctorDto>
            {
                Data = patients,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

        }
        public async Task<PatientDetailsResponse> GetPatientDetailsAsync(string doctorUserId, Guid patientId)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var patient = await _dbContext.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == patientId);
            if (patient == null)
                throw new Exception("Patient not found.");

            // العمر
            var age = patient.DateOfBirth.HasValue
                ? DateTime.UtcNow.Year - patient.DateOfBirth.Value.Year
                : (int?)null;

            // آخر Vitals
            var lastVital = await _dbContext.RecordedVitals
                .Where(v => v.PatientId == patientId)
                .OrderByDescending(v => v.CreatedAt)
                .FirstOrDefaultAsync();

            // Blood Pressure Trend — آخر 7 قراءات
            var trend = await _dbContext.RecordedVitals
                .Include(v => v.CreatedBy)
                .Where(v => v.PatientId == patientId && v.BloodPressure != null)
                .OrderByDescending(v => v.CreatedAt)
                .Take(7)
                .ToListAsync();

            // Medical History
            var medicalHistory = await _dbContext.MedicalRecords
                .Include(r => r.Doctor).ThenInclude(d => d.User)
                .Where(r => r.PatientId == patientId && r.RecordType == "Medical History")
                .OrderByDescending(r => r.RecordDate)
                .ToListAsync();

            // Medication Adherence
            var totalMeds = await _dbContext.Medications
                .Where(m => m.PatientId == patientId)
                .CountAsync();

            var activeMeds = await _dbContext.Medications
                .Where(m => m.PatientId == patientId && m.IsActive)
                .CountAsync();

            var adherencePercent = totalMeds > 0
                ? Math.Round((double)activeMeds / totalMeds * 100, 1)
                : 0;

            return new PatientDetailsResponse
            {
                Id = patient.Id,
                FullName = patient.User.FirstName + " " + patient.User.LastName,
                ProfilePictureUrl = patient.User.ProfilePictureUrl,
                Gender = patient.Gender,
                Age = age,
                Address = patient.Address,
                PhoneNumber = patient.PhoneNumber,

                LastBloodPressure = lastVital?.BloodPressure,
                LastBloodSugar = lastVital?.BloodSugarLevel,
                LastHeartRate = lastVital?.HeartRate,
                LastOxygenLevel = lastVital?.OxygenLevel,

                BloodPressureTrend = trend.Select(v => new VitalResponse
                {
                    Id = v.Id,
                    PatientId = v.PatientId,
                    BloodPressure = v.BloodPressure,
                    BloodSugarLevel = v.BloodSugarLevel,
                    HeartRate = v.HeartRate,
                    OxygenLevel = v.OxygenLevel,
                    RecordedBy = v.CreatedBy.FirstName + " " + v.CreatedBy.LastName,
                    CreatedAt = v.CreatedAt
                }).ToList(),

                MedicalHistory = medicalHistory.Select(r => new MedicalRecordResponse
                {
                    Id = r.Id,
                    PatientId = r.PatientId,
                    DoctorId = r.DoctorId,
                    DoctorName = r.Doctor != null ? r.Doctor.User.FirstName + " " + r.Doctor.User.LastName : null,
                    RecordType = r.RecordType,
                    Title = r.Title,
                    Description = r.Description,
                    FileUrl = r.FileUrl,
                    RecordDate = r.RecordDate,
                    CreatedAt = r.CreatedAt
                }).ToList(),

                TotalMedications = totalMeds,
                ActiveMedications = activeMeds,
                MedicationAdherencePercent = adherencePercent
            };
        }
    }

}

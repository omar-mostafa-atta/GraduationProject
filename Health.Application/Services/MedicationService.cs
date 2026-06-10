using Health.Application.IServices;
using Health.Application.Models;
using Health.Contracts.Common;
using Health.Contracts.Requests.Medications;
using Microsoft.EntityFrameworkCore;

namespace Health.Application.Services
{
    public class MedicationService : IMedicationService
    {
        private readonly WateenDbContext _dbContext;

        public MedicationService(WateenDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MedicationResponse> AddMedicationAsync(string doctorUserId, CreateMedicationRequest request)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.Id == request.PatientId);
            if (patient == null)
                throw new Exception("Patient not found.");

            var medication = new Medication
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                DoctorId = doctor.Id,
                Name = request.Name,
                Dosage = request.Dosage,
                Frequency = request.Frequency,
                Duration = request.Duration,
                Instructions = request.Instructions,
                NextReminderTime = request.StartDate,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Medications.Add(medication);
            await _dbContext.SaveChangesAsync();

            return MapToResponse(medication, doctor.User.FirstName + " " + doctor.User.LastName);
        }

        public async Task<MedicationResponse> UpdateMedicationAsync(string doctorUserId, Guid medicationId, CreateMedicationRequest request)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var medication = await _dbContext.Medications
                .Include(m => m.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == medicationId);

            if (medication == null)
                throw new Exception("Medication not found.");

            if (medication.DoctorId != doctor.Id)
                throw new Exception("Not authorized.");

            if (medication.StartDate != request.StartDate)
            {
                medication.NextReminderTime = request.StartDate;
            }

            medication.Name = request.Name;
            medication.Dosage = request.Dosage;
            medication.Frequency = request.Frequency;
            medication.Duration = request.Duration;
            medication.Instructions = request.Instructions;
            medication.StartDate = request.StartDate;
            medication.EndDate = request.EndDate;
            medication.IsActive = request.EndDate == null || request.EndDate > DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return MapToResponse(medication, doctor.User.FirstName + " " + doctor.User.LastName);
        }

        public async Task<PaginatedResponse<MedicationResponse>> GetMyMedicationsAsync(string patientUserId, bool? isActive, int pageNumber, int pageSize)
        {
            if (!Guid.TryParse(patientUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.User.Id == userGuid);
            if (patient == null)
                throw new Exception("Patient not found.");

            var query = _dbContext.Medications
                .Include(m => m.Doctor).ThenInclude(d => d.User)
                .Where(m => m.PatientId == patient.Id);

            if (isActive.HasValue)
                query = query.Where(m => m.IsActive == isActive.Value);

            var totalCount = await query.CountAsync();

            var medications = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<MedicationResponse>
            {
                Data = medications.Select(m => MapToResponse(m, m.Doctor != null ? m.Doctor.User.FirstName + " " + m.Doctor.User.LastName : null)).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PaginatedResponse<MedicationResponse>> GetPatientMedicationsByDoctorAsync(string doctorUserId, Guid patientId, int pageNumber, int pageSize)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var query = _dbContext.Medications
                .Include(m => m.Doctor).ThenInclude(d => d.User)
                .Where(m => m.PatientId == patientId && m.DoctorId == doctor.Id);

            var totalCount = await query.CountAsync();

            var medications = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<MedicationResponse>
            {
                Data = medications.Select(m => MapToResponse(m, doctor.User.FirstName + " " + doctor.User.LastName)).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<bool> DeleteMedicationAsync(string doctorUserId, Guid medicationId)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var medication = await _dbContext.Medications.FirstOrDefaultAsync(m => m.Id == medicationId);
            if (medication == null)
                throw new Exception("Medication not found.");

            if (medication.DoctorId != doctor.Id)
                throw new Exception("Not authorized.");

            _dbContext.Medications.Remove(medication);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private MedicationResponse MapToResponse(Medication m, string doctorName)
        {
            return new MedicationResponse
            {
                Id = m.Id,
                PatientId = m.PatientId,
                DoctorId = m.DoctorId,
                DoctorName = doctorName,
                Name = m.Name,
                Dosage = m.Dosage,
                Frequency = m.Frequency,
                Duration = m.Duration,
                Instructions = m.Instructions,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt
            };
        }
    }
}
using Health.Application.IServices;
using Health.Application.Models;
using Health.Contracts.Common;
using Health.Contracts.Requests.MedicalRecords;
using Microsoft.EntityFrameworkCore;

namespace Health.Application.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly WateenDbContext _dbContext;

        public MedicalRecordService(WateenDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MedicalRecordResponse> AddRecordAsync(string doctorUserId, CreateMedicalRecordRequest request)
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

            var record = new MedicalRecord
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                DoctorId = doctor.Id,
                RecordType = request.RecordType,
                Title = request.Title,
                Description = request.Description,
                FileUrl = request.FileUrl,
                RecordDate = request.RecordDate,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.MedicalRecords.Add(record);
            await _dbContext.SaveChangesAsync();

            return MapToResponse(record, doctor.User.FirstName + " " + doctor.User.LastName);
        }

        public async Task<MedicalRecordResponse> AddMyHistoryAsync(string patientUserId, CreateMedicalRecordRequest request)
        {
            if (!Guid.TryParse(patientUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.User.Id == userGuid);
            if (patient == null)
                throw new Exception("Patient not found.");

            var record = new MedicalRecord
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                DoctorId = null,
                RecordType = "Medical History",
                Title = request.Title,
                Description = request.Description,
                FileUrl = request.FileUrl,
                RecordDate = request.RecordDate,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.MedicalRecords.Add(record);
            await _dbContext.SaveChangesAsync();

            return MapToResponse(record, null);
        }

        public async Task<PaginatedResponse<MedicalRecordResponse>> GetMyRecordsAsync(string patientUserId, string? recordType, int pageNumber, int pageSize)
        {
            if (!Guid.TryParse(patientUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.User.Id == userGuid);
            if (patient == null)
                throw new Exception("Patient not found.");

            var query = _dbContext.MedicalRecords
                .Include(r => r.Doctor).ThenInclude(d => d.User)
                .Where(r => r.PatientId == patient.Id);

            if (!string.IsNullOrEmpty(recordType))
                query = query.Where(r => r.RecordType == recordType);

            var totalCount = await query.CountAsync();

            var records = await query
                .OrderByDescending(r => r.RecordDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<MedicalRecordResponse>
            {
                Data = records.Select(r => MapToResponse(r, r.Doctor != null ? r.Doctor.User.FirstName + " " + r.Doctor.User.LastName : null)).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PaginatedResponse<MedicalRecordResponse>> GetPatientRecordsAsync(string doctorUserId, Guid patientId, string? recordType, int pageNumber, int pageSize)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var query = _dbContext.MedicalRecords
                .Include(r => r.Doctor).ThenInclude(d => d.User)
                .Where(r => r.PatientId == patientId);

            if (!string.IsNullOrEmpty(recordType))
                query = query.Where(r => r.RecordType == recordType);

            var totalCount = await query.CountAsync();

            var records = await query
                .OrderByDescending(r => r.RecordDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<MedicalRecordResponse>
            {
                Data = records.Select(r => MapToResponse(r, r.Doctor != null ? r.Doctor.User.FirstName + " " + r.Doctor.User.LastName : null)).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<bool> DeleteRecordAsync(string userId, Guid recordId)
        {
            if (!Guid.TryParse(userId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var record = await _dbContext.MedicalRecords
                .Include(r => r.Patient).ThenInclude(p => p.User)
                .Include(r => r.Doctor).ThenInclude(d => d.User)
                .FirstOrDefaultAsync(r => r.Id == recordId);

            if (record == null)
                throw new Exception("Record not found.");

            var isPatient = record.Patient.User.Id == userGuid;
            var isDoctor = record.Doctor != null && record.Doctor.User.Id == userGuid;

            if (!isPatient && !isDoctor)
                throw new Exception("Not authorized.");

            _dbContext.MedicalRecords.Remove(record);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private MedicalRecordResponse MapToResponse(MedicalRecord r, string? doctorName)
        {
            return new MedicalRecordResponse
            {
                Id = r.Id,
                PatientId = r.PatientId,
                DoctorId = r.DoctorId,
                DoctorName = doctorName,
                RecordType = r.RecordType,
                Title = r.Title,
                Description = r.Description,
                FileUrl = r.FileUrl,
                RecordDate = r.RecordDate,
                CreatedAt = r.CreatedAt
            };
        }
    }
}
using Health.Application.IServices;
using Health.Application.Models;
using Health.Contracts.Common;
using Health.Contracts.Requests.Vitals;
using Health.Contracts.Responses.Vitals;
using Microsoft.EntityFrameworkCore;

namespace Health.Application.Services
{
    public class VitalService : IVitalService
    {
        private readonly WateenDbContext _dbContext;

        public VitalService(WateenDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<VitalResponse> AddMyVitalsAsync(string patientUserId, RecordVitalRequest request)
        {
            if (!Guid.TryParse(patientUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var patient = await _dbContext.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.User.Id == userGuid);
            if (patient == null)
                throw new Exception("Patient not found.");

            var vital = new RecordedVital
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                CreatedById = userGuid,
                BloodPressure = request.BloodPressure,
                BloodSugarLevel = request.BloodSugarLevel,
                HeartRate = request.HeartRate,
                Temperature = request.Temperature,
                Weight = request.Weight,
                OxygenLevel = request.OxygenLevel,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.RecordedVitals.Add(vital);
            await _dbContext.SaveChangesAsync();

            return MapToResponse(vital, patient.User.FirstName + " " + patient.User.LastName);
        }

        public async Task<VitalResponse> AddPatientVitalsAsync(string doctorUserId, Guid patientId, RecordVitalRequest request)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.Id == patientId);
            if (patient == null)
                throw new Exception("Patient not found.");

            var vital = new RecordedVital
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                CreatedById = userGuid,
                BloodPressure = request.BloodPressure,
                BloodSugarLevel = request.BloodSugarLevel,
                HeartRate = request.HeartRate,
                Temperature = request.Temperature,
                Weight = request.Weight,
                OxygenLevel = request.OxygenLevel,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.RecordedVitals.Add(vital);
            await _dbContext.SaveChangesAsync();

            return MapToResponse(vital, doctor.User.FirstName + " " + doctor.User.LastName);
        }

        public async Task<PaginatedResponse<VitalResponse>> GetMyVitalsAsync(string patientUserId, int pageNumber, int pageSize)
        {
            if (!Guid.TryParse(patientUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.User.Id == userGuid);
            if (patient == null)
                throw new Exception("Patient not found.");

            var query = _dbContext.RecordedVitals
                .Include(v => v.CreatedBy)
                .Where(v => v.PatientId == patient.Id);

            var totalCount = await query.CountAsync();

            var vitals = await query
                .OrderByDescending(v => v.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<VitalResponse>
            {
                Data = vitals.Select(v => MapToResponse(v, v.CreatedBy.FirstName + " " + v.CreatedBy.LastName)).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PaginatedResponse<VitalResponse>> GetPatientVitalsAsync(string doctorUserId, Guid patientId, int pageNumber, int pageSize)
        {
            if (!Guid.TryParse(doctorUserId, out var userGuid))
                throw new Exception("Invalid User ID.");

            var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.User.Id == userGuid);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var query = _dbContext.RecordedVitals
                .Include(v => v.CreatedBy)
                .Where(v => v.PatientId == patientId);

            var totalCount = await query.CountAsync();

            var vitals = await query
                .OrderByDescending(v => v.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<VitalResponse>
            {
                Data = vitals.Select(v => MapToResponse(v, v.CreatedBy.FirstName + " " + v.CreatedBy.LastName)).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<List<VitalResponse>> GetBloodPressureTrendAsync(Guid patientId)
        {
            var vitals = await _dbContext.RecordedVitals
                .Include(v => v.CreatedBy)
                .Where(v => v.PatientId == patientId && v.BloodPressure != null)
                .OrderByDescending(v => v.CreatedAt)
                .Take(7)
                .ToListAsync();

            return vitals.Select(v => MapToResponse(v, v.CreatedBy.FirstName + " " + v.CreatedBy.LastName)).ToList();
        }

        private VitalResponse MapToResponse(RecordedVital v, string recordedBy)
        {
            return new VitalResponse
            {
                Id = v.Id,
                PatientId = v.PatientId,
                BloodPressure = v.BloodPressure,
                BloodSugarLevel = v.BloodSugarLevel,
                HeartRate = v.HeartRate,
                Temperature = v.Temperature,
                Weight = v.Weight,
                OxygenLevel = v.OxygenLevel,
                RecordedBy = recordedBy,
                CreatedAt = v.CreatedAt
            };
        }
    }
}
using Health.Application.IServices;
using Health.Application.Models;
using Health.Contracts.Requests.Profile;
using Health.Contracts.Responses.Profile;
using Health.Contracts.Responses.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Health.Application.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<User> _userManager;
        private readonly WateenDbContext _dbContext;

        public ProfileService(UserManager<User> userManager, WateenDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<Patient> GetPatientDataAsync(Guid userId)
        {
            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.Id == userId);
            return patient;
        }

        public async Task<UpdateProfileResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return new UpdateProfileResponse { IsSuccess = false, Message = "User not found." };

            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
            {
                var emailExists = await _userManager.FindByEmailAsync(request.Email);
                if (emailExists != null)
                    return new UpdateProfileResponse { IsSuccess = false, Message = "Email already in use." };

                user.Email = request.Email;
                user.UserName = request.Email;
            }

            if (!string.IsNullOrWhiteSpace(request.FirstName)) user.FirstName = request.FirstName;
            if (!string.IsNullOrWhiteSpace(request.LastName)) user.LastName = request.LastName;
            if (!string.IsNullOrWhiteSpace(request.ProfilePictureUrl)) user.ProfilePictureUrl = request.ProfilePictureUrl;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new UpdateProfileResponse { IsSuccess = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Doctor"))
            {
                var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.Id == userId);
                if (doctor != null)
                {
                    if (!string.IsNullOrWhiteSpace(request.FirstName)) doctor.FirstName = request.FirstName;
                    if (!string.IsNullOrWhiteSpace(request.LastName)) doctor.LastName = request.LastName;
                    if (!string.IsNullOrWhiteSpace(request.Email)) doctor.Email = request.Email;
                    if (!string.IsNullOrWhiteSpace(request.ProfilePictureUrl)) doctor.ProfilePictureUrl = request.ProfilePictureUrl;
                }
            }
            else if (roles.Contains("Nurse"))
            {
                var nurse = await _dbContext.Nurses.FirstOrDefaultAsync(n => n.Id == userId);
                if (nurse != null)
                {
                    if (!string.IsNullOrWhiteSpace(request.FirstName)) nurse.FirstName = request.FirstName;
                    if (!string.IsNullOrWhiteSpace(request.LastName)) nurse.LastName = request.LastName;
                    if (!string.IsNullOrWhiteSpace(request.Email)) nurse.Email = request.Email;
                    if (!string.IsNullOrWhiteSpace(request.ProfilePictureUrl)) nurse.ProfilePictureUrl = request.ProfilePictureUrl;
                }
            }

            await _dbContext.SaveChangesAsync();

            return new UpdateProfileResponse
            {
                IsSuccess = true,
                Message = "Profile updated successfully.",
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl
            };
        }

        public async Task<UpdateProfileResponse> UpdatePatientProfileAsync(Guid userId, UpdatePatientProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return new UpdateProfileResponse { IsSuccess = false, Message = "User not found." };

            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
            {
                var emailExists = await _userManager.FindByEmailAsync(request.Email);
                if (emailExists != null)
                    return new UpdateProfileResponse { IsSuccess = false, Message = "Email already in use." };

                user.Email = request.Email;
                //user.UserName = request.Email;
            }

            if (!string.IsNullOrWhiteSpace(request.FirstName)) user.FirstName = request.FirstName;
            if (!string.IsNullOrWhiteSpace(request.LastName)) user.LastName = request.LastName;
            if (!string.IsNullOrWhiteSpace(request.ProfilePictureUrl)) user.ProfilePictureUrl = request.ProfilePictureUrl;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new UpdateProfileResponse { IsSuccess = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.Id == userId);
            if (patient != null)
            {
                if (!string.IsNullOrWhiteSpace(request.FirstName)) patient.FirstName = request.FirstName;
                if (!string.IsNullOrWhiteSpace(request.LastName)) patient.LastName = request.LastName;
                if (!string.IsNullOrWhiteSpace(request.Email)) patient.Email = request.Email;
                if (!string.IsNullOrWhiteSpace(request.ProfilePictureUrl)) patient.ProfilePictureUrl = request.ProfilePictureUrl;
                if (!string.IsNullOrWhiteSpace(request.Gender)) patient.Gender = request.Gender;
                if (request.DateOfBirth.HasValue) patient.DateOfBirth = request.DateOfBirth;
                if (!string.IsNullOrWhiteSpace(request.Address)) patient.Address = request.Address;
                if (request.SystolicPressure.HasValue) patient.SystolicPressure = request.SystolicPressure.Value;
                if (request.DiastolicPressure.HasValue) patient.DiastolicPressure = request.DiastolicPressure.Value;
                if (request.HeartRate.HasValue) patient.HeartRate = request.HeartRate.Value;
                if (request.Sugar.HasValue) patient.Sugar = request.Sugar.Value;
            }

            await _dbContext.SaveChangesAsync();

            return new UpdateProfileResponse
            {
                IsSuccess = true,
                Message = "Profile updated successfully.",
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl,

            };
        }

        public async Task<User> GetProfileAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user;
        }
        public async Task<GetNurseDataResponse> GetNurseDataAsync(Guid userId)
        {
            var nurse = await _dbContext.Nurses
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == userId);
            if (nurse == null)
                return null;

            return new GetNurseDataResponse
            {
                Id = nurse.Id,
                LicenseNumber = nurse.LicenseNumber,
                Specialization = nurse.Specialization,
                ExperienceYears = nurse.ExperienceYears,
                PhoneNumber = nurse.PhoneNumber,
                IsActive = nurse.IsActive,
                CompletedRequests = nurse.CompletedRequests,
                Government = nurse.Government

            };
        }

        public async Task<GetDoctorDataResponse> GetDoctorDataAsync(Guid userId)
        {
            var doctor = await _dbContext.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == userId);
            if (doctor == null)
                return null;

            return new GetDoctorDataResponse
            {
                Id = doctor.Id,
                Specialization = doctor.Specialization,
                LicenseNumber = doctor.LicenseNumber,
                Bio = doctor.Bio,
                PhoneNumber = doctor.PhoneNumber,
                AvailabilitySchedule = doctor.AvailabilitySchedule,
                ExperienceYears = doctor.ExperienceYears,
                WorkPlace = doctor.WorkPlace
            };
        }
    }
}
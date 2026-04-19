using Health.Application.IServices;
using Health.Application.Models;
using Health.Contracts.Requests.Auth;
using Health.Contracts.Requests.Users;
using Health.Contracts.Responses.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Health.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly WateenDbContext _dbContext;

        public AuthService(UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService,
            IEmailService emailService,
            WateenDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _dbContext = dbContext;
        }

        public async Task<AuthResponseDto> RegisterPatientAsync(RegisterPatientRequestDto request)
        {
            var userExists = await _userManager.FindByEmailAsync(request.Email);
            if (userExists != null)
            {
                return new AuthResponseDto { IsSuccess = false, Errors = new[] { "User with this email already exists." } };
            }

            var user = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                UserName = request.FirstName + request.LastName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Patient");


                var patient = new Patient
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Gender = request.Gender,
                    User = user,
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = request.DateOfBirth
                };


                _dbContext.Patients.Add(patient);


                await _dbContext.SaveChangesAsync();


                return new AuthResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = await _tokenService.CreateToken(user),
                    IsSuccess = true
                };
            }

            return new AuthResponseDto
            {
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }


        public async Task<AuthResponseDto> RegisterDoctorAsync(RegisterDoctorRequestDto request)
        {
            var userExists = await _userManager.FindByEmailAsync(request.Email);
            if (userExists != null)
            {
                return new AuthResponseDto { IsSuccess = false, Errors = new[] { "User with this email already exists." } };
            }


            var user = new User
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {

                const string doctorRole = "Doctor";
                await _userManager.AddToRoleAsync(user, doctorRole);

                var doctor = new Doctor
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Specialization = request.Specialization,
                    LicenseNumber = request.LicenseNumber,
                    PhoneNumber = request.PhoneNumber,
                    WorkPlace = request.WorkPlace,
                    ExperienceYears = request.ExperienceYears,
                    Email = request.Email,
                    Status = DoctorStatus.Pending,
                    User = user
                };

                try
                {
                    _dbContext.Doctors.Add(doctor);


                    await _dbContext.SaveChangesAsync();
                }
                catch
                {
                    await _userManager.DeleteAsync(user);
                    await _dbContext.SaveChangesAsync();

                    throw new Exception("An error occurred while saving the doctor information. Please try again.");

                }


                // 4. Generate Token and Response
                return new AuthResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = await _tokenService.CreateToken(user),
                    IsSuccess = true
                };
            }

            return new AuthResponseDto
            {
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }


        public async Task<AuthResponseDto> RegisterNurseAsync(RegisterNurseRequestDto request)
        {
            var userExists = await _userManager.FindByEmailAsync(request.Email);
            if (userExists != null)
            {
                return new AuthResponseDto { IsSuccess = false, Errors = new[] { "User with this email already exists." } };
            }


            var user = new User
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.NursePhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {

                const string NurseRole = "Nurse";
                await _userManager.AddToRoleAsync(user, NurseRole);

                var nurse = new Nurse
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Specialization = request.Specialization,
                    LicenseNumber = request.LicenseNumber,
                    ExperienceYears = request.ExperienceYears, // kont nasy
                    PhoneNumber = request.NursePhoneNumber,
                    IsActive = request.IsActive,
                    Email = request.Email,
                    User = user
                };


                _dbContext.Nurses.Add(nurse);

                await _dbContext.SaveChangesAsync();


                return new AuthResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = await _tokenService.CreateToken(user),
                    IsSuccess = true
                };
            }

            return new AuthResponseDto
            {
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };



        }
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return new AuthResponseDto { IsSuccess = false, Errors = new[] { "Invalid credentials." } };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Doctor"))
                {
                    var doctor = _dbContext.Doctors.SingleOrDefault(d => d.User.Id == user.Id);
                    if (doctor == null || doctor.Status != DoctorStatus.Approved)
                    {

                        return new AuthResponseDto { IsSuccess = false, Errors = new[] { "Your Doctor account is pending approval or has been rejected." } };
                    }
                }
                else if (roles.Contains("Nurse"))
                {
                    var nurse = _dbContext.Nurses.SingleOrDefault(n => n.User.Id == user.Id);
                    if (nurse == null || nurse.Status != NurseStatus.Approved)
                    {
                        return new AuthResponseDto { IsSuccess = false, Errors = new[] { "Your Nurse account is pending approval or has been rejected." } };
                    }
                }

                return new AuthResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = await _tokenService.CreateToken(user),
                    IsSuccess = true
                };
            }

            return new AuthResponseDto { IsSuccess = false, Errors = new[] { "Invalid credentials." } };
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return true;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailService.SendPasswordResetEmailAsync(user.Email, user.Id.ToString(), token);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return false;

            var tokenFromRequest = request.Token;

            var decodedToken = System.Net.WebUtility.UrlDecode(tokenFromRequest);


            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

            return result.Succeeded;
        }

        public async Task<string> DeleteUserAsync(DeleteUser request)
        {
            if (!Guid.TryParse(request.Id, out var userGuid))
            {
                throw new Exception("Invalid User ID format.");
            }
            var user = await _userManager.FindByIdAsync(userGuid.ToString());
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            var isPatient = await _userManager.IsInRoleAsync(user, "Patient");
            var isDoctor = await _userManager.IsInRoleAsync(user, "Doctor");
            var isNurse = await _userManager.IsInRoleAsync(user, "Nurse");
            if (isPatient)
            {
                var patient = await _dbContext.Patients.SingleOrDefaultAsync(p => p.Id == user.Id);
                if (patient != null)
                {
                    _dbContext.Patients.Remove(patient);
                }
            }
            else if (isDoctor)
            {
                var doctor = await _dbContext.Doctors.SingleOrDefaultAsync(d => d.Id == user.Id);
                if (doctor != null)
                {
                    _dbContext.Doctors.Remove(doctor);
                }
            }
            else if (isNurse)
            {
                var nurse = await _dbContext.Nurses.SingleOrDefaultAsync(n => n.Id == user.Id);
                if (nurse != null)
                {
                    _dbContext.Nurses.Remove(nurse);
                }
            }
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return "User deleted successfully.";
            }
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Failed to delete user: {errors}");


        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequestDto request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            return result.Succeeded;
        }
    }
}

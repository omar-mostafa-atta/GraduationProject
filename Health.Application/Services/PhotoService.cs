// Infrastructure/Services/PhotoService.cs
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Health.Application.IServices;
using Health.Application.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Health.Application.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string> UploadProfilePictureAsync(IFormFile file, string userId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided.");

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                throw new ArgumentException("Only JPEG, PNG, and WEBP images are allowed.");

            const long maxSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxSize)
                throw new ArgumentException("File size must not exceed 5MB.");

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = $"profile_pictures/{userId}",  // overwrites previous photo automatically
                Overwrite = true,
                Transformation = new Transformation()
                    .Width(300).Height(300)
                    .Crop("fill")
                    .Gravity("face")     // auto-center on face
                    .Quality("auto")
                    .FetchFormat("auto") // serves WebP to supported browsers
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new Exception($"Cloudinary upload failed: {result.Error.Message}");

            return result.SecureUrl.ToString();
        }

        public async Task DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}
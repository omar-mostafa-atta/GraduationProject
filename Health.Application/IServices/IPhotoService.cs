using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.IServices
{
    public interface IPhotoService
    {
        Task<string> UploadProfilePictureAsync(IFormFile file, string userId);
        Task DeletePhotoAsync(string publicId);
    }
}

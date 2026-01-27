using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace GymPL.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadProfilePictureAsync(IFormFile file, string userId);
        void DeleteProfilePicture(string imagePath);
        string GetProfilePicturePath(string imageName);
    }

    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5MB

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadProfilePictureAsync(IFormFile file, string userId)
        {
            if (file == null || file.Length == 0)
                return null;

            // Validate file size
            if (file.Length > _maxFileSize)
                throw new InvalidOperationException("File size cannot exceed 5MB.");

            // Validate file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Only JPG, JPEG, PNG, and GIF files are allowed.");

            // Create uploads directory if it doesn't exist
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "profiles");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Generate unique file name
            var fileName = $"{userId}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative path for database storage
            return $"/uploads/profiles/{fileName}";
        }

        public void DeleteProfilePicture(string imagePath)
        {
            
                var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            
        }

        public string GetProfilePicturePath(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
                return "/images/default-profile.png"; // Default image

            return imageName.StartsWith("/") ? imageName : $"/{imageName}";
        }
    }
}

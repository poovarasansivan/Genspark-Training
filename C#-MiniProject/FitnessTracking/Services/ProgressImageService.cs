using FitnessTracking.Contexts;
using FitnessTracking.Interfaces;
using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using FitnessTracking.Helpers;


namespace FitnessTracking.Services
{
    public class ProgressImageService : IProgressImageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly FitnessContext _context;

        public ProgressImageService(IHttpContextAccessor httpContextAccessor, FitnessContext context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ProgressImageModel> AddProgressImageAsync(AddProgressImageDto dto)
        {
            using var memoryStream = new MemoryStream();
            await dto.File.CopyToAsync(memoryStream);

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new InvalidOperationException("HTTP context is not available.");
            }

            var progress = await _context.ProgressUpdates.FindAsync(dto.ProgressId);
            if (progress == null)
            {
                throw new Exception("Invalid progress ID.");
            }

            var progressImage = new ProgressImageModel
            {
                Id = Guid.NewGuid(),
                ProgressId = dto.ProgressId,
                ImageData = memoryStream.ToArray(),
                ContentType = dto.File.ContentType,
                UploadedAt = DateTime.UtcNow
            };

            _context.ProgressImage.Add(progressImage);
            await _context.SaveChangesAsync();

            return progressImage;
        }

        public async Task<IEnumerable<ProgressImageDto>> GetAllProgressImageAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var baseUrl = $"{httpContext?.Request.Scheme}://{httpContext?.Request.Host}";

            return await _context.ProgressImage
                .Select(pi => new ProgressImageDto
                {
                    Id = pi.Id,
                    ContentType = pi.ContentType,
                    FileName = $"Progress_{pi.ProgressId}_{pi.UploadedAt:yyyyMMddHHmmss}.{GetExtension(pi.ContentType)}",
                    DownloadUrl = $"{baseUrl}/api/progressimage/download/{pi.Id}"
                })
                .ToListAsync();
        }

        public async Task<ProgressImageModel?> GetByIdAsync(Guid id)
        {
            return await _context.ProgressImage.FindAsync(id);
        }

        private string GetExtension(string contentType)
        {
            return contentType switch
            {
                "image/jpeg" => "jpg",
                "image/png" => "png",
                "image/gif" => "gif",
                "image/bmp" => "bmp",
                "image/webp" => "webp",
                _ => "bin"
            };
        }

    }
}
using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Interfaces;
using FitnessTracking.Contexts;
using FitnessTracking.Repositories;
using FitnessTracking.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using FitnessTracking.Helpers;

namespace FitnessTracking.Services
{
    public class ProgressService : IProgressService
    {
        private readonly FitnessContext _context;
        private readonly ProgressRepository _progressRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProgressService(FitnessContext context, ProgressRepository progressRepository, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _progressRepository = progressRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<Guid> AddProgressAsync(ProgressAddRequestDto progress)
        {
            if (progress == null)
                throw new CustomeExceptionHandler("Progress data cannot be null", 400);

            if (progress.WorkOutPlanId == Guid.Empty)
                throw new CustomeExceptionHandler("WorkOut Plan ID cannot be empty or null", 400);

            var progressModel = new ProgressModel
            {
                UserId = progress.UserId,
                WorkOutPlanId = progress.WorkOutPlanId,
                Date = progress.Date,
                Weight = progress.Weight,
                BodyFatPercentage = progress.BodyFatPercentage,
                MuscleMass = progress.MuscleMass,
                WaterPercentage = progress.WaterPercentage,
                Notes = progress.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProgressUpdates.Add(progressModel);
            await _context.SaveChangesAsync();

            return progressModel.Id; // 👈 Return the generated ID
        }



        public async Task<IEnumerable<ProgressResponseDto>> GetAllProgressAsync()
        {
            var progressUpdatesRaw = await _context.ProgressUpdates
                .Include(p => p.User)
                .Include(p => p.WorkOutPlan)
                .Include(p => p.ProgressImage)
                .ToListAsync();

            if (progressUpdatesRaw == null || !progressUpdatesRaw.Any())
                throw new CustomeExceptionHandler("No progress updates found", 404);

            var request = _httpContextAccessor.HttpContext?.Request;
            var scheme = request?.Scheme ?? "http";
            var host = request?.Host.ToString() ?? "localhost";

            var progressUpdates = progressUpdatesRaw.Select(p => new ProgressResponseDto
            {
                Id = p.Id,
                UserId = p.UserId,
                UserName = p.User != null ? p.User.Name : string.Empty,
                WorkOutPlanId = p.WorkOutPlanId,
                WorkOutPlanName = p.WorkOutPlan != null ? p.WorkOutPlan.Name : string.Empty,
                Date = p.Date,
                Weight = p.Weight,
                BodyFatPercentage = p.BodyFatPercentage,
                MuscleMass = p.MuscleMass,
                WaterPercentage = p.WaterPercentage,
                Notes = p.Notes,
                CreatedAt = p.CreatedAt,
                ProgressImage = p.ProgressImage.Select(img => new ProgressImageDto
                {
                    Id = img.Id,
                    ContentType = img.ContentType,
                    FileName = $"Progress_{img.ProgressId}_{img.UploadedAt:yyyyMMddHHmmss}.{ImageExtension.GetExtension(img.ContentType)}",
                    DownloadUrl = $"{scheme}://{host}/api/v1/progressimage/download/{img.Id}"
                }).ToList()
            }).ToList();

            return progressUpdates;
        }

        public async Task<ProgressResponseDto?> GetProgressByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new CustomeExceptionHandler("Progress ID cannot be empty or null", 400);

            var request = _httpContextAccessor.HttpContext?.Request;
            var scheme = request?.Scheme ?? "http";
            var host = request?.Host.ToString() ?? "localhost";

            var progress = await _context.ProgressUpdates
                .Include(p => p.User)
                .Include(p => p.WorkOutPlan)
                .Include(p => p.ProgressImage)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (progress == null)
                throw new CustomeExceptionHandler("Progress not found", 404);

            return new ProgressResponseDto
            {
                Id = progress.Id,
                UserId = progress.UserId,
                UserName = progress.User?.Name ?? string.Empty,
                WorkOutPlanId = progress.WorkOutPlanId,
                WorkOutPlanName = progress.WorkOutPlan?.Name ?? string.Empty,
                Date = progress.Date,
                Weight = progress.Weight,
                BodyFatPercentage = progress.BodyFatPercentage,
                MuscleMass = progress.MuscleMass,
                WaterPercentage = progress.WaterPercentage,
                Notes = progress.Notes,
                CreatedAt = progress.CreatedAt,
                ProgressImage = progress.ProgressImage.Select(img => new ProgressImageDto
                {
                    Id = img.Id,
                    ContentType = img.ContentType,
                    FileName = $"Progress_{img.ProgressId}_{img.UploadedAt:yyyyMMddHHmmss}.{ImageExtension.GetExtension(img.ContentType)}",
                    DownloadUrl = $"{scheme}://{host}/api/v1/progressimage/download/{img.Id}"
                }).ToList()
            };
        }

        public async Task<List<ProgressResponseDto>> GetProgressByUserIdAndWorkOutPlanIdAsync(Guid userId, Guid workOutPlanId)
        {
            if (userId == Guid.Empty || workOutPlanId == Guid.Empty)
                throw new CustomeExceptionHandler("User ID and WorkOut Plan ID cannot be empty or null", 400);

            var progresses = await _context.ProgressUpdates
                .Include(p => p.User)
                .Include(p => p.WorkOutPlan)
                .Include(p => p.ProgressImage)
                .Where(p => p.UserId == userId && p.WorkOutPlanId == workOutPlanId)
                .OrderBy(p => p.Date) // Optional: to show progress in timeline
                .ToListAsync();

            if (progresses == null || !progresses.Any())
                throw new CustomeExceptionHandler("No progress entries found for the specified user and workout plan", 404);

            var request = _httpContextAccessor.HttpContext?.Request;
            var scheme = request?.Scheme ?? "http";
            var host = request?.Host.ToString() ?? "localhost";

            return progresses.Select(progress => new ProgressResponseDto
            {
                Id = progress.Id,
                UserId = progress.UserId,
                UserName = progress.User?.Name ?? string.Empty,
                WorkOutPlanId = progress.WorkOutPlanId,
                WorkOutPlanName = progress.WorkOutPlan?.Name ?? string.Empty,
                Date = progress.Date,
                Weight = progress.Weight,
                BodyFatPercentage = progress.BodyFatPercentage,
                MuscleMass = progress.MuscleMass,
                WaterPercentage = progress.WaterPercentage,
                Notes = progress.Notes,
                CreatedAt = progress.CreatedAt,
                ProgressImage = progress.ProgressImage.Select(img => new ProgressImageDto
                {
                    Id = img.Id,
                    ContentType = img.ContentType,
                    FileName = $"Progress_{img.ProgressId}_{img.UploadedAt:yyyyMMddHHmmss}.{ImageExtension.GetExtension(img.ContentType)}",
                    DownloadUrl = $"{scheme}://{host}/api/v1/progressimage/download/{img.Id}"
                }).ToList()
            }).ToList();
        }

        public async Task<IEnumerable<ProgressResponseDto>> GetWorkOutProgressByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new CustomeExceptionHandler("WorkOut Plan ID cannot be empty or null", 400);

            var progressUpdates = await _context.ProgressUpdates
                .Include(p => p.User)
                .Include(p => p.WorkOutPlan)
                .Include(p => p.ProgressImage)
                .Where(p => p.UserId == userId)
                .ToListAsync();

            if (progressUpdates == null || !progressUpdates.Any())
                throw new CustomeExceptionHandler("No progress updates found for the specified workout plan", 404);

            var request = _httpContextAccessor.HttpContext?.Request;
            var scheme = request?.Scheme ?? "http";
            var host = request?.Host.ToString() ?? "localhost";

            var progress = progressUpdates.Select(p => new ProgressResponseDto
            {
                Id = p.Id,
                UserId = p.UserId,
                UserName = p.User != null ? p.User.Name : string.Empty,
                WorkOutPlanId = p.WorkOutPlanId,
                WorkOutPlanName = p.WorkOutPlan != null ? p.WorkOutPlan.Name : string.Empty,
                Date = p.Date,
                Weight = p.Weight,
                BodyFatPercentage = p.BodyFatPercentage,
                MuscleMass = p.MuscleMass,
                WaterPercentage = p.WaterPercentage,
                Notes = p.Notes,
                CreatedAt = p.CreatedAt,
                ProgressImage = p.ProgressImage.Select(img => new ProgressImageDto
                {
                    Id = img.Id,
                    ContentType = img.ContentType,
                    FileName = $"Progress_{img.ProgressId}_{img.UploadedAt:yyyyMMddHHmmss}.{ImageExtension.GetExtension(img.ContentType)}",
                    DownloadUrl = $"{scheme}://{host}/api/v1/progressimage/download/{img.Id}"
                }).ToList()
            }).ToList();

            return progress;

        }

        public async Task<PaginatedResult<ProgressResponseDto>> GetFilteredProgressUpdateAsync(ProgressUpdateFilterDto progressFilterDto)
        {
            var query = _context.ProgressUpdates
                .Include(p => p.WorkOutPlan)
                .Include(p => p.User)
                .Include(p => p.ProgressImage)
                .AsQueryable();

            if (progressFilterDto.WorkOutPlanId.HasValue && progressFilterDto.WorkOutPlanId != Guid.Empty)
            {
                query = query.Where(p => p.WorkOutPlanId == progressFilterDto.WorkOutPlanId);
            }

            if (!string.IsNullOrEmpty(progressFilterDto.SortBy))
            {
                var isAscending = progressFilterDto.SortDirection.ToLower() == "asc";
                switch (progressFilterDto.SortBy.ToLower())
                {
                    case "weight":
                        query = isAscending
                            ? query.OrderBy(p => p.Weight)
                            : query.OrderByDescending(p => p.Weight);
                        break;
                    case "name":
                        query = isAscending
                            ? query.OrderBy(p => p.User != null ? p.User.Name : string.Empty)
                            : query.OrderByDescending(p => p.User != null ? p.User.Name : string.Empty);
                        break;
                    case "bodyfatpercentage":
                        query = isAscending
                            ? query.OrderBy(p => p.BodyFatPercentage)
                            : query.OrderByDescending(p => p.BodyFatPercentage);
                        break;
                    case "musclemass":
                        query = isAscending
                            ? query.OrderBy(p => p.MuscleMass)
                            : query.OrderByDescending(p => p.MuscleMass);
                        break;
                    case "waterpercentage":
                        query = isAscending
                            ? query.OrderBy(p => p.WaterPercentage)
                            : query.OrderByDescending(p => p.WaterPercentage);
                        break;
                    case "createdat":
                        query = isAscending
                            ? query.OrderBy(p => p.CreatedAt)
                            : query.OrderByDescending(p => p.CreatedAt);
                        break;
                    case "date":
                        query = isAscending
                            ? query.OrderBy(p => p.Date)
                            : query.OrderByDescending(p => p.Date);
                        break;
                    default:
                        query = isAscending
                            ? query.OrderBy(p => p.CreatedAt)
                            : query.OrderByDescending(p => p.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(p => p.User.Name);
            }


            var httpContext = _httpContextAccessor.HttpContext;
            var baseUrl = $"{httpContext?.Request.Scheme}://{httpContext?.Request.Host}";
            int totalCount = await query.CountAsync();

            var paginatedData = await query
                .Skip((progressFilterDto.PageNumber - 1) * progressFilterDto.PageSize)
                .Take(progressFilterDto.PageSize)
                .ToListAsync();

            var resultData = paginatedData.Select(p => new ProgressResponseDto
            {
                Id = p.Id,
                UserId = p.UserId,
                UserName = p.User?.Name ?? string.Empty,
                WorkOutPlanId = p.WorkOutPlanId,
                WorkOutPlanName = p.WorkOutPlan?.Name ?? string.Empty,
                Date = p.Date,
                Weight = p.Weight,
                BodyFatPercentage = p.BodyFatPercentage,
                MuscleMass = p.MuscleMass,
                WaterPercentage = p.WaterPercentage,
                Notes = p.Notes,
                CreatedAt = p.CreatedAt,
                ProgressImage = p.ProgressImage?.Select(img => new ProgressImageDto
                {
                    Id = img.Id,
                    FileName = $"Progress_{img.ProgressId}_{img.UploadedAt:yyyyMMddHHmmss}.{GetExtension(img.ContentType)}",
                    ContentType = img.ContentType,
                    DownloadUrl = $"{baseUrl}/api/v1/progressimage/download/{img.Id}"
                }).ToList() ?? new List<ProgressImageDto>()
            }).ToList();
            return new PaginatedResult<ProgressResponseDto>
            {
                Data = resultData,
                TotalCount = totalCount,
                PageNumber = progressFilterDto.PageNumber,
                PageSize = progressFilterDto.PageSize
            };
        }

        public async Task<IEnumerable<ProgressResponseDto>> GetProgressByCoachIdAsync(Guid coachId)
        {
            if (coachId == Guid.Empty)
                throw new CustomeExceptionHandler("Coach ID cannot be empty or null", 400);

            var clientIds = await _context.CoachClientMaps
                .Where(m => m.CoachId == coachId)
                .Select(m => m.ClientId)
                .ToListAsync();

            if (!clientIds.Any())
                throw new CustomeExceptionHandler("No clients mapped to this coach", 404);

            var progressUpdatesRaw = await _context.ProgressUpdates
                .Include(p => p.User)
                .Include(p => p.WorkOutPlan)
                .Include(p => p.ProgressImage)
                .Where(p => clientIds.Contains(p.UserId))
                .ToListAsync();

            if (!progressUpdatesRaw.Any())
                throw new CustomeExceptionHandler("No progress updates found for the coach", 404);

            var request = _httpContextAccessor.HttpContext?.Request;
            var scheme = request?.Scheme ?? "http";
            var host = request?.Host.ToString() ?? "localhost";

            var progressUpdates = progressUpdatesRaw.Select(p => new ProgressResponseDto
            {
                Id = p.Id,
                UserId = p.UserId,
                UserName = p.User?.Name ?? string.Empty,
                WorkOutPlanId = p.WorkOutPlanId,
                WorkOutPlanName = p.WorkOutPlan?.Name ?? string.Empty,
                Date = p.Date,
                Weight = p.Weight,
                BodyFatPercentage = p.BodyFatPercentage,
                MuscleMass = p.MuscleMass,
                WaterPercentage = p.WaterPercentage,
                Notes = p.Notes,
                CreatedAt = p.CreatedAt,
                ProgressImage = p.ProgressImage?.Select(img => new ProgressImageDto
                {
                    Id = img.Id,
                    ContentType = img.ContentType,
                    FileName = $"Progress_{img.ProgressId}_{img.UploadedAt:yyyyMMddHHmmss}.{ImageExtension.GetExtension(img.ContentType)}",
                    DownloadUrl = $"{scheme}://{host}/api/v1/progressimage/download/{img.Id}"
                }).ToList()
            }).ToList();

            return progressUpdates;
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

        public async Task UpdateProgressAsync(Guid id, UpdateProgressDto updateProgressDto)
        {
           var progress = await _context.ProgressUpdates.FindAsync(id);
            if (progress == null)
                throw new CustomeExceptionHandler("Progress not found", 404);

            if (updateProgressDto.WorkOutLogId != Guid.Empty)
                progress.WorkOutPlanId = updateProgressDto.WorkOutLogId;

            if (updateProgressDto.Date != default)
            {
                progress.Date = updateProgressDto.Date;
            }
            if (updateProgressDto.Weight > 0)
            {
                progress.Weight = updateProgressDto.Weight;
            }
            if (updateProgressDto.BodyFatPercentage > 0)
            {
                progress.BodyFatPercentage = updateProgressDto.BodyFatPercentage;
            }
            if (updateProgressDto.MuscleMass > 0)
            {
                progress.MuscleMass = updateProgressDto.MuscleMass;
            }
            if (updateProgressDto.WaterPercentage > 0)
            {
                progress.WaterPercentage = updateProgressDto.WaterPercentage;
            }
            if (!string.IsNullOrEmpty(updateProgressDto.Notes))
            {
                progress.Notes = updateProgressDto.Notes;
            }

            _context.ProgressUpdates.Update(progress);
            await _context.SaveChangesAsync();
        }
    }
}
using FitnessTracking.Models;
using FitnessTracking.Models.DTOs;
using FitnessTracking.Interfaces;
using FitnessTracking.Contexts;
using FitnessTracking.Misc;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Services
{
    public class CoachClientMapService : ICoachMappingService
    {
        private readonly FitnessContext _context;
        private readonly INotificationService _notificationService;

        public CoachClientMapService(FitnessContext context, INotificationService notificationService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task<CoachMappingResponseDto> AddCoachClientMappingAsync(CoachMappingRequestDto dto)
        {
            if (dto == null || dto.CoachId == Guid.Empty || dto.ClientId == Guid.Empty)
                throw new ArgumentException("CoachId and ClientId must be provided.");

            if (dto.CoachId == dto.ClientId)
                throw new InvalidOperationException("Coach and Client cannot be the same user.");

            var coach = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.CoachId && u.Role == "Coach");
            var client = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.ClientId && u.Role == "User");

            if (coach == null || client == null)
                throw new KeyNotFoundException("Invalid Coach or Client ID.");

            var exists = await _context.CoachClientMaps.AnyAsync(m => m.CoachId == dto.CoachId && m.ClientId == dto.ClientId);
            if (exists)
                throw new InvalidOperationException("Mapping already exists.");

            var mapping = new CoachClientMap
            {
                Id = Guid.NewGuid(),
                CoachId = dto.CoachId,
                ClientId = dto.ClientId,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationService.SendNotificationAsync(
                dto.ClientId.ToString(),
                $"You have been assigned a new coach: {coach.Name}.",
                true);
            await _notificationService.SendNotificationAsync(
                dto.CoachId.ToString(),
                $"You have been assigned a new client: {client.Name}.",
                true);
                
            _context.CoachClientMaps.Add(mapping);
            await _context.SaveChangesAsync();

            return new CoachMappingResponseDto
            {
                Id = mapping.Id,
                CoachId = coach.Id,
                CoachName = coach.Name,
                CoachEmail = coach.Email,
                ClientId = client.Id,
                ClientName = client.Name,
                ClientEmail = client.Email,
                CreatedAt = mapping.CreatedAt
            };
        }

        public async Task<ApiSuccessResponseDto<IEnumerable<CoachMappingResponseDto>>> GetAllCoachClientMappingsAsync()
        {
            var mappings = await _context.CoachClientMaps
                .Include(m => m.Coach)
                .Include(m => m.Client)
                .ToListAsync();

            var result = mappings.Select(m => new CoachMappingResponseDto
            {
                CoachId = m.CoachId,
                CoachName = m.Coach?.Name ?? "",
                CoachEmail = m.Coach?.Email ?? "",
                ClientId = m.ClientId,
                ClientName = m.Client?.Name ?? "",
                ClientEmail = m.Client?.Email ?? "",
                CreatedAt = m.CreatedAt
            });

            return new ApiSuccessResponseDto<IEnumerable<CoachMappingResponseDto>>
            {
                Data = result,
                Message = "All coach-client mappings fetched successfully."
            };
        }


        public async Task<ApiSuccessResponseDto<IEnumerable<CoachMappingResponseDto>>> GetClientsByCoachIdAsync(Guid coachId)
        {
            var mappings = await _context.CoachClientMaps
                .Include(m => m.Client)
                .Where(m => m.CoachId == coachId)
                .ToListAsync();

            var result = mappings.Select(m => new CoachMappingResponseDto
            {
                CoachId = m.CoachId,
                CoachName = _context.Users.Find(m.CoachId)?.Name,
                CoachEmail = _context.Users.Find(m.CoachId)?.Email,
                ClientId = m.ClientId,
                ClientName = m.Client?.Name ?? "",
                ClientEmail = m.Client?.Email ?? "",
                CreatedAt = m.CreatedAt
            });

            return new ApiSuccessResponseDto<IEnumerable<CoachMappingResponseDto>>
            {
                Data = result,
                Message = "Clients fetched for coach."
            };
        }

        public async Task<ApiSuccessResponseDto<IEnumerable<CoachMappingResponseDto>>> GetCoachesByClientIdAsync(Guid clientId)
        {
            var mappings = await _context.CoachClientMaps
                .Include(m => m.Coach)
                .Where(m => m.ClientId == clientId)
                .ToListAsync();

            var result = mappings.Select(m => new CoachMappingResponseDto
            {
                CoachId = m.CoachId,
                CoachName = m.Coach?.Name ?? "",
                CoachEmail = m.Coach?.Email ?? "",
                ClientId = m.ClientId,
                ClientName = _context.Users.Find(m.ClientId)?.Name,
                ClientEmail = _context.Users.Find(m.ClientId)?.Email,
                CreatedAt = m.CreatedAt
            });

            return new ApiSuccessResponseDto<IEnumerable<CoachMappingResponseDto>>
            {
                Data = result,
                Message = "Coaches fetched for client."
            };
        }

        public async Task<ApiSuccessResponseDto<bool>> RemoveCoachClientMappingAsync(Guid coachId, Guid clientId)
        {
            var mapping = await _context.CoachClientMaps.FirstOrDefaultAsync(m =>
                m.CoachId == coachId && m.ClientId == clientId);

            if (mapping == null)
                return new ApiSuccessResponseDto<bool>
                {
                    Data = false,
                    Message = "Mapping not found."
                };

            _context.CoachClientMaps.Remove(mapping);
            await _context.SaveChangesAsync();

            return new ApiSuccessResponseDto<bool>
            {
                Data = true,
                Message = "Mapping removed successfully."
            };
        }
    }
}

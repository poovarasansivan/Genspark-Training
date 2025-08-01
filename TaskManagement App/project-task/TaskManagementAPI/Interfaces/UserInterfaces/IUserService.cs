using TaskManagementAPI.ApiResponses;
using TaskManagementAPI.DTOs.Users;

namespace TaskManagementAPI.Interfaces
{
    public interface IUserService
    {
        Task<PagedResponse<UserResponseDto>> GetAllAsync(int page, int pageSize, string? search = null, string? role = null);
        Task<ApiResponse<UserResponseDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<UserResponseDto>> CreateAsync(UserRequestDto dto, Guid createdById);
        Task<ApiResponse<UserResponseDto>> UpdateAsync(Guid id, UserUpdateDto dto, Guid updatedById);
        Task<ApiResponse<string>> DeleteAsync(Guid id);
        Task<ApiResponse<List<UserResponseDto>>> GetAllTeamMembersAsync();

    }
}

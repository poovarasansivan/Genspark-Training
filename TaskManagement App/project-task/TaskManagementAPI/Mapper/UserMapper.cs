using TaskManagementAPI.Models;
using TaskManagementAPI.DTOs.Users;

namespace TaskManagementAPI.Mapper
{
    public class UserMapper
    {
        public User MapUserRequestDtoToUser(UserRequestDto dto, string passwordHash)
        {
            return new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = dto.Role
            };
        }

        public void MapUserUpdateDtoToUser(UserUpdateDto dto, User user, string? passwordHash, Guid updatedById)
        {
            if (dto.FullName != null)
                user.FullName = dto.FullName;

            if (dto.Email != null)
                user.Email = dto.Email;

            if (dto.Password != null && passwordHash != null)
                user.PasswordHash = passwordHash;

            if (dto.Role.HasValue)
                user.Role = dto.Role.Value;

            user.UpdatedById = updatedById;
            user.UpdatedAt = DateTime.UtcNow;
        }

        public UserResponseDto MapToUserResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public IEnumerable<UserResponseDto> MapToUserResponseDtoList(IEnumerable<User> users)
        {
            return users.Select(MapToUserResponseDto).ToList();
        }
    }
}
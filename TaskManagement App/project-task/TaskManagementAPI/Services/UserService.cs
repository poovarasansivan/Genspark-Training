using TaskManagementAPI.ApiResponses;
using TaskManagementAPI.DTOs.Users;
using TaskManagementAPI.Interfaces;
using TaskManagementAPI.Mapper;
using TaskManagementAPI.Models;
using TaskManagementAPI.CustomExceptions;
using BCrypt.Net;
using TaskManagementAPI.Enums;

namespace TaskManagementAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserMapper _userMapper;

        public UserService(IUserRepository userRepository, UserMapper userMapper)
        {
            _userRepository = userRepository;
            _userMapper = userMapper;
        }

        public async Task<PagedResponse<UserResponseDto>> GetAllAsync(int page, int pageSize, string? search = null, string? role = null)
        {
            var allUsers = (await _userRepository.GetAll()).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                allUsers = allUsers.Where(u =>
                    u.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                allUsers = allUsers
                    .Where(u => string.Equals(u.Role.ToString(), role, StringComparison.OrdinalIgnoreCase));
            }

            var totalRecords = allUsers.Count();
            var pagedUsers = allUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtoList = _userMapper.MapToUserResponseDtoList(pagedUsers);
            return PagedResponse<UserResponseDto>.Create(dtoList, page, pageSize, totalRecords);
        }



        public async Task<ApiResponse<UserResponseDto>> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.Get(id);
            if (user == null)
                return ApiResponse<UserResponseDto>.ErrorResponse($"User with ID {id} not found", new Dictionary<string, List<string>>());


            var response = _userMapper.MapToUserResponseDto(user);
            return ApiResponse<UserResponseDto>.SuccessResponse(response);
        }

        public async Task<ApiResponse<UserResponseDto>> CreateAsync(UserRequestDto dto, Guid createdById)
        {

            if (!Enum.IsDefined(typeof(UserRole), dto.Role))
            {
                throw new BadRequestException("Invalid role. Allowed values: Manager, TeamMember.");
            }
            var existingUser = await _userRepository.GetByEmail(dto.Email);
            if (existingUser != null)
                throw new ConflictException("User with the given email already exists.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = _userMapper.MapUserRequestDtoToUser(dto, passwordHash);

            user.CreatedById = createdById;

            var created = await _userRepository.Add(user);

            var response = _userMapper.MapToUserResponseDto(created);


            return ApiResponse<UserResponseDto>.SuccessResponse(response, "User created successfully");
        }

        public async Task<ApiResponse<UserResponseDto>> UpdateAsync(Guid id, UserUpdateDto dto, Guid updatedById)
        {
            var user = await _userRepository.Get(id);
            if (user == null)
                throw new NotFoundException("User not found.");

            string? passwordHash = null;
            if (!string.IsNullOrEmpty(dto.Password))
            {
                passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            _userMapper.MapUserUpdateDtoToUser(dto, user, passwordHash, updatedById);
            await _userRepository.Update(id, user);

            var response = _userMapper.MapToUserResponseDto(user);

            return ApiResponse<UserResponseDto>.SuccessResponse(response, "User updated successfully");
        }

        public async Task<ApiResponse<string>> DeleteAsync(Guid id)
        {
            var user = await _userRepository.Get(id);
            if (user == null)
                throw new NotFoundException("User not found.");

            await _userRepository.Delete(id);
            return ApiResponse<string>.SuccessResponse("User deleted successfully");
        }

        public async Task<ApiResponse<List<UserResponseDto>>> GetAllTeamMembersAsync()
        {
            var allUsers = await _userRepository.GetAll();
            var teamMembers = allUsers
                .Where(u => u.Role == Enums.UserRole.TeamMember)
                .ToList();

            var dtoList = _userMapper.MapToUserResponseDtoList(teamMembers).ToList();
            return ApiResponse<List<UserResponseDto>>.SuccessResponse(dtoList, "Team members fetched successfully");
        }

    }
}

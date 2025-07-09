using Api.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Api.Context;
using Api.Interfaces;
using Api.Models;

namespace Api.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task AddUserAsync(UserAddRequestDto user)
        {
            var newUser = new UserModel
            {
                Id = Guid.NewGuid().ToString(),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
        }
    }
}
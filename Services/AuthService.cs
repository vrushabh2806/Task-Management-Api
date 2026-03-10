using Microsoft.EntityFrameworkCore;
using TaskManagement.DTOs;
using TaskManagement.Models;
using TaskManagement.pragma;
using TaskManagement.Services;
namespace TaskManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<UserResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var existinguUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email || u.Username == registerDto.Username);
            if (existinguUser != null)
            {
                throw new Exception("Username already exists.");
            }
            var passwordHash=BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            var user=new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };

        }
    }
}
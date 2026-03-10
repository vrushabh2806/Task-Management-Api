using TaskManagement.DTOs;

namespace TaskManagement.Services
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
    }
    
}
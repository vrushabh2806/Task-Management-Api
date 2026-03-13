using TaskManagement.DTOs;

namespace TaskManagement.Services
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto,string ipAddress);
        Task<UserResponseDto> RegisterWithRoleAsync(RegisterDto registerDto,string role);

        Task<RefreshTokenResponseDto> RefreshTokenAsync(string token, string ipAddress);  // NEW

        Task<bool> RevokeTokenAsync(string token,string ipAddress);
    }
    
}
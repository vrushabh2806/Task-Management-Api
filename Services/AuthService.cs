using System.Net.Http.Headers;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using TaskManagement.DTOs;
using TaskManagement.Models;
using TaskManagement.Data;
using TaskManagement.Services;
//using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
namespace TaskManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<UserResponseDto> RegisterAsync(RegisterDto registerDto)

        {
            var existinguUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email || u.Username == registerDto.Username);
            if (existinguUser != null)
            {
                throw new Exception("Username already exists.");
            }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            var user = new User
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
        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            var token = GenerateJwtToken(user);
            var expirationInMinutes=double.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60");
            return new LoginResponseDto
            {
                 Token=token,
                 User=new UserResponseDto{
                    Id=user.Id,
                    Username=user.Username,
                    Email=user.Email,
                    CreatedAt=user.CreatedAt
                 },
                 ExpiredAt=expirationInMinutes > 0 ? DateTime.UtcNow.AddMinutes(expirationInMinutes) : DateTime.UtcNow.AddHours(1)
            };


        }
        private string GenerateJwtToken(User user)
        {
            // var jwtSettings = _configuration.GetSection("JwtSettings");
            // var secretKey = jwtSettings["SecretKey"];
            // var issuer = jwtSettings["Issuer"];
            // var audience = jwtSettings["Audience"];
            // var expirationInMinutes = int.Parse(jwtSettings["ExpirationInMinutes"]);

            // var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            // var key = System.Text.Encoding.ASCII.GetBytes(secretKey);
            // var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            // {
            //     Subject = new System.Security.Claims.ClaimsIdentity(new[]
            //     {
            //         new System.Security.Claims.Claim("id", user.Id.ToString()),
            //         new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username ?? ""),
            //         new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email ?? "")
            //     }),
            //     Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
            //     Issuer = issuer,
            //     Audience = audience,
            //     SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key), Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
            // };
            // var token=tokenHandler.CreateToken(tokenDescriptor);
            // return tokenHandler.WriteToken(token);
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expirationInMinutes = double.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.UniqueName,user.Username ?? ""),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Email,user.Email ?? ""),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);



        }

    }


}




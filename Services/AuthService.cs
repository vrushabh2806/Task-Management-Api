using System.Net.Http.Headers;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using TaskManagement.DTOs;
using TaskManagement.Models;
using TaskManagement.Data;
using TaskManagement.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using TaskManagement.Constants;
using System.Net;
using System.Security.Cryptography;
using TaskManagement.Migrations;
using Microsoft.Identity.Client;
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
            // var existinguUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email || u.Username == registerDto.Username);
            // if (existinguUser != null)
            // {
            //     throw new Exception("Username already exists.");
            // }
            // var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            // var user = new User
            // {
            //     Username = registerDto.Username,
            //     Email = registerDto.Email,
            //     PasswordHash = passwordHash,
            //     CreatedAt = DateTime.UtcNow
            // };
            // _context.Users.Add(user);
            // await _context.SaveChangesAsync();
            // return new UserResponseDto
            // {
            //     Id = user.Id,
            //     Username = user.Username,
            //     Email = user.Email,
            //     CreatedAt = user.CreatedAt
            // };

            return await RegisterWithRoleAsync(registerDto, RoleConstants.User);

        }
        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto, string ipAddress)
        {
            var user = await _context.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.UserId = user.Id;
            _context.RefreshTokens.Add(refreshToken);
            await RemoveOldRefreshTokens(user.Id);
            await _context.SaveChangesAsync();
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60"));


            var expirationInMinutes = double.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60");
            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                User = new UserResponseDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    Role = user.Role?.Name
                },
                AccessTokenExpiresAt = accessTokenExpiry,
                RefreshTokenExpiresAt = refreshToken.ExpiresAt
            };


        }
        public async Task<UserResponseDto> RegisterWithRoleAsync(RegisterDto registerDto, string roleName)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email || u.Username == registerDto.Username);
            if (existingUser != null)
            {
                throw new Exception("Username already exists.");
            }
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null)
            {
                role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == RoleConstants.User);
                if (role == null)
                {
                    throw new Exception("Default user role not found. Please ensure the database is seeded with roles.");
                }
            }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                RoleId = role.Id,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _context.Entry(user).Reference(u => u.Role).LoadAsync();
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Role = user.Role?.Name
            };
        }

        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(string token, string ipAddress)
        {
            var user = await GetUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(rt => rt.Token == token);
            if (!refreshToken.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }
            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.RevokedAt = DateTime.UtcNow;

            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            _context.RefreshTokens.Add(newRefreshToken);
            await RemoveOldRefreshTokens(user.Id);
            await _context.SaveChangesAsync();
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60"));
            return new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                AccessTokenExpiresAt = accessTokenExpiry,
                RefreshTokenExpiresAt = newRefreshToken.ExpiresAt
            };
        }

        public async Task<bool> RevokeTokenAsync(string token, string ipAddress)
        {
            var user = await GetUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(rt => rt.Token == token);
            if (!refreshToken.IsActive)
            {
                return false;
            }
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            await _context.SaveChangesAsync();
            return true;
        }
        private string GenerateAccessToken(User user)
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
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new System.Security.Claims.Claim(ClaimTypes.Role,user.Role?.Name ?? "")
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

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            var refreshTokenExpiry = double.Parse(_configuration["JwtSettings:RefreshTokenExpirationInDays"] ?? "7");
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var refreshToken = Convert.ToBase64String(randomBytes);
            return new RefreshToken
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiry),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

        }

        private async Task RemoveOldRefreshTokens(int userId)
        {
            var tokensToRemove = await _context.RefreshTokens.Where(rt => rt.UserId == userId && !rt.IsActive && rt.CreatedAt.Value.AddDays(30) < DateTime.UtcNow).ToListAsync();
            if (tokensToRemove.Any())
            {
                _context.RefreshTokens.RemoveRange(tokensToRemove);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<User> GetUserByRefreshToken(string token)
        {
            var user = await _context.Users.Include(u => u.Role).Include(u => u.RefreshTokens).SingleOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == token));
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }
            return user;
        }

    }


}




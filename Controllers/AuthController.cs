using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using TaskManagement.DTOs;
using TaskManagement.Services;
using System.Net.Http.Headers;
using TokenManagement.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var user = await _authService.RegisterAsync(registerDto);
                return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var ipAddress = GetIpAddress();
                var response = await _authService.LoginAsync(loginDto, ipAddress);
                SetRefreshTokenCookie(response.RefreshToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefershToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var IpAddress = GetIpAddress();
                var response = await _authService.RefreshTokenAsync(request.RefreshToken, IpAddress);
                SetRefreshTokenCookie(response.RefreshToken);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        
        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var ipAddress = GetIpAddress();
                var success = await _authService.RevokeTokenAsync(request.RefreshToken, ipAddress);
                if (!success)
                {
                    return NotFound("Token not found.");
                }
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }  
        }
            
        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For:")) ;
            {
                return Request.Headers["X-Forwarded-For"].ToString();

            }
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/auth/refresh-token"
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    
    
    }
}
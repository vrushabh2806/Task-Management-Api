using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Services;
using TaskManagement.DTOs;
using TaskManagement.Constants;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "AdminOnly")]

    public class AdminController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _context;

        public AdminController(IAuthService authService, ApplicationDbContext context)
        {
            _context = context;
            _authService = authService;
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.Include(u => u.Role).Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    CreatedAt = u.CreatedAt,
                    Role = u.Role.Name
                }).ToListAsync();
                return Ok(users);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Role).Include(u => u.Tasks).Where(u => u.Id == id).Select(u => new
                {
                    Id = u.Id,
                    u.Username,
                    u.Email,
                    u.CreatedAt,
                    Role = u.Role.Name,
                    TasksCount = u.Tasks.Count
                }).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                return Ok(user);

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDto registerDto)
        {
            try
            {
                var roleName = string.IsNullOrWhiteSpace(registerDto.Role) ? RoleConstants.User : registerDto.Role;
                var user = await _authService.RegisterWithRoleAsync(registerDto, roleName);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
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
        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> ChangeUserRole(int id, [FromBody] ChangeRoleDto changeRoleDto)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound("User not found.");
                }
                var newRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == changeRoleDto.RoleName);
                if (newRole == null)
                {
                    return BadRequest("Invalid role.");
                }
                user.RoleId = newRole.Id;
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = $"User role updated to {newRole.Name} successfully.",
                    user = new UserResponseDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email,
                        CreatedAt = user.CreatedAt,
                        Role = newRole.Name
                    }
                });
                    
                

            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpDelete("users/{id}")]

        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Tasks).FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                _context.Tasks.RemoveRange(user.Tasks);
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        [HttpGet("stats")]
        public async Task<IActionResult> GetSystemStats()
        {
            try
            {
                var totalUsers= await _context.Users.CountAsync();
                var totalTasks= await _context.Tasks.CountAsync();
                var completedTasks= await _context.Tasks.CountAsync(t=>t.IsCompleted);
                var pendingTasks= totalTasks - completedTasks;
                return Ok(new
                {
                    TotalUsers=totalUsers,
                    TotalTasks=totalTasks,
                    CompletedTasks=completedTasks,
                    PendingTasks=pendingTasks
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }    
    
    }
}
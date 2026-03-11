using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using TaskManagement.DTOs;
using TaskManagement.Services;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using TaskManagement.Extensions;



namespace TaskManagement.Controllers
{
     [Route("api/[controller]")]   
      [ApiController]
      [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            try
            {
            //     var userId=int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
            //     var task=await _taskService.CreateTaskAsync(createTaskDto,userId);
            //     return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
            // 
            var userId=User.GetUserId();
            var task=await _taskService.CreateTaskAsync(createTaskDto,userId);
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
             catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");

            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserTasks()
        {
            try
            {
               var userId=User.GetUserId();
               var tasks=await _taskService. GetTasksUserTasksAsync(userId);
               return Ok(tasks);
            } 
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
             catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
       
       [HttpGet("{id}")]
       public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
             var userId=User.GetUserId();
             var task=await _taskService.GetTaskByIdAsync(id,userId);
                if(task==null)
                {
                    return NotFound();
                }
                return Ok(task);
            }
            catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
             catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }


    }
}
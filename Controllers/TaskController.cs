using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using TaskManagement.DTOs;
using TaskManagement.Services;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using TaskManagement.Extensions;
using System.Runtime.CompilerServices;



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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            try
            {
             var userId=User.GetUserId();
             var updatedTask=await _taskService.UpdateTaskAsync(id,updateTaskDto,userId);
                if(updatedTask==null)
                {
                    return NotFound();
                }
                return Ok(updatedTask);
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
              var userId=User.GetUserId();
              var success=await _taskService.DeleteTaskAsync(id,userId);
                if(!success)
                {
                    return NotFound();
                }
                return NoContent();   
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
   
           [HttpPatch("{id}/toggle-completion")]
        public async Task<IActionResult> ToggleTaskCompletion(int id)
        {
            try
            {
                var userId=User.GetUserId();
                var updatedTask=await _taskService.ToggleTaskCompletionAsync(id,userId);
                if(updatedTask==null)
                {
                    return NotFound();
                }
                return Ok(updatedTask);
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
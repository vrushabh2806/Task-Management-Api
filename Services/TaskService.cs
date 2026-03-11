using System.Net.Http.Headers;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using TaskManagement.DTOs;
using TaskManagement.Models;
using TaskManagement.Data;
using TaskManagement.Services;
using Microsoft.EntityFrameworkCore.Storage;
namespace TaskManagement.Services
{
    public class TaskService : ITaskService
    {
        public readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto createTaskDto, int userId)
        {
            var taskItem = new TaskItem
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                Priority = createTaskDto.Priority,
                DueDate = createTaskDto.DueDate,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            _context.Tasks.Add(taskItem);
            await _context.SaveChangesAsync();

            return new TaskResponseDto
            {
                Title = taskItem.Title,
                Description = taskItem.Description,
                Priority = taskItem.Priority,
                DueDate = taskItem.DueDate,
                IsCompleted = taskItem.IsCompleted,
                CreatedAt = taskItem.CreatedAt,
                UserId = taskItem.UserId
            };
        }
        public async Task<IEnumerable<TaskResponseDto>> GetTasksUserTasksAsync(int userId)
        {
            var tasks = await _context.Tasks.Where(t => t.UserId == userId).OrderByDescending(t => t.CreatedAt).Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                Priority = t.Priority,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                UserId = t.UserId
            }).ToListAsync();
            return tasks;
        }
        public async Task<TaskResponseDto> GetTaskByIdAsync(int taskId, int userId)
        {
            var task = await _context.Tasks.Where(t => t.UserId == userId && t.Id == taskId).OrderByDescending(t => t.CreatedAt).Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                Priority = t.Priority,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                UserId = t.UserId
            }).FirstOrDefaultAsync();
            return task;
        }
        public async Task<TaskResponseDto> UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto, int userId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
            if (task == null)
            {
                return null;
            }
            if (updateTaskDto.Title != null)
            {
                task.Title = updateTaskDto.Title;
            }
            if (updateTaskDto.Description != null)
            {
                task.Description = updateTaskDto.Description;
            }
            if (updateTaskDto.IsCompleted.HasValue)
            {
                task.IsCompleted = updateTaskDto.IsCompleted.Value;
            }
            if (updateTaskDto.Priority.HasValue)
            {
                task.Priority = updateTaskDto.Priority.Value;
            }
            if (updateTaskDto.DueDate.HasValue)
            {
                task.DueDate = updateTaskDto.DueDate.Value;
            }
            await _context.SaveChangesAsync();
            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                Priority = task.Priority,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                UserId = task.UserId

            };
        }

        public async Task<bool> DeleteTaskAsync(int taskId, int userId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
            if (task == null)
            {
                return false;
            }
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TaskResponseDto> ToggleTaskCompletionAsync(int taskId, int userId)
        {
            var task=await _context.Tasks.FirstOrDefaultAsync(t=>t.Id==taskId && t.UserId==userId);
            if(task==null)
            {
                return null;
            }
            task.IsCompleted=!task.IsCompleted;
            await _context.SaveChangesAsync();
            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                Priority = task.Priority,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                UserId = task.UserId
            };
        }

    }


}

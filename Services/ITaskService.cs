using TaskManagement.DTOs;

namespace TaskManagement.Services

{
    public interface ITaskService
    {
        Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto createTaskDto, int userId);
        Task<IEnumerable<TaskResponseDto>> GetTasksUserTasksAsync(int userId);
        Task<TaskResponseDto> GetTaskByIdAsync(int taskId, int userId);
    }
}
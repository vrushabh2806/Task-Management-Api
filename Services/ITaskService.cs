using TaskManagement.DTOs;

namespace TaskManagement.Services

{
    public interface ITaskService
    {
        Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto createTaskDto, int userId);
        Task<IEnumerable<TaskResponseDto>> GetUserTasksAsync(int userId);
        Task<TaskResponseDto> GetTaskByIdAsync(int taskId, int userId);
        Task<TaskResponseDto> UpdateTaskAsync(int taskId,UpdateTaskDto updateTaskDto,int userId);
        Task<bool> DeleteTaskAsync(int taskId,int userId); 

         Task<TaskResponseDto> ToggleTaskCompletionAsync(int taskId, int userId);
         Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync();
         Task<TaskResponseDto> GetAnyTaskByIdAsync(int taskId);
         Task<TaskResponseDto> UpdateAnyTaskAsync(int taskId, UpdateTaskDto updateTaskDto);
         Task<bool> DeleteAnyTaskAsync(int taskId);
         
    }
}
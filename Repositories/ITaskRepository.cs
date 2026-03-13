using TaskManagement.Models;
namespace TaskManagement.Repositories
{
    public interface ITaskRepository : IRepository<TaskItem>
    {
        Task<IEnumerable<TaskItem>> GetUserTasksAsync(int userId);
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetUserTaskByIdAsync(int taskId, int userId);
        Task<int> GetUserTaskCountAsync(int userId);
        


    }
}
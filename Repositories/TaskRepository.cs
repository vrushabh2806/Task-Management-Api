using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Models;
namespace TaskManagement.Repositories
{
    public class TaskRepository : Repository<TaskItem>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<TaskItem>> GetUserTasksAsync(int userId)
        {
            return await _dbSet.Where(t => t.UserId == userId).OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _dbSet.OrderByDescending(T=> T.CreatedAt).ToListAsync();

        }
        public async Task<TaskItem> GetUserTaskByIdAsync(int taskId,int userId)
        {
            return  await _dbSet.FirstOrDefaultAsync(t=> t.Id==taskId && t.UserId==userId);

        }
        public async Task<int> GetUserTaskCountAsync(int userId)
        {
            return await _dbSet.CountAsync(t=> t.UserId == userId);
        }

    }
}
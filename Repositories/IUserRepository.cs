using TaskManagement.Models;

namespace TaskManagement.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
            Task<User?> GetByEmailAsync(string email);
            Task<bool> EmailExistsAsync(string email);
            Task<bool> UsernameExistsAsync(string username);
            Task<User> GetUserWithRolesAsync(int userId);
            Task<User?> GetUserWithRefreshTokenAsync(string refreshToken);
            

    }
}
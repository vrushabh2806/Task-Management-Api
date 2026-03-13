using TaskManagement.Models;

namespace TaskManagement.Repositories
{
    public interface IRefreshTokenRepository:IRepository<RefreshToken>
    {
        Task<RefreshToken> GetByTokenAsync(string token);
        Task RemoveOldTokensAsync(int userId,int daysOld=30);

        

    }
}
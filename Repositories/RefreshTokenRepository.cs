using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Models;
namespace TaskManagement.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {

        }
        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            return await _dbSet.Include(rt => rt.User).ThenInclude(u => u.Role).FirstOrDefaultAsync(rt => rt.Token == token);

        }
        public async Task RemoveOldTokensAsync(int userId, int daysOld = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
            var oldTokens = await _dbSet.Where(rt => rt.UserId == userId && (rt.RevokedAt != null || rt.ExpiresAt < DateTime.UtcNow) && rt.CreatedAt < cutoffDate).ToListAsync();
            if (oldTokens.Any())
            {
                RemoveRange(oldTokens);
                await SaveChangesAsync();
            }


        }

    }

}
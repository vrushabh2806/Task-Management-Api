using System.Linq.Expressions;
using TaskManagement.Models;    

namespace TaskManagement.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);


        Task<T> AddAsync(T entity);
        Task<T> Update(T entity);
        Task Delete(T entity);

        void RemoveRange(IEnumerable<T> entities);
         
         Task<int> SaveChangesAsync();



    }
}
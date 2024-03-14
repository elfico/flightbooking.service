using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FlightBooking.Service.Data.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<int> BulkCreateAsync(IEnumerable<T> entities, bool isSave = true);

        Task<int> BulkDeleteAsync(IEnumerable<int> ids, bool isSave = true);

        Task<int> CreateAsync(T entity, bool isSave = true);

        Task<int> DeleteAsync(int id, bool isSave = true);

        Task<bool> EntityExistsAsync(int id);

        Task<T?> GetByIdAsync(int id);

        Task<T?> GetByGuidAsync(Guid id);

        Task<int> UpdateAsync(T entity, bool isSave = true);

        Task<int> SaveChangesToDbAsync();

        DbSet<T> GetDbSet();

        IQueryable<T> Query();

        Task<List<T>> TakeAndSkipAsync(IQueryable<T> data, int pageSize, int pageIndex);

        IQueryable<T> OrderByText(IQueryable<T> data, SortOrder order, Expression<Func<T, string>> expression);

        IQueryable<T> OrderByDate(IQueryable<T> data, SortOrder order, Expression<Func<T, DateTime>> expression);
    }
}
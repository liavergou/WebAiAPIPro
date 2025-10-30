using CoordExtractorApp.Data;

namespace CoordExtractorApp.Repositories
{
    public interface IBaseRepository<T> where T : BaseEntity
    {

        //ΒΑΣΙΚΟ CRUD
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);

        //ΒΑΣΙΚΑ GET
        Task<T?> GetAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<int> GetCountAsync();
    }
}

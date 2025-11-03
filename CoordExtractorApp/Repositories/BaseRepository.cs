using CoordExtractorApp.Data;
using Microsoft.EntityFrameworkCore;


namespace CoordExtractorApp.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {

        protected readonly TopoDbContext context;
        protected readonly DbSet<T> dbSet; //για να εφαρμοσθεί σε collection

        //constructor
        public BaseRepository(TopoDbContext context)
        { 
            this.context = context;
            dbSet = context.Set<T>(); //dynamically retrieves DbSet
        }

        //η ίδια η add δεν είναι async αλλά πριν γίνει το commit πρέπει να επικοινωνήσει με τη βαση για να πάρει το τελευταίο id
        public virtual async Task AddAsync(T entity) => await dbSet.AddAsync(entity);

         
        public virtual async Task AddRangeAsync(IEnumerable<T> entities) => await dbSet.AddRangeAsync(entities);

        //δεν είναι async γιατί έχω ήδη φέρει το entity με get γι αυτό κάνω Attach
        public virtual Task UpdateAsync(T entity)

        {
            entity.ModifiedAt = DateTime.UtcNow;
            dbSet.Attach(entity);
            context.Entry(entity).State = EntityState.Modified; //θα δουλεψει και χωρις?
            return Task.CompletedTask; //είναι σαν void
        }


        public virtual async Task<bool> DeleteAsync(int id)
        {
            T? existingEntity = await GetAsync(id);
            if (existingEntity == null) return false;        
            existingEntity.DeletedAt = DateTime.UtcNow;
            existingEntity.ModifiedAt = DateTime.UtcNow;       
            return true;

        }
        //IEnumerable Εχω βάλει global filters για τα deleted
        public virtual async Task<IEnumerable<T>> GetAllAsync() => await dbSet.ToListAsync();

        //null αν δεν υπάρχει
        public virtual async Task<T?> GetAsync(int id) => await dbSet.FindAsync(id);

        public virtual async Task<int> GetCountAsync() => await dbSet.CountAsync();

    }
}

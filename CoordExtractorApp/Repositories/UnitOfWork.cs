
using CoordExtractorApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CoordExtractorApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TopoDbContext context;

        public UnitOfWork(TopoDbContext context)
        {
            this.context = context;
        }

       
        public UserRepository UserRepository => new (context);
        public PromptRepository PromptRepository => new(context);

        public async Task<bool> SaveAsync()
        {
            return await context.SaveChangesAsync() > 0; //κανει commit ή αυτόματο rollback σε exception
        }
    }
}

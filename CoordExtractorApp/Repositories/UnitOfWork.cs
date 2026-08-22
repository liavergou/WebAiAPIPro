using CoordExtractorApp.Data;

namespace CoordExtractorApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TopoDbContext context;

        public UnitOfWork(TopoDbContext context)
        {
            this.context = context;
        }

       
        public IUserRepository UserRepository => new UserRepository(context);
        public IPromptRepository PromptRepository => new PromptRepository(context);

        public IProjectRepository ProjectRepository => new ProjectRepository(context);
        public IConversionJobRepository ConversionJobRepository => new ConversionJobRepository(context);
        

        public async Task<bool> SaveAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}

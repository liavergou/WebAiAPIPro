namespace CoordExtractorApp.Repositories
{
    public interface IUnitOfWork
    { 
        UserRepository UserRepository { get; } // Προσθήκη
        Task<bool> SaveAsync();
    }
}

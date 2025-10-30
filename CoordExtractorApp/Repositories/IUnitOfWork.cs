namespace CoordExtractorApp.Repositories
{
    public interface IUnitOfWork
    { 
        UserRepository UserRepository { get; } // Προσθήκη
        PromptRepository PromptRepository { get; }
        Task<bool> SaveAsync();
    }
}

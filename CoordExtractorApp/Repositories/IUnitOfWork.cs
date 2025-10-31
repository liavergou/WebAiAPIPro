namespace CoordExtractorApp.Repositories
{
    public interface IUnitOfWork
    { 
        UserRepository UserRepository { get; } // Προσθήκη
        PromptRepository PromptRepository { get; }
        ProjectRepository ProjectRepository { get; }

        Task<bool> SaveAsync();
    }
}

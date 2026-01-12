namespace CoordExtractorApp.Repositories
{
    /// <summary>
    /// Extension methods for configuring Dependency Injection for Repositories.
    /// </summary>
    public static class RepositoriesDIExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            //επεκτείνουμε το functionality. προσθέτει το 
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}

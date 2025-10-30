namespace CoordExtractorApp.Repositories
{
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

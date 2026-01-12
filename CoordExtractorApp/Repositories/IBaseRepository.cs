using CoordExtractorApp.Data;

namespace CoordExtractorApp.Repositories
{
    /// <summary>
    /// Generic repository interface responsible for basic CRUD operations.
    /// </summary>
    /// <typeparam name="T">The entity type which must inherit from <see cref="BaseEntity"/>.</typeparam>
    public interface IBaseRepository<T> where T : BaseEntity
    {

        //ΒΑΣΙΚΟ CRUD

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Adds a range of entities to the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param> 
        Task UpdateAsync(T entity);

        /// <summary>
        /// Deletes an entity by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>True if the entity was found and deleted, otherwise false.</returns>
        Task<bool> DeleteAsync(int id);

        //ΒΑΣΙΚΑ GET

        /// <summary>
        /// Retrieves an entity by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <returns>The entity if found, otherwise null.</returns>  
        Task<T?> GetAsync(int id);

        /// <summary>
        /// Retrieves all entities of type T.
        /// </summary>
        /// <returns>A collection of all entities.</returns>  
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Retrieves the total count of entities.
        /// </summary>
        /// <returns>The number of entities.</returns>
        Task<int> GetCountAsync();
    }
}

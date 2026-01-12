using CoordExtractorApp.Data;
using CoordExtractorApp.Models;
using System.Linq.Expressions;

namespace CoordExtractorApp.Repositories
{
    /// <summary>
    /// Repository interface for Conversion Job management.
    /// </summary>
    public interface IConversionJobRepository
    {
        /// <summary>
        /// Retrieves all conversion jobs belonging to a specific user.
        /// </summary>
        Task<List<ConversionJob>> GetJobsByUserIdAsync(int userId);

        /// <summary>
        /// Retrieves all conversion jobs associated with a specific project.
        /// </summary>
        Task<List<ConversionJob>> GetJobsByProjectIdAsync(int projectId);

        /// <summary>
        /// Retrieves all conversion jobs that used a specific prompt.
        /// </summary>
        Task<List<ConversionJob>> GetJobsByPromptIdAsync(int projectId);
    }
}

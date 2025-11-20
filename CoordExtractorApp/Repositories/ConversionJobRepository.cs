using CoordExtractorApp.Data;
using CoordExtractorApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CoordExtractorApp.Repositories
{
    public class ConversionJobRepository : BaseRepository<ConversionJob>, IConversionJobRepository
    {
        //constructor
        public ConversionJobRepository(TopoDbContext context) : base(context)
        {
        }

        public async Task<List<ConversionJob>> GetJobsByProjectIdAsync(int projectId)
        {
            List<ConversionJob> jobs = await context.Projects
                .Where(p => p.Id == projectId)
                .SelectMany(j => j.ConversionJobs)
                .ToListAsync();

            return jobs;

        }

        public async Task<List<ConversionJob>> GetJobsByPromptIdAsync(int promptId)
        {
           List<ConversionJob> jobs = await context.Prompts
                .Where(p => p.Id == promptId)
                .SelectMany(j => j.ConversionJobs)
                .ToListAsync();
            return jobs;
        }

        public async Task<List<ConversionJob>> GetJobsByUserIdAsync(int userId)
        {
            List<ConversionJob> jobs = await context.Users
                .Where(u => u.Id == userId)
                .SelectMany(j => j.ConversionJobs)
                .ToListAsync();

            return jobs;
        }

        public async Task<PaginatedResult<ConversionJob>> GetPaginatedJobsAsync(int pageNumber, int pageSize, List<Expression<Func<ConversionJob, bool>>> predicates)
        {
            IQueryable<ConversionJob> query = context.ConversionJobs;
            if (predicates != null) //αν εχει φιλτρα
            {
                foreach (var predicate in predicates) //loop για καθε φίλτρο
                {
                    query = query.Where(predicate); //προσθέτει τη συνθήκη
                }
            }
            int totalRecords = await query.CountAsync(); //μετράμε records στη βαση

            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(j => j.Id)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<ConversionJob>(data, totalRecords, pageNumber, pageSize);
        }

       
    }
}

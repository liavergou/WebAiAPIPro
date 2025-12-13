using CoordExtractorApp.Data;
using CoordExtractorApp.Models;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace CoordExtractorApp.Repositories
{
    public class PromptRepository : BaseRepository<Prompt>, IPromptRepository


    {
        //constructor
        public PromptRepository(TopoDbContext context) : base(context) { }

        public async Task<Prompt?> GetPromptByPromptNameAsync(string promptName)
        {
            var prompt = await context.Prompts
                .IgnoreQueryFilters() ////https://learn.microsoft.com/en-us/ef/core/querying/filters?tabs=ef10 disabling filters Για check existing στο create update
                .FirstOrDefaultAsync(p => p.PromptName == promptName);
            if (prompt == null) return null;

            return prompt;
            
        }


         public async Task<PaginatedResult<Prompt>> GetPaginatedPromptsAsync(int pageNumber, int pageSize, List<Expression<Func<Prompt, bool>>> predicates)
        {            
            
            IQueryable<Prompt> query = context.Prompts;

            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
            }

            int totalRecords = await query.CountAsync();
            var data = await query
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PaginatedResult<Prompt>
            {
                Data = data,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return result;
        }

       
    }
}

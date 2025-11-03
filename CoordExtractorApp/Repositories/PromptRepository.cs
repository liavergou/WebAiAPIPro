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
                .FirstOrDefaultAsync(p => p.PromptName == promptName);
            if (prompt == null) return null;

            return prompt;
            
        }


         public async Task<PaginatedResult<Prompt>> GetPaginatedPromptsAsync(int pageNumber, int pageSize)
        {            
            //χωρις φιλτρα οτι εχει μεσα
            IQueryable<Prompt> query = context.Prompts;               

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

using CoordExtractorApp.Data;
using CoordExtractorApp.Models;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace CoordExtractorApp.Repositories
{
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository


    {
        //constructor
        public ProjectRepository(TopoDbContext context) : base(context) { }

        public async Task<Project?> GetProjectByProjectNameAsync(string projectName)
        {
            var project = await context.Projects
                .FirstOrDefaultAsync(p => p.ProjectName == projectName);
            if (project == null) return null;

            return project;

        }

        public async Task<PaginatedResult<Project>> GetProjectsAsync(int pageNumber, int pageSize, List<Expression<Func<Project, bool>>> predicates)
        {
            IQueryable<Project> query = context.Projects;

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

            var result = new PaginatedResult<Project>
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
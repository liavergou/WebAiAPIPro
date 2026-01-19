using CoordExtractorApp.Data;
using CoordExtractorApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CoordExtractorApp.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(TopoDbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserByKeycloakIdAsync(string keycloakId)
        {
            var user = await context.Users
                 .FirstOrDefaultAsync(u => u.KeycloakId == keycloakId);

            if (user == null) return null;
            return user;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates)
        {
            IQueryable<User> query = context.Users;

            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
            }

            int totalRecords = await query.CountAsync();

            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(u => u.Username)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<User>(data, totalRecords, pageNumber, pageSize);
        }

        public async Task<List<int>> GetProjectIdsForUserAsync(int id)
        {
            return await context.Users
                .Where(u => u.Id == id)
                .SelectMany(u => u.Projects.Select(p => p.Id))
                .ToListAsync();
        }

        public async Task SetProjectsForUserAsync(int id, List<int> projectIds)
        {
            var user = await context.Users
                .Include(u => u.Projects)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return;
            }

            var projects = await context.Projects
                .Where(p => projectIds.Contains(p.Id))
                .ToListAsync();

            user.Projects.Clear();

            foreach (var project in projects)
            {
                user.Projects.Add(project);
            }
        }
    }
}

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
                 .FirstOrDefaultAsync(u => u.KeycloakId == keycloakId); //προσοχή το KeycloakId ερχεται ως string απο το json

            if (user == null) return null;
            return user;
            //TODO//ΝΑ ΔΟΚΙΜΑΣΩ ΚΑΙ ΜΕ LINQ ΝΑ ΠΑΡΩ ΜΟΝΟ ID ΑΝ ΑΡΓΕΙ
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await context.Users
                .IgnoreQueryFilters() ////https://learn.microsoft.com/en-us/ef/core/querying/filters?tabs=ef10 disabling filters Για check existing στο create update
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<PaginatedResult<User>> GetUsersAsync(int pageNumber, int pageSize,
            List<Expression<Func<User, bool>>> predicates)
        {
            IQueryable<User> query = context.Users; // δεν εκτελείται

            if (predicates != null && predicates.Count > 0)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate); // εκτελείται, υπονοείται το AND
                }
            }

            int totalRecords = await query.CountAsync(); // εκτελείται

            int skip = (pageNumber - 1) * pageSize;

            var data = await query
                .OrderBy(u => u.Username) // πάντα να υπάρχει ένα OrderBy πριν το Skip
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(); // εκτελείται

            return new PaginatedResult<User>(data, totalRecords, pageNumber, pageSize);
        }

        //για να παρω τη λθστα με τα user-projects
        public async Task<List<int>> GetProjectIdsForUserAsync(int id)
        {
            return await context.Users
                .Where(u => u.Id == id) //για τον user με αυτο το id
                .SelectMany(u => u.Projects.Select(p => p.Id)) //παιρνω τα projects απο το navigation property
                .ToListAsync();
        }

        //για την ενημέρωση του user-projects
        public async Task SetProjectsForUserAsync(int id, List<int> projectIds)
        {
            var user = await context.Users
                .Include(u => u.Projects) //ο user με τα curent projects του
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return;
            }

            var projects = await context.Projects
                .Where(p => projectIds.Contains(p.Id)) //παιρνω όλα τα projects για τη λιστα των project ids απο front
                .ToListAsync();

            user.Projects.Clear(); //!!delete τις παλιες σχέσεις


            foreach (var project in projects) //add τις νέες σχέσεις user-project
            {
                user.Projects.Add(project);
            }

        }
    }
}

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
                .FirstOrDefaultAsync(u=> u.KeycloakId == keycloakId); //προσοχή το KeycloakId ερχεται ως string απο το json

            if (user == null) return null;            
            return user;
            //TODO//ΝΑ ΔΟΚΙΜΑΣΩ ΚΑΙ ΜΕ LINQ ΝΑ ΠΑΡΩ ΜΟΝΟ ID ΑΝ ΑΡΓΕΙ
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Username == username);
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


    }
    }

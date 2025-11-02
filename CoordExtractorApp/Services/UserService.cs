using AutoMapper;
using CoordExtractorApp.Core.Enums;
using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.Data;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Exceptions;
using CoordExtractorApp.Models;
using CoordExtractorApp.Repositories;
using CoordExtractorApp.Security;
using Serilog;
using System.Linq.Expressions;

namespace CoordExtractorApp.Services
{
    public class UserService : IUserService
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<UserService> logger = new LoggerFactory().AddSerilog().CreateLogger<UserService>();


        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }



        public async Task<PaginatedResult<UserReadOnlyDTO>> GetPaginatedUsersFilteredAsync(int pageNumber, int pageSize,
            UserFiltersDTO userFiltersDTO)
        {
            List<User> users = [];
            //μετατροπή φίλτρων σε predicates

            List<Expression<Func<User, bool>>> predicates = [];


            if (!string.IsNullOrEmpty(userFiltersDTO.Username))
            {
                predicates.Add(u => u.Username == userFiltersDTO.Username);
            }


            if (!string.IsNullOrEmpty(userFiltersDTO.UserRole))
            {
                predicates.Add(u => u.UserRole.ToString() == userFiltersDTO.UserRole);
            }


            var result = await unitOfWork.UserRepository.GetUsersAsync(pageNumber, pageSize, predicates);


            var dtoResult = new PaginatedResult<UserReadOnlyDTO>()
            {
                Data = result.Data.Select(u => new UserReadOnlyDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Firstname = u.Firstname,
                    Lastname = u.Lastname,
                    UserRole = u.UserRole.ToString()!
                }).ToList(),
                TotalRecords = result.TotalRecords,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
            logger.LogInformation("Retrieved {Count} users", dtoResult.Data.Count);
            return dtoResult;
        }

        //Αναζήτηση χρήστη με βάση το id

        public async Task<User?> GetUserByIdAsync(int id)
        {
            User? user = null;

            try
            {
                user = await unitOfWork.UserRepository.GetAsync(id);
                logger.LogInformation("User found with ID: {Id}", id);
            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error retrieving user by ID: {Id}. {Message}", id, ex.Message);
            }
            return user;
        }


        // Αναζήτηση χρήστη με βάση το username
        public async Task<UserReadOnlyDTO?> GetUserByUsernameAsync(string username)
        {
            try
            {
                // ψάχνουμε user με αυτό το username & password.
                // Έπόμενο βήμα να γίνει έλεγχος authorization.
                User? user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
                if (user == null) //Λάθος credentials
                {
                    // Custom exception αν δεν βρεθεί
                    throw new EntityNotFoundException("User", "User with username: " + " not found");
                }

                logger.LogInformation("User found: {Username}", username);
                // Επιστροφή DTO (όχι entity με password κλπ)
                return new UserReadOnlyDTO
                {
                    Id = user.Id,
                    Username = user.Username!,
                    Email = user.Email!,
                    Firstname = user.Firstname!,
                    Lastname = user.Lastname!,
                    UserRole = user.UserRole.ToString()!
                };
            }
            catch (EntityNotFoundException ex)
            {
                // Log και re-throw για χειρισμό από controller
                logger.LogError("Error retrieving user by username: {Username}. {Message}", username, ex.Message);
                throw;
            }
        }

        // Έλεγχος credentials και επιστροφή user για login
        public async Task<User?> VerifyAndGetUserAsync(UserLoginDTO credentials)
        {
            User? user = null;
            try
            {
                // Ψάχνουμε user με αυτό το username & password
                user = await unitOfWork.UserRepository.GetUserAsync(credentials.Username!, credentials.Password!);

                if (user == null)
                {
                    // Λάθος credentials = custom exception
                    throw new EntityNotAuthorizedException("User", "Bad Credentials");

                    //see Resources/ Strings.resx for localization
                    //throw new EntityNotAuthorizedException("User", Resources.ErrorMessages.BadCredentials);
                }
                logger.LogInformation("User with username {Username} found", credentials.Username!);
            }
            catch (EntityNotAuthorizedException e)
            {
                // Log security event
                logger.LogError("Authentication failed for username {Username}. {Message}",
                    credentials.Username, e.Message);
            }
            return user;
        }


        public async Task CreateUserAsync(UserCreateDTO request)
        {
            User user = ExtractUser(request);
            try
            {

                User? existingUser = await unitOfWork.UserRepository.GetUserByUsernameAsync(user.Username);

                if (existingUser != null)
                {
                    throw new EntityAlreadyExistsException("User", "User with username " +
                            existingUser.Username + " already exists");
                }

                user.Password = EncryptionUtil.Encrypt(user.Password);

                await unitOfWork.UserRepository.AddAsync(user);


                await unitOfWork.SaveAsync();

                logger.LogInformation("User {Username} signed up successfully.", user);
            }
            catch (EntityAlreadyExistsException ex)
            {
                logger.LogError("Error creating user {Username}. {Message}", user, ex.Message);
                throw;
            }
        }
               
        //helper method (απο πρώτο project)
        private User ExtractUser(UserCreateDTO createDTO)
        {
            return new User()
            {
                Username = createDTO.Username!,
                Password = createDTO.Password!,
                Email = createDTO.Email!,
                Firstname = createDTO.Firstname!,
                Lastname = createDTO.Lastname!,
                UserRole = createDTO.UserRole
            };
        }

        public async Task<bool> UpdateUserAsync(int id, UserUpdateDTO userupdatedto)
        {
            try
            {
                var user = await unitOfWork.UserRepository.GetAsync(id);

                if (user == null)
                {
                    throw new EntityNotFoundException("User", $"User with id: {id} not found");
                }

                // Partial update - only update fields that are provided
                if (userupdatedto.Username != null) user.Username = userupdatedto.Username;
                if (userupdatedto.Email != null) user.Email = userupdatedto.Email;
                if (userupdatedto.Firstname != null) user.Firstname = userupdatedto.Firstname;
                if (userupdatedto.Lastname != null) user.Lastname = userupdatedto.Lastname;
                if (userupdatedto.UserRole != null) user.UserRole = userupdatedto.UserRole.Value;

                // Password update (with encryption)
                if (userupdatedto.Password != null)
                {
                    user.Password = EncryptionUtil.Encrypt(userupdatedto.Password);
                }

                // ModifiedAt set by Repository.UpdateAsync
                await unitOfWork.UserRepository.UpdateAsync(user);
                await unitOfWork.SaveAsync();

                logger.LogInformation("User {Id} updated successfully.", id);
                return true;
            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error updating user {Id}. {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var user = await unitOfWork.UserRepository.GetAsync(id);

                if (user == null)
                {
                    throw new EntityNotFoundException("User", $"User with id: {id} not found");
                }

                user.DeletedAt = DateTime.UtcNow;
                user.ModifiedAt = DateTime.UtcNow;


                await unitOfWork.SaveAsync();
                logger.LogInformation("User {id} deleted successfully.", user);
                return true;
            }
            catch (EntityNotFoundException ex)
            {
                logger.LogError("Error deleting user {id}. {Message}", id, ex.Message);
                throw;
            }


        }
    }
}
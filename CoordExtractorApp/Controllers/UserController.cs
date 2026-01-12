using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Models;
using CoordExtractorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoordExtractorApp.Controllers
{
    /// <summary>
    /// Manages users and their project assignments
    /// </summary>
    [ApiController]
    [Route("api/users")] // Base route: /api/users
    public class UserController : BaseController
    {


        //constructor
        public UserController(IApplicationService applicationService):
            base(applicationService)
        {

        }

        /// <summary>
        /// Creates a new user in both Keycloak and local database
        /// </summary>
        /// <param name="userCreateDto">User creation data</param>
        /// <returns>The created user</returns>
        //CREATE USER
        //POST /api/users
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(UserReadOnlyDTO), 201)] //Success
        [ProducesResponseType(400)] //Bad Request
        [ProducesResponseType(409)] //conflict όταν user exists
        [ProducesResponseType(500)] //Keycloak error
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO userCreateDto)
        {
            //business logic πήγε όλη στο service. και το mapping επισης.

            var dto = await this.applicationService.UserService.CreateUserWithKeycloakAsync(userCreateDto);

            // 201 Created. το Id απο το newUser
            return CreatedAtAction(nameof(GetUserById), new { id = dto.Id }, dto);
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="userUpdateDto">Updated user data</param>
        /// <returns>No content</returns>
        //UPDATE USERT
        //PUT /api/users/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")] // Μόνο Admin ή Manager
        [ProducesResponseType(204)] // Success
        [ProducesResponseType(404)] // User not found
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDTO userUpdateDto)
        {
            //το error θα το πιασει το service και θα το χειριστει το middlwere
            //Keycloak + Local DB μικτό από UpdateUserAsync            
            await applicationService.UserService.UpdateUserAsync(id, userUpdateDto);
            
            return NoContent(); // 204 No Content

        }

        /// <summary>
        /// Deletes a user from both Keycloak and local database
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>No content</returns>
        //DELETE USER
        // DELETE /api/users/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")] //Auth
        [ProducesResponseType(204)] //Success
        [ProducesResponseType(404)] //User not found
        public async Task<IActionResult> DeleteUser(int id)
        {

            // Keycloak-first delete: αν αποτύχει Keycloak → δεν διαγράφει local
            await applicationService.UserService.DeleteUserAsync(id);
            
            return NoContent(); // 204 No Content

        }

        /// <summary>
        /// Get a user by id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User details</returns>
        //GET USER BY ID
        // GET /api/users/{id}
        [HttpGet("{id}", Name = "GetUserById")]
        [Authorize] // Οποιοσδήποτε authenticated user
        [ProducesResponseType(typeof(UserReadOnlyDTO), 200)] //Success 200 OK
        [ProducesResponseType(404)] //User not found
        public async Task<IActionResult> GetUserById(int id)
        {
            //Από Database
            var dto = await this.applicationService.UserService.GetUserByIdAsync(id);
            return Ok(dto); //Success 200 OK
        }

        /// <summary>
        /// Get all users (Admin/Manager only)
        /// </summary>
        /// <returns>List of all users</returns>
        //GET ALL USERS
        // GET /api/users
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(IEnumerable<UserReadOnlyDTO>), 200)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await applicationService.UserService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Get paginated users with optional filtering
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Items per page</param>
        /// <returns>Paginated user list</returns>
        //GET ALL USERS paginated
        // GET /api/users?pageNumber=1&pageSize=10
        [HttpGet("paginated")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(PaginatedResult<UserReadOnlyDTO>), 200)]
        public async Task<IActionResult> GetUsersPaginated(

            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)

        {
            int page = pageNumber ?? 1;
            int size = pageSize ?? 10;

            //filters DTO κενό
            var filters = new UserFiltersDTO();

            //service με page 1 , pageSize 10. φίλτρο ίσως αργότερα.
            var result = await applicationService.UserService.GetPaginatedUsersFilteredAsync(page, size, filters);

            return Ok(result);

        }

        /// <summary>
        /// Get projects assigned to a user
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>List of project IDs</returns>
        //GET PROJECTS BY USER ID
        // GET /api/users/{id}/projects
        [HttpGet("{id}/projects")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(UserProjectsDTO), 200)]
        [ProducesResponseType(404)]//User not found
        public async Task<IActionResult> GetUserProjects(int id)
        {
            var dto = await applicationService.UserProjectsService.GetUserProjectsAsync(id);
            return Ok(dto);
        }

        /// <summary>
        /// Updates project assignments for a user
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="dto">Updated project assignments</param>
        /// <returns>Updated user projects</returns>
        //UPDATE USERPROJECTS
        //PUT /api/users/{id}/projects
        [HttpPut("{id}/projects")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(UserProjectsDTO), 200)]//με dto
        [ProducesResponseType(404)]//User not found
        public async Task<IActionResult> UpdateUserProjects(int id, [FromBody]UserProjectsUpdateDTO dto)
        {

            var updatedDto= await applicationService.UserProjectsService.UpdateUserProjectsAsync(id,dto);
            return Ok(updatedDto);
        }
    }
}

using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Models;
using CoordExtractorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoordExtractorApp.Controllers
{
    /// <summary>
    /// Manages users and their project assignments.
    /// </summary>
    [ApiController]
    [Route("api/users")]
    public class UserController : BaseController
    {
        public UserController(IApplicationService applicationService) : base(applicationService)
        {
        }

        /// <summary>
        /// Creates a new user in Keycloak and saves to local database.
        /// </summary>
        /// <param name="userCreateDto">User creation data.</param>
        /// <returns>The created user.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(UserReadOnlyDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO userCreateDto)
        {
            var dto = await this.applicationService.UserService.CreateUserWithKeycloakAsync(userCreateDto);
            return CreatedAtAction(nameof(GetUserById), new { id = dto.Id }, dto);
        }

        /// <summary>
        /// Updates an existing user in keycloak and local database.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <param name="userUpdateDto">Updated user data.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDTO userUpdateDto)
        {
            await applicationService.UserService.UpdateUserAsync(id, userUpdateDto);
            return NoContent();
        }

        /// <summary>
        /// Deletes a user from both Keycloak and local database.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await applicationService.UserService.DeleteUserAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Gets a user by id.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <returns>User details.</returns>
        [HttpGet("{id}", Name = "GetUserById")]
        [Authorize]
        [ProducesResponseType(typeof(UserReadOnlyDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var dto = await this.applicationService.UserService.GetUserByIdAsync(id);
            return Ok(dto);
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>List of all users.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(IEnumerable<UserReadOnlyDTO>), 200)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await applicationService.UserService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Gets paginated users with optional filtering.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Items per page.</param>
        /// <returns>Paginated user list.</returns>
        [HttpGet("paginated")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(PaginatedResult<UserReadOnlyDTO>), 200)]
        public async Task<IActionResult> GetUsersPaginated(
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)
        {
            int page = pageNumber ?? 1;
            int size = pageSize ?? 10;
            var filters = new UserFiltersDTO();
            var result = await applicationService.UserService.GetPaginatedUsersFilteredAsync(page, size, filters);
            return Ok(result);
        }

        /// <summary>
        /// Gets projects assigned to a user.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <returns>List of project IDs.</returns>
        [HttpGet("{id}/projects")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(UserProjectsDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserProjects(int id)
        {
            var dto = await applicationService.UserProjectsService.GetUserProjectsAsync(id);
            return Ok(dto);
        }

        /// <summary>
        /// Updates project assignments for a user.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <param name="dto">Updated project assignments.</param>
        /// <returns>Updated user projects.</returns>
        [HttpPut("{id}/projects")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(UserProjectsDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUserProjects(int id, [FromBody] UserProjectsUpdateDTO dto)
        {
            var updatedDto = await applicationService.UserProjectsService.UpdateUserProjectsAsync(id, dto);
            return Ok(updatedDto);
        }
    }
}

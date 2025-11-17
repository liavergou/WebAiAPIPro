using AutoMapper;
using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.Data;
using CoordExtractorApp.DTO;
using CoordExtractorApp.DTO.Keycloak;
using CoordExtractorApp.Exceptions;
using CoordExtractorApp.Models;
using CoordExtractorApp.Services;
using CoordExtractorApp.Services.Keycloak;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoordExtractorApp.Controllers
{
    [ApiController]
    [Route("api/users")] // Base route: /api/users
    public class UserController : BaseController
    {
        private readonly IMapper mapper;

        //constructor
        public UserController(IApplicationService applicationService,IMapper mapper)
            : base(applicationService)
        {

            this.mapper = mapper;
        }

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
            //business logic πήγε όλη στο service

            User newUser = await this.applicationService.UserService.CreateUserWithKeycloakAsync(userCreateDto);

            //δημιουργία του dto για το response
            var readOnlyDto = this.mapper.Map<UserReadOnlyDTO>(newUser);

            // 201 Created. το Id απο το newUser
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, readOnlyDto);
        }

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
            bool success = await applicationService.UserService.UpdateUserAsync(id, userUpdateDto);
            if (!success)
            {
                return StatusCode(500, "Failed to update user in Keycloak.");

            }

                
                return NoContent(); // 204 No Content

        }

        //DELETE USER
        // DELETE /api/users/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")] //Auth
        [ProducesResponseType(204)] //Success
        [ProducesResponseType(404)] //User not found
        public async Task<IActionResult> DeleteUser(int id)
        {
         
                // Keycloak-first delete: αν αποτύχει Keycloak → δεν διαγράφει local
                bool success = await applicationService.UserService.DeleteUserAsync(id);
                if (!success)
                {
                    // αν αποτύχει το Keycloak delete
                    return StatusCode(500, "Failed to delete user from identity provider.");
                }
                return NoContent(); // 204 No Content
           
        }

        //GET USER BY ID
        // GET /api/users/{id}
        [HttpGet("{id}", Name = "GetUserById")]
        [Authorize] // Οποιοσδήποτε authenticated user
        [ProducesResponseType(typeof(UserReadOnlyDTO), 200)] //Success 200 OK
        [ProducesResponseType(404)] //User not found
        public async Task<IActionResult> GetUserById(int id)
        {
            //Από Database
            var user = await this.applicationService.UserService.GetUserByIdAsync(id);
            // entity σε DTO
            var dto = this.mapper.Map<UserReadOnlyDTO>(user);
            return Ok(dto); //Success 200 OK
        }

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
            int size = pageSize ?? 1;

            //filters DTO κενό
            var filters = new UserFiltersDTO();

            //service με page 1 , pageSize 10 
            var result = await applicationService.UserService.GetPaginatedUsersFilteredAsync(page, size, filters);

            return Ok(result);
            
        }
    }
}

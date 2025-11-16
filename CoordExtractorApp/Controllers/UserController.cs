using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using CoordExtractorApp.DTO;
using CoordExtractorApp.DTO.Keycloak;
using CoordExtractorApp.Services;
using CoordExtractorApp.Data;
using CoordExtractorApp.Exceptions;
using CoordExtractorApp.Services.Keycloak;

namespace CoordExtractorApp.Controllers
{
    [ApiController]
    [Route("api/users")] // Base route: /api/users
    public class UserController : BaseController
    {
        private readonly IKeycloakAdminService keycloakAdminService;
        private readonly IMapper mapper;

        //constructor
        public UserController(IApplicationService applicationService,IKeycloakAdminService keycloakAdminService,IMapper mapper)
            : base(applicationService)
        {
            this.keycloakAdminService = keycloakAdminService;
            this.mapper = mapper;
        }

        //CREATE USER
        //POST /api/users
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(UserReadOnlyDTO), 201)] //Success
        [ProducesResponseType(400)] //Bad Request
        [ProducesResponseType(500)] //Keycloak error
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO userCreateDto)
        {
            // DTO για το Keycloak API
            var newkeycloakUser = new KeycloakUserDTO
            {
                Username = userCreateDto.Username,
                Email = userCreateDto.Email,
                FirstName = userCreateDto.Firstname,
                LastName = userCreateDto.Lastname,
                Credentials = new List<KeycloakCredentials>
                {
                    new KeycloakCredentials { Value = userCreateDto.Password! }
                }
            };

            //Create user στο Keycloak
            string? keycloakId = await this.keycloakAdminService.CreateUserAsync(newkeycloakUser);

            if (string.IsNullOrEmpty(keycloakId))
            {
                //σε περίπτωση σφάλματος->error να μη κανει user στη βαση
                return StatusCode(500, "Error creating user in Keycloak");
            }

            //ανάθεση ρόλου
            if (string.IsNullOrEmpty(userCreateDto.Role))
            {
                return BadRequest("Role is required");
            }

            bool roleAssigned = await this.keycloakAdminService.AssignUserRoleToUserAsync(keycloakId, userCreateDto.Role);

            if (!roleAssigned)
            {
                await this.keycloakAdminService.DeleteUserAsync(keycloakId); //αν απετυχε το role να διαγαφεί ο user από το keycloak
                return StatusCode(500, "Failed to assign role to user in Keycloak. User deleted");
            }

            //ενημέρωση βάσης
            var userToSave = this.mapper.Map<User>(userCreateDto);
            userToSave.KeycloakId = keycloakId; // Σύνδεση με Keycloak user
            User newUser = await this.applicationService.UserService.CreateUserAsync(userToSave);

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
            try
            {
                //Keycloak + Local DB μικτό από UpdateUserAsync
                bool success = await applicationService.UserService.UpdateUserAsync(id, userUpdateDto);
                return NoContent(); // 204 No Content
            }
            catch (EntityNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        //DELETE USER
        // DELETE /api/users/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")] //Auth
        [ProducesResponseType(204)] //Success
        [ProducesResponseType(404)] //User not found
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
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
            catch (EntityNotFoundException e)
            {
                return NotFound(e.Message);
            }
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
    }
}

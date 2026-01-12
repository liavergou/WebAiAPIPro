using CoordExtractorApp.DTO;
using CoordExtractorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoordExtractorApp.Exceptions;

namespace CoordExtractorApp.Controllers
{
    /// <summary>
    /// Manages assigned projects for the current user
    /// </summary>
    [ApiController]
    [Route("api/account")]
    public class UserProjectsController : BaseController
    {
        public UserProjectsController(IApplicationService applicationService) : base(applicationService)
        {

        }

        /// <summary>
        /// Get projects assigned to the current logged-in user
        /// </summary>
        /// <returns>List of assigned projects</returns>
        //GET USER PROJECTS FOR LOGGED IN USER
        //GET /api/account/projects
        [HttpGet("projects")]
        [Authorize]
        [ProducesResponseType(typeof(List<ProjectReadOnlyDTO>), 200)]
        //[ProducesResponseType(401)] //unautorized

        public async Task<IActionResult> GetAssignedProjects()
        {
            //Ο user που συνδέεται με το conversion job
            var user = await GetUserInfoAsync(); //(base) για τον current user

            if (user.Id == null) throw new EntityNotAuthorizedException("User", "User id not found");

            var projects = await applicationService.UserProjectsService.GetUserProjectsByUserIdAsync(user.Id.Value);
            
            return Ok(projects);
        }


    }
}

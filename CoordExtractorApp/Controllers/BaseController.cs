using CoordExtractorApp.Models;
using CoordExtractorApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoordExtractorApp.Controllers
{
    /// <summary>
    /// Base controller providing functionality for all API controllers.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public readonly IApplicationService applicationService;

        /// <summary>
        /// Initializes a new instance of the BaseController.
        /// </summary>
        /// <param name="applicationService">The application service.</param>
        public BaseController(IApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        /// <summary>
        /// Retrieves the current authenticated user by combining JWT claims with database info.
        /// </summary>
        /// <returns>ApplicationUser containing database ID, claims and role.</returns>
        protected async Task<ApplicationUser> GetUserInfoAsync()
        {
            return await this.applicationService.UserService.GetUserInfoAsync(this.User);
        }
    }
}

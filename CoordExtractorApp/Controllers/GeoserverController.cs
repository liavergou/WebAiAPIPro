using CoordExtractorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoordExtractorApp.Controllers
{
    public class GeoserverController : BaseController
    {

        public GeoserverController(IApplicationService applicationService) : base(applicationService) 
        { 

        }

        [HttpGet("projects/{id}/jobs")]
        [Authorize(Roles = "Admin, Manager")]

        public async Task<IActionResult>GetProjectGeoserverJobs(int id)
        {
            var user = await GetUserInfoAsync();

            string geoJson = await applicationService.GeoserverService.GetProjectJobsGeoserverAsync(id, user.Username, user.Role);

            return Content(geoJson, "application/json");
        }


    }
}

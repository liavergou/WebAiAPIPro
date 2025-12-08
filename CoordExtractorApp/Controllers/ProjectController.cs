using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Models;
using CoordExtractorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CoordExtractorApp.Controllers
{
    
    [ApiController]    
    [Route("api/projects")] // Base route: /api/projects
  
    public class ProjectController : BaseController
    {
        public ProjectController(IApplicationService applicationService) : base(applicationService)
        {

        }

        //CREATE PROJECT
        //POST /api/projects
        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(ProjectDTO), 201)] //success
        [ProducesResponseType(400)]//Bad Request
        [ProducesResponseType(409)] //allready exists

        public async Task<IActionResult> CreateProject([FromBody] ProjectCreateDTO projectCreateDTO)
        {
            ProjectDTO projectDTO = await applicationService.ProjectService.CreateProjectAsync(projectCreateDTO);
            return CreatedAtAction(nameof(GetProjectById), new { id = projectDTO.Id }, projectDTO);
        }


        //GET PROJECT BY ID
        //GET /api/projects/{id}
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ProjectDTO), 200)] //Success 200 OK
        [ProducesResponseType(404)] //Project not found
        public async Task<ActionResult> GetProjectById(int id)
        {
            ProjectDTO? projectDTO = await applicationService.ProjectService.GetProjectByIdAsync(id);
            return Ok(projectDTO);
        }

        //GET ALL PROJECTS (για τα project cards, μόνο admin manager. οι member θα το παρουν απο το userProject controller)
        // GET /api/projects/all
        [HttpGet("all")]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(ProjectReadOnlyDTO), 200)] //Success 200 OK
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await applicationService.ProjectService.GetAllProjectsAsync();
            return Ok(projects);
        }


        //GET ALL PROJECTS paginated για το management
        // GET /api/projects?pageNumber=1&pageSize=10
        [HttpGet]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(PaginatedResult<ProjectDTO>), 200)] //Success 200 OK
        [ProducesResponseType(404)] //Project not found
        public async Task<IActionResult> GetProjectPaginated(

            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromQuery] string? projectName)
        

        {
            var predicates = new ProjectFilterDTO { ProjectName = projectName };
            int page = pageNumber ?? 1;
            int size = pageSize ?? 10;


            //service με page 1 , pageSize 10. φίλτρο ίσως αργότερα.
            var result = await applicationService.ProjectService.GetPaginatedProjectsAsync(page, size, predicates);

            return Ok(result);

        }

        //UPDATE PROJECT
        //PUT /api/projects/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")] // Μόνο Admin ή Manager
        [ProducesResponseType(204)] // Success
        [ProducesResponseType(404)] // Project not found
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectUpdateDTO projectUpdateDto)
        {

            await applicationService.ProjectService.UpdateProjectAsync(id, projectUpdateDto);

            return NoContent(); // 204 No Content

        }

        //DELETE PROJECT
        //DELETE /api/projects/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")] // Μόνο Admin ή Manager
        [ProducesResponseType(204)] // Success
        [ProducesResponseType(404)] // Project not found

        public async Task<IActionResult> DeleteProject(int id)
        {
           await applicationService.ProjectService.DeleteProjectAsync(id);
            
            return NoContent();
        }
        

        //GET GEOSERVER JOBS GEOJSON BY PROJECT ID
        //GET /api/projects/{id}/jobs
        [HttpGet("{id}/jobs")]
        [Authorize(Roles = "Admin, Manager")]

        public async Task<IActionResult> GetProjectGeoserverJobs(int id)
        {
            var user = await GetUserInfoAsync();

            string geoJson = await applicationService.GeoserverService.GetProjectJobsGeoserverAsync(id, user.Username, user.Role);

            return Content(geoJson, "application/json");
        }


    }


}

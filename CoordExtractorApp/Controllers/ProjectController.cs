using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Exceptions;
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
        //GET /api/projects/{id}/conversion-jobs
        [HttpGet("{id}/conversion-jobs")]
        [Authorize]

        public async Task<IActionResult> GetProjectGeoserverJobs(int id)
        {
            var user = await GetUserInfoAsync();

            string geoJson = await applicationService.GeoserverService.GetProjectJobsGeoserverAsync(id, user.Username, user.Role);

            return Content(geoJson, "application/json");
        }

        //GET GEOSERVER JOBS SHP BY PROJECT ID
        //GET /api/projects/{id}/jobs/shp
        [HttpGet("{id}/conversion-jobs/shp")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> ExportSHPProjectGeoserverJobs(int id)
        {
            var user = await GetUserInfoAsync();

            byte[] shp = await applicationService.GeoserverService.ExportProjectJobsGeoserverSHPAsync(id, user.Username, user.Role);

            return File(shp, "application/zip", $"project_{id}.zip");
        }

        //CREATE CONVERSION JOB
        //POST /api/projects/{projectId}/conversion-jobs/new
        [HttpPost("{projectId}/conversion-jobs/new")]
        [Authorize]
        [ProducesResponseType(typeof(ConversionJobReadOnlyDTO), 200)] //success
        [ProducesResponseType(typeof(ConversionJobReadOnlyDTO), 422)] //The server understands the content type and syntax of the request entity, but it is still unable to process the request for some reason.
        [ProducesResponseType(400)] //Bad Request
        public async Task<IActionResult> CreateConversionJob(int projectId, [FromForm] ConversionJobInsertDTO dto)
        {
            //Ο user που συνδέεται με το conversion job
            var user = await GetUserInfoAsync(); //(base) για τον current user

            if (user.Id == null) throw new EntityNotAuthorizedException("User", "User id not found");

            // Service
            var resultDto = await this.applicationService.ConversionJobService
                .CreateAndProcessJobAsync(dto, user.Id.Value);

            //200
            return Ok(resultDto);
        }

        //UPDATE CONVERSION JOB
        //PUT /api/projects/{projectId}/conversion-jobs/{jobId}
        [HttpPut("{projectId}/conversion-jobs/{jobId}")]
        [Authorize]
        [ProducesResponseType(typeof(ConversionJobReadOnlyDTO), 200)]
        [ProducesResponseType(400)]//Bad Request (μη valid πολυγωνο)
        [ProducesResponseType(404)] //Not found

        public async Task<IActionResult> UpdateConversionJob(int projectId, int jobId, [FromBody] ConversionJobUpdateDTO dto)
        {
            var user = await GetUserInfoAsync();
            if (user.Id == null) throw new EntityNotAuthorizedException("User", "User id not found");
            var result = await applicationService.ConversionJobService.UpdateConversionJobAsync(jobId, dto, user.Id.Value);
            return Ok(result);
        }

        //DELETE CONVERSION JOB
        //DELETE /api/projects/{projectId}/conversion-jobs/{jobId}
        [HttpDelete("{projectId}/conversion-jobs/{jobId}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]//not found
        public async Task<IActionResult> DeleteConversionJob(int projectId, int jobId)
        {
            var user = await GetUserInfoAsync();
            if (user.Id == null) throw new EntityNotAuthorizedException("User", "User id not found");
            var result = await applicationService.ConversionJobService.DeleteConversionJobAsync(jobId, user.Id.Value);
            return NoContent();

        }

    }


}

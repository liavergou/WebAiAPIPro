using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Exceptions;
using CoordExtractorApp.Models;
using CoordExtractorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using CoordExtractorApp.Core.Constants;

namespace CoordExtractorApp.Controllers
{
    /// <summary>
    /// Manages projects and their associated conversion jobs.
    /// </summary>
    [ApiController]
    [Route("api/projects")]
    public class ProjectController : BaseController
    {
        public ProjectController(IApplicationService applicationService) : base(applicationService)
        {
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="projectCreateDTO">Project creation data.</param>
        /// <returns>The created project.</returns>
        [HttpPost]
        [Authorize(Roles = AuthConstants.AdminOrManager)]
        [ProducesResponseType(typeof(ProjectDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> CreateProject([FromBody] ProjectCreateDTO projectCreateDTO)
        {
            ProjectDTO projectDTO = await applicationService.ProjectService.CreateProjectAsync(projectCreateDTO);
            return CreatedAtAction(nameof(GetProjectById), new { id = projectDTO.Id }, projectDTO);
        }

        /// <summary>
        /// Gets a project by id.
        /// </summary>
        /// <param name="id">Project id.</param>
        /// <returns>Project details.</returns>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ProjectDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetProjectById(int id)
        {
            ProjectDTO? projectDTO = await applicationService.ProjectService.GetProjectByIdAsync(id);
            return Ok(projectDTO);
        }

        /// <summary>
        /// Gets all projects.
        /// </summary>
        /// <returns>List of all projects.</returns>
        [HttpGet("all")]
        [Authorize(Roles = AuthConstants.AdminOrManager)]
        [ProducesResponseType(typeof(ProjectReadOnlyDTO), 200)]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await applicationService.ProjectService.GetAllProjectsAsync();
            return Ok(projects);
        }

        /// <summary>
        /// Gets paginated projects with optional filtering.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Items per page.</param>
        /// <param name="projectName">Project name filter (optional).</param>
        /// <returns>Paginated project list.</returns>
        [HttpGet]
        [Authorize(Roles = AuthConstants.AdminOrManager)]
        [ProducesResponseType(typeof(PaginatedResult<ProjectDTO>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetProjectPaginated(
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromQuery] string? projectName)
        {
            var predicates = new ProjectFilterDTO { ProjectName = projectName };
            int page = pageNumber ?? 1;
            int size = pageSize ?? 10;

            var result = await applicationService.ProjectService.GetPaginatedProjectsAsync(page, size, predicates);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing project.
        /// </summary>
        /// <param name="id">Project id.</param>
        /// <param name="projectUpdateDto">Updated project data.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = AuthConstants.AdminOrManager)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectUpdateDTO projectUpdateDto)
        {
            await applicationService.ProjectService.UpdateProjectAsync(id, projectUpdateDto);
            return NoContent();
        }

        /// <summary>
        /// Deletes a project.
        /// </summary>
        /// <param name="id">Project id.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = AuthConstants.AdminOrManager)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteProject(int id)
        {
            await applicationService.ProjectService.DeleteProjectAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Gets conversion jobs for a project as GeoJSON.
        /// </summary>
        /// <param name="id">Project id.</param>
        /// <returns>GeoJSON feature collection.</returns>
        [HttpGet("{id}/conversion-jobs")]
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(JObject), 200)]
        public async Task<IActionResult> GetProjectGeoserverJobs(int id)
        {
            var user = await GetUserInfoAsync();
            string geoJson = await applicationService.GeoserverService.GetProjectJobsGeoserverAsync(id, user.Username, user.Role);
            return Content(geoJson, "application/json");
        }

        /// <summary>
        /// Exports conversion jobs for a project as Shapefile.
        /// </summary>
        /// <param name="id">Project id.</param>
        /// <returns>Shapefile in ZIP format.</returns>
        [HttpGet("{id}/conversion-jobs/shp")]
        [Authorize(Roles = AuthConstants.AdminOrManager)]
        [Produces("application/zip")]
        [ProducesResponseType(typeof(FileResult), 200)]
        public async Task<IActionResult> ExportSHPProjectGeoserverJobs(int id)
        {
            var user = await GetUserInfoAsync();
            byte[] shp = await applicationService.GeoserverService.ExportProjectJobsGeoserverSHPAsync(id, user.Username, user.Role);
            return File(shp, "application/zip", $"project_{id}.zip");
        }

        /// <summary>
        /// Creates a new conversion job for a project.
        /// </summary>
        /// <param name="projectId">Project id.</param>
        /// <param name="dto">Conversion job data (image file and settings).</param>
        /// <returns>The created conversion job with processing results.</returns>
        [HttpPost("{projectId}/conversion-jobs/new")]
        [Authorize]
        [ProducesResponseType(typeof(ConversionJobReadOnlyDTO), 200)]
        [ProducesResponseType(typeof(ConversionJobReadOnlyDTO), 422)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateConversionJob(int projectId, [FromForm] ConversionJobInsertDTO dto)
        {
            var user = await GetUserInfoAsync();
            if (user.Id == null) throw new EntityNotAuthorizedException("User", "User id not found");

            var resultDto = await this.applicationService.ConversionJobService
                .CreateAndProcessJobAsync(dto, user.Id.Value);

            return Ok(resultDto);
        }

        /// <summary>
        /// Gets a conversion job by id.
        /// </summary>
        /// <param name="projectId">Project id.</param>
        /// <param name="jobId">Conversion job id.</param>
        /// <returns>Conversion job details.</returns>
        [HttpGet("{projectId}/conversion-jobs/{jobId}")]
        [Authorize]
        [ProducesResponseType(typeof(ConversionJobReadOnlyDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetConversionJobById(int projectId, int jobId)
        {
            var user = await GetUserInfoAsync();
            if (user.Id == null) throw new EntityNotAuthorizedException("User", "User id not found");
            var result = await applicationService.ConversionJobService.GetConversionJobByIdAsync(jobId, user.Id.Value);
            return Ok(result);
        }

        /// <summary>
        /// Updates a conversion job.
        /// </summary>
        /// <param name="projectId">Project id.</param>
        /// <param name="jobId">Conversion job id.</param>
        /// <param name="dto">Updated conversion job data.</param>
        /// <returns>The updated conversion job.</returns>
        [HttpPut("{projectId}/conversion-jobs/{jobId}")]
        [Authorize]
        [ProducesResponseType(typeof(ConversionJobReadOnlyDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateConversionJob(int projectId, int jobId, [FromBody] ConversionJobUpdateDTO dto)
        {
            var user = await GetUserInfoAsync();
            if (user.Id == null) throw new EntityNotAuthorizedException("User", "User id not found");
            var result = await applicationService.ConversionJobService.UpdateConversionJobAsync(jobId, dto, user.Id.Value);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a conversion job.
        /// </summary>
        /// <param name="projectId">Project id.</param>
        /// <param name="jobId">Conversion job id.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{projectId}/conversion-jobs/{jobId}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteConversionJob(int projectId, int jobId)
        {
            var user = await GetUserInfoAsync();
            if (user.Id == null) throw new EntityNotAuthorizedException("User", "User id not found");
            await applicationService.ConversionJobService.DeleteConversionJobAsync(jobId, user.Id.Value);
            return NoContent();
        }
    }
}
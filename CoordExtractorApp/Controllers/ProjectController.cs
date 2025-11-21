using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.DTO;
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
        [ProducesResponseType(typeof(ProjectReadOnlyDTO), 201)] //success
        [ProducesResponseType(400)]//Bad Request
        [ProducesResponseType(409)] //allready exists

        public async Task<IActionResult> CreateProject([FromBody] ProjectCreateDTO projectCreateDTO)
        {
            ProjectReadOnlyDTO projectReadOnlyDTO = await applicationService.ProjectService.CreateProjectAsync(projectCreateDTO);
            return CreatedAtAction(nameof(GetProjectById), new { id = projectReadOnlyDTO.Id }, projectReadOnlyDTO);
        }


        //GET PROJECT BY ID
        //GET /api/projects/{id}
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ProjectReadOnlyDTO), 200)] //Success 200 OK
        [ProducesResponseType(404)] //Project not found
        public async Task<ActionResult> GetProjectById(int id)
        {
            ProjectReadOnlyDTO? projectReadOnlyDTO = await applicationService.ProjectService.GetProjectByIdAsync(id);
            return Ok(projectReadOnlyDTO);
        }

        //GET ALL PROJECTS paginated
        // GET /api/projects?pageNumber=1&pageSize=10
        [HttpGet("paginated")]
        [Authorize]
        [ProducesResponseType(typeof(ProjectReadOnlyDTO), 200)] //Success 200 OK
        [ProducesResponseType(404)] //Project not found
        public async Task<ActionResult> GetProjectPaginated(

            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)

        {
            int page = pageNumber ?? 1;
            int size = pageSize ?? 10;


            //service με page 1 , pageSize 10. φίλτρο ίσως αργότερα.
            var result = await applicationService.ProjectService.GetPaginatedProjectsAsync(page, size);

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

            bool success = await applicationService.ProjectService.UpdateProjectAsync(id, projectUpdateDto);
            if (!success)
            {
                return StatusCode(500, "Failed to update project");

            }

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


    }


}

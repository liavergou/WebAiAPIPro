using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoordExtractorApp.Controllers
{
    /// <summary>
    /// Manages prompts for coordinate extraction
    /// </summary>
    [ApiController]
    [Route("api/prompts")] // Base route: /api/prompts
    public class PromptController :BaseController
    {
        public PromptController(IApplicationService applicationService) :
            base (applicationService)
        {

        }

        /// <summary>
        /// Creates a new prompt
        /// </summary>
        /// <param name="promptCreateDTO">Prompt creation data</param>
        /// <returns>The created prompt</returns>
        //CREATE Prompt
        //POST /api/prompts
        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        [ProducesResponseType(typeof(PromptReadOnlyDTO), 201)] //success
        [ProducesResponseType(400)]//Bad Request
        [ProducesResponseType(409)] //allready exists

        public async Task<IActionResult> CreatePrompt([FromBody] PromptCreateDTO promptCreateDTO)
        {
            PromptReadOnlyDTO promptReadOnlyDTO = await applicationService.PromptService.CreatePromptAsync(promptCreateDTO);
            return CreatedAtAction(nameof(GetPromptById), new { id = promptReadOnlyDTO.Id }, promptReadOnlyDTO);
        }

        /// <summary>
        /// Get a prompt by id
        /// </summary>
        /// <param name="id">Prompt id</param>
        /// <returns>Prompt details</returns>
        //GET PROMPT BY ID
        //GET /api/prompts/{id}
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(PromptReadOnlyDTO), 200)] //Success 200 OK
        [ProducesResponseType(404)] //Prompt not found
        public async Task<ActionResult> GetPromptById(int id)
        {
            PromptReadOnlyDTO? promptReadOnlyDTO = await applicationService.PromptService.GetPromptByIdAsync(id);
            return Ok(promptReadOnlyDTO);
        }

        /// <summary>
        /// Get all prompts
        /// </summary>
        /// <returns>List of all prompts</returns>
        //GET ALL PROMPTS (μενου για χρήστη)
        // GET /api/prompts/all
        [HttpGet("all")]
        [Authorize]
        [ProducesResponseType(typeof(PromptReadOnlyDTO), 200)] //Success 200 OK
        public async Task<IActionResult> GetAllPrompts()
        {
            var prompts = await applicationService.PromptService.GetAllPromtsAsync();
            return Ok(prompts);
        }

        /// <summary>
        /// Get paginated prompts with optional filtering
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Items per page</param>
        /// <param name="promptName">Prompt name filter (optional)</param>
        /// <returns>Paginated prompt list</returns>
        //GET ALL PROMPTS paginated
        // GET /api/prompts?pageNumber=1&pageSize=10
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")] // Μόνο Admin ή Manager
        [ProducesResponseType(typeof(PromptReadOnlyDTO), 200)] //Success 200 OK
        [ProducesResponseType(404)] //Prompt not found
        public async Task<ActionResult> GetPromptPaginated(

            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromQuery] string? promptName)

        {
            var predicates = new PromptFilterDTO { PromptName = promptName };
            int page = pageNumber ?? 1;
            int size = pageSize ?? 10;


            //service με page 1 , pageSize 10. φίλτρο ίσως αργότερα.
            var result = await applicationService.PromptService.GetPaginatedPromptsAsync(page, size, predicates);

            return Ok(result);

        }

        /// <summary>
        /// Updates an existing prompt
        /// </summary>
        /// <param name="id">Prompt id</param>
        /// <param name="promptUpdateDto">Updated prompt data</param>
        /// <returns>No content</returns>
        //UPDATE PROMPT
        //PUT /api/prompts/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")] // Μόνο Admin ή Manager
        [ProducesResponseType(204)] // Success
        [ProducesResponseType(404)] // Prompt not found
        public async Task<IActionResult> UpdatePrompt(int id, [FromBody] PromptUpdateDTO promptUpdateDto)
        {

            await applicationService.PromptService.UpdatePromptAsync(id, promptUpdateDto);

            return NoContent(); // 204 No Content

        }

        /// <summary>
        /// Deletes a prompt
        /// </summary>
        /// <param name="id">Prompt id</param>
        /// <returns>No content</returns>
        //DELETE PROMPT
        //DELETE /api/prompts/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")] // Μόνο Admin ή Manager
        [ProducesResponseType(204)] // Success
        [ProducesResponseType(404)] // Prompt not found

        public async Task<IActionResult> DeletePrompt(int id)
        {

            await applicationService.PromptService.DeletePromptAsync(id);

            return NoContent();
        }



    }

}

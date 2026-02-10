using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoordExtractorApp.Core.Constants;

namespace CoordExtractorApp.Controllers
{
    /// <summary>
    /// Manages prompts for coordinate extraction.
    /// </summary>
    [ApiController]
    [Route("api/prompts")]
    public class PromptController : BaseController
    {
        public PromptController(IApplicationService applicationService) : base(applicationService)
        {
        }

        /// <summary>
        /// Creates a new prompt.
        /// </summary>
        /// <param name="promptCreateDTO">Prompt creation data.</param>
        /// <returns>The created prompt.</returns>
        [HttpPost]
        [Authorize(Roles = AuthConstants.AdminOrManager)]
        [ProducesResponseType(typeof(PromptReadOnlyDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> CreatePrompt([FromBody] PromptCreateDTO promptCreateDTO)
        {
            PromptReadOnlyDTO promptReadOnlyDTO = await applicationService.PromptService.CreatePromptAsync(promptCreateDTO);
            return CreatedAtAction(nameof(GetPromptById), new { id = promptReadOnlyDTO.Id }, promptReadOnlyDTO);
        }

        /// <summary>
        /// Gets a prompt by id.
        /// </summary>
        /// <param name="id">Prompt id.</param>
        /// <returns>Prompt details.</returns>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(PromptReadOnlyDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetPromptById(int id)
        {
            PromptReadOnlyDTO? promptReadOnlyDTO = await applicationService.PromptService.GetPromptByIdAsync(id);
            return Ok(promptReadOnlyDTO);
        }

        /// <summary>
        /// Gets all prompts.
        /// </summary>
        /// <returns>List of all prompts.</returns>
        [HttpGet("all")]
        [Authorize]
        [ProducesResponseType(typeof(PromptReadOnlyDTO), 200)]
        public async Task<IActionResult> GetAllPrompts()
        {
            var prompts = await applicationService.PromptService.GetAllPromtsAsync();
            return Ok(prompts);
        }

        /// <summary>
        /// Gets paginated prompts with optional filtering.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Items per page.</param>
        /// <param name="promptName">Prompt name filter (optional).</param>
        /// <returns>Paginated prompt list.</returns>
        [HttpGet]
        [Authorize(Roles = AuthConstants.AdminOrManager)]
        [ProducesResponseType(typeof(PromptReadOnlyDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetPromptPaginated(
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromQuery] string? promptName)
        {
            var predicates = new PromptFilterDTO { PromptName = promptName };
            int page = pageNumber ?? 1;
            int size = pageSize ?? 10;

            var result = await applicationService.PromptService.GetPaginatedPromptsAsync(page, size, predicates);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing prompt.
        /// </summary>
        /// <param name="id">Prompt id.</param>
        /// <param name="promptUpdateDto">Updated prompt data.</param>
        /// <returns>No content.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = AuthConstants.AdminOrManager)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePrompt(int id, [FromBody] PromptUpdateDTO promptUpdateDto)
        {
            await applicationService.PromptService.UpdatePromptAsync(id, promptUpdateDto);
            return NoContent();
        }

        /// <summary>
        /// Deletes a prompt.
        /// </summary>
        /// <param name="id">Prompt id.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletePrompt(int id)
        {
            await applicationService.PromptService.DeletePromptAsync(id);
            return NoContent();
        }
    }
}

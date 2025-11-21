using CoordExtractorApp.Core.Filters;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoordExtractorApp.Controllers
{
    [ApiController]
    [Route("api/prompts")] // Base route: /api/prompts
    public class PromptController :BaseController
    {
        public PromptController(IApplicationService applicationService) :
            base (applicationService)
        {

        }

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

        //GET ALL PROMPTS paginated
        // GET /api/prompts?pageNumber=1&pageSize=10
        [HttpGet("paginated")]
        [Authorize]
        [ProducesResponseType(typeof(PromptReadOnlyDTO), 200)] //Success 200 OK
        [ProducesResponseType(404)] //Prompt not found
        public async Task<ActionResult> GetPromptPaginated(

            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize)

        {
            int page = pageNumber ?? 1;
            int size = pageSize ?? 10;


            //service με page 1 , pageSize 10. φίλτρο ίσως αργότερα.
            var result = await applicationService.PromptService.GetPaginatedPromptsAsync(page, size);

            return Ok(result);

        }

        //UPDATE PROMPT
        //PUT /api/prompts/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")] // Μόνο Admin ή Manager
        [ProducesResponseType(204)] // Success
        [ProducesResponseType(404)] // Prompt not found
        public async Task<IActionResult> UpdatePrompt(int id, [FromBody] PromptUpdateDTO promptUpdateDto)
        {

            bool success = await applicationService.PromptService.UpdatePromptAsync(id, promptUpdateDto);
            if (!success)
            {
                return StatusCode(500, "Failed to update prompt");

            }

            return NoContent(); // 204 No Content

        }

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

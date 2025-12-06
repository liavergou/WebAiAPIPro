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

        //GET ALL PROMPTS (μενου για χρήστη)
        // GET /api/prompts/all
        [HttpGet("all")]
        [Authorize]
        [ProducesResponseType(typeof(PromptReadOnlyDTO), 200)] //Success 200 OK
        public async Task<IActionResult> GetAllProjects()
        {
            var prompts = await applicationService.PromptService.GetAllPromtsAsync();
            return Ok(prompts);
        }



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

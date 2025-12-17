using CoordExtractorApp.DTO;
using CoordExtractorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoordExtractorApp.Exceptions;

namespace CoordExtractorApp.Controllers
{
    [ApiController]
    [Route("api/conversion-jobs")]
    public class ConversionJobController : BaseController
    {
        public ConversionJobController(IApplicationService applicationService) : base(applicationService)
        {
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ConversionJobReadOnlyDTO), 200)] //success
        [ProducesResponseType(typeof(ConversionJobReadOnlyDTO), 422)] //The server understands the content type and syntax of the request entity, but it is still unable to process the request for some reason.
        [ProducesResponseType(400)] //Bad Request
        public async Task<IActionResult> CreateConversionJob([FromForm] ConversionJobInsertDTO dto)
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

        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ConversionJobReadOnlyDTO), 200)]
        [ProducesResponseType(400)]//Bad Request (μη valid πολυγωνο)
        [ProducesResponseType(404)] //Not found

        public async Task<IActionResult>UpdateConversionJob(int id, [FromBody] ConversionJobUpdateDTO dto)
        {
            var user = await GetUserInfoAsync();
            if (user.Id == null) throw new EntityNotAuthorizedException("User", "User id not found");
            var result = await applicationService.ConversionJobService.UpdateConversionJobAsync(id, dto, user.Id.Value);
            return Ok(result);
        }


        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]//not found
        public async Task<IActionResult> DeleteConversionJob(int id)
        {
            var user = await GetUserInfoAsync();
            if (user.Id == null) throw new EntityNotAuthorizedException("User", "User id not found");
            var result = await applicationService.ConversionJobService.DeleteConversionJobAsync(id, user.Id.Value);
            return NoContent();

        }



    }
}
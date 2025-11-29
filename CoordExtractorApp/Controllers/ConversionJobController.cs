using CoordExtractorApp.Core.Enums;
using CoordExtractorApp.DTO;
using CoordExtractorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
            
            if (user.Id == null)
            {
                return StatusCode(401, "User is null");
            }

            // Service
            var resultDto = await this.applicationService.ConversionJobService
                .CreateAndProcessJobAsync(dto, user.Id.Value);

            //failed job status.Errors στο response 
            if (resultDto.Status == JobStatus.Failed)
            {
                return StatusCode(422,resultDto);
            }

            //200
            return Ok(resultDto);
        }
    }
}
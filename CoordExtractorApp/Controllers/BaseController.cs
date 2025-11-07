using CoordExtractorApp.Data;
using CoordExtractorApp.Models;
using CoordExtractorApp.Repositories;
using CoordExtractorApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoordExtractorApp.Controllers
    //ο PolicyDecisionPoint είναι ο external Keycloak server ενω ο base controller εχει τη θεση του PolicyEnforcementPoint
{   //επιτρέπει σε κάθε controller που κληρονομεί να εχει πρόσβαση στα δεδομενα του χρήστη (βλ.Models/ApplicationUser) που βρίσκονται στο Token μεσω της ιδιοτητας AppUser
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public readonly IApplicationService applicationService;

        //inject το application service
        public BaseController(IApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        //καλεί το service το οποιο καλεί το repo για να παρει το id της βασης
        protected async Task<ApplicationUser> GetUserInfoAsync()
        {
            return await this.applicationService.UserService.GetUserInfoAsync(this.User);
        }


    }
}

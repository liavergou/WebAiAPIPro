using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoordExtractorApp.Helpers
{
    /// <summary>
    /// /// Εξασφαλίζει ότι το swagger ui δείχνει σωστά την απαίτηση για σύνδεση token και δικαιώματα roles
    /// /// Προσθέτει κουμπί για να εισάγουμε το JWT Token
    /// /// Εμφανίζει τα σφάλματα 401 και 403 για ενημέρωση του χρήστη
    /// /// Εμφανίζει ποιούς ρόλους απαιτεί η κλήση
    /// /// </summary>
    public class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {   //reflection μεθοδος. χειρίζεται το εσωτερικό κατα το runtime (τι μεθοδους εχει, τι attributes εχει αυτη η κλάση κλπ
            var authAttributes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>() //δείξε μονο τα Authorize attributes
                .Distinct();

            if (authAttributes.Any())
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

                // Add security requirement(προς το swagger). δημιουργουμε λίστα για να παρουμε τους ρολους
                operation.Security = new List<OpenApiSecurityRequirement>();

                var roles = context.MethodInfo.GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>()
                    .Where(attr => !string.IsNullOrEmpty(attr.Roles))
                    .SelectMany(attr => attr.Roles!.Split(',')); //αλλαγή για να πάρει το [Authorize(Roles = "Admin,Manager")]

                // Add the security requirment for JWT Bearer with specified roles
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                                Description = "Add token to header",
                                Name = "Authorization",
                                Type = SecuritySchemeType.Http,
                                In = ParameterLocation.Header,
                                Scheme = JwtBearerDefaults.AuthenticationScheme,
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = JwtBearerDefaults.AuthenticationScheme
                                }
                        },
                        roles.ToList()
                    }
                });
            }
        }
    }
}

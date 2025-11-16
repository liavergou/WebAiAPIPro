using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CoordExtractorApp.Configuration
{
    public static class AuthenticationDIExtensions
    {

        //Επέκταση του IServiceCollection με Keycloak JWT Authantication configuration
        //https://www.keycloak.org/docs-api/latest/rest-api/index.html
        // https://dev.to/kayesislam/integrating-openid-connect-to-your-application-stack-25chservices
        //https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer.jwtbeareroptions?view=aspnetcore-8.0
        public static IServiceCollection AddKeycloakAuthentication
            (this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //ρυθμιση services και ρυθμιση middleware. 
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["Keycloak:Authority"]; //ρύθμιση του keycloak server. κατεβαζει το αρχειο metadata στο endpoint. παιρνει τις διευθυνσεις για τα keys για την κρυπτογράφηση. Την επίσημη τιμή για τον Issuer. Γι αυτο δεν βάζω ValidIssuer
                    options.Audience = configuration["Keycloak:Audience"]; //ποιος χρησιμοποιεί το token                    
                    options.RequireHttpsMetadata = false; //για να αγνοήσει το https (dev mode)

                    options.MapInboundClaims = false; //απενεργοποιηση της αυτόματης μετονομασίας των claims για να χρησιμοποιήσω τα ονόματα όπως τα βλέπω στο JWT

                    options.TokenValidationParameters = new TokenValidationParameters //ρύθμιση για την αναγνωση των roles
                    {
                        ValidateIssuer = true, //ενεργοποιεί τον έλεγχο του issuer. λογω του .Authority, η .net κατεβαζει την τιμή του issuer απο τα metadata
                        ValidateAudience = true,
                        RoleClaimType = "role" //πως να διαβάσει τους ρόλους απο το token.
                    };
                });
            return services;
        }
    }
}
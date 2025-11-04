using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Text.Json;

namespace CoordExtractorApp.Helpers
{
  
        public static class KeycloakRoleExtensions
        {           
            public static Func<TokenValidatedContext, Task> MapKeycloakRolesToClaims() //event handler που διορθώνει τους ρόλους
        {
                return context =>
                {
                    var principal = context.Principal;
                    if (principal?.Identity is ClaimsIdentity identity)
                    {
                        
                        var realmAccessClaim = identity.FindFirst("realm_access"); // Ψάχνει το πεδίο claim 'realm_access' μεσα στο token

                        if (realmAccessClaim != null)
                        {
                            var realmAccess = JsonDocument.Parse(realmAccessClaim.Value); //διαβάζει το json και κανει object τη τιμή για να μπορεί η c# να κάνει navigate μέσα στο json
                            
                            if (realmAccess.RootElement.TryGetProperty("roles", out var rolesElement) && rolesElement.ValueKind == JsonValueKind.Array) // Ψάχνει το array 'roles' μέσα στο realm_access object
                            {
                                foreach (var role in rolesElement.EnumerateArray())
                                {
                                    
                                    identity.AddClaim(new Claim(ClaimTypes.Role, role.GetString()!)); // Προσθέτει κάθε ρόλο ως ClaimTypes.Role
                                }
                            }
                        }
                    }
                    return Task.CompletedTask;
                };
            }
        }
    
}
//Το Keycloak βάζει τις πληροφορίες του ρόλου σε ένα περίπλοκο JSON πεδίο μέσα στο Token), που το ονομάζει realm_access.
//Όμως η c# περιμένει να βρει τον ρόλο σε ένα πρότυπο πεδίο ClaimTypes.Role


using Azure.Identity;
using CoordExtractorApp.Configuration;
using CoordExtractorApp.Helpers;
using CoordExtractorApp.Repositories;
using CoordExtractorApp.Services;
using CoordExtractorApp.Services.Keycloak;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetTopologySuite.IO;
using Newtonsoft.Json.Converters;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Serilog;
using System.Security.Claims;
using System.Text.Json.Serialization;


namespace CoordExtractorApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //για το keyvault του azure
            var keyVaultUri = builder.Configuration["KeyVaultUri"];

            if (!string.IsNullOrWhiteSpace(keyVaultUri))
            {
                builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
            }

            //generativeAI (gemini key)
            var geminiApiKey = builder.Configuration["Gemini:Credentials:ApiKey"];

            // **CONNECTION STRING**
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");

            // **SERVICES ΣΤΟ DI CONTAINER**
        
            //POSTGIS
            builder.Services.AddDbContext<CoordExtractorApp.Data.TopoDbContext>(options =>
            options.UseNpgsql(connString, o => o.UseNetTopologySuite()));
                        
            //Repositories & Unit of Work (μέσω της Extension Method)
            builder.Services.AddRepositories();

            // Services (για το dependency injection)
            builder.Services.AddScoped<IApplicationService, ApplicationService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPromptService, PromptService>();
            builder.Services.AddScoped<IKeycloakAdminService, KeycloakAdminService>();
            builder.Services.AddScoped<IKeycloakAdminTokenService, KeycloakAdminTokenService>();

            // AutoMapper
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperConfig>());
            builder.Host.UseSerilog((ctx, lc) =>
                lc.ReadFrom.Configuration(ctx.Configuration));

            //**AUTHENTICATION ME KEYCLOAK**
            builder.Services.AddKeycloakAuthentication(builder.Configuration); //προστεθηκε η AuthenticationDIExtensions για να απλοποιηθεί στο program 
           
            //**CORS**
            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("ReactClient",
            //        b => b.WithOrigins(
            //            "http://localhost:5173",
            //            "http://localhost:5174",
            //            "http://localhost:5175",
            //            "http://localhost:5176"
            //            )
            //            .AllowAnyMethod()
            //            .AllowAnyHeader()
            //    );
            //});
            //Μονο για Testing!!!
            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("LocalClient",
            //        b => b.WithOrigins("https://localhost:5001")
            //            .AllowAnyMethod()
            //            .AllowAnyHeader()
            //    );
            //});

            // CORS Policy για Development - Επιτρέπει όλους (ΠΡΟΣΟΧΗ: Μόνο για testing!)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    b => b.AllowAnyOrigin()   // Επιτρέπει ΟΛΟΥΣ (security risk σε production!)
                          .AllowAnyMethod()   // GET, POST, PUT, DELETE κλπ
                          .AllowAnyHeader()   // Όλα τα HTTP headers
                );
            });

            //μετατροπή των dto σε json πριν σταλουν στον client
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include; //αν ενα πεδιο στο DTO είναι κενό να το βάλει
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize; //για να αντιμετωπίσει το προβλημα του circular
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
           
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "CoordExtractor App", Version = "v1" });
                // options.SupportNonNullableReferenceTypes(); // default true > .NET 6
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT"
                    });
                options.OperationFilter<AuthorizeOperationFilter>();
            });

 
            //δήλωση HttpClient
            builder.Services.AddHttpClient("KeycloakAdminClient");
            

            //--------------------------------------------------------------
            var app = builder.Build();
      
            // Configure the HTTP request pipeline.


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoordExtractor App v1"));
            }
            //προσοχή στο ordering
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();

            }
            

            app.UseCors("AllowAll");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<ErrorHandlerMiddleware>(); //οπως στο schoolApp αλλά χωρίς το registration error handler

            app.MapControllers();

            app.Run();
        }
    }
}

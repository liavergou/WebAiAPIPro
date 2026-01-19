using Azure.Identity;
using CoordExtractorApp.Configuration;
using CoordExtractorApp.Helpers;
using CoordExtractorApp.Repositories;
using CoordExtractorApp.Services;
using CoordExtractorApp.Services.GenerativeAI;
using CoordExtractorApp.Services.Geoserver;
using CoordExtractorApp.Services.Keycloak;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Serilog;


namespace CoordExtractorApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //azure keyvault
            if (builder.Environment.IsDevelopment())
            {
                var keyVaultUri = builder.Configuration["KeyVaultUri"];
                if (!string.IsNullOrEmpty(keyVaultUri))
                {
                    builder.Configuration.AddAzureKeyVault(
                        new Uri(keyVaultUri),
                        new DefaultAzureCredential());
                }
            }

            //generativeAI (api key)
            var geminiApiKey = builder.Configuration["Gemini:Credentials:ApiKey"];

            //Connection string
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");

            //SERVICES - DI CONTAINER
        
            //PostGIS
            builder.Services.AddDbContext<CoordExtractorApp.Data.TopoDbContext>(options =>
            options.UseNpgsql(connString, o => o.UseNetTopologySuite()));
                        
            //Repositories & Unit of Work
            builder.Services.AddRepositories();

            // Services
            builder.Services.AddScoped<IApplicationService, ApplicationService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPromptService, PromptService>();
            builder.Services.AddScoped<IKeycloakAdminService, KeycloakAdminService>();
            builder.Services.AddScoped<IKeycloakAdminTokenService, KeycloakAdminTokenService>();
            builder.Services.AddScoped<IGenerativeAIService, GenerativeAIService>();
            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.AddScoped<IConversionJobService, ConversionJobService>();
            builder.Services.AddScoped<IUserProjectsService, UserProjectsService > ();
            builder.Services.AddScoped<IGeoserverService, GeoserverService > ();


            // AutoMapper
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperConfig>());
            builder.Host.UseSerilog((ctx, lc) =>
                lc.ReadFrom.Configuration(ctx.Configuration));

            //Authentication-Keycloak
            builder.Services.AddKeycloakAuthentication(builder.Configuration);


            // CORS Policy WARNING: only for development and test mode-Replace if needed for production mode to specific client origins
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    b => b.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                );
            });

            //dto convert to json
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
           
            
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "CoordExtractor App", Version = "v1" });
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

 
            //HttpClient (keycloak &  geoserver)
            builder.Services.AddHttpClient("KeycloakAdminClient");
            builder.Services.AddHttpClient("GeoserverClient");
            



            var app = builder.Build();
      
            // Configure the HTTP request pipeline.


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoordExtractor App v1"));
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();

            }
            

            app.UseCors("AllowAll");

            // Serve static images in Development mode (in Production, IIS virtual directory handles this)
            if (app.Environment.IsDevelopment())
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(app.Environment.ContentRootPath, "storage")),
                    RequestPath = "/storage"
                });
            }

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}

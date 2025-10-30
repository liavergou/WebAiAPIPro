
using Azure.Identity;
using CoordExtractorApp.Repositories;
using Microsoft.EntityFrameworkCore;

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

            var geminiApiKey = builder.Configuration["Gemini:Credentials:ApiKey"];

            // **CONNECTION STRING**
            var connString = builder.Configuration.GetConnectionString("DefaultConnection");

            // **SERVICES ΣΤΟ DI CONTAINER**
            builder.Services.AddDbContext<CoordExtractorApp.Data.TopoDbContext>(options =>
            options.UseNpgsql(connString));

            //Repositories & Unit of Work (μέσω της Extension Method)
            builder.Services.AddRepositories();
            builder.Services.AddScoped<IUserRepository, UserRepository>(); // Καταχώρηση UserRepository


            // Services (για το dependency injection

            // AutoMapper

            //**AUTHENTICATION ME JWT**

            //**CORS**
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            //KEY VAULT TEST CONNECTION
            app.MapGet("/kv-test", (IConfiguration cfg) =>
            {
                //var val = cfg["ConnectionStrings:DefaultConnection"];
                //return Results.Ok(new { hasValue = !string.IsNullOrEmpty(val) });
                var safeConn = connString?.Replace("Password=", "Password=***");
                return Results.Ok(new { connectionString = safeConn });
            });

        
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

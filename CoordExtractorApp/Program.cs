
using Azure.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoordExtractorApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //για το keyvault του azure
            var keyVaultUri = builder.Configuration["https://kv-coordextractorapp-dev.vault.azure.net/"];

            if (!string.IsNullOrWhiteSpace(keyVaultUri))
            {
                builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
            }

           

            var connString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<CoordExtractorApp.Data.TopoDbContext>(options =>
            options.UseSqlServer(connString));

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            //key vault test connection
            app.MapGet("/kv-test", (IConfiguration cfg) =>
            {
                var val = cfg["ConnectionStrings:DefaultConnection"];
                return Results.Ok(new { hasValue = !string.IsNullOrEmpty(val) });
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

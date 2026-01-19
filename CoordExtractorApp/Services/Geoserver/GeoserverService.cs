using CoordExtractorApp.Exceptions;
using Serilog;

namespace CoordExtractorApp.Services.Geoserver
{
    /// <summary>
    /// Service implementation for communicating with GeoServer.
    /// Constructs secure WFS requests to GeoServer endpoints and returns the raw geospatial data (GeoJSON) or Shapefile to the client.
    /// </summary>
    public class GeoserverService : IGeoserverService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;
        private readonly ILogger<GeoserverService> logger =
           new LoggerFactory().AddSerilog().CreateLogger<GeoserverService>();

        public GeoserverService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        public async Task<string> GetProjectJobsGeoserverAsync(int projectId, string? username, string? role)
        {
            string baseUrl = configuration["Geoserver:BaseUrl"] ?? throw new InvalidOperationException("Geoserver:BaseUrl configuration is missing.");
            string typeName = configuration["Geoserver:ConversionJobsLayer"] ?? throw new InvalidOperationException("Geoserver:ConversionJobsLayer configuration is missing.");

            string cqlFilter = $"ProjectId={projectId}";

            string encodedCqlFilter = Uri.EscapeDataString(cqlFilter);


            string url = $"{baseUrl}?service=WFS&request=GetFeature&typeName={typeName}&outputFormat=application/json&srsName=EPSG:4326&cql_filter={encodedCqlFilter}&format_options=filename:project_{projectId}";


            var client = this.httpClientFactory.CreateClient("GeoserverClient");

            try
            {


                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
                {
                    throw new EntityNotAuthorizedException("Geoserver", "User and role not found.Cannot authenticate");
                }

                client.DefaultRequestHeaders.Add("Keycloak-User", username);
                client.DefaultRequestHeaders.Add("Keycloak-Role", role);

                logger.LogInformation("Fetching data from GeoServer url: {Url}", url);


                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new ServerException("Geoserver", $"Failed to fetch data from Geoserver.");

                }

                string content = await response.Content.ReadAsStringAsync();
                return content;
            }

            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Error connecting to GeoServer");
                throw new ServerException("Geoserver", "Could not connect to Geoserver");
            }

        }

        public async Task<byte[]> ExportProjectJobsGeoserverSHPAsync(int projectId, string? username, string? role)
        {
            string baseUrl = configuration["Geoserver:BaseUrl"] ?? throw new InvalidOperationException("Geoserver:BaseUrl configuration is missing."); ;
            string typeName = configuration["Geoserver:ConversionJobsLayer"] ?? throw new InvalidOperationException("Geoserver:ConversionJobsLayer configuration is missing.");

            string cqlFilter = $"ProjectId={projectId}";
            string options = $"filename:project_{projectId};CHARSET:UTF-8";

            string encodedCqlFilter = Uri.EscapeDataString(cqlFilter);
            string encodedOptions = Uri.EscapeDataString(options);


            string url = $"{baseUrl}?service=WFS&request=GetFeature&typeName={typeName}&outputFormat=shape-zip&srsName=EPSG:2100&cql_filter={encodedCqlFilter}&format_options={encodedOptions}";

            var client = this.httpClientFactory.CreateClient("GeoserverClient");

            try
            {


                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
                {
                    throw new EntityNotAuthorizedException("Geoserver", "User and role not found.Cannot authenticate");
                }

                client.DefaultRequestHeaders.Add("Keycloak-User", username);
                client.DefaultRequestHeaders.Add("Keycloak-Role", role);

                logger.LogInformation("Exporting shapefile from GeoServer url: {Url}", url);


                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new ServerException("Geoserver", $"Failed to export shapefile from Geoserver.");

                }

                byte[] content = await response.Content.ReadAsByteArrayAsync();
                return content;
            }

            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Error connecting to GeoServer");
                throw new ServerException("Geoserver", "Could not connect to Geoserver");
            }

        }
    } 
}
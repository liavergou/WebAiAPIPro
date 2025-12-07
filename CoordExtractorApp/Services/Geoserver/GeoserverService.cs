using CoordExtractorApp.Exceptions;
using Serilog;

namespace CoordExtractorApp.Services.Geoserver
{
    public class GeoserverService : IGeoserverService
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly ILogger<GeoserverService> logger =
           new LoggerFactory().AddSerilog().CreateLogger<GeoserverService>();

        public GeoserverService(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
        }

        public async Task<string> GetProjectJobsGeoserverAsync(int projectId, string? username, string? role)
        {
            string baseUrl = configuration["Geoserver:BaseUrl"] ?? throw new InvalidOperationException("Geoserver:BaseUrl configuration is missing."); ;
            string typeName = configuration["Geoserver:ConversionJobsLayer"] ?? throw new InvalidOperationException("Geoserver:ConversionJobsLayer configuration is missing."); ;

            string cqlFilter = $"ProjectId={projectId} AND DeletedAt IS NULL";

            string encodedCqlFilter = Uri.EscapeDataString(cqlFilter); //το 20% δεν δουλεψε

            //http://localhost:8085/geoserver/wfs?service=WFS&request=GetFeature&typeName=topo_app:ConversionJobs&outputFormat=application/json&srsName=EPSG:4326 + το φίλτρο
            string url = $"{baseUrl}?service=WFS&request=GetFeature&typeName={typeName}&outputFormat=application/json&srsName=EPSG:4326&cql_filter={encodedCqlFilter}";


            try
            {
                //ελεγχος να υπάρχει username role
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
                {
                    throw new EntityNotAuthorizedException("Geoserver", "User and role not found.Cannot authenticate");
                }
                //προσθήκη στο header
                httpClient.DefaultRequestHeaders.Add("Keycloak-User", username);
                httpClient.DefaultRequestHeaders.Add("Keycloak-Role", role);

                logger.LogInformation("Fetching data from GeoServer url: {Url}", url);


                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new ServerException("Geoserver", $"Failed to fetch data from Geoserver.");

                }
                //επιστρέφουμε το περιεχομενο του response(geojson). πρεπει να μετατραπεί σε string γιατι ερχεται σαν data stream
                string content = await response.Content.ReadAsStringAsync();
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
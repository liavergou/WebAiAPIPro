using CoordExtractorApp.Exceptions;
using Serilog;

namespace CoordExtractorApp.Services.Geoserver
{
    public class GeoserverService : IGeoserverService
    {
        private readonly IHttpClientFactory httpClientFactory; //διαχειριστής του pool με τις available συνδέσεις (connection pooling).
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
            string baseUrl = configuration["Geoserver:BaseUrl"] ?? throw new InvalidOperationException("Geoserver:BaseUrl configuration is missing."); ;
            string typeName = configuration["Geoserver:ConversionJobsLayer"] ?? throw new InvalidOperationException("Geoserver:ConversionJobsLayer configuration is missing."); ;

            string cqlFilter = $"ProjectId={projectId}"; //το DeletedAt IS NULL μεταφερθηκε στο sql του view του geoserver

            string encodedCqlFilter = Uri.EscapeDataString(cqlFilter); //το 20% δεν δουλεψε

            //http://localhost:8085/geoserver/wfs?service=WFS&request=GetFeature&typeName=topo_app:ConversionJobsView&outputFormat=application/json&srsName=EPSG:4326 + το φίλτρο
            //http://localhost:8085/geoserver/wfs?service=WFS&request=GetFeature&typeName=topo_app:ConversionJobsView&outputFormat=application/json&srsName=EPSG:4326&cql_filter=ProjectId%3D21
            string url = $"{baseUrl}?service=WFS&request=GetFeature&typeName={typeName}&outputFormat=application/json&srsName=EPSG:4326&cql_filter={encodedCqlFilter}";

            //διορθωση σε δημιουργία client instance απο το factory για καθε request. δεν ήταν σωστό να χρησιμοποιώ τον ίδιο HttpClient. πιθανό Mix στα credentials σε δδιαφορετικές κλήσεις
            var client = this.httpClientFactory.CreateClient("GeoserverClient");

            try
            {

                //ελεγχος να υπάρχει username role
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
                {
                    throw new EntityNotAuthorizedException("Geoserver", "User and role not found.Cannot authenticate");
                }
                //προσθήκη στο header
                client.DefaultRequestHeaders.Add("Keycloak-User", username);
                client.DefaultRequestHeaders.Add("Keycloak-Role", role);

                logger.LogInformation("Fetching data from GeoServer url: {Url}", url);


                var response = await client.GetAsync(url);

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
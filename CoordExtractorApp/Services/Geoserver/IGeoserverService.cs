namespace CoordExtractorApp.Services.Geoserver
{
    /// <summary>
    /// Service interface for communicating with GeoServer
    /// Constructs secure WFS requests to GeoServer endpoints and returns the raw geospatial data (GeoJSON) or Shapefile to the client.
    /// </summary>
    public interface IGeoserverService
    {
        /// <summary>
        /// Retrieves the conversion jobs for a specific project from GeoServer as GeoJSON.
        /// </summary>
        /// <param name="projectId">The project ID to filter jobs.</param>
        /// <param name="username">The username for Keycloak authentication.</param>
        /// <param name="role">The user role for Keycloak authentication.</param>
        /// <returns>A string containing the GeoJSON response from GeoServer.</returns>
        /// <exception cref="InvalidOperationException">Thrown if GeoServer configuration is missing.</exception>
        /// <exception cref="EntityNotAuthorizedException">Thrown if username or role were not provided.</exception>
        /// <exception cref="ServerException">Thrown if the connection to GeoServer fails or returns an error.</exception>
        Task<string> GetProjectJobsGeoserverAsync(int projectId, string? username, string? role);

        /// <summary>
        /// Exports the conversion jobs for a specific project from GeoServer as a Shapefile.
        /// </summary>
        /// <param name="projectId">The project ID to filter jobs.</param>
        /// <param name="username">The username for Keycloak authentication.</param>
        /// <param name="role">The user role for Keycloak authentication.</param>
        /// <returns>A byte array containing the Shapefile (ZIP) data.</returns>
        /// <exception cref="InvalidOperationException">Thrown if GeoServer configuration is missing.</exception>
        /// <exception cref="EntityNotAuthorizedException">Thrown if username or role were not provided.</exception>
        /// <exception cref="ServerException">Thrown if the connection to GeoServer fails or returns an error.</exception>
        Task<byte[]> ExportProjectJobsGeoserverSHPAsync(int projectId, string? username, string? role);
    }
}

namespace CoordExtractorApp.Services.Geoserver
{
    public interface IGeoserverService
    {

        Task<string> GetProjectJobsGeoserverAsync(int projectId, string? username, string? role);
    }
}

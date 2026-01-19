namespace CoordExtractorApp.Models
{
    /// <summary>
    /// Represents a generic paginated result set used for API responses.
    /// </summary>
    /// <typeparam name="T">The type of the data contained in the result list (e.g., User, Project).</typeparam>
    public class PaginatedResult<T>
    {
        

        public List<T> Data { get; set; } = [];
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        public PaginatedResult()
        {
        }

        public PaginatedResult(List<T> data, int totalRecords, int pageNumber, int pageSize)
        {
            Data = data;
            TotalRecords = totalRecords;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}

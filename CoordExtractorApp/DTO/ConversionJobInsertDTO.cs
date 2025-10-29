namespace CoordExtractorApp.DTO
{
    public class ConversionJobInsertDTO
    {
        public IFormFile File { get; set; } = null!;

        public int ProjectId { get; set; }
        public int PromptId { get; set; }
        public Guid ClientRequestId { get; set; } //.net 8 αντι για uuid. (επειδή θα είναι async)

    }
}

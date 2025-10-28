using CoordExtractorApp.Core.Enums;

namespace CoordExtractorApp.Data
{
    public class ConversionJob : BaseEntity
    {
      public int Id { get; set; }
        public string OriginalFileName { get; set; } = null!;
        public string CroppedFileName { get; set; } = null!;

        public string ModelUsed {  get; set; } = null!;

        public string WktOutput { get; set; } = null!;

        public string ImageFileId { get; set; } = null!;

        public int PromptId { get; set; } //foreign key     
        public int UserId { get; set; } //foreign key     

        public virtual Prompt Prompt { get; set; } = null!; 

        public virtual User User { get; set; } = null!;

    }
}

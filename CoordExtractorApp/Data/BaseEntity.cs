namespace CoordExtractorApp.Data
{
    public abstract class BaseEntity
    {

        public DateTime InsertAt {  get; set; }= DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; }

        public bool? IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }
    }
}

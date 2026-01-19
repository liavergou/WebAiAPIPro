namespace CoordExtractorApp.Data
{
    /// <summary>
    /// Base class for all entities
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime InsertedAt {  get; set; }= DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

    }
}

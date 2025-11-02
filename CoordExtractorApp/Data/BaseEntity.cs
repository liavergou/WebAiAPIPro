namespace CoordExtractorApp.Data
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime InsertedAt {  get; set; }= DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        //public int InsertedBy { get; set; }
        //public int ModifiedBy { get; set; }

        //public int? DeletedBy { get; set; }


    }
}

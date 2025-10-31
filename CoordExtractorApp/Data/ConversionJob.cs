using CoordExtractorApp.Core.Enums;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoordExtractorApp.Data
{
    public class ConversionJob : BaseEntity
    {
      public int Id { get; set; }
        public string OriginalFileName { get; set; } = null!;
        //public string? CroppedFileName { get; set; } = null!;

        public string? ModelUsed {  get; set; }

        //public string WktOutput { get; set; } = null!;
        //αντικατάσταση με property τυπου Geometry. Προσοχή! 2100 egsa 87. 4326 είναι για wgs84 
        [Column(TypeName = "geometry(Polygon, 2100)")]
        public Geometry? Geom { get; set; } = null!; //επίσης αλλαγή σε nullable

        public string? MongoImageFileId { get; set; } //mongo file id. αλλαγή σε nullable

        public JobStatus Status { get; set; } = JobStatus.Pending;

        public int PromptId { get; set; } //foreign key     
        public int UserId { get; set; } //foreign key     
        public int ProjectId { get; set; } //foreign key

        public virtual Prompt Prompt { get; set; } = null!; 

        public virtual User User { get; set; } = null!;

        public virtual Project Project { get; set; } = null!;

             
        //TODO ???? error απο το gemini
    }
}

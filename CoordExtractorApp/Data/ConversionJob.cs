using CoordExtractorApp.Core.Enums;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoordExtractorApp.Data
{
    /// <summary>
    /// Conversion job entity storing OCR results and geometry.
    /// </summary>
    public class ConversionJob : BaseEntity
    {
        public string OriginalFileName { get; set; } = null!;
        public string? CroppedFileName { get; set; } = null!;
        public string? ModelUsed { get; set; }

        [Column(TypeName = "geometry(Polygon, 2100)")]
        public Geometry? Geom { get; set; } = null!;

        public JobStatus Status { get; set; } = JobStatus.Pending;
        public string? ErrorMessage { get; set; }

        public int PromptId { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }

        public virtual Prompt Prompt { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual Project Project { get; set; } = null!;
    }
}

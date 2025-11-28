using CoordExtractorApp.Core.Enums;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoordExtractorApp.DTO
{
    public class ConversionJobReadOnlyDTO
    {
        public int Id { get; set; }

        public string OriginalFileName { get; set; } = null!;
        public string? CroppedFileName { get; set; }
        public string? ModelUsed { get; set; }
        public List<CoordinateDTO> Coordinates { get; set; } = new();
        public JobStatus Status { get; set; } = JobStatus.Pending;
        public string? ErrorMessage { get; set; }

    }
}

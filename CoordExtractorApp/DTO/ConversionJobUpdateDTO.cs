namespace CoordExtractorApp.DTO
{
    /// <summary>
    /// Data Transfer Object for updating coodinates of a conversion job
    /// </summary>
    public class ConversionJobUpdateDTO
    {
        /// <summary>
        /// The list of coordinates of a conversion job polygon
        /// </summary>
        public List<CoordinateDTO> Coordinates { get; set; } = new();
    }
}

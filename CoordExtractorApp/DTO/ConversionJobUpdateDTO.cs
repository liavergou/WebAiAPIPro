namespace CoordExtractorApp.DTO
{
    /// <summary>
    /// Data Transfer Object for updating coodinates of a conversion job
    /// </summary>
    public class ConversionJobUpdateDTO
    {
        /// <summary>
        /// The list of corrected coordinates
        /// </summary>
        public List<CoordinateDTO> Coordinates { get; set; } = new();
    }
}

namespace CoordExtractorApp.DTO
{
    /// <summary>
    /// Data Transfer Object representing a single coordinate point (X, Y).
    /// </summary>
    public class CoordinateDTO
    {
        //για την μετατροπή από wkt σε x,y

        /// <summary>
        /// The sequence order of the coordinate in the polygon.
        /// </summary>
        public int Order {  get; set; }

        /// <summary>
        /// The X coordinate
        /// </summary>
        public double X {  get; set; }

        /// <summary>
        /// The Y coordinate
        /// </summary>
        public double Y { get; set; }
    }
}

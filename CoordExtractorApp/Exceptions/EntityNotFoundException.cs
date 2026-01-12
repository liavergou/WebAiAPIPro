namespace CoordExtractorApp.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested entity cannot be found.
    /// </summary>
    public class EntityNotFoundException : AppException
    {
        private static readonly string DEFAULT_CODE = "NotFound";

        public EntityNotFoundException(string code, string message)
            : base(code + DEFAULT_CODE, message)
        {
        }
    }
}

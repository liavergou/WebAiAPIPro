namespace CoordExtractorApp.Exceptions
{
    /// <summary>
    /// Exception thrown when a deletion operation is forbidden.
    /// </summary>
    public class DeletionForbiddenException : AppException
    {
        private static readonly string DEFAULT_CODE = "DeletionForbidden";

        public DeletionForbiddenException(string code, string message)
            : base(code + DEFAULT_CODE,message)
        {
        }
    }
}

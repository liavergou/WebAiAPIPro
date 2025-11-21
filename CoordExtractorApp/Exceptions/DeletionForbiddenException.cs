namespace CoordExtractorApp.Exceptions
{
    public class DeletionForbiddenException : AppException
    {
        private static readonly string DEFAULT_CODE = "DeletionForbidden";

        public DeletionForbiddenException(string code, string message)
            : base(code + DEFAULT_CODE,message)
        {
        }
    }
}

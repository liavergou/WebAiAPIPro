namespace CoordExtractorApp.Exceptions
{
    /// <summary>
    /// Exception thrown when a user attempts to perform an action they are not authorized for.
    /// </summary>
    public class EntityNotAuthorizedException : AppException
    {
        private static readonly string DEFAULT_CODE = "NotAuthorized";

        public EntityNotAuthorizedException(string code, string message)
            : base(code + DEFAULT_CODE, message)
        {
        }
    }
}
namespace CoordExtractorApp.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested resource or action is forbidden (403).
    /// </summary>
    public class EntityForbiddenException : AppException
    {
        private static readonly string DEFAULT_CODE = "Forbidden";

        public EntityForbiddenException(string code, string message) : base(code + DEFAULT_CODE, message) { }

    }
}
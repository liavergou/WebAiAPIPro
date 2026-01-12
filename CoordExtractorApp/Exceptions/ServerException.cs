namespace CoordExtractorApp.Exceptions
{
    /// <summary>
    /// Exception thrown when an unexpected server error occurs (500).
    /// </summary>
    public class ServerException : AppException
    {
        public ServerException(string code, string message)
            : base(code, message)
        {
        }


    }
}

namespace CoordExtractorApp.Exceptions
{
    /// <summary>
    /// Base exception class for the application.
    /// </summary>
    public abstract class AppException : Exception
    {
        public string Code { get; set; }

        public AppException(string code, string message) : base(message)
        {
            Code = code;
        }


    }
}

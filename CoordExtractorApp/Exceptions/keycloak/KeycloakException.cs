namespace CoordExtractorApp.Exceptions.keycloak
{
    /// <summary>
    /// Exception thrown when an error occurs during interactions with the Keycloak service.
    /// </summary>
    public class KeycloakException : Exception
    {
        public string ServiceName { get; }

        public KeycloakException(string serviceName, string message)
            : base(message)
        {
            ServiceName = serviceName;
        }
    }
}

namespace CoordExtractorApp.Exceptions.keycloak
{
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

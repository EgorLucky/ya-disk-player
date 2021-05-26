namespace WebApplication1.YandexAuthentication
{
    public record YandexAppOauthConfiguration
    {
        public string AppName { get; init; }

        public string ClientId { get; init; }

        public string ClientSecretId { get; init; }

        public string AuthorizationEndpoint { get; init; }
    }
}
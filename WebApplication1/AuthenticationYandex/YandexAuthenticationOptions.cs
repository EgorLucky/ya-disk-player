using Microsoft.AspNetCore.Authentication;

namespace WebApplication1.YandexAuthentication
{
    public class YandexAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Realm { get; set; }

        public string UserInfoEndpoint { get; set; }
    }
}
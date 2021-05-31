using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApplication1.YandexAuthentication
{
    public class YandexAuthenticationHandler : AuthenticationHandler<YandexAuthenticationOptions>
    {
        private readonly HttpClient _client;

        public YandexAuthenticationHandler(
            IOptionsMonitor<YandexAuthenticationOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock,
            IHttpClientFactory factory
            ) 
            : base(options, logger, encoder, clock)
        {
            _client = factory.CreateClient();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                //Authorization header not in request
                return AuthenticateResult.NoResult();
            }

            if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out AuthenticationHeaderValue headerValue))
            {
                //Invalid Authorization header
                return AuthenticateResult.NoResult();
            }

            if (!"Bearer".Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                //Not Basic authentication header
                return AuthenticateResult.NoResult();
            }

            try
            {
                var userInfoEndpoint = string.IsNullOrEmpty(Options.UserInfoEndpoint) ?
                    YandexSchemeDefaults.UserInformationEndpoint
                    : Options.UserInfoEndpoint;

                var httpReq = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(userInfoEndpoint)
                };

                httpReq.Headers.Authorization = new AuthenticationHeaderValue("OAuth", headerValue.Parameter);

                var response = await _client.SendAsync(httpReq);

                response.EnsureSuccessStatusCode();

                var str = await response.Content.ReadAsStringAsync();

                var user = JsonDocument.Parse(str);

                var claims = new[] {
                    new Claim("userId", user.RootElement.GetProperty("id").GetString() ?? ""),
                    new Claim("last_name", user.RootElement.GetProperty("last_name").GetString() ?? ""),
                    new Claim("sex", user.RootElement.GetProperty("sex").GetString() ?? ""),
                    new Claim("default_email", user.RootElement.GetProperty("default_email").GetString() ?? ""),
                    new Claim("first_name", user.RootElement.GetProperty("first_name").GetString() ?? ""),
                    new Claim("login", user.RootElement.GetProperty("login").GetString() ?? ""),
                    new Claim(ClaimTypes.Surname, user.RootElement.GetProperty("last_name").GetString() ?? ""),
                    new Claim(ClaimTypes.Gender, user.RootElement.GetProperty("sex").GetString() ?? ""),
                    new Claim(ClaimTypes.Email, user.RootElement.GetProperty("default_email").GetString() ?? ""),
                    new Claim(ClaimTypes.Name, user.RootElement.GetProperty("first_name").GetString() ?? "") 

                };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception e)
            {
                return AuthenticateResult.Fail("Invalid acceessTOKEN");
            }
            
        }
    }
}
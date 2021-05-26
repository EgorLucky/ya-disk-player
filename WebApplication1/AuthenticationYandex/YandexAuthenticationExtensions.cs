using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.YandexAuthentication
{
    public static class YandexAuthenticationExtensions
    {
        public static AuthenticationBuilder AddYandexScheme(this AuthenticationBuilder builder)
        {
            return AddYandexScheme(builder, YandexSchemeDefaults.AuthenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddYandexScheme(this AuthenticationBuilder builder, string authenticationScheme)
        {
            return AddYandexScheme(builder, authenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddYandexScheme(this AuthenticationBuilder builder, Action<YandexAuthenticationOptions> configureOptions)
        {
            return AddYandexScheme(builder, YandexSchemeDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddYandexScheme(this AuthenticationBuilder builder, string authenticationScheme, Action<YandexAuthenticationOptions> configureOptions)
        {
            builder.Services.AddSingleton<IPostConfigureOptions<YandexAuthenticationOptions>,
                                YandexAuthenticationPostConfigureOptions>();

            return builder.AddScheme<YandexAuthenticationOptions, YandexAuthenticationHandler>(
                authenticationScheme, configureOptions);
        }
    }
}

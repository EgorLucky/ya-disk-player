using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.YandexAuthentication
{
    public class YandexAuthenticationPostConfigureOptions : IPostConfigureOptions<YandexAuthenticationOptions>
    {
        public void PostConfigure(string name, YandexAuthenticationOptions options)
        {
        }
    }
}

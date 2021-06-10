using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic
{
    public record YandexAppOauthConfiguration
    {
        public string AppName { get; init; }

        public string ClientId { get; init; }

        public string ClientSecretId { get; init; }

        public string AuthorizationEndpoint { get; init; }

        public string TokenEndpoint { get; init; }
    }
}

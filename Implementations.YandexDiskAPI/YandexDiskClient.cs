using DomainLogic;
using DomainLogic.YandexApiEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Implementations.YandexDiskAPI
{
    public class YandexDiskClient: IYandexDiskApi
    {
        const string _YANDEX_OAUTH_SCHEME = "OAuth";
        private readonly HttpClient _client;
        private readonly YandexAppOauthConfiguration _configuration;

        public YandexDiskClient(HttpClient client, YandexAppOauthConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
        }

        public async Task<ResourcesFileResponse> ResourcesFiles(ResourcesFilesRequest request, YandexToken token)
        {
            var uri = "https://cloud-api.yandex.net/v1/disk/resources/files";

            uri += $"?limit={request.Limit}&" +
                $"offset={request.Offset}&" +
                $"media_type={request.MediaType}&" +
                $"fields={WebUtility.UrlEncode(string.Join(",", request.Fields))}"; 

            var result = await DoHttpRequest<ResourcesFileResponse>(HttpMethod.Get, uri, token);

            return result;
        }

        async Task<T> DoHttpRequest<T>(HttpMethod method, string uri, YandexToken token)
        {
            var ok = false;
            var response = default(HttpResponseMessage);
            do
            {
                var reqMessage = new HttpRequestMessage
                {
                    Method = method,
                    RequestUri = new Uri(uri)
                };

                reqMessage.Headers.Authorization = new AuthenticationHeaderValue(_YANDEX_OAUTH_SCHEME, token.AccessToken);
                response = await _client.SendAsync(reqMessage);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await RefreshToken(token);
                    ok = true;
                }
                else
                    ok = false;
            }
            while (ok);

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions() 
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return result;
        }

        private async Task RefreshToken(YandexToken token)
        {
            var message = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_configuration.TokenEndpoint}"),
                Content = new FormUrlEncodedContent(new Dictionary<string, string>() 
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", token.RefreshToken }
                }),
            };

            message.Headers.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_configuration.ClientId}:{_configuration.ClientSecretId}")));
            var response = await _client.SendAsync(message);

            if(response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jDoc = JsonSerializer.Deserialize<JsonDocument>(jsonString);

                token.AccessToken = jDoc.RootElement.GetProperty("access_token").GetString();
                token.RefreshToken = jDoc.RootElement.GetProperty("refresh_token").GetString();
            }
            else if(response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new TokenExpiredException("AccessToken and RefreshToken expired");
            }
            else
            {
                throw new Exception($"Yandex HttpClientException response code {response.StatusCode}"); 
            }
        }
    }
}

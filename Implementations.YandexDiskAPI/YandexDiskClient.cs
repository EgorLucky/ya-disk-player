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

            var result = await DoHttpRequestWithDeserializedReponse<ResourcesFileResponse>(HttpMethod.Get, uri, null, token);

            return result;
        }

        public async Task<ResourcesDownloadResult> ResourcesDownload(string path, YandexToken token)
        {
            var uri = $"https://cloud-api.yandex.net/v1/disk/resources/download?path={WebUtility.UrlEncode(path)}";

            var result = await DoHttpRequestWithDeserializedReponse<ResourcesDownloadResult>(HttpMethod.Get, uri, null, token);

            return result;
        }

        public async Task RefreshToken(YandexToken token)
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

                token.OauthToken = jDoc.RootElement.GetProperty("access_token").GetString();
                token.RefreshToken = jDoc.RootElement.GetProperty("refresh_token").GetString();

                var jwtResponse = await GetJwtToken(token);

                token.JwtToken = jwtResponse;
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

        public async Task<YandexToken> GetToken(string code)
        {
            var oauthTokenResponseJson = await DoHttpRequestWithDeserializedReponse<JsonDocument>(HttpMethod.Post, 
                                _configuration.TokenEndpoint,
                                new FormUrlEncodedContent(new Dictionary<string, string>()
                                {
                                    { "code", code },
                                    { "grant_type", "authorization_code" },
                                    { "client_id", _configuration.ClientId },
                                    { "client_secret", _configuration.ClientSecretId }
                                }), 
                                null);

            if(oauthTokenResponseJson.RootElement.TryGetProperty("error", out JsonElement value))
            {
                var message = oauthTokenResponseJson.RootElement.GetProperty("error_description").GetString();
                throw new Exception(message);
            }

            var accessToken = oauthTokenResponseJson.RootElement.GetProperty("access_token").GetString();
            var refreshToken = oauthTokenResponseJson.RootElement.GetProperty("refresh_token").GetString();
            var yandexToken = new YandexToken(
                oauthToken: accessToken, 
                refreshToken: refreshToken);

            var jwtResponse = await GetJwtToken(yandexToken);

            yandexToken.JwtToken = jwtResponse;

            return yandexToken;
        }

        Task<string> GetJwtToken(YandexToken yandexToken) => DoHttpRequest(HttpMethod.Get,
                "https://login.yandex.ru/info?format=jwt", null, yandexToken);

        async Task<T> DoHttpRequestWithDeserializedReponse<T>(HttpMethod method, string uri, HttpContent content, YandexToken token)
        {
            var jsonString = await DoHttpRequest(method, uri, content, token);
            
            var result = JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return result;
        }

        async Task<string> DoHttpRequest(HttpMethod method, string uri, HttpContent content, YandexToken token)
        {
            var ok = false;
            var response = default(HttpResponseMessage);
            do
            {
                var reqMessage = new HttpRequestMessage
                {
                    Method = method,
                    RequestUri = new Uri(uri),
                    Content = content
                };

                if (token != null)
                    reqMessage.Headers.Authorization = new AuthenticationHeaderValue(_YANDEX_OAUTH_SCHEME, token.OauthToken);
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
     
            return await response.Content.ReadAsStringAsync();
        }
    }
}

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

        public YandexDiskClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<ResourcesFileResponse> ResourcesFiles(ResourcesFilesRequest request, string accessToken)
        {
            var uri = "https://cloud-api.yandex.net/v1/disk/resources/files";

            uri += $"?limit={request.Limit}&" +
                $"offset={request.Offset}&" +
                $"media_type={request.MediaType}&" +
                $"fields={WebUtility.UrlEncode(string.Join(",", request.Fields))}"; 

            var result = await DoHttpRequest<ResourcesFileResponse>(HttpMethod.Get, uri, accessToken);

            return result;
        }

        async Task<T> DoHttpRequest<T>(HttpMethod method, string uri, string accessToken)
        {
            var reqMessage = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(uri)
            };

            reqMessage.Headers.Authorization = new AuthenticationHeaderValue(_YANDEX_OAUTH_SCHEME, accessToken);

            var response = await _client.SendAsync(reqMessage);

            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions() 
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return result;
        }
    }
}

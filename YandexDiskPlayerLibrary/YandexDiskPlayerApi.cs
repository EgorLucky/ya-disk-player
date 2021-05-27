using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using YandexDiskPlayerLibrary.YandexDiskApi;
using YandexDiskPlayerLibrary.YandexDiskApi.Models;

namespace YandexDiskPlayerLibrary
{
    public class YandexDiskPlayerApi
    {
        protected string _accessToken;
        protected readonly YandexDiskClient _client;
        public string AccessToken { set { _accessToken = value; } }

        public YandexDiskPlayerApi(IHttpClientFactory httpClientFactory)
        {
            _client = new YandexDiskClient(httpClientFactory.CreateClient());
        }

        public YandexDiskPlayerApi(IHttpClientFactory httpClientFactory, string accessToken)
        {
            _accessToken = accessToken;
            _client = new YandexDiskClient(httpClientFactory.CreateClient());
        }

        public async Task<object> LoadFiles()
        {
            var request = new ResourcesFilesRequest
            {
                Fields = new List<string> {
                    "items.name",
                    "items.resource_id",
                    "items.path",
                    "items.file"
                },

                Limit = 10,
                Offset = 0,
                MediaType = "audio"
            };

            var result = await _client.ResourcesFiles(request, _accessToken);

            return result;
        }
    }
}

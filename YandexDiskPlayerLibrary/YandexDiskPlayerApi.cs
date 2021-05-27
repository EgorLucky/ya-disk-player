using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using YandexDiskPlayerLibrary.YandexDiskApi;
using YandexDiskPlayerLibrary.YandexDiskApi.Models;
using YandexDiskPlayerLibrary.Entities;
using System.Linq;

namespace YandexDiskPlayerLibrary
{
    public class YandexDiskPlayerApi
    {
        protected string _accessToken;
        protected readonly YandexDiskClient _client;
        public string AccessToken { set { _accessToken = value; } }

        static readonly List<string> ResourceFilesRequestFields = new List<string> 
        {
            "items.name",
            "items.resource_id",
            "items.path",
            "items.file"
        };

        readonly int ResourcesFilesRequestLimit = 100;

        public YandexDiskPlayerApi(IHttpClientFactory httpClientFactory)
        {
            _client = new YandexDiskClient(httpClientFactory.CreateClient());
        }

        public YandexDiskPlayerApi(IHttpClientFactory httpClientFactory, string accessToken)
        {
            _accessToken = accessToken;
            _client = new YandexDiskClient(httpClientFactory.CreateClient());
        }

        public async Task SynchronizeFiles()
        {
            var syncProcess = new SynchronizationProcess
            {
                State = SynchronizationProcessState.Runnig,
                StartDate = DateTimeOffset.Now
            };

            var allLoaded = false;
            var offset = 0;

            while (allLoaded == false)
            {
                var response = await Get(ResourcesFilesRequestLimit, offset);

                if(response.Items.Count == 0)
                {
                    allLoaded = true;
                    syncProcess.EndDate = DateTimeOffset.Now;
                    syncProcess.State = SynchronizationProcessState.Finished;


                    continue;
                }

                var files = response.Items;

                if(files.Any(f => f.ResourceId == syncProcess.LastFileId))
                {

                }
            }


        }

        async Task<ResourcesFileResponse> Get(int limit, int offset)
        {
            var request = new ResourcesFilesRequest
            {
                Fields = ResourceFilesRequestFields,
                Limit = limit,
                Offset = offset,
                MediaType = "audio"
            };

            var response = await _client.ResourcesFiles(request, _accessToken);
            return response;
        }
    }
}

using DomainLogic.Entities;
using DomainLogic.ResponseModels;
using DomainLogic.YandexApiEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic
{
    public class SynchronizationService
    {
        private readonly ISynchronizationHistoryRepository _repository;
        private readonly ISynchronizationMessageService _messageService;
        private readonly IYandexDiskApi _yandexDiskApi;

        static readonly List<string> ResourceFilesRequestFields = new List<string>
        {
            "items.name",
            "items.resource_id",
            "items.path",
            "items.file"
        };

        readonly int ResourcesFilesRequestLimit = 100;

        public SynchronizationService(
            ISynchronizationHistoryRepository repository, 
            ISynchronizationMessageService messageSevice,
            IYandexDiskApi yandexDiskApi)
        {
            _repository = repository;
            _messageService = messageSevice;
            _yandexDiskApi = yandexDiskApi;
        }

        public async Task<SynchronizationStartResponseModel> Start(string yandexId, string accessToken, string refreshToken)
        {
            var unfinishedProcess = await _repository.GetRunningProcess(yandexId);

            if (unfinishedProcess != null)
                return new SynchronizationStartResponseModel(
                    Success: false,
                    ErrorMessage: $"Running synchronization process already exists"
                    );

            var synchProcess = new SynchronizationProcess(
                Id: Guid.NewGuid(),
                CreateDateTime: DateTimeOffset.Now,
                YandexUserId: yandexId
                );
            await _repository.Add(synchProcess);

            await _messageService.Send(synchProcess.Id, accessToken, refreshToken);

            return new SynchronizationStartResponseModel(
                Success: true,
                SynchronizationProcessId: synchProcess.Id
            );
        }


        public async Task Synchronize(Guid processId, string accessToken, string refreshToken)
        {
            var process = await _repository.GetProcessById(processId);

            process = process with 
            { 
                State = SynchronizationProcessState.Runnig,
                StartDateTime = DateTimeOffset.Now
            };

            var allLoaded = false;
            var offset = 0;

            while (allLoaded == false)
            {
                var response = await Get(ResourcesFilesRequestLimit, offset, accessToken);

                if (response.Items.Count == 0)
                {
                    allLoaded = true;

                    process = process with
                    {
                        FinishedDateTime = DateTimeOffset.Now,
                        State = SynchronizationProcessState.Finished
                    };


                    continue;
                }

                var files = response.Items;

                if (files.Any(f => f.ResourceId == process.LastFileId))
                {

                }


                files.ForEach(f => Console.WriteLine(f));

                offset += 100;
            }
        }

        async Task<ResourcesFileResponse> Get(int limit, int offset, string accessToken)
        {
            var request = new ResourcesFilesRequest(
                Fields: ResourceFilesRequestFields,
                Limit: limit,
                Offset: offset,
                MediaType: "audio"
            );

            var response = await _yandexDiskApi.ResourcesFiles(request, accessToken);
            return response;
        }
    }
}

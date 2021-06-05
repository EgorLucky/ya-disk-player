using DomainLogic.Entities;
using DomainLogic.YandexApiEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic
{
    public class SynchronizationBackgroundService
    {
        private readonly ISynchronizationHistoryRepository _repository;
        private readonly IYandexDiskApi _yandexDiskApi;

        static readonly List<string> ResourceFilesRequestFields = new List<string>
        {
            "items.name",
            "items.resource_id",
            "items.path",
            "items.file"
        };

        readonly int ResourcesFilesRequestLimit = 100;

        public SynchronizationBackgroundService(ISynchronizationHistoryRepository repository, IYandexDiskApi yandexDiskApi)
        {
            _repository = repository;
            _yandexDiskApi = yandexDiskApi;
        }


        public async Task Synchronize(Guid processId, string accessToken, string refreshToken)
        {
            var process = await _repository.GetProcessById(processId);

            process = process with
            {
                State = SynchronizationProcessState.Runnig,
                StartDateTime = DateTimeOffset.Now
            };

            await _repository.Update(process);

            var stopCycle = false;
            var endState = SynchronizationProcessState.Finished;
            while (stopCycle == false)
            {
                var response = await Get(ResourcesFilesRequestLimit, process.Offset, accessToken);

                if (response.Items.Count == 0)
                {
                    stopCycle = true;
                    continue;
                }

                var cancelledByUser = await _repository.IsCancelledByUser(processId);

                if (cancelledByUser)
                {
                    endState = SynchronizationProcessState.CanceledByUser;
                    stopCycle = true;
                    continue;
                }
                    

                var files = response.Items;

                if (files.Any(f => f.ResourceId == process.LastFileId))
                {

                }


                files.ForEach(f => Console.WriteLine(f));

                process = process with { Offset = process.Offset + 100 };

                await _repository.Update(process);
            }

            process = process with
            {
                State = endState,
                FinishedDateTime = DateTimeOffset.Now
            };

            await _repository.Update(process);
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

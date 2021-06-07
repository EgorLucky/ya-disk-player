using AutoMapper;
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
        private readonly IMapper _mapper;

        static readonly List<string> ResourceFilesRequestFields = new List<string>
        {
            "items.name",
            "items.resource_id",
            "items.path",
            "items.file",
            "items.type"
        };

        readonly int ResourcesFilesRequestLimit = 100;

        public string YandexUserId { get; private set; }
        public Guid SynchronizationProcessId { get; private set; }

        public SynchronizationBackgroundService(
            ISynchronizationHistoryRepository repository, 
            IYandexDiskApi yandexDiskApi,
            IMapper mapper)
        {
            _repository = repository;
            _yandexDiskApi = yandexDiskApi;
            _mapper = mapper;
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
                try
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


                    var resourceFiles = response.Items;

                    if (resourceFiles.Any(f => f.ResourceId == process.LastFileId))
                    {
                        process = process with { Offset = 0 };
                        continue;
                    }

                    var folders = GetFolders(resourceFiles);

                    folders = folders.Select(f => f with
                    {
                        YandexUserId = process.YandexUserId,
                        SynchronizationProcessId = processId
                    })
                    .ToList();

                    var resourceIds = resourceFiles.Select(r => r.ResourceId);

                    var files = resourceFiles.Select(r => _mapper.Map<File>(r));


                    //

                    process = process with { Offset = process.Offset + 100 };

                    await _repository.Update(process);
                }
                catch(Exception ex)
                {
                    //
                }
            }

            process = process with
            {
                State = endState,
                FinishedDateTime = DateTimeOffset.Now
            };

            await _repository.Update(process);
        }

        private List<File> GetFolders(List<ResourcesFileItem> resourceFiles)
        {
            var paths = resourceFiles
                            .Select(r => r.Path.Replace(r.Name, ""))
                            .Distinct();

            var folders = new List<File>();

            foreach(var path in paths)
            {
                var pathFolders = path.Split("/", StringSplitOptions.RemoveEmptyEntries).ToList();
                do
                {
                    var folder = new File(
                        Name: pathFolders.LastOrDefault(),
                        Path: string.Join("/", pathFolders.Take(pathFolders.Count)))
                    {
                        ParentFolderPath = string.Join("/", pathFolders.Take(pathFolders.Count - 1)),
                        ParentFolder = pathFolders.Take(pathFolders.Count - 1).LastOrDefault(),
                        Type = "folder"
                    };

                    if (folders.Any(f => f.Path == folder.Path) == false)
                        folders.Add(folder);
                    else pathFolders.Clear();

                    pathFolders = pathFolders.Take(pathFolders.Count - 1).ToList();
                }
                while (pathFolders.Count > 0);
            }

            return folders;
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

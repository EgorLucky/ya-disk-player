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
        private readonly IFileRepository _fileRepository;
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

        public SynchronizationBackgroundService(
            ISynchronizationHistoryRepository repository,
            IFileRepository fileRepository,
            IYandexDiskApi yandexDiskApi,
            IMapper mapper
            )
        {
            _repository = repository;
            _yandexDiskApi = yandexDiskApi;
            _fileRepository = fileRepository;
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

                    var resourceIds = resourceFiles.Select(r => r.ResourceId);

                    var files = resourceFiles.Select(r => _mapper.Map<File>(r));

                    files = files.Select(f => f with
                    {
                        YandexUserId = process.YandexUserId
                    })
                    .ToList();

                    var existingFiles = await _fileRepository.GetFilesByResourceId(files);

                    var filesToUpdate = existingFiles
                            .Where(ef => ef.SynchronizationProcessId != processId)
                            .Select(ef =>
                            {
                                var newFile = files.Where(f => f.ResourceId == ef.ResourceId).First();
                                _mapper.Map(newFile, ef);
                                ef.SynchronizationProcessId = processId;

                                return ef;
                            })
                            .ToList();

                    await _fileRepository.Update(filesToUpdate);

                    var newFiles = files
                                        .Where(f => existingFiles.Any(ef => ef.ResourceId == f.ResourceId) == false)
                                        .Select(f => f with
                                        {
                                            SynchronizationProcessId = processId
                                        })
                                        .ToList();

                    await _fileRepository.Add(newFiles);

                    process = process with { Offset = process.Offset + resourceFiles.Count };

                    await _repository.Update(process);
                }
                catch(Exception ex)
                {
                    //
                    Console.WriteLine(ex);
                }
            }

            var parentFolderPaths = await _fileRepository.GetParentFolderPaths(processId);

            var folders = GetFolders(parentFolderPaths);

            folders = folders.Select(f => f with
            {
                YandexUserId = process.YandexUserId
            })
            .ToList();

            var existingFolders = await _fileRepository.GetFilesByPaths(folders);

            var foldersToUpdate = existingFolders
                    .Where(ef => ef.SynchronizationProcessId != processId)
                    .Select(f => f with
                    {
                        SynchronizationProcessId = processId
                    })
                    .ToList();

            await _fileRepository.Update(foldersToUpdate);

            var newFolders = folders
                                .Where(f => existingFolders.Any(ef => ef.Path == f.Path) == false)
                                .Select(f => f with
                                {
                                    SynchronizationProcessId = processId
                                })
                                .ToList();

            await _fileRepository.Add(newFolders);

            process = process with
            {
                State = endState,
                FinishedDateTime = DateTimeOffset.Now
            };

            await _repository.Update(process);

            await _fileRepository.DeleteOld(processId);
        }

        private List<File> GetFolders(List<string> paths)
        {
            var folders = new List<File>();

            foreach(var path in paths)
            {
                var pathFolders = path.Split("/", StringSplitOptions.RemoveEmptyEntries).ToList();
                do
                {
                    var folder = _mapper.Map<File>(pathFolders);

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

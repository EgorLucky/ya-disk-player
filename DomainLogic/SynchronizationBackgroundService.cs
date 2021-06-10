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

            var yandexToken = new YandexToken(accessToken, refreshToken);

            await _repository.Update(process);

            var stopCycle = false;
            var endState = SynchronizationProcessState.Finished;
            while (stopCycle == false)
            {
                try
                {
                    var response = await GetFilesFromYandexDisk(ResourcesFilesRequestLimit, process.Offset, yandexToken);

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

                    var resourceIds = resourceFiles
                                        .Select(r => r.ResourceId)
                                        .ToList();

                    var files = resourceFiles
                        .Select(r => _mapper.Map<File>(r) with
                    {
                        YandexUserId = process.YandexUserId
                    })
                    .ToList();

                    var existingFiles = await _fileRepository.GetFilesByResourceId(resourceIds, process.YandexUserId);

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

                    process = process with 
                    { 
                        Offset = process.Offset + resourceFiles.Count,
                        LastFileId = response.Items.Last().ResourceId
                    };

                    await _repository.Update(process);
                }
                catch (TokenExpiredException ex)
                {
                    endState = SynchronizationProcessState.TokenExpired;
                    Console.WriteLine(endState);
                    stopCycle = true;
                }
                catch(Exception ex)
                {
                    endState = SynchronizationProcessState.CanceledBySystem;
                    Console.WriteLine(ex);
                    //todo: saveException
                    stopCycle = true;
                }
            }

            if (endState == SynchronizationProcessState.Runnig)
            {
                try
                {
                    var parentFolderPaths = await _fileRepository.GetParentFolderPaths(processId);

                    var folders = GetFolders(parentFolderPaths);

                    folders = folders.Select(f => f with
                    {
                        YandexUserId = process.YandexUserId
                    })
                    .ToList();

                    var folderPaths = folders
                                        .Select(f => f.Path)
                                        .ToList();

                    var existingFolders = await _fileRepository.GetFilesByPaths(folderPaths, process.YandexUserId);

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

                    await _fileRepository.DeleteOld(processId);
                }
                catch(Exception ex)
                {
                    endState = SynchronizationProcessState.CanceledBySystem;
                    Console.WriteLine(ex);
                    //todo: saveException
                }
            }

            process = process with
            {
                State = endState,
                FinishedDateTime = DateTimeOffset.Now
            };

            await _repository.Update(process);

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

        async Task<ResourcesFileResponse> GetFilesFromYandexDisk(int limit, int offset, YandexToken token)
        {
            var request = new ResourcesFilesRequest(
                Fields: ResourceFilesRequestFields,
                Limit: limit,
                Offset: offset,
                MediaType: "audio"
            );

            var response = await _yandexDiskApi.ResourcesFiles(request, token);
            return response;
        }

    }
}

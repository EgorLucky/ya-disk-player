using AutoMapper;
using DomainLogic.Entities;
using DomainLogic.Repositories;
using DomainLogic.YandexApiEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Services
{
    public class SynchronizationBackgroundService
    {
        private readonly ISynchronizationHistoryRepository _repository;
        private readonly FileSynchronizationService _fileSynchronizer;
        private readonly IIgnorePathRepository _ignorePathRepository;
        private readonly IErrorRepoistory _errorRepository;
        private readonly IYandexDiskApi _yandexDiskApi;
        private readonly IMapper _mapper;

        static readonly List<string> ResourceFilesRequestFields = new List<string>
        {
            "items.name",
            "items.resource_id",
            "items.path",
            "items.type"
        };

        readonly int ResourcesFilesRequestLimit = 100;

        public SynchronizationBackgroundService(
            ISynchronizationHistoryRepository repository,
            FileSynchronizationService fileSynchronizer,
            IIgnorePathRepository ignorePathRepository,
            IErrorRepoistory errorRepository,
            IYandexDiskApi yandexDiskApi,
            IMapper mapper)
        {
            _repository = repository;
            _yandexDiskApi = yandexDiskApi;
            _ignorePathRepository = ignorePathRepository;
            _errorRepository = errorRepository;
            _mapper = mapper;
            _fileSynchronizer = fileSynchronizer;
        }

        public async Task Synchronize(Guid processId, string accessToken, string refreshToken)
        {
            var process = default(SynchronizationProcess);
            var endState = SynchronizationProcessState.Finished;
            try
            {
                process = await _repository.GetProcessById(processId);

                process = process with
                {
                    State = SynchronizationProcessState.Runnig,
                    StartDateTime = DateTimeOffset.Now
                };
                await _repository.Update(process);

                var yandexToken = new YandexToken(accessToken, refreshToken);
                var stopCycle = false;

                var ignorePaths = await _ignorePathRepository.GetAll(process.YandexUserId);
                
                while (stopCycle == false)
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

                    var files = resourceFiles
                        .Where(r => ignorePaths.Any(i => r.Path.StartsWith(i)) == false)
                        .Select(r => _mapper.Map<File>(r) with
                        {
                            YandexUserId = process.YandexUserId,
                            SynchronizationProcessId = processId
                        })
                        .ToList();

                    await _fileSynchronizer.SynchronizeFiles(files);

                    process = process with
                    {
                        Offset = process.Offset + resourceFiles.Count,
                        LastFileId = response.Items.Last().ResourceId
                    };

                    await _repository.Update(process);
                }

                if (process.State == SynchronizationProcessState.Runnig && endState == SynchronizationProcessState.Finished)
                {
                    await _fileSynchronizer.SynchronizeFoldersByFilePaths(processId, process.YandexUserId);
                }
            }
            catch (TokenExpiredException ex)
            {
                endState = SynchronizationProcessState.TokenExpired;
                Console.WriteLine(endState);
            }
            catch (Exception ex)
            {
                endState = SynchronizationProcessState.CanceledBySystem;
                Console.WriteLine(ex);
                await _errorRepository.Add(ex, processId);
            }
            finally
            {
                process = process with
                {
                    State = endState,
                    FinishedDateTime = DateTimeOffset.Now
                };

                try
                {
                    await _repository.Update(process);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    //todo: think what to do
                }
            }
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

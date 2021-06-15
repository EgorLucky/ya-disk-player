using AutoMapper;
using DomainLogic.Entities;
using DomainLogic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Services
{
    public class FileSynchronizationService
    {
        private readonly IFileRepository _repository;
        private readonly IMapper _mapper;

        public FileSynchronizationService(
            IFileRepository repository, 
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        internal async Task SynchronizeFiles(List<File> files)
        {
            var resourceIds = files
                            .Select(r => r.ResourceId)
                            .ToList();
            var yandexUserId = files
                            .Select(f => f.YandexUserId)
                            .FirstOrDefault();
            var processId = files
                            .Select(f => f.SynchronizationProcessId)
                            .FirstOrDefault();

            var existingFiles = await _repository.GetFilesByResourceId(resourceIds, yandexUserId);

            var filesToUpdate = existingFiles
                    .Where(ef => ef.SynchronizationProcessId != processId)
                    .Select(ef =>
                    {
                        var newFile = files.Where(f => f.ResourceId == ef.ResourceId).First();
                        ef = newFile with
                        {
                            CreateDateTime = ef.CreateDateTime,
                            LastUpdateDateTime = DateTimeOffset.Now,
                            SynchronizationProcessId = processId
                        };

                        return ef;
                    })
                    .ToList();

            await _repository.Update(filesToUpdate);

            var newFiles = files
                            .Where(f => existingFiles.Any(ef => ef.ResourceId == f.ResourceId) == false)
                            .Select(f => f with
                            {
                                SynchronizationProcessId = processId,
                                CreateDateTime = DateTimeOffset.Now
                            })
                            .ToList();

            await _repository.Add(newFiles);
        }

        internal async Task SynchronizeFoldersByFilePaths(Guid processId, string yandexUserId)
        {
            var parentFolderPaths = await _repository.GetParentFolderPaths(processId);

            var folders = GetFolders(parentFolderPaths);

            folders = folders.Select(f => f with
            {
                YandexUserId = yandexUserId
            })
            .ToList();

            var folderPaths = folders
                                .Select(f => f.Path)
                                .ToList();

            var existingFolders = await _repository.GetFilesByPaths(folderPaths, yandexUserId);

            var foldersToUpdate = existingFolders
                    .Where(ef => ef.SynchronizationProcessId != processId)
                    .Select(f => f with
                    {
                        SynchronizationProcessId = processId,
                        LastUpdateDateTime = DateTimeOffset.Now
                    })
                    .ToList();

            await _repository.Update(foldersToUpdate);

            var newFolders = folders
                                .Where(f => existingFolders.Any(ef => ef.Path == f.Path) == false)
                                .Select(f => f with
                                {
                                    SynchronizationProcessId = processId,
                                    CreateDateTime = DateTimeOffset.Now
                                })
                                .ToList();

            await _repository.Add(newFolders);
            await _repository.DeleteAllExceptWithSynchronizationProcessId(processId, yandexUserId);
        }


        private List<File> GetFolders(List<string> paths)
        {
            var folders = new List<File>();

            foreach (var path in paths)
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
    }
}

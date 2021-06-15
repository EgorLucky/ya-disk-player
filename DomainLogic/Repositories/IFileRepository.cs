using DomainLogic.Entities;
using DomainLogic.RequestModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainLogic.Repositories
{
    public interface IFileRepository
    {
        Task<List<File>> GetFilesByPaths(List<string> paths, string yandexUserId);
        Task Update(List<File> existingFiles);
        Task Add(List<File> files);
        Task<List<File>> GetFilesByResourceId(List<string> resourceIds, string yandexUserId);
        Task DeleteAllExceptWithSynchronizationProcessId(Guid lastProcessId, string yandexUserId);
        Task<List<string>> GetParentFolderPaths(Guid lastProcessId);
        Task<List<File>> GetFilesByParentFolderPath(GetFilesRequestModel request);
    }
}
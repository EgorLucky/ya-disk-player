using DomainLogic.Entities;
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
        Task DeleteOld(Guid lastProcessId);
        Task<List<string>> GetParentFolderPaths(Guid lastProcessId);
    }
}
using DomainLogic.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainLogic
{
    public interface IFileRepository
    {
        Task<List<File>> GetFilesByPaths(List<File> files);
        Task Update(List<File> existingFiles);
        Task Add(List<File> files);
        Task<List<File>> GetFilesByResourceId(IEnumerable<File> files);
        Task DeleteOld(Guid lastProcessId);
    }
}
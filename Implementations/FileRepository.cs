using Implementations.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DomainLogic.Repositories;
using DomainLogic.RequestModels;

using DomainFile = DomainLogic.Entities.File;
using DBFile = Implementations.EFModels.File;

namespace Implementations
{
    public class FileRepository : IFileRepository
    {
        private readonly YaDiskPlayerDbContext _context;
        private readonly IMapper _mapper;

        public FileRepository(YaDiskPlayerDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task Add(List<DomainFile> files)
        {
            var dbFiles = files.Select(f => _mapper.Map<DBFile>(f))
                            .ToList();

            await _context.AddRangeAsync(dbFiles);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAllExceptWithSynchronizationProcessId(Guid lastProcessId, string yandexUserId)
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Files\" WHERE \"YandexUserId\"={0} and \"SynchronizationProcessId\"!={1}", 
                yandexUserId, 
                lastProcessId);
        }

        public async Task<List<DomainFile>> GetFilesByParentFolderPath(GetFilesRequestModel request, string yandexUserId)
        {
            var skip = (request.Page - 1) * request.Take;
            var query = _context
                        .Files
                        .Where(f => f.YandexUserId == yandexUserId);
            if (request.Recursive)
                query = query
                    .Where(f => f.ParentFolderPath.StartsWith(request.ParentFolderPath))
                    .Where(f => f.Type == "file");
            else
                query = query.Where(f => f.ParentFolderPath == request.ParentFolderPath);

            if(!string.IsNullOrEmpty(request.Search))
                query = query.Where(f => f.Name.ToLower().Contains(request.Search.ToLower()));

            if (request.Recursive)
                query = query.OrderBy(f => f.Name);
            else 
                query = query
                        .OrderBy(f => f.Type == "file")
                        .ThenBy(f => f.Name);
            query = query
                    .Skip(skip)
                    .Take(request.Take)
                    .AsQueryable();

            var files = await query.ToListAsync();
            var result = files
                .Select(f => _mapper.Map<DomainFile>(f))
                .ToList();

            return result;
        }

        public async Task<List<DomainFile>> GetFilesByPaths(List<string> paths, string yandexUserId)
        {
            var dbFiles = await _context.Files
                                    .Where(f => paths.Contains(f.Path) && 
                                                yandexUserId == f.YandexUserId)
                                    .ToListAsync();

            var files = dbFiles.Select(f => _mapper.Map<DomainFile>(f))
                            .ToList();

            return files;
        }

        public async Task<List<DomainFile>> GetFilesByResourceId(List<string> resourceIds, string yandexUserId)
        {
            var dbFiles = await _context.Files
                                    .Where(f => resourceIds.Contains(f.YandexResourceId) &&
                                                yandexUserId == f.YandexUserId)
                                    .ToListAsync();

            var result = dbFiles.Select(f => _mapper.Map<DomainFile>(f))
                            .ToList();

            return result;
        }

        public async Task<List<string>> GetParentFolderPaths(Guid lastProcessId)
        {
            var paths = await _context.Files
                                .Where(f => f.SynchronizationProcessId == lastProcessId)
                                .Where(f => !string.IsNullOrEmpty(f.ParentFolderPath))
                                .Select(f => f.ParentFolderPath)
                                .Distinct()
                                .ToListAsync();

            return paths;
        }

        public async Task Update(List<DomainFile> existingFiles)
        {
            var paths = existingFiles.Select(f => f.Path).ToList();
            var yandexUserIds = existingFiles.Select(f => f.YandexUserId)
                                    .Distinct()
                                    .ToList();

            var dbFiles = _context.ChangeTracker
                                    .Entries<DBFile>()
                                    .Select(e => e.Entity)
                                    .Where(f => paths.Contains(f.Path) &&
                                                yandexUserIds.Contains(f.YandexUserId))
                                    .ToList();

            if(dbFiles.Count == 0)
                dbFiles = await _context.Files
                                    .Where(f => paths.Contains(f.Path) &&
                                                yandexUserIds.Contains(f.YandexUserId))
                                    .ToListAsync();

            dbFiles.ForEach(f =>
            {
                var updateData = existingFiles.Where(ef => ef.Path == f.Path).First();
                _mapper.Map(updateData, f);
            });

            await _context.SaveChangesAsync();
        }
    }
}

using DomainLogic;
using DomainLogic.Entities;
using Implementations.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DomainFile = DomainLogic.Entities.File;
using DBFile = Implementations.EFModels.File;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

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

        public async Task DeleteOld(Guid lastProcessId)
        {
            await _context.Database.ExecuteSqlRawAsync("DELETE FROM \"Files\" WHERE \"SynchronizationProcessId\"!={0}", lastProcessId);
        }

        public async Task<List<DomainFile>> GetFilesByPaths(List<DomainFile> files)
        {
            var paths = files.Select(f => f.Path).ToList();
            var yandexUserIds = files.Select(f => f.YandexUserId)
                                    .Distinct()
                                    .ToList();

            var dbFiles = await _context.Files
                                    .Where(f => paths.Contains(f.Path) && 
                                                yandexUserIds.Contains(f.YandexUserId))
                                    .ToListAsync();

            files = dbFiles.Select(f => _mapper.Map<DomainFile>(f))
                            .ToList();

            return files;
        }

        public async Task<List<DomainFile>> GetFilesByResourceId(IEnumerable<DomainFile> files)
        {
            var resourceIds = files.Select(f => f.ResourceId).ToList();
            var yandexUserIds = files.Select(f => f.YandexUserId)
                                    .Distinct()
                                    .ToList();

            var dbFiles = await _context.Files
                                    .Where(f => resourceIds.Contains(f.YandexResourceId) &&
                                                yandexUserIds.Contains(f.YandexUserId))
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

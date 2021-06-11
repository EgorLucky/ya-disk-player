using AutoMapper;
using DomainLogic;
using DomainLogic.ResponseModels;
using Implementations.EFModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DbIgnorePath = Implementations.EFModels.IgnorePath;
using DomainIgnorePath = DomainLogic.Entities.IgnorePath;

namespace Implementations
{
    public class IgnorePathRepository: IIgnorePathRepository
    {
        private readonly YaDiskPlayerDbContext _context;
        private readonly IMapper _mapper;

        public IgnorePathRepository(YaDiskPlayerDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<string>> GetAll(string yandexUserId)
        {
            var result = new List<string>();
            var offset = 0;

            var notAllLoaded = true;
            do
            {
                var ignorePaths = await _context
                    .IgnorePaths
                    .Where(ip => ip.YandexUserId == yandexUserId)
                    .OrderBy(ip => ip.Path)
                    .Skip(offset)
                    .Take(100)
                    .Select(ip => ip.Path)
                    .ToListAsync();

                result.AddRange(ignorePaths);

                offset += 100;

                notAllLoaded = ignorePaths.Count != 0;
            }
            while (notAllLoaded);

            return result;
        }

        public async Task Add(List<string> paths, string yandexUserId)
        {
            var ignorePaths = paths.Select(p => new DbIgnorePath
            {
                Id = Guid.NewGuid(),
                Path = p,
                YandexUserId = yandexUserId
            });

            await _context.AddRangeAsync(ignorePaths);
            await _context.SaveChangesAsync();
        }

        public async Task<List<DomainIgnorePath>> Get(int take, int page, List<string> search, string yandexUserId)
        {
            search = search
                    .Select(s => s.ToLower())
                    .ToList();

            var offset = (page - 1) * take;
            var ignorePaths = await _context
                                    .IgnorePaths
                                    .Where(ip => ip.YandexUserId == yandexUserId)
                                    .Where(ip => search.Any(s => ip.Path.ToLower().Contains(s)))
                                    .OrderBy(ip => ip.Path)
                                    .Skip(offset)
                                    .Take(take)
                                    .ToListAsync();

            var result = ignorePaths
                            .Select(i => _mapper.Map<DomainIgnorePath>(i))
                            .ToList();

            return result;
        }

        public async Task Delete(List<string> ignorePath, string yandexUserId)
        {
            ignorePath = ignorePath
                    .Select(s => s.ToLower())
                    .ToList();

            var toDelete = _context
                        .ChangeTracker
                        .Entries<DbIgnorePath>()
                        .Select(e => e.Entity)
                        .Where(ip => ip.YandexUserId == yandexUserId)
                        .Where(ip => ignorePath.Any(s => ip.Path.ToLower().Contains(s)))
                        .ToList();

            if(toDelete.Count != ignorePath.Count)
                toDelete = await _context
                                .IgnorePaths
                                .Where(ip => ip.YandexUserId == yandexUserId)
                                .Where(ip => ignorePath.Any(s => ip.Path.ToLower().Contains(s)))
                                .ToListAsync();

            _context.RemoveRange(toDelete);
            await _context.SaveChangesAsync();
        }
    }
}

using DomainLogic;
using DomainLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

using DomainSyncProcess = DomainLogic.Entities.SynchronizationProcess;
using DBSyncProcess = Implementations.EFModels.SynchronizationProcess;
using DbContext = Implementations.EFModels.YaDiskPlayerDbContext;


namespace Implementations
{
    public class SynchronizationRepository : ISynchronizationHistoryRepository
    {
        private readonly IMapper _mapper;
        private readonly DbContext _context;

        public SynchronizationRepository(IMapper mapper, DbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task Add(SynchronizationProcess synchProcess)
        {
            var syncProcDB = _mapper.Map<DBSyncProcess>(synchProcess);
            var userId = await _context.Users
                                        .Where(u => u.YandexId == synchProcess.YandexUserId)
                                        .Select(u => u.Id)
                                        .FirstOrDefaultAsync();

            syncProcDB.UserId = userId;

            await _context.AddAsync(syncProcDB);
            await _context.SaveChangesAsync();
        }

        public async Task<SynchronizationProcess> GetRunningProcess(string yandexId)
        {
            var userId = await _context.Users
                                        .Where(u => u.YandexId == yandexId)
                                        .Select(u => u.Id)
                                        .FirstOrDefaultAsync();

            var syncProcDB = await _context.SynchronizationProcesses
                                            .Where(s => s.UserId == userId)
                                            .FirstOrDefaultAsync();

            var syncProc = _mapper.Map<DomainSyncProcess>(syncProcDB);
            
            if(syncProc != null)
                syncProc = syncProc with 
                {
                    YandexUserId = yandexId
                };

            return syncProc;
        }


    }
}

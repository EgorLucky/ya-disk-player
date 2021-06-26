using DomainLogic;
using DomainLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DomainLogic.Repositories;

using DomainSyncProcess = DomainLogic.Entities.SynchronizationProcess;
using DbSyncProcess = Implementations.EFModels.SynchronizationProcess;
using DbContext = Implementations.EFModels.YaDiskPlayerDbContext;
using DomainCancellation = DomainLogic.SynchronizationProcessUserCancellation;
using DbCancellation = Implementations.EFModels.SynchronizationProcessUserCancellation;


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
            var syncProcDB = _mapper.Map<DbSyncProcess>(synchProcess);
            var userId = await _context.Users
                                        .Where(u => u.YandexId == synchProcess.YandexUserId)
                                        .Select(u => u.Id)
                                        .FirstOrDefaultAsync();

            syncProcDB.UserId = userId;

            await _context.AddAsync(syncProcDB);
            await _context.SaveChangesAsync();
        }

        public async Task<DomainSyncProcess> GetProcessById(Guid processId)
        {
            var syncProcDB = await _context.SynchronizationProcesses
                                            .Where(s => s.Id == processId)
                                            .FirstOrDefaultAsync();

            var yandexId = "";
            if (syncProcDB != null)
            {
                yandexId = await _context.Users
                                        .Where(u => u.Id == syncProcDB.UserId)
                                        .Select(u => u.YandexId)
                                        .FirstOrDefaultAsync();
            }

            var syncProc = _mapper.Map<DomainSyncProcess>(syncProcDB);

            if (syncProcDB != null)
                syncProc = syncProc with
                {
                    YandexUserId = yandexId
                };

            return syncProc;
        }

        public async Task<SynchronizationProcess> GetRunningProcess(string yandexUserId)
        {
            var userId = await _context.Users
                                        .Where(u => u.YandexId == yandexUserId)
                                        .Select(u => u.Id)
                                        .FirstOrDefaultAsync();

            var syncProcDB = await _context.SynchronizationProcesses
                                            .Where(s => s.UserId == userId)
                                            .Where(s => s.FinishedDateTime == null)
                                            .Where(s => s.State != SynchronizationProcessState.Finished)
                                            .FirstOrDefaultAsync();

            var syncProc = _mapper.Map<DomainSyncProcess>(syncProcDB);
            
            if(syncProc != null)
                syncProc = syncProc with 
                {
                    YandexUserId = yandexUserId
                };

            return syncProc;
        }

        public async Task<bool> IsCancelledByUser(Guid processId)
        {
            var userCancellation = await _context.SynchronizationProcessUserCancellations
                                                .Where(s => s.SynchronizationProcessId == processId)
                                                .FirstOrDefaultAsync();

            return userCancellation != null;
        }

        public async Task Update(DomainSyncProcess process)
        {
            var processDB = _context.ChangeTracker
                                .Entries<DbSyncProcess>()
                                .Where(u => u.Entity.Id == process.Id)
                                .Select(u => u.Entity)
                                .FirstOrDefault();
            if (processDB == null)
                processDB = await _context.SynchronizationProcesses
                                .Where(u => u.Id == process.Id)
                                .FirstOrDefaultAsync();

            if (processDB == null)
                return;
            _mapper.Map(process, processDB);

            await _context.SaveChangesAsync();
        }

        public async Task AddCancellation(SynchronizationProcessUserCancellation processCancellation)
        {
            var cancellationDB = _mapper.Map<DbCancellation>(processCancellation);
            cancellationDB.Id = Guid.NewGuid();
            await _context.AddAsync(cancellationDB);
            await _context.SaveChangesAsync();
        }

    }
}

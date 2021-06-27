﻿using DomainLogic.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainLogic.Repositories
{
    public interface ISynchronizationHistoryRepository
    {
        Task<SynchronizationProcess> GetRunningProcess(string yandexUserId);
        Task Add(SynchronizationProcess synchProcess);
        Task<SynchronizationProcess> GetProcessById(Guid processId);
        Task Update(SynchronizationProcess process);
        Task<bool> IsCancelledByUser(Guid processId);
        Task AddCancellation(SynchronizationProcessUserCancellation processCancellation);
        Task<List<SynchronizationProcess>> GetWhereLastUpdatedLessThan(DateTimeOffset minLastUpdated, int take);
        Task Update(List<SynchronizationProcess> processes);
    }
}
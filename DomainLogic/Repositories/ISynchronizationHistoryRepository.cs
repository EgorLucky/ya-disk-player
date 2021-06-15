using DomainLogic.Entities;
using System;
using System.Threading.Tasks;

namespace DomainLogic.Repositories
{
    public interface ISynchronizationHistoryRepository
    {
        Task<SynchronizationProcess> GetRunningProcess(string yandexId);
        Task Add(SynchronizationProcess synchProcess);
        Task<SynchronizationProcess> GetProcessById(Guid processId);
        Task Update(SynchronizationProcess process);
        Task<bool> IsCancelledByUser(Guid processId);
    }
}
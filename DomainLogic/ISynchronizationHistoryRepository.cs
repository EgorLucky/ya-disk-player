using DomainLogic.Entities;
using System;
using System.Threading.Tasks;

namespace DomainLogic
{
    public interface ISynchronizationHistoryRepository
    {
        Task<SynchronizationProcess> GetRunningProcess(string yandexId);
        Task Add(SynchronizationProcess synchProcess);
        Task<SynchronizationProcess> GetProcessById(Guid processId);
    }
}
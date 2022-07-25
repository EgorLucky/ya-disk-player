using DomainLogic.Entities;
using DomainLogic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Services
{
    public class UnclosedSynchronizationProcessProcessor
    {
        private readonly ISynchronizationHistoryRepository _repository;

        public UnclosedSynchronizationProcessProcessor(ISynchronizationHistoryRepository repository)
        {
            _repository = repository;
        }

        public async Task Process()
        {
            var minLastUpdated = DateTimeOffset.UtcNow.AddMinutes(-5);
            var take = 10;

            var processes = await  _repository.GetNotFinishedAndLastUpdatedLessThan(minLastUpdated, take);

            processes = processes.Select(p => p with 
            {
                FinishedDateTime = DateTimeOffset.UtcNow,
                State = SynchronizationProcessState.CanceledBySystem,
                LastUpdateDateTime = DateTimeOffset.UtcNow
            })
            .ToList();

            await _repository.Update(processes);
        }
    }
}

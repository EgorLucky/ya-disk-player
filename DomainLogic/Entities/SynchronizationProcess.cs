using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Entities
{
    public record SynchronizationProcess(
        Guid? Id = null,
        DateTimeOffset? CreateDateTime = null,
        DateTimeOffset? StartDateTime = null,
        DateTimeOffset? FinishedDateTime = null,
        int Offset = 0,
        string LastFileId = null,
        SynchronizationProcessState State = SynchronizationProcessState.Created,
        string YandexUserId = null
    );

    public enum SynchronizationProcessState
    {
        Created,
        Runnig,
        Paused,
        CanceledByUser,
        CanceledBySystem,
        Finished
    }
}

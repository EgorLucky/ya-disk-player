﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLogic.Entities
{
    public record SynchronizationProcess(
        Guid Id,
        DateTimeOffset? CreateDateTime = null,
        DateTimeOffset? StartDateTime = null,
        DateTimeOffset? FinishedDateTime = null,
        DateTimeOffset? LastUpdateDateTime = null,
        int Offset = 0,
        string LastFileId = null,
        SynchronizationProcessState State = SynchronizationProcessState.Created,
        string YandexUserId = null
    );

    public enum SynchronizationProcessState
    {
        Created,
        Running,
        Paused,
        CanceledByUser,
        CanceledBySystem,
        Finished,
        TokenExpired
    }
}

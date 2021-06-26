using System;

namespace DomainLogic
{
    public record SynchronizationProcessUserCancellation(
        Guid SynchronizationProcessId, 
        DateTimeOffset CreateDateTime
    );
}
using DomainLogic;
using System;
using System.Threading.Tasks;

namespace Implementations.Mq
{
    public class SynchronizationMessageService : ISynchronizationMessageService
    {
        public Task Send(Guid value, string accessToken, string refreshToken)
        {
            throw new NotImplementedException();
        }
    }
}

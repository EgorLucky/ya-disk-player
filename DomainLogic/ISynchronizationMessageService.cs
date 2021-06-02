using System;
using System.Threading.Tasks;

namespace DomainLogic
{
    public interface ISynchronizationMessageService
    {
        Task Send(Guid value, string accessToken, string refreshToken);
    }
}
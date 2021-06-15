using System;
using System.Threading.Tasks;

namespace DomainLogic.Services
{
    public interface ISynchronizationMessageService
    {
        Task Send(Guid value, string accessToken, string refreshToken);
    }
}
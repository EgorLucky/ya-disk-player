using DomainLogic;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Implementations.Mq
{
    public class SynchronizationMessageService : ISynchronizationMessageService
    {
        readonly IPublishEndpoint _publishEndpoint;

        public SynchronizationMessageService(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Send(Guid value, string accessToken, string refreshToken)
        {
            var message = new YandexDiskPlayerSynchronization(
                Id: value,
                AccessToken : accessToken,
                RefreshToken : refreshToken
            );

            await _publishEndpoint.Publish(message);
        }
    }
}

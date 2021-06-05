using DomainLogic;
using Implementations.Mq;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WorkerService
{
    public class YandexDiskPlayerSynchronizationConsumer :
        IConsumer<YandexDiskPlayerSynchronization>
    {
        readonly ILogger<YandexDiskPlayerSynchronizationConsumer> _logger;
        readonly SynchronizationBackgroundService _syncService;

        public YandexDiskPlayerSynchronizationConsumer(
            ILogger<YandexDiskPlayerSynchronizationConsumer> logger,
            IServiceProvider provider
            )
        {
            _logger = logger;
            var scope = provider.CreateScope();
            _syncService = scope.ServiceProvider.GetRequiredService<SynchronizationBackgroundService>();
        }

        public Task Consume(ConsumeContext<YandexDiskPlayerSynchronization> context)
        {
            _logger.LogInformation("Received Text: {Text}", context.Message.Id);

            var message = context.Message;
            _syncService.Synchronize(message.Id, message.AccessToken, message.RefreshToken);

            return Task.CompletedTask;
        }
    }
}
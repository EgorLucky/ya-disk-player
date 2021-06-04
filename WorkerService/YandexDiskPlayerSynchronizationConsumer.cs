using DomainLogic;
using Implementations.Mq;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace WorkerService
{
    public class YandexDiskPlayerSynchronizationConsumer :
        IConsumer<YandexDiskPlayerSynchronization>
    {
        readonly ILogger<YandexDiskPlayerSynchronizationConsumer> _logger;
        readonly SynchronizationService _syncService;

        public YandexDiskPlayerSynchronizationConsumer(
            ILogger<YandexDiskPlayerSynchronizationConsumer> logger,
            SynchronizationService syncService)
        {
            _logger = logger;
            _syncService = syncService;
        }

        public Task Consume(ConsumeContext<YandexDiskPlayerSynchronization> context)
        {
            _logger.LogInformation("Received Text: {Text}", context.Message.Id);

            return Task.CompletedTask;
        }
    }
}
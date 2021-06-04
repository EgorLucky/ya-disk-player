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

        public YandexDiskPlayerSynchronizationConsumer(ILogger<YandexDiskPlayerSynchronizationConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<YandexDiskPlayerSynchronization> context)
        {
            _logger.LogInformation("Received Text: {Text}", context.Message.Id);

            return Task.CompletedTask;
        }
    }
}
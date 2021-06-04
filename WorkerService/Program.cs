using DomainLogic;
using Implementations;
using Implementations.EFModels;
using Implementations.Mq;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        var rabbitMqConfigJson = Environment.GetEnvironmentVariable("yadplayerRabbitMqConfig");
                        var RabbitMqConfig = JsonSerializer.Deserialize<RabbitMqConfig>(rabbitMqConfigJson);

                        x.AddConsumer<YandexDiskPlayerSynchronizationConsumer>();
                        x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config => 
                        {
                            config.Host(RabbitMqConfig.Host, RabbitMqConfig.VirtualHost, h =>
                            {
                                h.Username(RabbitMqConfig.Username);
                                h.Password(RabbitMqConfig.Password);

                            });

                            config.ReceiveEndpoint("YandexDiskPlayerSynchronization", ep =>
                            {
                                ep.PrefetchCount = 16;
                                //ep.UseMessageRetry(r => r.Interval(2, 100));
                                ep.ConfigureConsumer<YandexDiskPlayerSynchronizationConsumer>(provider);
                            });
                        }));
                    });
                    services.AddMassTransitHostedService(true);

                    services.AddScoped<SynchronizationService>()
                    .AddScoped<ISynchronizationHistoryRepository, SynchronizationRepository>()
                    .AddScoped<ISynchronizationMessageService, SynchronizationMessageService>()
                    .AddDbContext<YaDiskPlayerDbContext>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("yadplayerConnectionString")))
                    .AddAutoMapper(typeof(MappingProfile));

                    services.AddHostedService<Worker>();
                });
    }
}

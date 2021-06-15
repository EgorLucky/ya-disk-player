using DomainLogic;
using DomainLogic.Repositories;
using DomainLogic.Services;
using Implementations;
using Implementations.EFModels;
using Implementations.Mq;
using Implementations.YandexDiskAPI;
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
                    var yandexOauthJson = Environment.GetEnvironmentVariable("yaDiskPlayerApp");
                    var yandexAppOauthConfiguration = JsonSerializer.Deserialize<YandexAppOauthConfiguration>(yandexOauthJson);

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

                    services
                    .AddSingleton(yandexAppOauthConfiguration)
                    .AddScoped<SynchronizationBackgroundService>()
                    .AddScoped<ISynchronizationHistoryRepository, SynchronizationRepository>()
                    .AddScoped<FileSynchronizationService>()
                    .AddScoped<IFileRepository, FileRepository>()
                    .AddScoped<IErrorRepoistory, ErrorRepository>()
                    .AddScoped<IIgnorePathRepository, IgnorePathRepository>()
                    .AddDbContext<YaDiskPlayerDbContext>(options => options.UseNpgsql(Environment.GetEnvironmentVariable("yadplayerConnectionString")))
                    .AddAutoMapper(typeof(Implementations.MappingProfile), typeof(DomainLogic.MappingProfile))
                    .AddHttpClient<IYandexDiskApi, YandexDiskClient>();

                    services.AddHostedService<Worker>();
                });
    }
}

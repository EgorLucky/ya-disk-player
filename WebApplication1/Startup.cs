using DomainLogic;
using DomainLogic.Repositories;
using DomainLogic.Services;
using Implementations;
using Implementations.EFModels;
using Implementations.Mq;
using Implementations.YandexDiskAPI;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication1.AuthorizationPolicies.Admin;
using WebApplication1.AuthorizationPolicies.User;
using WebApplication1.YandexAuthentication;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var yandexOauthJson = Configuration.GetValue<string>("yaDiskPlayerApp");
            YandexAppOauthConfiguration = JsonSerializer.Deserialize<YandexAppOauthConfiguration>(yandexOauthJson);

            var rabbitMqConfigJson = Configuration.GetValue<string>("yadplayerRabbitMqConfig");
            RabbitMqConfig = JsonSerializer.Deserialize<RabbitMqConfig>(rabbitMqConfigJson);
        }

        public IConfiguration Configuration { get; }

        public YandexAppOauthConfiguration YandexAppOauthConfiguration  { get; } 

        public RabbitMqConfig RabbitMqConfig { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(YandexAppOauthConfiguration)
                .AddScoped<UserService>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<SynchronizationService>()
                .AddScoped<ISynchronizationHistoryRepository, SynchronizationRepository>()
                .AddScoped<ISynchronizationMessageService, SynchronizationMessageService>()
                .AddScoped<IIgnorePathRepository, IgnorePathRepository>()
                .AddScoped<IgnorePathService>()
                .AddScoped<FileService>()
                .AddScoped<IFileRepository, FileRepository>()
                .AddDbContext<YaDiskPlayerDbContext>(options => options.UseNpgsql(Configuration.GetValue<string>("yadplayerConnectionString"), options => options.MigrationsAssembly(typeof(Startup).Assembly.FullName)))
                .AddHttpClient<IYandexDiskApi, YandexDiskClient>();

            services.AddMassTransit(x =>
            {
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    config.Host(RabbitMqConfig.Host, RabbitMqConfig.VirtualHost, h =>
                    {
                        h.Username(RabbitMqConfig.Username);
                        h.Password(RabbitMqConfig.Password);
                        
                    });
                }));
            });
            //services.AddMassTransitHostedService();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplication1", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(YandexAppOauthConfiguration.AuthorizationEndpoint, UriKind.Absolute)
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, 
                            Id = "oauth2" }
                        },
                        new string[] { }
                    }
                });
            });
            services.AddHttpClient();

            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy("Admin", policy =>
                        policy.Requirements.Add(new AdminRightsRequirement(true)));

                    options.AddPolicy("RegistredUser", policy =>
                        policy.Requirements.Add(new RegistredUserRequirement(true)));
                })
                .AddAuthentication("Bearer")
                .AddYandexScheme("Bearer");

            services.AddScoped<IAuthorizationHandler, AdminRightsHandler>()
                .AddScoped<IAuthorizationHandler, RegistredUserHandler>();
            services.AddAutoMapper(typeof(Implementations.MappingProfile));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 v1");
                    c.OAuthClientId(YandexAppOauthConfiguration.ClientId);
                    c.OAuthClientSecret(YandexAppOauthConfiguration.ClientSecretId);
                    c.OAuthAppName(YandexAppOauthConfiguration.AppName);
                    c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
                });
            }

            using var scope = app.ApplicationServices.CreateScope();
            var dBcontext = scope.ServiceProvider.GetService<YaDiskPlayerDbContext>();
            dBcontext.Database.Migrate();
            dBcontext.Dispose();

            app.UseCors(builder => builder
                          .AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .WithExposedHeaders());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

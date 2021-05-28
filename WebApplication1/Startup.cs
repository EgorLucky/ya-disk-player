using DomainLogic;
using Implementations;
using Implementations.EFModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
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
using WebApplication1.YandexAuthentication;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var yandexOauthJson = Environment.GetEnvironmentVariable("yaDiskPlayerApp");
            YandexAppOauthConfiguration = JsonSerializer.Deserialize<YandexAppOauthConfiguration>(yandexOauthJson);
        }

        public IConfiguration Configuration { get; }

        public YandexAppOauthConfiguration YandexAppOauthConfiguration  { get; } 

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<UserService>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddDbContext<YaDiskPlayerDbContext>(options => options.UseInMemoryDatabase("db"));

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

            services.AddAuthorization()
                .AddAuthentication("Bearer")
                .AddYandexScheme("Bearer");
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

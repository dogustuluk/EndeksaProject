using Endeksa.BackgroundServices;
using Endeksa.Services.Abstract;
using Endeksa.Services.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using StackExchange.Redis;
using System;

namespace Endeksa.Services.Extension
{
    public static class ServiceCollectionExtensions
    {
        
        public static IConfiguration Configuration { get; }

       
        public static IServiceCollection LoadMyServices(this IServiceCollection servicesCollection)
        {
            #region old
            //servicesCollection.AddSingleton<RedisService>();
            //servicesCollection.AddSingleton(serviceProvider => new ConnectionFactory { Uri = new Uri(Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true });
            //servicesCollection.AddSingleton<RabbitMQClientService>();
            //servicesCollection.AddSingleton<RabbitMQPublisher>();
            //serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IPDetectorBackgroundService>();
            // services.AddSingleton<IRedisService,RedisService>();
            // services.AddStackExchangeRedisCache<RedisService>();
            //servicesCollection.AddSingleton<IpDetectorService>();
            #endregion

            servicesCollection.AddHostedService<IPDetectorBackgroundService>();           
            servicesCollection.AddSingleton<IDatabase>(sp =>
            {
                var redisService = sp.GetRequiredService<IRedisService>();
                return redisService.GetDb(0);
            });
            servicesCollection.AddSingleton<IRedisService, RedisService>();
            servicesCollection.AddSingleton<IRabbitMQClientService, RabbitMQClientService>();
            servicesCollection.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();
            servicesCollection.AddSingleton<IIpDetectorService, IpDetectorService>();

            return servicesCollection;
        }
    }
}

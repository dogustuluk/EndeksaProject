using Endeksa.BackgroundServices;
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
            servicesCollection.AddSingleton<RedisService>();

            //servicesCollection.AddSingleton(serviceProvider => new ConnectionFactory { Uri = new Uri(Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true });

            servicesCollection.AddSingleton<RabbitMQClientService>();
            servicesCollection.AddSingleton<RabbitMQPublisher>();
            servicesCollection.AddHostedService<IPDetectorBackgroundService>();
            // services.AddSingleton<IRedisService,RedisService>();
            // services.AddStackExchangeRedisCache<RedisService>();
            servicesCollection.AddSingleton<IDatabase>(sp =>
            {
                var redisService = sp.GetRequiredService<RedisService>();
                return redisService.GetDb(0);
            });

            return servicesCollection;
        }
    }
}

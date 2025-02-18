﻿using Endeksa.Models;
using Endeksa.Services.Abstract;
using Endeksa.Services.Concrete;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Endeksa.BackgroundServices
{
    public class IPDetectorBackgroundService : BackgroundService
    {
        private readonly IRabbitMQClientService _rabbitMQClientService;
        private readonly ILogger<IPDetectorBackgroundService> _logger;
        private IModel _channel;
        private readonly IRedisService _redisService;
        public IPDetectorBackgroundService(ILogger<IPDetectorBackgroundService> logger, IRabbitMQClientService rabbitMQClientService, IRedisService redisService, IServiceScopeFactory factory)
        {
            _logger = logger;
            _rabbitMQClientService = rabbitMQClientService;
            /*addscoped ->
             * Eğer servisler scope olarak eklendiyse BackgroundService'ten miras alan sınıf için IServiceScopeFactory'yi constructor içerisine eklemeliyiz. İlgili sınıfta servis eklemesi yapacak isek alttaki gibi ekleme yapılarak scoped olarak eklenmiş olan servislerin yaşam döngüsünü burada kırıp, singleton olarak çalışmasını sağlar.
           // _rabbitMQClientService = factory.CreateScope().ServiceProvider.GetRequiredService<IRabbitMQClientService>();
           // _redisService = factory.CreateScope().ServiceProvider.GetRequiredService<IRedisService>();
           */
            _redisService = redisService;

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientService.Connect();
            _channel.BasicQos(0, 1, false);
           
            return base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(RabbitMQClientService.QueueName, false, consumer);
            consumer.Received += Consumer_Received;

            return Task.CompletedTask;
        }                                                       

        private Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            //Task.Delay(5000).Wait();
            try
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<UserIPDetectedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));
                var ip = data.IP;
                var city = data.City;
                _channel.BasicAck(@event.DeliveryTag, false);

                _logger.LogInformation($"işlem tamamlandı. IP:{ip} - City:{city} - thread:{Thread.CurrentThread.ManagedThreadId}");
                //redis
              //  var expiryTime = DateTimeOffset.Now.AddSeconds(30);
                _redisService.SetData<string>(ip,city,ip);
                _logger.LogInformation("Redise ip adresi kaydedildi.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Hata:{ex}");
            }
            
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}

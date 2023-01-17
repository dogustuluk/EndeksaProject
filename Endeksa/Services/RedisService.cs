using Endeksa.BackgroundServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Text.Json;

namespace Endeksa.Services
{
    public class RedisService
    {
        private readonly string _redisHost;
        private readonly string _redisPort;
        private ConnectionMultiplexer _redis;
        public IDatabase db { get; set; }
        private readonly ILogger<RedisService> _logger;

        public RedisService(IConfiguration configuration, ILogger<RedisService> logger)
        {
            _redisHost = configuration["Redis:Host"];
            _redisHost = configuration["Redis:Port"];
            _logger = logger;
        }

        public void Connect()
        {
            var configString = $"{_redisHost}:{_redisPort}";

            _redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
            db = _redis.GetDatabase();
            _logger.LogInformation("redis ile bağlantı kuruldu.");
        }
        public IDatabase GetDb(int db)
        {

            return _redis.GetDatabase(db);
        }

        public T GetIP<T>(string ip)
        {
            var value = db.StringGet(ip);
            throw new NotImplementedException();
        }

        public bool SetData<T>(string key, T value)
        {
           // var expirtyTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return db.StringSet(key, JsonSerializer.Serialize(value));
        }
    }
}

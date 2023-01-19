using Endeksa.BackgroundServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading;

namespace Endeksa.Services
{
    public class RedisService
    {
        public const string IpKey = "";
        public string hashKey { get; set; } = "IpNCity";
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

           // _redis = ConnectionMultiplexer.Connect("localhost:6379");
            _redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
            db = _redis.GetDatabase();
            _logger.LogInformation("redis ile bağlantı kuruldu. thread:"+ Thread.CurrentThread.ManagedThreadId);
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
        public string GetIp(string ip)
        {
            var value = db.StringGet(ip);
            return value;
        }

        public bool SetData<T>(string key,string city ,T value)
        {
            key = JsonSerializer.Serialize(value);
            // var expirtyTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return db.HashSet(key,JsonSerializer.Serialize(value),city);
            //return db.StringSet(key, JsonSerializer.Serialize(value));
        }
        public bool isKeyExist(string hashkey)
        {
            //var data = db.StringGet(value);
            
           // var data = db.HashGet(hashkey, value);
            
            //if (data.HasValue.ToString() == JsonSerializer.Serialize(value))
            //{
            //    _logger.LogInformation($"data cache içerisinde bulunmaktadır:{value} - thread:{Thread.CurrentThread.ManagedThreadId}");
            //    return true;
            //}
            //if (db.HashExists(hashkey,value))
            //{
            //    _logger.LogInformation($"data cache içerisinde bulunmaktadır:{value} - thread:{Thread.CurrentThread.ManagedThreadId}");
            //    return true;
            //}
            if (db.KeyExists(hashKey))
            {
                _logger.LogInformation($"data cache içerisinde bulunmaktadır:{hashkey} - thread:{Thread.CurrentThread.ManagedThreadId}");
                return true;
            }
            return false;
        }
    }
}

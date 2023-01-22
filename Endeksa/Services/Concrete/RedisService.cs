using Endeksa.BackgroundServices;
using Endeksa.Services.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Endeksa.Services.Concrete
{
    public class RedisService : IRedisService
    {
        public const string IpKey = "";
        public string hashKey { get; set; } = "IpNCity";
        private readonly string _redisHost;
        private readonly string _redisPort;
        private ConnectionMultiplexer _redis;
        public IDatabase db { get; set; }
        private readonly ILogger<IRedisService> _logger;

        public RedisService(IConfiguration configuration, ILogger<IRedisService> logger)
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
            _logger.LogInformation("redis ile bağlantı kuruldu. thread:" + Thread.CurrentThread.ManagedThreadId);
        }
        public IDatabase GetDb(int db)
        {
            return _redis.GetDatabase(db);
        }

        public bool SetData<T>(string key, string city, T value)
        {
            //key = JsonSerializer.Serialize(value);
            // var expirtyTime = expirationTime.DateTime.Subtract(DateTime.Now);

            return db.HashSet(hashKey, System.Text.Json.JsonSerializer.Serialize(value), city);
        }
        /// <summary>
        /// Kullanıcının ip'sinden şehir bilgisini redisten çekip, byte dizisine dönüştürür ve geri döner.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetValue(string value)
        {
            //city name
            var data = db.HashGet(hashKey, System.Text.Json.JsonSerializer.Serialize(value));
            string datas = JsonConvert.SerializeObject(data);
            //city name by byte
            byte[] values = Encoding.UTF8.GetBytes(datas);
            _logger.LogInformation($"ip adresi cache'te bulundu. IP:{value} - City:{datas}");
            // Console.WriteLine($"ip adresi cache'te bulundu. IP:{value} - City:{datas}");
            return Utf8Json.JsonSerializer.Deserialize<string>(values);
        }

    }
}

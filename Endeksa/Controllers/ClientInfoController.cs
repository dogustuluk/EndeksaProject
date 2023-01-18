using Endeksa.BackgroundServices;
using Endeksa.Models;
using Endeksa.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Endeksa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientInfoController : ControllerBase
    {
        private readonly RabbitMQClientService _rabbitmqClientService;
        private readonly RabbitMQPublisher _rabbitMQPublisher;
        private readonly RedisService _redisService;
        //private readonly IDatabase _cache;
        private readonly ILogger<ClientInfoController> _logger;
        public ClientInfoController(RabbitMQClientService rabbitmqClientService, RabbitMQPublisher rabbitMQPublisher, RedisService redisService, ILogger<ClientInfoController> logger)
        {
            _rabbitmqClientService = rabbitmqClientService;
            _rabbitMQPublisher = rabbitMQPublisher;
            // _cache = _redisService.GetDb(1);
            _redisService = redisService;
           _logger = logger;
        }

        [HttpGet]
        /*swagger error
        //[Authorize]
        //[SwaggerOperation(
        //    Summary = "Get User IP and location",
        //    Description = "Get User and location by accessing an external API",
        //    OperationId = "GetUserAndLocation",
        //    Tags = new[] {"IP"}
        //    )]
        */
        public ActionResult<UserLocation> GetIP()
        {
            // _rabbitmqClientService.Connect();
            
            //kullanıcının ip adresi alınır.
            string ip = GetUserIP();
            //IP adresinin kullanılarak istekte bulunan kullanıcının konum bilgileri alınır.
            string location = GetLocation(ip);
            //if (await _cache.KeyExistsAsync(RedisService.IpKey))
            //{
            //    _rabbitMQPublisher.Publish(new UserIPDetectedEvent() { IP = ip, City = location });
            //}
            // _logger.LogInformation("ip adresi cachte bulundu.");
            

            //metot içerisinde önce get ile redisten ip adresini al ondan sonra gönderilen ip ile gelen ip adresini karşılaştır.

            if (!_redisService.isKeyExist(RedisService.IpKey))
            {
                _rabbitMQPublisher.Publish(new UserIPDetectedEvent() { IP = ip, City = location });
            }
            return Ok(new UserLocation { IP = ip, Location = location });
        }
        /// <summary>
        /// Gelen isteğin IP adresini api'ye bağlanarak bulan method. Json formatında bir değer alır ve geriye string türünde değer döndürür. Gelen değer GetIP metodunda çalıştırılır.
        /// </summary>
        /// <returns></returns>
        private string GetUserIP()
        {
            string ip = "";
            try
            {
                //IP adresi için api çağrısı yapılır.
                string apiUrl = "http://api.ipstack.com/check?access_key=4514fa9e4e267c19febbb4a54b901e43\r\n";
                var json = new WebClient().DownloadString( apiUrl );
                var data = JObject.Parse( json );

                //IP adresi alınır
                ip = data["ip"].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine( ex.Message );
            }

            return ip;
        }
        /// <summary>
        /// Gelen isteğin sahip olduğu ip adresine bağlı olarak geriye ülke ve şehir bilgisini döndürür. Json formatında bir değer alır ve geriye string türünde değer döndürür. Gelen değer GetIP metodunda çalıştırılır.
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private string GetLocation(string ip)
        {
            string location = "";
            try
            {
                //Konum bilgilerinin ip adresi kullanılarak gelmesi için api çağrısı yapılır.
                string apiUrl = $"http://api.ipstack.com/{ip}?access_key=4514fa9e4e267c19febbb4a54b901e43\r\n";
                var json = new WebClient().DownloadString( apiUrl );
                var data = JObject.Parse( json );

                //Şehir ve ülke bilgileri alınır
                string city = data["city"].ToString();
                string country = data["country_name"].ToString();

                location = $"{city}, {country}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return location;
        }
    }
}

using Newtonsoft.Json.Linq;
using System.Net;
using System;
using Endeksa.Services.Abstract;
using System.Threading.Tasks;

namespace Endeksa.Services.Concrete
{
    public class IpDetectorService : IIpDetectorService
    {
        private const string API_URL = "http://api.ipstack.com/";
        private const string API_KEY = "7ce20d99b72388966043b533e6e738c6";
        /// <summary>
        /// Gelen isteğin IP adresini api'ye bağlanarak bulan method. Json formatında bir değer alır ve geriye string türünde değer döndürür. Gelen değer GetIP metodunda çalıştırılır.
        /// </summary>
        /// <returns></returns>
        public string GetUserIP()
        {
            string ip = "";
            try
            {
                //IP adresi için api çağrısı yapılır.
                string apiUrl = $"{API_URL}check?access_key={API_KEY}";
                var json = new WebClient().DownloadString(apiUrl);
                var data = JObject.Parse(json);

                //IP adresi alınır
                ip = data["ip"].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return ip;
        }
        /// <summary>
        /// Gelen isteğin sahip olduğu ip adresine bağlı olarak geriye ülke ve şehir bilgisini döndürür. Json formatında bir değer alır ve geriye string türünde değer döndürür. Gelen değer GetIP metodunda çalıştırılır.
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public string GetLocation(string ip)
        {
            string location = "";
            try
            {
                //Konum bilgilerinin ip adresi kullanılarak gelmesi için api çağrısı yapılır.
                string apiUrl = $"{API_URL}{ip}?access_key={API_KEY}";
                var json = new WebClient().DownloadString(apiUrl);
                var data = JObject.Parse(json);

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

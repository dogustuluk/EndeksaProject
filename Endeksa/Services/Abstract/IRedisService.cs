using StackExchange.Redis;

namespace Endeksa.Services.Abstract
{
    public interface IRedisService
    {
        public void Connect();
        public IDatabase GetDb(int db);
        public bool SetData<T>(string key, string city, T value);
        public string GetValue(string value);
    }
}

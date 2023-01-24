using StackExchange.Redis;

namespace Endeksa.Services.Abstract
{
    public interface IRedisService
    {
        void Connect();
        IDatabase GetDb(int db);
        bool SetData<T>(string key, string city, T value);
        string GetValue(string value);
    }
}

using StackExchange.Redis;
using System;

namespace Endeksa.Services
{
    public interface IRedisService
    {
        void Connect();
        T GetIP<T>(string ip);
        bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
    }
}

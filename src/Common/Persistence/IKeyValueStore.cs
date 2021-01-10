using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Common.Persistence
{
    public interface IKeyValueStore
    {
        Task<T> GetValue<T>(string key);
        Task<object> GetValue(string key);
        Task SetValue(string key, object value);
    }
}

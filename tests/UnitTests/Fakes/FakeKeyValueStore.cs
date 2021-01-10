using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTests.Fakes
{
    class FakeKeyValueStore : Common.Persistence.IKeyValueStore
    {
        private readonly Dictionary<string, object> _store = new Dictionary<string, object>();

        public Task<T> GetValue<T>(string key)
        {
            var obj = (T)_store[key];
            return Task.FromResult(obj);
        }

        public Task<object> GetValue(string key)
        {
            var obj = _store[key];
            return Task.FromResult(obj);
        }

        public Task SetValue(string key, object value)
        {
            _store[key] = value;
            return Task.CompletedTask;
        }
    }
}

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace Common.Persistence
{
    public class RedisKeyValueStore : IKeyValueStore, IDisposable
    {
        private readonly ILogger<RedisKeyValueStore> _logger;
        private readonly RedisOptions _options;
        private ConnectionMultiplexer _connection;
        private IDatabase _db;
        public RedisKeyValueStore(
            ILogger<RedisKeyValueStore> logger,
            IOptions<RedisOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public void Dispose()
        {
            CloseConnection();
        }

        public async Task<T> GetValue<T>(string key)
        {
            OpenConnection();

            try
            {
                var valueString = await _db.StringGetAsync(key);
                return JsonConvert.DeserializeObject<T>(valueString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<object> GetValue(string key)
        {
            OpenConnection();

            try
            {
                var jsonValue = await _db.StringGetAsync(key);
                return JsonConvert.DeserializeObject(jsonValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SetValue(string key, object value)
        {
            OpenConnection();

            try
            {
                var jsonValue = JsonConvert.SerializeObject(value);
                await _db.StringSetAsync(key, jsonValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void OpenConnection()
        {
            if (_connection == null)
            {
                _connection = ConnectionMultiplexer.Connect(_options.Host);
                _db = _connection.GetDatabase();
            }
        }

        public void CloseConnection()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
        }
    }
}

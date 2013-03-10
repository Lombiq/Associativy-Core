using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Caching.Services;

namespace Associativy.Tests.Stubs
{
    public class StubCacheService : ICacheService
    {
        private readonly Dictionary<string, object> _cache;


        public StubCacheService()
        {
            _cache = new Dictionary<string, object>();
        }


        public object Get(string key)
        {
            if (!_cache.ContainsKey(key)) return null;
            return _cache[key];
        }

        public void Put(string key, object value)
        {
            _cache[key] = value;
        }

        public void Put(string key, object value, TimeSpan validFor)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }
}

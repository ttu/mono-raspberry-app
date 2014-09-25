using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisDataStorage
{
    // TODO: Check how to use ServiceStack.Redis correctly

    public class RedisStorageMono
    {
        private PooledRedisClientManager _manager;

        public RedisStorageMono()
        {
            _manager = new PooledRedisClientManager(20, 60, "127.0.0.1:6379");
        }

        public string Get(string key)
        {
            using (var db = _manager.GetClient())
            {
                return db.Get<string>(key);
            }
        }

        public bool Set(string key, string value)
        {
            using (var db = _manager.GetClient())
            {
                return db.Set<string>(key, value);
            }
        }

        public bool Get<T>(string key, out T value)
        {
            using (var db = _manager.GetClient())
            {
                value = db.Get<T>(key);
                return true;
            }
        }

        public void Set<T>(string key, T value)
        {
            using (var db = _manager.GetClient())
            {
                db.Set<T>(key, value);
            }
        }

        public void Remove(string key)
        {
            using (var db = _manager.GetClient())
            {
                db.Remove(key);
            }
        }
    }
}
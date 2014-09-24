using Newtonsoft.Json;
using StackExchange.Redis;

namespace RedisDataStorage
{
    public class RedisStorage
    {
        private ConnectionMultiplexer redis;

        public RedisStorage()
        {
            redis = ConnectionMultiplexer.Connect("localhost");
        }

        public string Get(string key)
        {
            IDatabase db = redis.GetDatabase();
            return db.StringGet(key);
        }

        public bool Get<T>(string key, out T value)
        {
            IDatabase db = redis.GetDatabase();

            try
            {
                string json = db.StringGet(key);
                value = JsonConvert.DeserializeObject<T>(json);
            }
            catch (System.Exception)
            {
                value = default(T);
                return false;
            }

            return true;
        }

        public void Set<T>(string key, T value)
        {
            IDatabase db = redis.GetDatabase();

            string json = JsonConvert.SerializeObject(value);

            db.StringSet(key, json);
        }
    }
}
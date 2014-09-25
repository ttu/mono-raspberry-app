using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedisDataStorage;

namespace DebuggingHelper
{
    [TestClass]
    public class RedisTests
    {
        [TestMethod]
        public void GetAndSet()
        {
            var redis = new RedisStorage();

            RedisTestData data;
            var found = redis.Get<RedisTestData>("1", out data);
            var json = redis.Get("1");

            var good = new RedisTestData { Id = 2, Value = "two" };
            redis.Set("2", good);
            json = redis.Get("2");

            RedisTestData goodCheck;
            var success = redis.Get<RedisTestData>("2", out goodCheck);
        }

        [TestMethod]
        public void GetAndSetString()
        {
            var redis = new RedisStorage();

            var c = redis.Get("ctest");

            Assert.IsNull(c);

            redis.Set("ctest2", "1");
            var test = redis.Get("ctest2");

            Assert.AreEqual("1", test);

            redis.Remove("ctest2");
            test = redis.Get("ctest2");

            Assert.IsNull(test);
        }

        [TestMethod]
        public void GetAndSetMono()
        {
            var redis = new RedisStorageMono();

            RedisTestData data;
            var found = redis.Get<RedisTestData>("1", out data);
            var json = redis.Get("1");

            var good = new RedisTestData { Id = 2, Value = "two" };
            redis.Set("2", good);
            json = redis.Get("2");

            RedisTestData goodCheck;
            var success = redis.Get<RedisTestData>("2", out goodCheck);
        }

        [TestMethod]
        public void GetAndSetStringMono()
        {
            var redis = new RedisStorageMono();

            var c = redis.Get("ctest");

            Assert.IsNull(c);

            redis.Set("ctest2", "1");
            var test = redis.Get("ctest2");

            Assert.AreEqual("1", test);

            redis.Remove("ctest2");
            test = redis.Get("ctest2");

            Assert.IsNull(test);
        }

        public class RedisTestData
        {
            public int Id { get; set; }

            public string Value { get; set; }
        }
    }
}

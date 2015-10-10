using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StackExchange.Redis.Extender
{
    public static class StackExchangeRedisExtension
    {
        public static T Get<T>(this IDatabase cache, string key)
        {
            return FromJson<T>(cache.StringGet(key));
        }

        public static object Get(this IDatabase cache, string key)
        {
            return FromJson<object>(cache.StringGet(key));
        }

        public static bool Set(this IDatabase cache, string key, object value)
        {
            return cache.StringSet(key, ToJson(value));
        }

        public static bool Set(this IDatabase cache, string key, object value, TimeSpan? expiry)
        {
            return cache.StringSet(key, ToJson(value), expiry);
        }

        public static bool SetAdd(this IDatabase cache, string key, object value)
        {
            return cache.SetAdd(key, ToJson(value));
        }

        public static bool SetContains(this IDatabase cache, string key, object value)
        {
            return cache.SetContains(key, ToJson(value));
        }

        public static bool SetMove(this IDatabase cache, string source, string destination, object value)
        {
            return cache.SetMove(source, destination, ToJson(value));
        }

        public static bool SetRemove(this IDatabase cache, string key, object value)
        {
            return cache.SetRemove(key, ToJson(value));
        }

        public static T GetSet<T>(this IDatabase cache, string key, object value)
        {
            return FromJson<T>(cache.StringGetSet(key, ToJson(value)));
        }

        public static object GetSet(this IDatabase cache, string key, object value)
        {
            return FromJson<object>(cache.StringGetSet(key, ToJson(value)));
        }

        public static async Task<bool> SetAsync(this IDatabase cache, string key, object value)
        {
            return await cache.StringSetAsync(key, ToJson(value));
        }

        public static async Task<bool> SetAsync(this IDatabase cache, string key, object value, TimeSpan? expiry)
        {
            return await cache.StringSetAsync(key, ToJson(value), expiry);
        }

        public static async Task<bool> SetAddAsync(this IDatabase cache, string key, object value)
        {
            return await cache.SetAddAsync(key, ToJson(value));
        }

        public static async Task<bool> SetContainsAsync(this IDatabase cache, string key, object value)
        {
            return await cache.SetContainsAsync(key, ToJson(value));
        }

        public static async Task<bool> SetMoveAsync(this IDatabase cache, string source, string destination, object value)
        {
            return await cache.SetMoveAsync(source, destination, ToJson(value));
        }

        public static async Task<bool> SetRemoveAsync(this IDatabase cache, string key, object value)
        {
            return await cache.SetRemoveAsync(key, ToJson(value));
        }

        private static string ToJson(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
        
        private static T FromJson<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}

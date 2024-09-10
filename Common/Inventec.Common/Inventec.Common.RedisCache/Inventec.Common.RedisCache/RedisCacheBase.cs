using ServiceStack.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Inventec.Common.RedisCache
{
    public abstract class RedisCacheBase
    {
        protected string host;
        protected readonly static IDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();
        protected bool isUseRamSaveTemp;
        protected bool isUsePreNamespaceByFolder;
        protected string formatTypeId = "{0}:{1}";
        protected const string formatKey = "urn:{0}";
        protected const string formatKeyWithPreNamespaceFolder = "{0}:{1}";
        protected string PreNamespaceFolder { get; set; }

        public RedisCacheBase()
            : this("", false)
        {
        }

        public RedisCacheBase(bool isUseRamSaveTemp)
            : this("", isUseRamSaveTemp)
        {
        }

        public RedisCacheBase(string host, bool isUseRamSaveTemp)
            : this(host, isUseRamSaveTemp, false, "")
        {

        }

        public RedisCacheBase(string host, bool isUseRamSaveTemp, bool isUsePreNamespaceByFolder, string preNamespaceFolder)
        {
            this.host = host;
            this.isUseRamSaveTemp = isUseRamSaveTemp;
            this.isUsePreNamespaceByFolder = isUsePreNamespaceByFolder;
            this.PreNamespaceFolder = preNamespaceFolder;
        }

        protected RedisClient GetRedisClient()
        {
            return (String.IsNullOrEmpty(host) ? new RedisClient() : new RedisClient(host));
        }

        protected void DeleteByKey(string key)
        {
            using (RedisClient redisClient = GetRedisClient())
            {
                redisClient.Remove(key);
            }
        }

        protected bool ConstainByKey(string key)
        {
            using (RedisClient redisClient = GetRedisClient())
            {
                return redisClient.ContainsKey(key);
            }
        }

        protected string GetKey(string key)
        {
            return GetKey(key, false, false);
        }

        protected string GetKey(string key, bool isProcessKeyName, bool isUseFormat)
        {
            string keyFix = key;

            if (isProcessKeyName)
                keyFix = keyFix.Replace(".", "_").ToLower();

            if (this.isUsePreNamespaceByFolder)
            {
                string preNamspace = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(ApplicationStoreLocation.ApplicationDirectory));
                keyFix = String.Format(formatKeyWithPreNamespaceFolder, (!String.IsNullOrEmpty(this.PreNamespaceFolder) ? this.PreNamespaceFolder : preNamspace), keyFix);
            }

            if (isUseFormat)
                keyFix = String.Format(formatKey, keyFix);

            return keyFix;
        }
    }
}

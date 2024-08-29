using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.RedisCache
{
    public interface ICacheProvider
    {
        void LoadIntoCache<T>();

        void LoadIntoCache<T>(string key);

        List<T> GetList<T>();

        List<T> GetList<T>(string key);

        bool CreateList<T>(string key, List<T> values);

        bool CreateOrUpdateList<T>(string key, List<T> values);

        bool Remove(string key);

        bool IsInCache(string key);
    }
}

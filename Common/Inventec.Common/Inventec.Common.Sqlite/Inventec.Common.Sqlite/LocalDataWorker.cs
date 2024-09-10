using Inventec.Common.Sqlite.Base;
using Inventec.Core;
using System;

namespace Inventec.Common.Sqlite
{
    public enum LocalDataType
    {
        BackendData,
        LocalData,
    }

    public class LocalDataWorker : BussinessBase
    {
        public LocalDataWorker(CommonParam param)
            : base(param)
        {

        }

        public LocalDataWorker()
            : base()
        {

        }

        public bool GenerateDBFile(string dbFilename)
        {
            bool success = false;
            try
            {

                success = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                success = false;
            }
            return success;
        }

        public T Get<T>(string key, object tableName)
        {
            T result = default(T);
            try
            {
                IDelegacyT delegacy = new LocalStorageDataGet(param, key, tableName);
                result = delegacy.Execute<T>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = default(T);
            }
            return result;
        }

        public T Set<T>(object value, object tableName)
        {
            T result = default(T);
            try
            {
                IDelegacyT delegacy = new LocalStorageDataBackendSet(param, value, tableName);
                result = delegacy.Execute<T>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = default(T);
            }
            return result;
        }
    }
}

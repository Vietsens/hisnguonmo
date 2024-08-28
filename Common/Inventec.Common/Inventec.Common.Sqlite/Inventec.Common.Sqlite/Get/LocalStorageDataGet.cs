using Inventec.Common.Sqlite.Base;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Sqlite
{
    class LocalStorageDataGet : BussinessBase, IDelegacyT
    {
        string entity;
        object tableNameOrType;

        internal LocalStorageDataGet(CommonParam param, string key, object tablenameOrType)
            : base(param)
        {
            entity = key;
            tableNameOrType = tablenameOrType;
        }

        T IDelegacyT.Execute<T>()
        {
            T result = default(T);
            try
            {
                IDelegacy behavior = LocalStorageDataLocalGetBehaviorFactory.MakeIGet(param, entity, tableNameOrType);
                result = behavior != null ? (T)System.Convert.ChangeType(behavior.Run(), typeof(T)) : default(T);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = default(T);
            }
            return result;
        }
    }
}

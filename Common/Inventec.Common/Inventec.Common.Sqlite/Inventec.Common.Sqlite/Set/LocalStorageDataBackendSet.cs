using Inventec.Common.Sqlite.Base;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Sqlite
{
    class LocalStorageDataBackendSet : BussinessBase, IDelegacyT
    {
        object data;
        object tableNameOrType;

        internal LocalStorageDataBackendSet(CommonParam param, object _data, object tablenameOrType)
            : base(param)
        {
            data = _data;
            tableNameOrType = tablenameOrType;
        }

        T IDelegacyT.Execute<T>()
        {
            T result = default(T);
            try
            {
                IDelegacyT behavior = LocalStorageDataBackendSetBehaviorFactory.MakeIGet(param, data, tableNameOrType);
                result = behavior != null ? (T)System.Convert.ChangeType(behavior.Execute<T>(), typeof(T)) : default(T);
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

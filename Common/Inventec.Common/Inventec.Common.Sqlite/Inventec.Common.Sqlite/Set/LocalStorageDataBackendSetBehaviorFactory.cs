using Inventec.Common.Sqlite.Base;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.Sqlite
{
    class LocalStorageDataBackendSetBehaviorFactory
    {
        internal static IDelegacyT MakeIGet(CommonParam param, object data, object tableNameOrType)
        {
            IDelegacyT result = null;
            string tablenameOrType = "";
            try
            {
                if (tableNameOrType is string)
                {
                    tablenameOrType = (tableNameOrType ?? "").ToString();
                }
                else if (tableNameOrType is LocalDataType)
                {
                    var type = (LocalDataType)tableNameOrType;
                    switch (type)
                    {
                        case LocalDataType.BackendData:
                            tablenameOrType = SqliteConfig.TABLE_SPACE__BACKEND_DATA;
                            //result = new LocalStorageDataBackendSetBehavior(param,  data, tablenameOrType);
                            break;
                        default:
                            tablenameOrType = "";
                            break;
                    }
                }

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data) + tableNameOrType.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tableNameOrType), tableNameOrType), ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}

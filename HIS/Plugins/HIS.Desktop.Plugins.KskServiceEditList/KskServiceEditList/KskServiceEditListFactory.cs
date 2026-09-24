using Inventec.Core;
using System;

namespace HIS.Desktop.Plugins.KskServiceEditList.KskServiceEditList
{
    class KskServiceEditListFactory
    {
        internal static IKskServiceEditList MakeIControl(CommonParam param, object[] data)
        {
            IKskServiceEditList result = null;
            try
            {
                result = new KskServiceEditListBehavior(param, data);
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString()
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
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

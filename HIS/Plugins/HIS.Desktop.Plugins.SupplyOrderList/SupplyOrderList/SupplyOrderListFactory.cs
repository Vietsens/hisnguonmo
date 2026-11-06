using Inventec.Core;
using System;

namespace HIS.Desktop.Plugins.SupplyOrderList.SupplyOrderList
{
    class SupplyOrderListFactory
    {
        internal static ISupplyOrderList MakeISupplyOrderList(CommonParam param, object[] data)
        {
            ISupplyOrderList result = null;
            try
            {
                result = new SupplyOrderListBehavior(param, data);
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
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

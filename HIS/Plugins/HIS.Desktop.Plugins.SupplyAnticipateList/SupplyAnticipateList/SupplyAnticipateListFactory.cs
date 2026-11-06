using Inventec.Core;
using System;

namespace HIS.Desktop.Plugins.SupplyAnticipateList.SupplyAnticipateList
{
    class SupplyAnticipateListFactory
    {
        internal static ISupplyAnticipateList MakeISupplyAnticipateList(CommonParam param, object[] data)
        {
            ISupplyAnticipateList result = null;
            try
            {
                result = new SupplyAnticipateListBehavior(param, data);
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

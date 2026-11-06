using Inventec.Core;
using System;

namespace HIS.Desktop.Plugins.SupplyAnticipateCreate.SupplyAnticipateCreate
{
    class SupplyAnticipateCreateFactory
    {
        internal static ISupplyAnticipateCreate MakeISupplyAnticipateCreate(CommonParam param, object[] data)
        {
            ISupplyAnticipateCreate result = null;
            try
            {
                result = new SupplyAnticipateCreateBehavior(param, data);
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

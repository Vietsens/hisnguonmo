using Inventec.Core;
using System;

namespace HIS.Desktop.Plugins.CallPatientCashierV3
{
    class CallPatientCashierV3Factory
    {
        internal static ICallPatientCashierV3 MakeICallPatientCashierV3(CommonParam param, object[] args)
        {
            ICallPatientCashierV3 result = null;
            try
            {
                result = new CallPatientCashierV3Behavior(param, args);
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => args), args), ex);
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

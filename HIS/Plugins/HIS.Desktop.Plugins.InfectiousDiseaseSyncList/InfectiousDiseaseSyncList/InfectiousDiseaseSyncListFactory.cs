/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList */
using Inventec.Core;
using System;

namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.InfectiousDiseaseSyncList
{
    class InfectiousDiseaseSyncListFactory
    {
        internal static IInfectiousDiseaseSyncList MakeIControl(CommonParam param, object[] data)
        {
            IInfectiousDiseaseSyncList result = null;
            try
            {
                result = new InfectiousDiseaseSyncListBehavior(param, data);
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error(
                    "Factory không khởi tạo được đối tượng."
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data),
                    ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

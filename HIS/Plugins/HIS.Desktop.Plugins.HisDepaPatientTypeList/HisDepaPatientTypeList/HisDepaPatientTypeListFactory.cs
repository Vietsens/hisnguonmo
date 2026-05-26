using Inventec.Core;
using System;

namespace HIS.Desktop.Plugins.HisDepaPatientTypeList.HisDepaPatientTypeList
{
    class HisDepaPatientTypeListFactory
    {
        internal static IHisDepaPatientTypeList MakeIHisDepaPatientTypeList(CommonParam param, object[] data)
        {
            IHisDepaPatientTypeList result = null;
            try
            {
                result = new HisDepaPatientTypeListBehavior(param, data);
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error(
                    "Factory khong khoi tao duoc doi tuong. Type="
                    + (data != null ? data.GetType().ToString() : "null")
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data),
                    ex);
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

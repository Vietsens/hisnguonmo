using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisSurgQuota.Config
{
    class HisSurgQuotaFactory
    {
        internal static IHisSurgQuota MakeIControl(CommonParam param, object[] data)
        {
            IHisSurgQuota result = null;
            try
            {
                result = new HisSurgQuotaBehavior(param, data);

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException e)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong tao duoc " + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                throw;
            }
            return result;
        }
    }
}

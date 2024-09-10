using Inventec.Core;
using SAR.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.WordContent.Sar.SarPrintType.Get.List
{
    public class SarPrintTypeBehaviorFactory
    {
        internal static ISarPrintTypeGetList MakeIGetList(CommonParam param, object data)
        {
            ISarPrintTypeGetList result = null;
            try
            {
                if (data.GetType() == typeof(SarPrintTypeFilter))
                {
                    result = new SarPrintTypeBehavior(param, (SarPrintTypeFilter)data);
                }
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
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

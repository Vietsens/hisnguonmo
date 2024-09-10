using Inventec.Core;
using SAR.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.WordContent.SarPrint.Get.List
{
    public class SarPrintBehaviorFactory
    {
        internal static ISarPrintGetList MakeIGetList(CommonParam param, object data)
        {
            ISarPrintGetList result = null;
            try
            {
                if (data.GetType() == typeof(SarPrintFilter))
                {
                    result = new SarPrintBehavior(param, (SarPrintFilter)data);
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

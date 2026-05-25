using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ApprovalExamAnesthesia.ApprovalExamAnesthesia
{
    class ApprovalExamAnesthesiaFactory
    {
        internal static IApprovalExamAnesthesia MakeIApprovalExamAnesthesia(CommonParam param, object[] filter)
        {
            IApprovalExamAnesthesia result = null;
            try
            {
                result = new ApprovalExamAnesthesiaBehavior(param, filter);
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + filter.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter), ex);
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

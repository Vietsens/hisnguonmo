using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Set.Delegate
{
    class SetDelegateHasException : ISetDelegateHasException
    {
        public bool SetDelegate(System.Windows.Forms.UserControl UC, ProcessHasException hasException)
        {
            bool result = false;
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCReportList = (Design.Template1.Template1)UC;
                    result = UCReportList.SetDelegateHasException(hasException);
                }
                else if (UC.GetType() == typeof(Design.Template2.Template2))
                {
                    Design.Template2.Template2 UCReportList = (Design.Template2.Template2)UC;
                    result = UCReportList.SetDelegateHasException(hasException);
                }
                else if (UC.GetType() == typeof(Design.Template3.Template3))
                {
                  Design.Template3.Template3 UCReportList = (Design.Template3.Template3)UC;
                  result = UCReportList.SetDelegateHasException(hasException);
                }
                if (!result)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hasException), hasException));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}

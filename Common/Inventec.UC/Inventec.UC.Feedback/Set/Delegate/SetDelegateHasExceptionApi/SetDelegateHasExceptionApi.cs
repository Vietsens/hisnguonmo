using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Feedback.Set.Delegate.SetDelegateHasExceptionApi
{
    class SetDelegateHasExceptionApi : ISetDelegateHasExceptionApi
    {
        public bool SetDelegateHasException(UserControl UC, HasExceptionApi HasException)
        {
            bool result = false;
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCFeedback = (Design.Template1.Template1)UC;
                    result = UCFeedback.SetDelegateHasExcep(HasException);
                }

                if (result == false)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HasException), HasException));
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

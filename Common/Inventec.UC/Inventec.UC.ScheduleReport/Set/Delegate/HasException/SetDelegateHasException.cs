using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ScheduleReport.Set.Delegate.HasException
{
    class SetDelegateHasException : ISetDelegateHasException
    {
        public bool SetDelegate(System.Windows.Forms.UserControl UC, ProcessHasException HasException)
        {
            bool result = false;
            try
            {
                if (UC.GetType() == typeof(Design.Template1.Template1))
                {
                    Design.Template1.Template1 UCSchedule = (Design.Template1.Template1)UC;
                    result = UCSchedule.SetDelegateHasException(HasException);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

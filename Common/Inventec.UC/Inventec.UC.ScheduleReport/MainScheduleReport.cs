using Inventec.UC.ScheduleReport.Init;
using Inventec.UC.ScheduleReport.Set.Delegate.HasException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ScheduleReport
{
    public partial class MainScheduleReport
    {
        public enum EnumTemplate
        {
            TEMPLATE1
        }

        public UserControl Init(EnumTemplate Template, Data.DataInit Data)
        {
            UserControl result = null;
            try
            {
                result = InitFactory.MakeIInit().InitUC(Template, Data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public bool SetDelegateHasException(UserControl UC, ProcessHasException HasException)
        {
            bool result = false;
            try
            {
                result = SetDelegateHasExceptionFactory.MakeISetDelegateHasException().SetDelegate(UC, HasException);
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
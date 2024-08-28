using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.EventLogControl.Init
{
    class Init : IInit
    {
        public System.Windows.Forms.UserControl InitUC(MainEventLog.EnumTemplate Template, Data.DataInit Data)
        {
            UserControl result = null;
            try
            {
                if (Template == MainEventLog.EnumTemplate.TEMPLATE1)
                {
                    result = new Design.Template1.Template1(Data);
                }
                else if (Template == MainEventLog.EnumTemplate.TEMPLATE2)
                {
                    result = new Design.Template2.Template2(Data);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Template), Template));
                }
                if (result == null) Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Data), Data));
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

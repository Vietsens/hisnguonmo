using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.EventLogControl.Init
{
    class Init3 : IInit3
    {
        public UserControl InitUC(Inventec.UC.EventLogControl.Data.DataInit3 Data)
        {
            UserControl userControl = null;
            try
            {
                if (Data != null)
                {
                    userControl = new Inventec.UC.EventLogControl.Design.Template3.Template3(Data);
                }
                if (userControl == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Data), Data));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                userControl = null;
            }
            return userControl;
        }
    }
}

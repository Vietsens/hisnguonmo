using Inventec.UC.ComboProvince.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboProvince.Init
{
    class Init : IInit
    {
        public System.Windows.Forms.UserControl InitCombo(string template, DataInitProcinve Data)
        {
            UserControl result = null;
            try
            {
                if (template == MainComboProvince.TEMPLATE1)
                {
                    result = new Inventec.UC.ComboProvince.Design.Template1.Template1(Data);
                    if (result == null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Data), Data));
                    }
                }
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

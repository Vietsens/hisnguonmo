using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboDistrict.Init
{
    class Init : IInit
    {
        public System.Windows.Forms.UserControl InitCombo(string Template, Data.DataInitDistrict Data)
        {
            UserControl result = null;
            try
            {
                if (Template == MainComboDistrict.TEMPLATE1)
                {
                    result = new Desgin.Template1.Template1(Data);
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

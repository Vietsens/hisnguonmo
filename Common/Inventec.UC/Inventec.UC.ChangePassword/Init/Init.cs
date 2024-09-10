using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ChangePassword.Init
{
    class Init : IInit
    {
        public System.Windows.Forms.UserControl InitUC(MainChangePassword.EnumTemplate Template, Data.DataInitChangePass Data)
        {
            UserControl result = null;
            try
            {
                if (Template == MainChangePassword.EnumTemplate.TEMPLATE2)
                {
                    result = new Design.Template2.Template2(Data);
                    
                }
                
                if (result == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Template), Template));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Data), Data));
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

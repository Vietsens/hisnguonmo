using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ServerConfig.Init
{
    class Init : IInit
    {
        public System.Windows.Forms.UserControl InitUC(MainServerConfig.EnumTemplate Template, CloseFormConfigSystem CloseForm)
        {
            UserControl result = null;
            try
            {
                if (Template == MainServerConfig.EnumTemplate.TEMPLATE1)
                {
                    result = new Design.Template1.Template1(CloseForm);
                }

                if (result == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Template), Template));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => CloseForm), CloseForm));
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

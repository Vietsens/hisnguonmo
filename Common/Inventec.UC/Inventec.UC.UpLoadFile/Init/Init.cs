using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.UpLoadFile.Init
{
    class Init : IInit
    {
        public System.Windows.Forms.UserControl InitUC(MainUpLoadFile.EnumTemplate Template, UpLoadFileToServer UpLoad)
        {
            UserControl result = null;
            try
            {
                if (Template == MainUpLoadFile.EnumTemplate.TEMPLATE1)
                {
                    result = new Design.Template1.Template1(UpLoad);
                }
                if (result == null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UpLoad), UpLoad));
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

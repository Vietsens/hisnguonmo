using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ListReports.Init
{
    class Init : IInit
    {

        public System.Windows.Forms.UserControl InitUC(MainListReports.EnumTemplate template, Data.InitData Data)
        {
            UserControl result = null;
            try
            {
                if (template == MainListReports.EnumTemplate.TEMPLATE1)
                {
                    result = new Design.Template1.Template1(Data);
                }
                else if (template == MainListReports.EnumTemplate.TEMPLATE2)
                {
                    result = new Design.Template2.Template2(Data);
                }
                else if (template == MainListReports.EnumTemplate.TEMPLATE3)
                {
                  result = new Design.Template3.Template3(Data);
                }
                else
                {
                  Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => template), template));
                }
                if (result == null) Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Data), Data));
            }
            catch (Exception ex) 
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

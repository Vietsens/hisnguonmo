using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.CreateReport.Init
{
    class Init : IInit
    {

        public System.Windows.Forms.UserControl InitUC(MainCreateReport.EnumTemplate template, Data.InitData Data)
        {
            UserControl result = null;
            try
            {
                if (template == MainCreateReport.EnumTemplate.TEMPLATE1)
                {
                    result = new Design.TemplateBetweenTimeFilterOnly.TemplateBetweenTimeFilterOnly(Data);
                }
                else if (template == MainCreateReport.EnumTemplate.TEMPLATE2)
                {
                    result = new Design.TemplateBetweenTimeFilterOnly2.TemplateBetweenTimeFilterOnly2(Data);
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

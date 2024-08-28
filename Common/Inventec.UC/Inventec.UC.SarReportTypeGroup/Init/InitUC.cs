using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ListReportTypeGroup.Init
{
    class InitUC : IInitUC
    {
        private Data.InitData Data { get; set; }

        internal InitUC(Data.InitData data)
        {
            Data = data;
        }

        System.Windows.Forms.UserControl IInitUC.Init(MainListReportTypeGroup.EnumTemplate template)
        {
            UserControl result = null;
            try
            {
                bool valid = true;
                valid = valid & (Data != null);
                if (valid)
                {
                    if (template == MainListReportTypeGroup.EnumTemplate.TEMPLATE1)
                    {
                        result = new Design.Template1.Template1(Data);
                    }

                    if (result == null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Khoi tao UC Template khong duoc. EnumTemplate." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => template), template));
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Data), Data));
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

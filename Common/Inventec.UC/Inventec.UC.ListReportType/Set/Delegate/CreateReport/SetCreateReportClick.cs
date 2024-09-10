using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportType.Set.Delegate.CreateReport
{
    class SetCreateReportClick : ISetCreateReportClick
    {
        private CreateReport_Click Data { get; set; }

        internal SetCreateReportClick(CreateReport_Click data)
        {
            this.Data = data;
        }

        bool ISetCreateReportClick.Set(System.Windows.Forms.UserControl UC)
        {
            bool result = false;
            try
            {
                if (UC != null)
                {
                    if (UC.GetType() == typeof(Design.Template1.Template1))
                    {
                        Design.Template1.Template1 ucReportType = (Design.Template1.Template1)UC;
                        result = ucReportType.SetDelegateCreateReport_Click(Data);
                    }

                    if (!result)
                    {
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}

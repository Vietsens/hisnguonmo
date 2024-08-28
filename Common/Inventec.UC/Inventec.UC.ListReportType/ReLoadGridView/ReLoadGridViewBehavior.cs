using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportType.ReLoadGridView
{
    class ReLoadGridViewBehavior : IReLoadGridView
    {
        private long ReportTypeGroupId { get; set; }

        internal ReLoadGridViewBehavior(long ReportTypeGroupId)
        {
            this.ReportTypeGroupId = ReportTypeGroupId;
        }

        bool IReLoadGridView.ReLoad(System.Windows.Forms.UserControl UC)
        {
            bool result = false;
            try
            {
                if (UC != null)
                {
                    if (UC.GetType() == typeof(Design.Template1.Template1))
                    {
                        Design.Template1.Template1 ucReportType = (Design.Template1.Template1)UC;
                        result = ucReportType.SetDataForGridControl(ReportTypeGroupId);
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

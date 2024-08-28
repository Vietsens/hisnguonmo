using Inventec.UC.ListReportType.ReLoadGridView;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ListReportType
{
    public partial class MainListReportType
    {

        public void ReLoadGridView(UserControl UC, long ReportTypeGroupId)
        {
            try
            {
                ReLoadGridViewFactory.MakeIReLoadGridView(ReportTypeGroupId).ReLoad(UC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}

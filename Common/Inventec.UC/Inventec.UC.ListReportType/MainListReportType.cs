using Inventec.UC.ListReportType.Init;
using Inventec.UC.ListReportType.Set.Delegate.CreateReport;
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
        public enum EnumTemplate
        {
            TEMPLATE1
        }

        public UserControl Init(EnumTemplate Template, Data.InitData data)
        {
            UserControl result = null;
            try
            {
                result = InitUCFactory.MakeIInitUC(data).Init(Template);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public bool SetDelegateCreateReport(UserControl UC, CreateReport_Click Click)
        {
            bool result = false;
            try
            {
                result = SetCreateReportClickFactory.MakeISetCreateReportClick(Click).Set(UC);
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

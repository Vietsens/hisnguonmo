using Inventec.UC.ListReportTypeGroup.Init;
using Inventec.UC.ListReportTypeGroup.Set.Delegate.RowCellClick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ListReportTypeGroup
{
    public partial class MainListReportTypeGroup
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

        public bool SetDelegateCreateReport(UserControl UC, RowCellClickDelegate Click)
        {
            bool result = false;
            try
            {
                result = RowCellClickFactory.MakeIRowCellClick(Click).Set(UC);
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

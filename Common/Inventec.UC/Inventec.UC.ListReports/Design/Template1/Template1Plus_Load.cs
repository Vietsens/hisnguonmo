using DevExpress.XtraEditors;
using Inventec.UC.ListReports.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Design.Template1
{
    internal partial class Template1
    {
        public void MeShow()
        {
            try
            {
                SetDefaultControl();
                txtPageSize.EditValue = GlobalStore.NumberPage;
                ButtonSearchAndPagingClick(true);
                txtSearch.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultControl()
        {
            try
            {
                DateTime dtStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dtTimeForm.EditValue = dtStart;
                dtTimeTo.EditValue = DateTime.Now;
                txtSearch.Text = "";
                checkAll.Checked = true;
                checkNoProcess.Checked = false;
                checkProcessing.Checked = false;
                checkFinish.Checked = false;
                checkCancel.Checked = false;
                checkError.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

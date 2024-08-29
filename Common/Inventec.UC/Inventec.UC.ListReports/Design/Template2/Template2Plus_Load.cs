using Inventec.UC.ListReports.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Design.Template2
{
    internal partial class Template2
    {
        public void MeShow()
        {
            try
            {
                SetDefaultControl();
                txtPageSize.EditValue = GlobalStore.NumberPage;
                ButtonSearchAndPagingClick(true);
                txtSearch.Text = "";      
                txtSearch.Focus();
                txtSearch.SelectAll();
                
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
                DateTime dtStart = DateTime.Now;
                dtTimeForm.EditValue = dtStart;
                dtTimeTo.EditValue = DateTime.Now;
                txtSearch.Text = "";
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

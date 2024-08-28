using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Design.Template1
{
    internal partial class Template1
    {

        public void MeShow()
        {
            try
            {
                pagingGrid = new Base.PagingGrid();
                SetDefaultValueControl();
                txtPageSize.EditValue = Inventec.UC.EventLogControl.Base.GlobalStore.NumPageSize;
                ButtonSearchAndPagingClick(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                DateTime timeFrom = DateTime.Now;
                dtTimeFrom.EditValue = timeFrom;
                dtTimeTo.EditValue = DateTime.Now;
                txtKeyWord.Text = "";
                txtKeyWord.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        
    }
}

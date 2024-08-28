using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.EventLogControl.Design.Template1
{
    internal partial class Template1
    {

        private void txtPageSize_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFirstPage_Click(object sender, EventArgs e)
        {
            try
            {
                pagingGrid.FirstPage();
                ButtonSearchAndPagingClick(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPreviousPage_Click(object sender, EventArgs e)
        {
            try
            {
                pagingGrid.PreviousPage();
                ButtonSearchAndPagingClick(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            try
            {
                pagingGrid.NextPage();
                ButtonSearchAndPagingClick(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            try
            {
                pagingGrid.LastPage();
                ButtonSearchAndPagingClick(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefreshPage_Click(object sender, EventArgs e)
        {
            try
            {
                ButtonSearchAndPagingClick(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

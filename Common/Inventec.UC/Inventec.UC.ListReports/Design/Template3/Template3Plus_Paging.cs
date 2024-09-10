using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Design.Template3
{
  internal partial class Template3
    {

        private void btnFirstPage_Click(object sender, EventArgs e)
        {
            try
            {
                pagingGrid.FirstPage();
                //ButtonSearchAndPagingClick(false);
                SetInitPaging();
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
                //ButtonSearchAndPagingClick(false);
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
                //ButtonSearchAndPagingClick(false);
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
                //ButtonSearchAndPagingClick(false);
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
                //ButtonSearchAndPagingClick(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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
    }
}

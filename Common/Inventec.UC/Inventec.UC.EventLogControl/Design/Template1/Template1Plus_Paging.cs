/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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

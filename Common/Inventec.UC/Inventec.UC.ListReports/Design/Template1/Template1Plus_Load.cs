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

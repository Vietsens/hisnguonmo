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
using DevExpress.Utils;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportType.Design.Template1
{
    internal partial class Template1
    {
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Data.SearchData data = new Data.SearchData();
                SetSearchData(data);
                FillDataToGridControl(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControl();
                Data.SearchData data = new Data.SearchData();
                SetSearchData(data);
                FillDataToGridControl(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridControl(Data.SearchData data)
        {
            try
            {
                FillDataToGridControl(new CommonParam(0, (int)this.pageSize));
                SetInitPaging();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnCreateReport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var reportType = gridViewReportType.GetFocusedRow();
                if (_createReport != null) _createReport(reportType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSearchData(Data.SearchData data)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtKeyWord.Text))
                {
                    data.KeyWord = txtKeyWord.Text;
                }

                if (this.ReportTypeGroupId>0)
                {
                    data.ReportTypeGroupId = this.ReportTypeGroupId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

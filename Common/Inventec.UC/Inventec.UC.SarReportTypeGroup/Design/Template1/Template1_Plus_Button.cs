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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Logging;
using Inventec.Core;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReportTypeGroup.Design.Template1
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
                //SetInitPaging();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewReportTypeGroup_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    SAR_REPORT_TYPE_GROUP data = (SAR_REPORT_TYPE_GROUP)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    chkAllReportType.Checked = false;
                    if (_rowCellClick != null) _rowCellClick(data);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAllReportType_Click(object sender, EventArgs e)
        {
            try
            {
                if (!chkAllReportType.Checked)
                {
                    if (_rowCellClick != null) _rowCellClick(new SAR_REPORT_TYPE_GROUP());
                }
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

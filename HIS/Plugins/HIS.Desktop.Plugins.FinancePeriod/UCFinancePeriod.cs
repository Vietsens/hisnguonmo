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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.IsAdmin;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;

namespace HIS.Desktop.Plugins.FinancePeriod
{
    public partial class UCFinancePeriod : HIS.Desktop.Utility.UserControlBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        int rowCount = 0;
        int dataTotal = 0;
        int numPageSize;

        public UCFinancePeriod(Inventec.Desktop.Common.Modules.Module moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void UCFinancePeriod_Load(object sender, EventArgs e)
        {
            try
            {
                LoadComboBranch();
                LoadGridFinancePeriod();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFinancePeriod_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_FINANCE_PERIOD dataRow = (V_HIS_FINANCE_PERIOD)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + ((ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "PREVIOUS_PERIOD_TIME_DISPLAY")
                    {
                        if (dataRow.PREVIOUS_PERIOD_TIME.HasValue)
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.PREVIOUS_PERIOD_TIME.Value);
                    }
                    else if (e.Column.FieldName == "PERIOD_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.PERIOD_TIME);
                    }
                    else if (e.Column.FieldName == "TOTAL_DEPOSIT_DISPLAY")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.TOTAL_DEPOSIT ?? 0, ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "TOTAL_REPAY_AMOUNT_DISPLAY")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.TOTAL_REPAY_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "TOTAL_BILL_AMOUNT_DISPLAY")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.TOTAL_BILL_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "TOTAL_BILL_TRANSFER_AMOUNT_DISPLAY")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.TOTAL_BILL_TRANSFER_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "TOTAL_BILL_EXEMPTION_DISPLAY")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.TOTAL_BILL_EXEMPTION ?? 0, ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "TOTAL_BILL_FUND_DISPLAY")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.TOTAL_BILL_FUND ?? 0, ConfigApplications.NumberSeperator);
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFinancePeriodCreate_Click(object sender, EventArgs e)
        {
            try
            {

                frmFinancePeriodCreate frm = new frmFinancePeriodCreate(HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId(), RefeshData);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshData()
        {
            try
            {
                LoadGridFinancePeriod();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFinancePeriod_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                V_HIS_FINANCE_PERIOD data = null;
                if (e.RowHandle > -1)
                {
                    var index = gridViewFinancePeriod.GetDataSourceRowIndex(e.RowHandle);
                    data = (V_HIS_FINANCE_PERIOD)((IList)((BaseView)sender).DataSource)[index];
                }
                if (e.RowHandle >= 0)
                {
                    if (data != null)
                    {
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        if (e.Column.FieldName == "ACTION_DELETE")
                        {
                            if (data.CREATOR == loginName || CheckLoginAdmin.IsAdmin(loginName))
                            {
                                e.RepositoryItem = repositoryItemButtonDelete;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonDelete_Disable;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                DialogResult myResult;
                myResult = MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (myResult == DialogResult.OK)
                {
                    V_HIS_FINANCE_PERIOD financePeriod = gridViewFinancePeriod.GetFocusedRow() as V_HIS_FINANCE_PERIOD;
                    List<V_HIS_FINANCE_PERIOD> financePeriods = gridControlFinancePeriod.DataSource as List<V_HIS_FINANCE_PERIOD>;
                    if (financePeriod != null && CheckFKFinancePeriod(financePeriod.ID))
                    {
                        //Xoa va refesh dữ liệu
                        CommonParam param = new CommonParam();
                        bool deleteAction = new BackendAdapter(param)
                        .Post<bool>("api/HisFinancePeriod/Delete", ApiConsumers.MosConsumer, financePeriod.ID, param);
                        if (deleteAction)
                        {
                            financePeriods.Remove(financePeriod);
                            gridControlFinancePeriod.RefreshDataSource();
                            btnFinancePeriod.Focus();
                        }
                        MessageManager.Show(this.ParentForm, param, deleteAction);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckFKFinancePeriod(long financePeriodID)
        {
            bool result = true;
            try
            {
                CommonParam param = new CommonParam();
                HisFinancePeriodViewFilter filter = new HisFinancePeriodViewFilter();
                filter.PREVIOUS_ID = financePeriodID;
                List<V_HIS_FINANCE_PERIOD> financePeriods = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_FINANCE_PERIOD>>("api/HisFinancePeriod/GetView", ApiConsumers.MosConsumer, filter, param);
                if (financePeriods != null && financePeriods.Count > 0)
                {
                    MessageBox.Show("Tồn tại là kỳ đầu của một kỳ khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            LoadGridFinancePeriod();
        }
    }
}

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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HTC.EFMODEL.DataModels;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpenseList
{
    public partial class UCExpenseList
    {

        private void txtPeriodCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtPeriodCode.Text))
                    {
                        string key = txtPeriodCode.Text.Trim().ToLower();
                        var listData = BackendDataWorker.Get<HTC_PERIOD>().Where(o => o.PERIOD_CODE.ToLower().Contains(key) || o.PERIOD_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            txtPeriodCode.Text = listData.First().PERIOD_CODE;
                            cboPeriod.EditValue = listData.First().ID;
                            txtExpenseTypeCode.Focus();
                            txtExpenseTypeCode.SelectAll();
                        }
                        else
                        {
                            cboPeriod.Focus();
                            cboPeriod.ShowPopup();
                        }
                    }
                    else
                    {
                        cboPeriod.Focus();
                        cboPeriod.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPeriod_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboPeriod.EditValue != null)
                    {
                        var period = BackendDataWorker.Get<HTC_PERIOD>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPeriod.EditValue));
                        if (period != null)
                        {
                            txtPeriodCode.Text = period.PERIOD_CODE;
                        }
                    }
                    txtExpenseTypeCode.Focus();
                    txtExpenseTypeCode.Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpenseTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtExpenseTypeCode.Text))
                    {
                        string key = txtExpenseTypeCode.Text.Trim().ToLower();
                        var listData = BackendDataWorker.Get<HTC_EXPENSE_TYPE>().Where(o => o.EXPENSE_TYPE_CODE.ToLower().Contains(key) || o.EXPENSE_TYPE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            txtExpenseTypeCode.Text = listData.First().EXPENSE_TYPE_CODE;
                            cboExpenseType.EditValue = listData.First().ID;
                            txtDepartmentCode.Focus();
                            txtDepartmentCode.SelectAll();
                        }
                        else
                        {
                            cboExpenseType.Focus();
                            cboExpenseType.ShowPopup();
                        }
                    }
                    else
                    {
                        cboExpenseType.Focus();
                        cboExpenseType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExpenseType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboExpenseType.EditValue != null)
                    {
                        var expenseType = BackendDataWorker.Get<HTC_EXPENSE_TYPE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpenseType.EditValue));
                        if (expenseType != null)
                        {
                            txtExpenseTypeCode.Text = expenseType.EXPENSE_TYPE_CODE;
                        }
                    }
                    txtDepartmentCode.Focus();
                    txtDepartmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExpenseTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtDepartmentCode.Focus();
                    txtDepartmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDepartmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtDepartmentCode.Text))
                    {
                        string key = txtDepartmentCode.Text.Trim().ToLower();
                        var listData = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.DEPARTMENT_CODE.ToLower().Contains(key) || o.DEPARTMENT_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            txtDepartmentCode.Text = listData.First().DEPARTMENT_CODE;
                            cboDepartment.EditValue = listData.First().ID;
                            txtPrice.Focus();
                            txtPrice.SelectAll();
                        }
                        else
                        {
                            cboDepartment.Focus();
                            cboDepartment.ShowPopup();
                        }
                    }
                    else
                    {
                        cboDepartment.Focus();
                        cboDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboDepartment.EditValue != null)
                    {
                        var period = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboDepartment.EditValue));
                        if (period != null)
                        {
                            txtDepartmentCode.Text = period.DEPARTMENT_CODE;
                        }
                    }
                    txtPrice.Focus();
                    txtPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnAdd.Enabled || !dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HTC_EXPENSE data = new HTC_EXPENSE();
                var period = BackendDataWorker.Get<HTC_PERIOD>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPeriod.EditValue));
                if (period == null)
                {
                    goto End;
                }
                if (period.IS_ACTIVE == IMSys.DbConfig.HTC_RS.COMMON.IS_ACTIVE__FALSE)
                {
                    //MessageUtil.SetMessage(param, HIS.Desktop.LibraryMessage.Message.Enum.KyDuLieuDangBiKhoa);
                    goto End;
                }
                data.PERIOD_ID = period.ID;
                var expenseType = BackendDataWorker.Get<HTC_EXPENSE_TYPE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpenseType.EditValue));
                if (expenseType == null)
                {
                    goto End;
                }
                if (expenseType.IS_ACTIVE == IMSys.DbConfig.HTC_RS.COMMON.IS_ACTIVE__FALSE)
                {
                    //MessageUtil.SetMessage(param, HIS.Desktop.LibraryMessage.Message.Enum.LoaiChiDangBiKhoa);
                    goto End;
                }
                data.EXPENSE_TYPE_ID = expenseType.ID;
                if (dtExpenseTime.EditValue != null && dtExpenseTime.DateTime != DateTime.MinValue)
                {
                    data.EXPENSE_TIME = Convert.ToInt64(dtExpenseTime.DateTime.ToString("yyyyMMddHHmmss"));
                }
                var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboDepartment.EditValue));
                if (department == null)
                {
                    goto End;
                }
                data.DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                data.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                data.PRICE = txtPrice.Value;
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HTC_EXPENSE>(HtcRequestUriStore.HTC_EXPENSE__CREATE, ApiConsumers.HtcConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    FillDataToGrid();
                }

            End:
                WaitingManager.Hide();
                MessageManager.Show(param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnEdit.Enabled || !dxValidationProvider1.Validate() || expense == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HTC_EXPENSE data = new HTC_EXPENSE();
                Inventec.Common.Mapper.DataObjectMapper.Map<HTC_EXPENSE>(data, expense);
                var period = BackendDataWorker.Get<HTC_PERIOD>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPeriod.EditValue));
                if (period == null)
                {
                    goto End;
                }
                if (period.IS_ACTIVE == IMSys.DbConfig.HTC_RS.COMMON.IS_ACTIVE__FALSE)
                {
                    //MessageUtil.SetMessage(param, HIS.Desktop.LibraryMessage.Message.Enum.KyDuLieuDangBiKhoa);
                    goto End;
                }
                data.PERIOD_ID = period.ID;
                var expenseType = BackendDataWorker.Get<HTC_EXPENSE_TYPE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpenseType.EditValue));
                if (expenseType == null)
                {
                    goto End;
                }
                if (expenseType.IS_ACTIVE == IMSys.DbConfig.HTC_RS.COMMON.IS_ACTIVE__FALSE)
                {
                    //MessageUtil.SetMessage(param, HIS.Desktop.LibraryMessage.Message.Enum.LoaiChiDangBiKhoa);
                    goto End;
                }
                data.EXPENSE_TYPE_ID = expenseType.ID;
                if (dtExpenseTime.EditValue != null && dtExpenseTime.DateTime != DateTime.MinValue)
                {
                    data.EXPENSE_TIME = Convert.ToInt64(dtExpenseTime.DateTime.ToString("yyyyMMddHHmmss"));
                }
                else
                {
                    data.EXPENSE_TIME = 0;
                }
                var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboDepartment.EditValue));
                if (department == null)
                {
                    goto End;
                }
                data.DEPARTMENT_CODE = department.DEPARTMENT_CODE;
                data.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                data.PRICE = txtPrice.Value;
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HTC_EXPENSE>(HtcRequestUriStore.HTC_EXPENSE__UPDATE, ApiConsumers.HtcConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    FillDataToGrid();
                }

            End:
                WaitingManager.Hide();
                MessageManager.Show(param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                ResetValueControlCreate();
                SetEnableButtonCreate(true);
                txtPeriodCode.Focus();
                txtPeriodCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueControlUpdate()
        {
            try
            {
                if (expense != null)
                {
                    var period = BackendDataWorker.Get<HTC_PERIOD>().FirstOrDefault(o => o.ID == expense.PERIOD_ID);
                    if (period != null)
                    {
                        txtPeriodCode.Text = period.PERIOD_CODE;
                        cboPeriod.EditValue = period.ID;
                    }
                    else
                    {
                        txtPeriodCode.Text = "";
                        cboPeriod.EditValue = null;
                    }

                    txtExpenseTypeCode.Text = expense.EXPENSE_TYPE_CODE;
                    cboExpenseType.EditValue = expense.EXPENSE_TYPE_ID;
                    var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.DEPARTMENT_CODE == expense.DEPARTMENT_CODE);
                    if (department != null)
                    {
                        txtDepartmentCode.Text = department.DEPARTMENT_CODE;
                        cboDepartment.EditValue = department.ID;
                    }
                    else
                    {
                        txtDepartmentCode.Text = "";
                        cboDepartment.EditValue = null;
                    }

                    var dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(expense.EXPENSE_TIME);
                    if (dt.HasValue)
                    {
                        dtExpenseTime.DateTime = dt.Value;
                    }
                    txtPrice.EditValue = expense.PRICE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableButtonCreate(bool isEnable)
        {
            try
            {
                btnAdd.Enabled = isEnable;
                var edit = GlobalVariables.AcsAuthorizeSDO.ControlInRoles != null ? GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BTN_EDIT_CONTROL_CODE) : null;
                if (edit != null)
                {
                    btnEdit.Enabled = !isEnable;
                }
                else
                {
                    btnEdit.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetValueControlCreate()
        {
            try
            {
                expense = null;
                txtDepartmentCode.Text = "";
                cboDepartment.EditValue = null;
                txtExpenseTypeCode.Text = "";
                cboExpenseType.EditValue = null;
                txtPeriodCode.Text = "";
                cboPeriod.EditValue = null;
                dtExpenseTime.EditValue = null;
                txtPrice.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

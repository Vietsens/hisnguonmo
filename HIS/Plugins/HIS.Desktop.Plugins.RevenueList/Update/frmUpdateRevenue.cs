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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.RevenueList.Update.Validation;
using HTC.EFMODEL.DataModels;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RevenueList.Update
{
    public partial class frmUpdateRevenue : HIS.Desktop.Utility.FormBase
    {
        HTC_REVENUE revenue = null;
        Inventec.Desktop.Common.Modules.Module moduleData;

        int positionHandleControl = -1;

        public frmUpdateRevenue(HTC_REVENUE data, Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                this.revenue = data;
                this.moduleData = _moduleData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmUpdateRevenue_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.revenue != null)
                {
                    this.ValidControl();
                    this.LoadDataToCboExeDepart();
                    this.LoadDataToCboReqDepart();
                    this.SetValueToControl();
                }
                else
                {
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboReqDepart()
        {
            try
            {
                cboReqDepartment.Properties.DataSource = BackendDataWorker.Get<HIS_DEPARTMENT>().OrderBy(o => o.NUM_ORDER).ToList();
                cboReqDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboReqDepartment.Properties.ValueMember = "ID";
                cboReqDepartment.Properties.ForceInitialize();
                cboReqDepartment.Properties.Columns.Clear();
                cboReqDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_CODE", "Mã", 60));
                cboReqDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_NAME", "Tên", 200));
                cboReqDepartment.Properties.ShowHeader = true;
                cboReqDepartment.Properties.ImmediatePopup = true;
                cboReqDepartment.Properties.DropDownRows = 10;
                cboReqDepartment.Properties.PopupWidth = 260;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCboExeDepart()
        {
            try
            {
                cboExeDepartment.Properties.DataSource = BackendDataWorker.Get<HIS_DEPARTMENT>().OrderBy(o => o.NUM_ORDER).ToList();
                cboExeDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboExeDepartment.Properties.ValueMember = "ID";
                cboExeDepartment.Properties.ForceInitialize();
                cboExeDepartment.Properties.Columns.Clear();
                cboExeDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_CODE", "Mã", 60));
                cboExeDepartment.Properties.Columns.Add(new LookUpColumnInfo("DEPARTMENT_NAME", "Tên", 200));
                cboExeDepartment.Properties.ShowHeader = true;
                cboExeDepartment.Properties.ImmediatePopup = true;
                cboExeDepartment.Properties.DropDownRows = 10;
                cboExeDepartment.Properties.PopupWidth = 260;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetValueToControl()
        {
            try
            {
                if (this.revenue != null)
                {
                    lblRevenueCode.Text = this.revenue.REVENUE_CODE;
                    lblServiceCode.Text = this.revenue.SERVICE_CODE;
                    lblServiceName.Text = this.revenue.SERVICE_NAME;
                    lblServiceTypeName.Text = this.revenue.SERVICE_TYPE_NAME;
                    lblPatientCode.Text = this.revenue.PATIENT_CODE;
                    lblPatientName.Text = this.revenue.PATIENT_NAME;
                    lblDobYear.Text = this.revenue.DOB.HasValue ? this.revenue.DOB.Value.ToString().Substring(0, 4) : "";
                    lblInTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.revenue.IN_TIME ?? 0);
                    lblOutTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this.revenue.OUT_TIME ?? 0);
                    lblXanhponSymbol.Text = this.revenue.XANHPON_SYMBOL;
                    lblBidNumber.Text = this.revenue.BILL_NUMBER.HasValue ? this.revenue
                        .BILL_NUMBER.Value + "" : "";
                    spinAmount.Value = this.revenue.AMOUNT;
                    spinPrice.Value = this.revenue.PRICE;
                    if (!String.IsNullOrEmpty(this.revenue.REQUEST_DEPARTMENT_CODE))
                    {
                        var reqDepart = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.DEPARTMENT_CODE == this.revenue.REQUEST_DEPARTMENT_CODE);
                        if (reqDepart != null)
                        {
                            cboReqDepartment.EditValue = reqDepart.ID;
                        }
                    }

                    if (!String.IsNullOrEmpty(this.revenue.EXECUTE_DEPARTMENT_CODE))
                    {
                        var exeDepart = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.DEPARTMENT_CODE == this.revenue.EXECUTE_DEPARTMENT_CODE);
                        if (exeDepart != null)
                        {
                            cboExeDepartment.EditValue = exeDepart.ID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidControlReqDepartment();
                ValidControlExeDepartment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlReqDepartment()
        {
            try
            {
                ReqDepartmentValidationRule reqDepartRule = new ReqDepartmentValidationRule();
                reqDepartRule.txtReqDepartmentCode = txtReqDepartmentCode;
                reqDepartRule.cboReqDepartment = cboReqDepartment;
                dxValidationProvider1.SetValidationRule(txtReqDepartmentCode, reqDepartRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlExeDepartment()
        {
            try
            {
                ExeDepartmentValidationRule exeDepartRule = new ExeDepartmentValidationRule();
                exeDepartRule.txtExeDepartmentCode = txtExeDepartmentCode;
                exeDepartRule.cboExeDepartment = cboExeDepartment;
                dxValidationProvider1.SetValidationRule(txtExeDepartmentCode, exeDepartRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinPrice.Focus();
                    spinPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtReqDepartmentCode.Focus();
                    txtReqDepartmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRedDepartmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtReqDepartmentCode.Text))
                    {
                        var key = txtReqDepartmentCode.Text.ToLower().Trim();
                        var listData = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.DEPARTMENT_CODE.ToLower().Contains(key) || o.DEPARTMENT_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboReqDepartment.EditValue = listData.First().ID;
                            txtExeDepartmentCode.Focus();
                            txtExeDepartmentCode.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboReqDepartment.Focus();
                        cboReqDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReqDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtExeDepartmentCode.Focus();
                    txtExeDepartmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReqDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtReqDepartmentCode.Text = "";
                if (cboReqDepartment.EditValue != null)
                {
                    var depart = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboReqDepartment.EditValue));
                    if (depart != null)
                    {
                        txtReqDepartmentCode.Text = depart.DEPARTMENT_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExeDepartmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtExeDepartmentCode.Text))
                    {
                        var key = txtExeDepartmentCode.Text.ToLower().Trim();
                        var listData = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.DEPARTMENT_CODE.ToLower().Contains(key) || o.DEPARTMENT_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboExeDepartment.EditValue = listData.First().ID;
                            SendKeys.Send("{TAB}");
                        }
                    }
                    if (!valid)
                    {
                        cboExeDepartment.Focus();
                        cboExeDepartment.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExeDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExeDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtExeDepartmentCode.Text = "";
                if (cboExeDepartment.EditValue != null)
                {
                    var depart = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboExeDepartment.EditValue));
                    if (depart != null)
                    {
                        txtExeDepartmentCode.Text = depart.DEPARTMENT_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnSave.Enabled || this.revenue == null || !dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                var reqDepart = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboReqDepartment.EditValue));
                if (reqDepart != null)
                {
                    this.revenue.REQUEST_DEPARTMENT_CODE = reqDepart.DEPARTMENT_CODE;
                    this.revenue.REQUEST_DEPARTMENT_NAME = reqDepart.DEPARTMENT_NAME;
                }

                var exeDepart = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboExeDepartment.EditValue));
                if (exeDepart != null)
                {
                    this.revenue.EXECUTE_DEPARTMENT_CODE = exeDepart.DEPARTMENT_CODE;
                    this.revenue.EXECUTE_DEPARTMENT_NAME = exeDepart.DEPARTMENT_NAME;
                }

                this.revenue.AMOUNT = spinPrice.Value;
                this.revenue.PRICE = spinPrice.Value;

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HTC_REVENUE>("api/HtcRevenue/Update", ApiConsumers.HtcConsumer, this.revenue, param);
                if (rs != null)
                {
                    success = true;
                }
                WaitingManager.Hide();
                MessageManager.Show(param, success);
                if (success)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}

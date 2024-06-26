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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImpMestPay
{
    public partial class frmImpMestPay : FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        long _ImpMestProposeId;
        int numPageSize;
        int rowCount = 0;
        int dataTotal = 0;
        V_HIS_IMP_MEST_PROPOSE impMestProposeChoose;
        public int positionHandle = -1;
        PEnum.ACTION_TYPE actionType;
        public long? impMestPayId;
        public long? impMestProposeId;


        public frmImpMestPay()
        {
            InitializeComponent();
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmImpMestPay(Inventec.Desktop.Common.Modules.Module _module, long _impMestProposeId)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _module;
                this._ImpMestProposeId = _impMestProposeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmImpMestPay_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                actionType = PEnum.ACTION_TYPE.VIEW;
                spinAmount.EditValue = null;
                spinNextAmount.EditValue = null;
                ValidateControl();
                LoadCombo();
                FillDataToControl();
                InitEnabledControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            FillDataToControl();
        }

        private void txtImpMestCodeCreate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    WaitingManager.Show();
                    if (String.IsNullOrEmpty(txtImpMestCodeCreate.Text)) return;
                    string code = txtImpMestCodeCreate.Text.Trim();
                    if (code.Length < 8 && checkDigit(code))
                    {
                        code = string.Format("{0:00000000}", Convert.ToInt64(code));
                        txtImpMestCodeCreate.Text = code;
                    }

                    CommonParam param = new CommonParam();

                    HisImpMestProposeViewFilter _propose = new HisImpMestProposeViewFilter();
                    _propose.IMP_MEST_PROPOSE_CODE__EXACT = code;

                    var impMestProposes = new BackendAdapter(param)
                    .Get<List<V_HIS_IMP_MEST_PROPOSE>>("api/HisImpMestPropose/GetView", ApiConsumers.MosConsumer, _propose, param);
                    if (impMestProposes != null && impMestProposes.Count > 0)
                    {
                        impMestProposeChoose = impMestProposes[0];
                        actionType = PEnum.ACTION_TYPE.CREATE;
                        impMestProposeId = impMestProposeChoose.ID;
                        LoadToControl(impMestProposeChoose);
                        InitEnabledControl();
                        dtPayForm.Focus();
                        dtPayForm.SelectAll();
                    }
                    else
                    {
                        WaitingManager.Hide();
                        return;
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate() || !Check())
                    return;
                HIS_IMP_MEST_PAY impMestPay = new HIS_IMP_MEST_PAY();
                this.CreateData(ref impMestPay);

                WaitingManager.Show();
                var result = new BackendAdapter(param)
                    .Post<HIS_IMP_MEST_PAY>("api/HisImpMestPay/Update", ApiConsumers.MosConsumer, impMestPay, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    FillDataToControl();
                    impMestPayId = result.ID;
                    actionType = PEnum.ACTION_TYPE.UPDATE;
                    InitEnabledControl();
                }
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate() || !Check())
                    return;
                HIS_IMP_MEST_PAY impMestPay = new HIS_IMP_MEST_PAY();
                this.CreateData(ref impMestPay);

                WaitingManager.Show();
                var result = new BackendAdapter(param).Post<HIS_IMP_MEST_PAY>("api/HisImpMestPay/Create", ApiConsumers.MosConsumer, impMestPay, param);
                WaitingManager.Hide();
                if (result != null)
                {
                    success = true;
                    FillDataToControl();
                    impMestPayId = result.ID;
                    actionType = PEnum.ACTION_TYPE.UPDATE;
                    InitEnabledControl();
                }
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadToControl(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItemCtrlS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnUpdate.Enabled)
                btnUpdate_Click(null, null);
        }

        private void barButtonItemCtrlN_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnCreate.Enabled)
                btnCreate_Click(null, null);
        }

        private void barButtonItemCtrlR_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnRefesh.Enabled)
                btnRefesh_Click(null, null);
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

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMestPay_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                V_HIS_IMP_MEST_PAY impMestPay = gridViewImpMestPay.GetFocusedRow() as V_HIS_IMP_MEST_PAY;
                if (impMestPay != null)
                {
                    impMestProposeId = impMestPay.IMP_MEST_PROPOSE_ID;
                    LoadImpMestPayEdit(impMestPay);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPayerLoginname_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadPayerLoginname(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboPayer_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPayer.EditValue != null)
                    {
                        ACS_USER data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboPayer.EditValue.ToString());
                        if (data != null)
                        {
                            txtPayerLoginname.Text = data.LOGINNAME;
                            cboPayer.Properties.Buttons[1].Visible = true;
                            txtPayFormCode.Focus();
                            txtPayFormCode.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPayForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPayForm.EditValue != null)
                    {
                        HIS_PAY_FORM data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPayForm.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtPayFormCode.Text = data.PAY_FORM_CODE;
                            cboPayForm.Properties.Buttons[1].Visible = true;
                            spinAmount.Focus();
                            spinAmount.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPayer_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPayer.Properties.Buttons[1].Visible = false;
                    cboPayer.EditValue = null;
                    txtPayerLoginname.Text = "";
                    txtPayerLoginname.Focus();
                    txtPayerLoginname.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPayForm_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPayForm.Properties.Buttons[1].Visible = false;
                    cboPayForm.EditValue = null;
                    txtPayFormCode.Text = "";
                    txtPayFormCode.Focus();
                    txtPayFormCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMestPay_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_IMP_MEST_PAY dataRow = (V_HIS_IMP_MEST_PAY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + ((ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "PAY_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.PAY_TIME);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        if (dataRow.MODIFY_TIME.HasValue)
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.MODIFY_TIME.Value);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        if (dataRow.CREATE_TIME.HasValue)
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.CREATE_TIME.Value);
                    }
                    else if (e.Column.FieldName == "NEXT_PAY_TIME_DISPLAY")
                    {
                        if (dataRow.NEXT_PAY_TIME.HasValue)
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataRow.NEXT_PAY_TIME.Value);
                    }
                    else if (e.Column.FieldName == "DOCUMENT_PRICE_DISPLAY")
                    {
                        decimal total = GetImpMestAmount(dataRow.IMP_MEST_PROPOSE_ID);
                        e.Value = Inventec.Common.Number.Convert.NumberToString(total, ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "AMOUNT_DISPLAY")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString(dataRow.AMOUNT, ConfigApplications.NumberSeperator);
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

        private void repositoryItemButtonEditDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                DialogResult myResult;
                myResult = MessageBox.Show("Bạn có muốn xóa thông tin thanh toán?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (myResult != DialogResult.OK)
                {
                    return;
                }

                List<V_HIS_IMP_MEST_PAY> impMestPays = gridControlImpMestPay.DataSource as List<V_HIS_IMP_MEST_PAY>;
                V_HIS_IMP_MEST_PAY impMestPay = gridViewImpMestPay.GetFocusedRow() as V_HIS_IMP_MEST_PAY;
                if (impMestPay != null)
                {
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();

                    bool result = new BackendAdapter(param)
                    .Post<bool>("api/HisImpMestPay/Delete", ApiConsumers.MosConsumer, impMestPay.ID, param);
                    WaitingManager.Hide();
                    if (result)
                    {
                        impMestPays.Remove(impMestPay);
                        gridControlImpMestPay.RefreshDataSource();
                        LoadToControl(null);
                    }
                    MessageManager.Show(this, param, result);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpMestPay_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                V_HIS_IMP_MEST_PAY data = null;
                if (e.RowHandle > -1)
                {
                    var index = gridViewImpMestPay.GetDataSourceRowIndex(e.RowHandle);
                    data = (V_HIS_IMP_MEST_PAY)((IList)((BaseView)sender).DataSource)[index];
                }
                if (e.RowHandle >= 0)
                {
                    if (data != null)
                    {
                        if (e.Column.FieldName == "ACTION_DELETE")
                        {

                            string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                            if (data.CREATOR == loginName || CheckLoginAdmin.IsAdmin(loginName))
                            {
                                e.RepositoryItem = repositoryItemButtonEditDelete;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEditDelete_Disabled;
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

        private void spinAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spinAmount.ContainsFocus)
                {
                    decimal documentPrice = Inventec.Common.TypeConvert.Parse.ToDecimal(lblDocumentPrice.Text);
                    decimal impMestPayAmount = Inventec.Common.TypeConvert.Parse.ToDecimal(lblImpMestPayAmount.Text);
                    lblRemainAmount.Text = (documentPrice - impMestPayAmount - spinAmount.Value).ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtPayForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPayerLoginname.Focus();
                    txtPayerLoginname.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtPayFormCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadPayerForm(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtNextPayTime.Focus();
                    dtNextPayTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtNextPayTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinNextAmount.Focus();
                    spinNextAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItemCtrlF_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnFind.Enabled)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtImpMestCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDocumentNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Utility;
using System.IO;
using Inventec.Fss.Client;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LibraryMessage;
using SDA.Filter;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.SdaLicense.Validation;
using HIS.Desktop.Plugins.SdaLicense.ADO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Configuration;

namespace HIS.Desktop.Plugins.SdaLicense
{
    public partial class frmSdaLicense : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int ActionType = -1;
        int positionHandle = -1;
        SDA_LICENSE currentData { get; set; }
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<ACS_APPLICATION> lstAcsApplition { get; set; }
        List<HIS_BRANCH> lstHisBranch { get; set; }
        string defaultURLVplus = "https://v.vietsens.vn/ords/vietsens/licensephanmem/licensephanmem?mã_khách_hàng={0}&mã_phần_mềm={1}";
        string URLVPLUS = null;
        #endregion
        public frmSdaLicense(Inventec.Desktop.Common.Modules.Module moduleData)
           : base(moduleData)
        {
            try
            {
                InitializeComponent();

                this.moduleData = moduleData;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                    URLVPLUS = ConfigurationManager.AppSettings["VPlus.PathLicense"] ?? defaultURLVplus;
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void frmSdaLicense_Load(object sender, EventArgs e)
        {
            try
            {
                lstAcsApplition = BackendDataWorker.Get<ACS_APPLICATION>().Where(o => o.IS_LICENSE_ISSUED == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                lstHisBranch = BackendDataWorker.Get<HIS_BRANCH>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                ValidateForm();
                LoadDataToGrid();
                SetDefaultValue();
                LoadComboData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboData()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("APPLICATION_CODE", "Tên", 50, 1));
                columnInfos.Add(new ColumnInfo("APPLICATION_NAME", "Mã", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("APPLICATION_NAME", "ID", columnInfos, true, 200);
                ControlEditorLoader.Load(cboApplication, lstAcsApplition, controlEditorADO);

                List<ColumnInfo> columnInfos2 = new List<ColumnInfo>();
                columnInfos2.Add(new ColumnInfo("HEIN_MEDI_ORG_CODE", "Mã KCBBĐ", 50, 1));
                columnInfos2.Add(new ColumnInfo("BRANCH_NAME", "Tên", 150, 2));
                ControlEditorADO controlEditorADO2 = new ControlEditorADO("BRANCH_NAME", "ID", columnInfos2, true, 200);
                ControlEditorLoader.Load(cboBranch, lstHisBranch, controlEditorADO2);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGrid()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                SdaLicenseFilter filter = new SdaLicenseFilter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridControl1.DataSource = null;
                var apiResult = new BackendAdapter(paramCommon).Get<List<SDA.EFMODEL.DataModels.SDA_LICENSE>>("api/SdaLicense/Get", ApiConsumers.SdaConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    gridControl1.DataSource = apiResult;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Validation
        private void ValidateForm()
        {
            try
            {
                ValidateMaxlength valid = new ValidateMaxlength();
                valid.maxLength = 1000;
                valid.mme = txtLicense;
                dxValidationProvider1.SetValidationRule(txtLicense, valid);

                //ValidateComboText validcboApp = new ValidateComboText();
                //validcboApp.txt = txtApplication;
                //validcboApp.cbo = cboApplication;
                //dxValidationProvider1.SetValidationRule(txtApplication, validcboApp);

                //ValidateComboText validcboBrach = new ValidateComboText();
                //validcboBrach.txt = txtBranch;
                //validcboBrach.cbo = cboBranch;
                //dxValidationProvider1.SetValidationRule(txtBranch, validcboBrach);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SDA.EFMODEL.DataModels.SDA_LICENSE pData = (SDA.EFMODEL.DataModels.SDA_LICENSE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_str")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)pData.CREATE_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_str")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)pData.MODIFY_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "EXPIRED_DATE_str")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)pData.EXPIRED_DATE);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "Delete" || e.Column.FieldName == "Update")
                    return;
                var rowData = (SDA.EFMODEL.DataModels.SDA_LICENSE)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    ActionType = GlobalVariables.ActionEdit;
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(SDA_LICENSE rowData)
        {
            try
            {
                txtLicense.Text = rowData != null ? rowData.LICENSE : null;
                txtAppCode.Text = rowData != null ? rowData.APP_CODE : null;
                txtClientCode.Text = rowData != null ? rowData.CLIENT_CODE : null;
                txtDate.Text = rowData != null ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(rowData.EXPIRED_DATE ?? 0) : null;
                EnableControlChanged(this.ActionType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                ChangedDataRow(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLicense_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnCheck_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (!dxValidationProvider1.Validate(txtLicense))
                    return;

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                SDA.SDO.SdaLicenseSDO data = new BackendAdapter(param).Post<SDA.SDO.SdaLicenseSDO>("api/SdaLicense/Decode", ApiConsumers.SdaConsumer, txtLicense.Text.Trim(), param);
                if (data != null)
                {
                    txtAppCode.Text = data.AppCode;
                    txtClientCode.Text = data.ClientCode;
                    txtDate.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.ExpiredDate ?? 0);
                }
                WaitingManager.Hide();
                if(data == null)
                    MessageManager.Show(this.ParentForm, param, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (!dxValidationProvider1.Validate(txtLicense))
                    return;

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                SDA.SDO.SdaLicenseSDO sdo = new SDA.SDO.SdaLicenseSDO();
                sdo.License = txtLicense.Text.Trim();
                sdo.AppCode = txtAppCode.Text.Trim();
                sdo.ClientCode = txtClientCode.Text.Trim();
                object data = new BackendAdapter(param).Post<object>("api/SdaLicense/Update", ApiConsumers.SdaConsumer, sdo, param);
                if (data != null)
                {
                    LoadDataToGrid();
                    ChangedDataRow(null);
                }

                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, data != null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (!dxValidationProvider1.Validate(txtLicense))
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                object data = new BackendAdapter(param).Post<object>("api/SdaLicense/Create", ApiConsumers.SdaConsumer, txtLicense.Text.Trim(), param);
                if (data != null)
                {
                    LoadDataToGrid();
                    ChangedDataRow(null);
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, data != null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (xtraTabControl1.SelectedTabPageIndex == 0)
                {
                    if(btnEdit.Enabled)
                        btnEdit_Click(null, null);
                }
                else
                {
                    if(btnSave.Enabled)
                        btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (xtraTabControl1.SelectedTabPageIndex == 0)
                {
                    if(btnAdd.Enabled)
                        btnAdd_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (xtraTabControl1.SelectedTabPageIndex == 0)
                {
                    btnReset_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    positionHandle = -1;
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    object data = new BackendAdapter(param).Post<object>("api/SdaLicense/Create", ApiConsumers.SdaConsumer, txtCodeActive.Text.Trim(), param);
                    if (data != null)
                    {
                        LoadDataToGrid();
                        txtCodeActive.Text = string.Empty;
                        txtDateEx.Text = String.Empty;
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, data != null);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repDeleEna_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (SDA.EFMODEL.DataModels.SDA_LICENSE)gridView1.GetFocusedRow();
                if (MessageBox.Show("Bạn có chắc muốn xóa dữ liệu không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>("api/SdaLicense/Delete", ApiConsumers.SdaConsumer, rowData, param);
                        if (success)
                        {
                            LoadDataToGrid();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCodeActive_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtApplication.Text.Trim()) || string.IsNullOrEmpty(txtBranch.Text.Trim()))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa nhập thông tin ứng dụng/cơ sở", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                    return;
                }
                CallApiVietSen(txtApplication.Text.Trim(), txtBranch.Text.Trim(), true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async void CallApiVietSen(string applicationCode, string branchCode,bool IsCallFromButton)
        {
            try
            {
                CommonParam param = new CommonParam();
                string url = String.Format(URLVPLUS, branchCode, applicationCode);
                HttpClient client = new HttpClient();
                HttpResponseMessage response = null;
                string responseBody = null;
                try
                {
                    response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    responseBody = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(URLVPLUS + "______" + ex);
                    XtraMessageBox.Show("Kết nối đến hệ thống cấp mã kích hoạt thất bại", "Thông báo", MessageBoxButtons.OK);
                    return;
                }
               

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => responseBody), responseBody));

                var api = JsonConvert.DeserializeObject<LicenseADO>(responseBody);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => api), api));
                if(api == null)
                {
                    XtraMessageBox.Show("Kết nối đến hệ thống cấp mã kích hoạt thất bại", "Thông báo", MessageBoxButtons.OK);
                }
                else if(api.items != null && api.items.Count > 0)
                {
                    WaitingManager.Show();
                    if (IsCallFromButton)
                    {
                        SDA.SDO.SdaLicenseSDO data = new BackendAdapter(param).Post<SDA.SDO.SdaLicenseSDO>("api/SdaLicense/Decode", ApiConsumers.SdaConsumer, api.items[0].license, param);
                        if (data != null)
                        {
                            txtCodeActive.Text = api.items[0].license;
                            txtDateEx.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.ExpiredDate ?? 0);
                        }
                        WaitingManager.Hide();
                        if(data == null)
                            MessageManager.Show(this.ParentForm, param, false);
                    }
                    else
                    {
                        var rowData = (SDA.EFMODEL.DataModels.SDA_LICENSE)gridView1.GetFocusedRow();
                        if (rowData == null)
                            return;
                        SDA.SDO.SdaLicenseSDO sdo = new SDA.SDO.SdaLicenseSDO();
                        sdo.License = rowData.LICENSE;
                        sdo.AppCode = rowData.APP_CODE;
                        sdo.ClientCode = rowData.CLIENT_CODE;
                        object data = new BackendAdapter(param).Post<object>("api/SdaLicense/Update", ApiConsumers.SdaConsumer, sdo, param);
                        if (data != null)
                        {
                            LoadDataToGrid();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, data != null);
                    }
                }
                else
                {
                    XtraMessageBox.Show("Hệ thống cấp mã kích hoạt thất bại", "Thông báo", MessageBoxButtons.OK);
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repUpdateEna_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (SDA.EFMODEL.DataModels.SDA_LICENSE)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    CallApiVietSen(rowData.APP_CODE, rowData.CLIENT_CODE, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboApplication_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboApplication.EditValue != null)
                {
                    txtCodeActive.Text = string.Empty;
                    txtDateEx.Text = String.Empty;
                    var dta = lstAcsApplition.FirstOrDefault(o => o.ID == Int64.Parse(cboApplication.EditValue.ToString()));
                    if (dta != null)
                    {
                        txtApplication.Text = dta.APPLICATION_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBranch_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboBranch.EditValue != null)
                {
                    txtCodeActive.Text = string.Empty;
                    txtDateEx.Text = String.Empty;
                    var dta = lstHisBranch.FirstOrDefault(o => o.ID == Int64.Parse(cboBranch.EditValue.ToString()));
                    if (dta != null)
                    {
                        txtBranch.Text = dta.HEIN_MEDI_ORG_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCodeActive_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                btnSave.Enabled = !string.IsNullOrEmpty(txtCodeActive.Text);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

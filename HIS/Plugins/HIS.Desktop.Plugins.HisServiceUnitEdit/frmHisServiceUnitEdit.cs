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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisServiceUnitEdit.Resources;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisServiceUnitEdit
{
    public partial class frmHisServiceUnitEdit : HIS.Desktop.Utility.FormBase
    {
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int ActionType = -1;
        int positionHandle = -1;
        PagingGrid pagingGrid;
        long serviceUnitEditId;
        MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT currentData;
        Inventec.Desktop.Common.Modules.Module moduleData;
        HIS_SERVICE_UNIT updateDTO = new HIS_SERVICE_UNIT();
        List<HIS_SERVICE_UNIT> listServiceUnit;
        List<ServiceUnitADO> listServiceUnitADO { get; set; }

        public frmHisServiceUnitEdit(Inventec.Desktop.Common.Modules.Module moduleData)
            : this(null, moduleData)
        {
        }

        public frmHisServiceUnitEdit(HIS_SERVICE_UNIT serviceUnitEdit, Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                pagingGrid = new PagingGrid();
                this.Text = moduleData.text;

                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                if (serviceUnitEdit != null)
                {
                    this.currentData = serviceUnitEdit;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmHisServiceUnitEdit_Load(object sender, EventArgs e)
        {
            try
            {

                FillDataToGridControl();
                //Fill data into datasource combo
                FillDataToControlsForm();
                ValidateForm();
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                ResetFormData();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControlsForm()
        {
            try
            {
                InitComboServiceUnit();
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboServiceUnit()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceUnitFilter filter = new HisServiceUnitFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_SERVICE_UNIT>>("api/HisServiceUnit/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                listServiceUnit = new List<HIS_SERVICE_UNIT>();
                listServiceUnit = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboConvert, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //languageKey
        private void SetCaptionByLanguageKey()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Validation
        private void dxValidationProviderEditorInfo_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void ValidateForm()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, bool IsValidControl, IsValid IsValid, int? MaxLength)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.IsValidControl = IsValidControl;
                validRule.IsValid = IsValid;
                validRule.Maxlength = MaxLength;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region sự kiện phím tắt

        private void barButtonItem_Find_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItemEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit.Focus();
                    btnEdit_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItemAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    btnAdd.Focus();
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItemCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        //load data to grid
        private void FillDataToGridControl()
        {
            try
            {
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridserviceUnitEdit(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridserviceUnitEdit, param, numPageSize, this.gridControlServiceUnit);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridserviceUnitEdit(object data)
        {
            try
            {
                WaitingManager.Show();
                start = ((CommonParam)data).Start ?? 0;
                limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(start, limit);
                listServiceUnitADO = new List<ServiceUnitADO>();
                Inventec.Core.ApiResultObject<List<HIS_SERVICE_UNIT>> apiResult = null;
                HisServiceUnitFilter filter = new HisServiceUnitFilter();
                filter.KEY_WORD = txtFind.Text;
                filter.ORDER_DIRECTION = "MODIFY_TIME";
                filter.ORDER_FIELD = "DESC";
                apiResult = new BackendAdapter(param).GetRO<List<HIS_SERVICE_UNIT>>(
                    "api/HisServiceUnit/Get", ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null)
                {
                    listServiceUnitADO = (
                     from m in ((List<HIS_SERVICE_UNIT>)apiResult.Data) select new ServiceUnitADO(m)
                        ).ToList();

                    gridControlServiceUnit.DataSource = null;
                    gridControlServiceUnit.DataSource = listServiceUnitADO;
                    rowCount = (listServiceUnitADO == null ? 0 : listServiceUnitADO.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //xử lý click 1 dòng trên grid
        private void gridViewServiceUnit_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                currentData = (HIS_SERVICE_UNIT)gridViewServiceUnit.GetFocusedRow();

                if (currentData != null)
                {
                    serviceUnitEditId = currentData.ID;
                }

                ChangedDataRow(currentData);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceUnit_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                currentData = (HIS_SERVICE_UNIT)gridViewServiceUnit.GetFocusedRow();

                if (currentData != null)
                {
                    serviceUnitEditId = currentData.ID;
                }

                ChangedDataRow(currentData);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region fill data to control
        private void ChangedDataRow(HIS_SERVICE_UNIT data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    if (data.IS_ACTIVE == 0)
                    {
                        this.ActionType = -1;
                    }
                    EnableControlChanged(this.ActionType);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //fill data to control
        private void FillDataToEditorControl(HIS_SERVICE_UNIT data)
        {
            try
            {
                if (data != null)
                {
                    txtServiceUnitCode.EditValue = data.SERVICE_UNIT_CODE;
                    txtServiceUnitName.EditValue = data.SERVICE_UNIT_NAME;
                    txtServiceUnitSymbol.EditValue = data.SERVICE_UNIT_SYMBOL;
                    //checkEdit1.Checked = (data.HAS_SHELL == 1 ? true : false);
                    spinMaterialNumOder.EditValue = data.MATERIAL_NUM_ORDER;
                    spinMedicineNumOder.EditValue = data.MEDICINE_NUM_ORDER;
                    cboConvert.EditValue = data.CONVERT_ID;
                    txtConvertRatio.Text = data.CONVERT_RATIO == null ? null : data.CONVERT_RATIO.ToString();
                    if (data.IS_PRIMARY == 1)
                        chkServiceUnitMin.Checked=true;
                    else
                        chkServiceUnitMin.Checked = false;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void gridViewServiceUnit_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_SERVICE_UNIT data = (HIS_SERVICE_UNIT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + start;
                            }
                            catch (Exception ex)
                            {

                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri STT", ex);
                            }
                        }

                        else if (e.Column.FieldName == "CONVERT_SERVICE_UNIT_NAME")
                        {
                            e.Value = (listServiceUnit.FirstOrDefault(o => o.ID == data.CONVERT_ID) ?? new HIS_SERVICE_UNIT()).SERVICE_UNIT_NAME;
                        }
                        else if (e.Column.FieldName == "IS_PRIMARY_STR")
                        {
                            e.Value = data != null && data.IS_PRIMARY == 1 ? true : false;

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceUnit_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    short isActive = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewServiceUnit.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnUnLock : btnLock);
                    }
                    else if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnDELETE : btnDELETE_Disable);
                    }
                    else if (e.Column.FieldName == "MEDICINE_NUM_ORDER")
                    {
                        e.RepositoryItem = (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? repositoryItemSpinEdit_medicine : repositoryItemSpinEdit_medicine_Disable);
                    }
                    else if (e.Column.FieldName == "MATERIAL_NUM_ORDER")
                    {
                        e.RepositoryItem = (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? repositoryItemSpinEdit_Material : repositoryItemSpinEdit_Material_Disable);
                    }

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //xử lý tìm kiếm
        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFind_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind.Focus();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //reset form
        private void ResetFormData()
        {
            try
            {
                txtServiceUnitSymbol.EditValue = null;
                txtServiceUnitName.EditValue = null;
                txtServiceUnitCode.EditValue = null;
                //checkEdit1.Checked = false;
                spinMedicineNumOder.EditValue = null;
                spinMaterialNumOder.EditValue = null;
                cboConvert.EditValue = null;
                txtConvertRatio.Text = null;
                chkServiceUnitMin.Checked = false;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnLock_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    BackendDataWorker.Reset<HIS_SERVICE_UNIT>();
                    LockDataProcess();
                }
                else
                {
                    return;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //xử lý khóa và mở khóa dữ liệu
        private void LockDataProcess()
        {
            try
            {
                CommonParam param = new CommonParam();
                HIS_SERVICE_UNIT serviceUnitEdit = new HIS_SERVICE_UNIT();
                bool notHandler = false;
                WaitingManager.Show();
                serviceUnitEdit = new BackendAdapter(param).Post<HIS_SERVICE_UNIT>(
                    "api/HisServiceUnit/ChangeLock", ApiConsumers.MosConsumer, currentData.ID, param);
                if (serviceUnitEdit != null)
                {
                    notHandler = true;
                    // FillDataToGridControl();
                    txtFind.Focus();
                    UpdateRowDataAfterEdit(serviceUnitEdit);
                }
                MessageManager.Show(this.ParentForm, param, notHandler);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonMoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    BackendDataWorker.Reset<HIS_SERVICE_UNIT>();
                    LockDataProcess();
                }
                else
                {
                    return;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //xóa dữ liệu
        private void btnDELETE_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (MessageBox.Show(String.Format(ResourceMessage.BanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>("api/HisServiceUnit/Delete", ApiConsumers.MosConsumer, currentData.ID, param);
                    if (success)
                    {
                        BackendDataWorker.Reset<HIS_SERVICE_UNIT>();
                        FillDataToGridControl();
                    }
                    MessageManager.Show(this, param, success);
                }
                else
                {
                    return;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Gọi Api thêm và sửa dữ liệu
        private void SaveProcess()
        {
            try
            {
                bool success = false;
                if (!btnAdd.Enabled && !btnEdit.Enabled)
                {
                    return;
                }
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ValidationSingleControl(txtServiceUnitName, false,  IsRequiredAndMaxLength,100);
                ValidationSingleControl(txtServiceUnitCode, false, IsRequiredAndMaxLength,3);
                if (this.cboConvert.EditValue != null)
                {
                    ValidationSingleControl(txtConvertRatio, false, IsNumberAndNotNullAndLessThan1000000, 19);
                }
                else
                {
                    ValidationSingleControl(txtConvertRatio, true, null, null);
                }
                if (txtConvertRatio.Text != null && txtConvertRatio.Text != "")
                {
                    ValidationSingleControl(cboConvert, false, IsRequired, null);
                }
                else
                {
                    ValidationSingleControl(cboConvert, true, null, null);
                }
                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HIS_SERVICE_UNIT updateDTO = new HIS_SERVICE_UNIT();

                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>(
                        "api/HisServiceUnit/Create", ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    if (serviceUnitEditId > 0 && currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        updateDTO.ID = serviceUnitEditId;
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>(
                        "api/HisServiceUnit/Update", ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            UpdateRowDataAfterEdit(resultData);
                            FillDataToGridControl();
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu đang bị khóa", "Thông báo");
                        return;
                    }
                }
                BackendDataWorker.Reset<HIS_SERVICE_UNIT>();
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IsNumberAndNotNullAndLessThan1000000(ControlEditValidationRule validADO, out string errorText)
        {
            bool result = false;
            errorText = "";
            try
            {
                if (validADO.editor != null && validADO.editor is TextEdit)
                {
                    TextEdit textEdit = validADO.editor as TextEdit;
                    if (textEdit.Text == null || textEdit.Text == "")
                    {
                        errorText = String.Format(ResourceMessage.ChuaNhapTruongDuLieuBatBuoc);
                        return true;
                    }
                    if (!IsNumber(textEdit.Text ?? ""))
                    {
                        errorText = "Dữ liệu không phải là số thực";
                        return true;
                    }
                    if (!string.IsNullOrEmpty(textEdit.Text.Trim()) && Encoding.UTF8.GetByteCount(textEdit.Text.Trim()) > validADO.Maxlength)
                    {
                        errorText = "Trường dữ liệu vượt quá ký tự cho phép";
                        return true;
                    }
                    if (Inventec.Common.TypeConvert.Parse.ToDecimal((txtConvertRatio.Text ?? "").ToString()) > 1000000)
                    {
                        errorText = "Không được nhập quá 1000000";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool IsNumber(string pText)
        {
            Regex regexVi = new Regex(@"^[-+]?[0-9]*\,?[0-9]+$");
            Regex regexEn = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
            return regexVi.IsMatch(pText) || regexEn.IsMatch(pText);
        }

        private bool IsRequired(ControlEditValidationRule validAdo, out string errorText)
        {
            bool result = false;
            errorText = String.Format(ResourceMessage.ChuaNhapTruongDuLieuBatBuoc);
            try
            {
                if (validAdo.editor != null)
                {
                    if (validAdo.editor is LookUpEdit)
                    {
                        if (((LookUpEdit)validAdo.editor).EditValue == null)
                        {
                            return true;
                        }
                    }
                    else if (validAdo.editor is GridLookUpEdit)
                    {
                        if (((GridLookUpEdit)validAdo.editor).EditValue == null)
                        {
                            return true;
                        }
                    }
                    else if (validAdo.editor is TextEdit)
                    {
                        if (string.IsNullOrEmpty(((TextEdit)validAdo.editor).Text.Trim()))
                        {
                            return true;
                        }
                    }
                    else if (validAdo.editor is MemoEdit)
                    {
                        if (string.IsNullOrEmpty(((MemoEdit)validAdo.editor).Text.Trim()))
                        {
                            return true;
                        }
                    }
                    else if (validAdo.editor is ComboBoxEdit)
                    {
                        if (((ComboBoxEdit)validAdo.editor).EditValue == null || Convert.ToInt64(((ComboBoxEdit)validAdo.editor).EditValue) == 0)
                        {
                            return true;
                        }
                    }
                    else if (validAdo.editor is ButtonEdit)
                    {
                        if (((ButtonEdit)validAdo.editor).Text == null)
                        {
                            return true;
                        }
                    }
                    else if (validAdo.editor is DateEdit)
                    {
                        if (((DateEdit)validAdo.editor).EditValue == null || ((DateEdit)validAdo.editor).DateTime == DateTime.MinValue)
                        {
                            return true;
                        }
                    }
                    else if (validAdo.editor is SpinEdit)
                    {
                        if (((SpinEdit)validAdo.editor).EditValue == null)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool IsRequiredAndMaxLength(ControlEditValidationRule validAdo, out string errorText)
        {
            bool result = false;
            errorText = String.Format(ResourceMessage.ChuaNhapTruongDuLieuBatBuoc);
            try
            {
                if (validAdo.editor != null)
                {
                    if (validAdo.editor is TextEdit)
                    {
                        if (string.IsNullOrEmpty(((TextEdit)validAdo.editor).Text.Trim()))
                        {
                            return true;
                        }
                        if (Encoding.UTF8.GetByteCount(((TextEdit)validAdo.editor).Text.Trim()) > validAdo.Maxlength)
                        {
                            errorText = "Vượt quá độ dài cho phép";
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        
        //update dòng khi sửa
        private void UpdateRowDataAfterEdit(HIS_SERVICE_UNIT data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT) is null");
                var rowData = (MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT)gridViewServiceUnit.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>(rowData, data);
                    gridViewServiceUnit.RefreshRow(gridViewServiceUnit.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //Update dữ liệu từ From
        private void UpdateDTOFromDataForm(ref HIS_SERVICE_UNIT updateDTO)
        {
            try
            {
                updateDTO.SERVICE_UNIT_CODE = (string)txtServiceUnitCode.EditValue;
                updateDTO.SERVICE_UNIT_NAME = (string)txtServiceUnitName.EditValue;
                if (cboConvert.EditValue != null)
                {
                    updateDTO.CONVERT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboConvert.EditValue ?? "0").ToString());
                }
                else
                {
                    updateDTO.CONVERT_ID = null;
                }
                if (cboConvert.EditValue != null)
                {
                    updateDTO.CONVERT_RATIO = Inventec.Common.TypeConvert.Parse.ToDecimal((txtConvertRatio.Text ?? "").ToString());
                }
                else
                {
                    updateDTO.CONVERT_RATIO = null;
                }

                //updateDTO.HAS_SHELL = (short)(checkEdit1.Checked ? 1 : 0);
                if (spinMaterialNumOder.EditValue != null)
                {
                    updateDTO.MATERIAL_NUM_ORDER = (long)spinMaterialNumOder.Value;
                }
                else
                {
                    updateDTO.MATERIAL_NUM_ORDER = null;
                }
                if (spinMedicineNumOder.EditValue != null)
                {
                    updateDTO.MEDICINE_NUM_ORDER = (long)spinMedicineNumOder.Value;
                }
                else
                {
                    updateDTO.MEDICINE_NUM_ORDER = null;
                }
                if (chkServiceUnitMin.Checked)
                    updateDTO.IS_PRIMARY = 1;
                else
                    updateDTO.IS_PRIMARY = 0;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region xử lý focus
        private void txtServiceUnitCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtServiceUnitName.Focus();
                    txtServiceUnitName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceUnitName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtServiceUnitSymbol.Focus();
                    txtServiceUnitSymbol.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceUnitSymbol_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinMedicineNumOder.Focus();
                    spinMedicineNumOder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinHasShell_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinMedicineNumOder.Focus();
                    spinMedicineNumOder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinMedicineNumOder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinMaterialNumOder.Focus();
                    spinMaterialNumOder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinMaterialNumOder_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboConvert.Focus();
                    cboConvert.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboConvert_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtConvertRatio.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void txtConvertRatio_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            if (btnAdd.Enabled == true)
        //            {
        //                btnAdd.Focus();
        //            }
        //            else
        //            {
        //                btnEdit.Focus();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void checkEdit1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled == true)
                    {
                        spinMedicineNumOder.Focus();
                    }
                    else
                    {
                        spinMedicineNumOder.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void EditProcess()
        {
            try
            {
                bool success = false;
                positionHandle = -1;
                CommonParam param = new CommonParam();
                if (serviceUnitEditId > 0)
                {
                    updateDTO.ID = serviceUnitEditId;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_SERVICE_UNIT>(
                    "api/HisServiceUnit/Update", ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        UpdateRowDataAfterEdit(resultData);
                        ResetFormData();
                    }
                }

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceUnit_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                currentData = (HIS_SERVICE_UNIT)gridViewServiceUnit.GetFocusedRow();

                if (currentData != null)
                {
                    serviceUnitEditId = currentData.ID;
                    updateDTO.SERVICE_UNIT_CODE = currentData.SERVICE_UNIT_CODE;
                    updateDTO.SERVICE_UNIT_NAME = currentData.SERVICE_UNIT_NAME;
                    updateDTO.SERVICE_UNIT_SYMBOL = currentData.SERVICE_UNIT_SYMBOL;
                    //updateDTO.HAS_SHELL = currentData.HAS_SHELL;
                    updateDTO.MATERIAL_NUM_ORDER = currentData.MATERIAL_NUM_ORDER;
                    updateDTO.MEDICINE_NUM_ORDER = currentData.MEDICINE_NUM_ORDER;
                    updateDTO.IS_PRIMARY = currentData.IS_PRIMARY;
                    EditProcess();

                }
                ChangedDataRow(currentData);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboConvert_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboConvert.EditValue = null;
                    this.lcConvertRatio.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboConvert_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                //if (e.CloseMode == PopupCloseMode.Normal)
                //{
                //    txtConvertRatio.Focus();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtConvertRatio_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled == true)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnEdit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboConvert_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtConvertRatio_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtConvertRatio_EditValueChanged(object sender, EventArgs e)
        {

        }


    }
}

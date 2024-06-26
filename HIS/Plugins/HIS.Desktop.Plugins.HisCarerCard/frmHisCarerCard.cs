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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
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

namespace HIS.Desktop.Plugins.HisCarerCard
{
    public partial class frmHisCarerCard : FormBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<HIS_SERVICE> lstService = new List<HIS_SERVICE>();
        int ActionType = -1;
        int positionHandle = -1;
        HIS_CARER_CARD currentData = new HIS_CARER_CARD();

        public frmHisCarerCard()
        {
            InitializeComponent();
        }
        public frmHisCarerCard(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisCarerCardImport", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmHisCarerCard_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MeShow()
        {
            try
            {
                SetDefaultValue();
                EnableControlChanged(this.ActionType);
                InitComboService();
                FillDataFormList();
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(cboService);
                ValidationRequireAndMaxLength(txtCardNumber, dxValidationProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationRequireAndMaxLength(DevExpress.XtraEditors.TextEdit txtCardNumber, DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ValidationRequireAndMaxLength validRule = new ValidationRequireAndMaxLength();
                validRule.textEdit = txtCardNumber;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(txtCardNumber, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(DevExpress.XtraEditors.GridLookUpEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtService, validRule);
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

                txtKeyword.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboService()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceFilter filter = new HisServiceFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KHAC;
                var data = new BackendAdapter(param).Get<List<HIS_SERVICE>>("api/HisService/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                lstService = new List<HIS_SERVICE>();
                lstService = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboService, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataFormList()
        {
            try
            {
                HisCarerCardFilter filter = new HisCarerCardFilter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                SetFilter(ref filter);
                var lstCarerCard = new BackendAdapter(new CommonParam()).Get<List<HIS_CARER_CARD>>("api/HisCarerCard/Get", ApiConsumers.MosConsumer, filter, null);
                grdCarerCard.DataSource = null;
                grdCarerCard.DataSource = lstCarerCard;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref HisCarerCardFilter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtKeyword.Text))
                {
                    filter.KEY_WORD = txtKeyword.Text.Trim();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (XtraMessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (HIS_CARER_CARD)grvCarerCard.GetFocusedRow();
                    if (rowData != null)
                    {

                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>("api/HisCarerCard/Delete", ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<HIS_CARER_CARD>();
                            this.ActionType = 1;
                            EnableControlChanged(this.ActionType);
                            FillDataFormList();
                        }
                        MessageManager.Show(this, param, success);
                        btnCancel_Click(null, null);
                    }
                }
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

        private void btnCancel_Click(object sender, EventArgs e)
        {

            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                txtCardNumber.Text = "";
                txtKeyword.Text = "";
                txtService.Text = "";
                cboService.EditValue = null;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                FillDataFormList();

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
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                WaitingManager.Show();
                HIS_CARER_CARD updateDTO = new HIS_CARER_CARD();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<HIS_CARER_CARD>("api/HisCarerCard/Create", ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {

                        success = true;
                        
                        txtKeyword.Text = "";
                        txtCardNumber.Text = "";
                        txtService.Text = "";
                        cboService.EditValue = null;
                        FillDataFormList();
                        currentData = new HIS_CARER_CARD();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<HIS_CARER_CARD>("api/HisCarerCard/Update", ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        
                        txtKeyword.Text = "";
                        txtCardNumber.Text = "";
                        txtService.Text = "";
                        cboService.EditValue = null;
                        FillDataFormList();
                        currentData = new HIS_CARER_CARD();
                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<HIS_CARER_CARD>();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref HIS_CARER_CARD updateDTO)
        {
            try
            {
                updateDTO.SERVICE_ID = Convert.ToInt64(cboService.EditValue);
                updateDTO.CARER_CARD_NUMBER = txtCardNumber.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref HIS_CARER_CARD currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisCarerCardFilter filter = new HisCarerCardFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<HIS_CARER_CARD>>("api/HisCarerCard/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvCarerCard_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                HIS_CARER_CARD data = null;
                if (e.RowHandle > -1)
                {
                    data = (HIS_CARER_CARD)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? repositoryItemButtonLock : repositoryItemButtonUnlock);
                    }
                    else if (e.Column.FieldName == "Lost")
                    {
                        e.RepositoryItem = (data.IS_LOST == 1 ? repositoryItemButtonUnBorrowCard : repositoryItemButtonBorrowCard);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool rs = false;
            HIS_CARER_CARD success = new HIS_CARER_CARD();
            //bool notHandler = false;
            try
            {

                HIS_CARER_CARD data = (HIS_CARER_CARD)grvCarerCard.GetFocusedRow();
                if (XtraMessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_CARER_CARD>("api/HiscarerCard/ChangeLock", ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<HIS_CARER_CARD>();
                        rs = true;
                        FillDataFormList();
                    }
                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, rs);
                    #endregion
                    btnCancel_Click(null, null);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonUnlock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool rs = false;
            HIS_CARER_CARD success = new HIS_CARER_CARD();
            //bool notHandler = false;

            try
            {

                HIS_CARER_CARD data = (HIS_CARER_CARD)grvCarerCard.GetFocusedRow();
                if (XtraMessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_CARER_CARD>("api/HiscarerCard/ChangeLock", ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<HIS_CARER_CARD>();
                        rs = true;
                        FillDataFormList();
                    }
                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, rs);
                    #endregion
                    btnCancel_Click(null, null);
                }

            }

            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonBorrowCard_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool rs = false;
            //bool notHandler = false;

            try
            {

                HIS_CARER_CARD data = (HIS_CARER_CARD)grvCarerCard.GetFocusedRow();
                if (XtraMessageBox.Show("Bạn có muốn báo mất không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisCarerCard/Lost", ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (rs)
                    {
                        BackendDataWorker.Reset<HIS_CARER_CARD>();
                        FillDataFormList();
                    }
                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, rs);
                    #endregion
                    btnCancel_Click(null, null);
                }

            }

            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonUnBorrowCard_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool rs = false;
            //bool notHandler = false;

            try
            {

                HIS_CARER_CARD data = (HIS_CARER_CARD)grvCarerCard.GetFocusedRow();
                if (XtraMessageBox.Show("Bạn có muốn hủy báo mất không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisCarerCard/CancelLost", ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (rs)
                    {
                        BackendDataWorker.Reset<HIS_CARER_CARD>();
                        FillDataFormList();
                    }
                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, rs);
                    #endregion
                    btnCancel_Click(null, null);
                }

            }

            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void grvCarerCard_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (HIS_CARER_CARD)grvCarerCard.GetFocusedRow();
                if (this.currentData != null)
                {
                    ChangedDataRow(this.currentData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(HIS_CARER_CARD data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(HIS_CARER_CARD data)
        {
            try
            {
                if (data != null)
                {
                    txtCardNumber.Text = data.CARER_CARD_NUMBER;
                    cboService.EditValue = data.SERVICE_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grvCarerCard_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_CARER_CARD pData = (HIS_CARER_CARD)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }

                    else if (e.Column.FieldName == "Is_Borrowed")
                    {
                        e.Value = pData.IS_BORROWED == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "Is_Lost")
                    {
                        e.Value = pData.IS_LOST == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboService_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboService.EditValue != null)
                {
                    txtService.Text = lstService.Where(o => o.ID == Convert.ToInt64(cboService.EditValue)).FirstOrDefault().SERVICE_CODE.ToString();
                }
                else
                {
                    txtService.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtService_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                var sv = sender as TextEdit;
                if (e.KeyCode == Keys.Enter)
                {
                   
                    if (!string.IsNullOrEmpty(sv.Text))
                    {
                        var service = lstService.Where(o => o.SERVICE_CODE.ToLower().Contains(sv.Text.ToLower())).ToList();
                        if (service != null)
                        {
                            txtService.Text = service.First().SERVICE_CODE;
                            cboService.EditValue = service.First().ID;
                        }
                    }
                    else
                    {
                        cboService.EditValue = null;
                    }
                    cboService.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonHistory_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (HIS_CARER_CARD)grvCarerCard.GetFocusedRow();
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("currentData_________", currentData));
                
                frmCarerCardBorrow frm = new frmCarerCardBorrow(this.currentData.ID);
                frm.ShowDialog();
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

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataFormList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnImport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataFormList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCardNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtService.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboService_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboService.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

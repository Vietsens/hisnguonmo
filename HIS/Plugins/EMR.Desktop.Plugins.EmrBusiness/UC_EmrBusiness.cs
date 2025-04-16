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
using Inventec.Desktop.Common.Modules;
using Inventec.UC.Paging;
using Inventec.Common.Logging;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using EMR.Desktop.Plugins.EmrBusiness.Validate;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using EMR.Filter;
using EMR.EFMODEL.DataModels;
using Inventec.UC.Paging;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using EMR.URI;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using System.Resources;
using EMR.Desktop.Plugins.EmrBusiness.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;

namespace EMR.Desktop.Plugins.EmrBusiness
{
    public partial class UC_EmrBusiness : HIS.Desktop.Utility.UserControlBase
    {
        #region Declera variable
        Module moduleData;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandler = -1;
        int startpage = 0;
        int dataTotal = 0;
        int rowdata = 0;
        long EmrBusinessID;
        EMR_BUSINESS currentData;
        List<HIS_DEPARTMENT> listdata = new List<HIS_DEPARTMENT>();
        #endregion

        #region --private method
        public UC_EmrBusiness(Module mudule)
            : base(mudule)
        {

            InitializeComponent();
            pagingGrid = new PagingGrid();
            this.moduleData = mudule;
            try
            {

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }

        }

        private void UC_EmrBusiness_Load(object sender, EventArgs e)
        {
            try
            {
                Meshow();
                //LoadDataToComboDepartment();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void Meshow()
        {
            try
            {
                SetDefautValue();
                EnableControlChanged(this.ActionType);
                ValidateForm();
                FillDataToGridControl();
                SetCaptionByLanguageKey();
                LoadDataToComboDepartment();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void SetDefautValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtSearch.Text = "";
                txtBussinessCode.Text = "";
                txtBussinessName.Text = "";
                cboDepartment.EditValue = null;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int Action)
        {
            try
            {
                btnEdit.Enabled = (Action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (Action == GlobalVariables.ActionAdd);

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void RestFormData()
        {
            try
            {
                if (!lcEditorInfo.IsInitialized)
                    return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            txtBussinessCode.Focus();
                            txtBussinessCode.SelectAll();
                        }
                    }

                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPagin(object commonParam)
        {
            try
            {
                this.startpage = ((CommonParam)commonParam).Start ?? 0;
                int limit = ((CommonParam)commonParam).Limit ?? 0;
                CommonParam param = new CommonParam(startpage, limit);
                ApiResultObject<List<EMR_BUSINESS>> apiResult = null;
                EmrBusinessFilter filter = new EmrBusinessFilter();
                SetFilter(ref filter);
                grdControlEmrBussiness.DataSource = null;
                grdViewEmrBussiness.BeginUpdate();
                apiResult = new BackendAdapter(param).GetRO<List<EMR_BUSINESS>>(EMR.URI.EmrBusiness.GET, ApiConsumers.EmrConsumer, filter, param);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        grdControlEmrBussiness.DataSource = data;
                        rowdata = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                grdViewEmrBussiness.EndUpdate();
                #region process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                grdViewEmrBussiness.EndUpdate();
                LogSystem.Error(ex);
            }
        }
        //Dangth
        private void LoadDataToComboDepartment()
        {
            try
            {
                List<HIS_DEPARTMENT> data = BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "DEPARTMENT_CODE", columnInfos, false, 350);
                //Load data vao combobox
                ControlEditorLoader.Load(cboDepartment, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFilter(ref EmrBusinessFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.KEY_WORD = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("EMR.Desktop.Plugins.EmrBusiness.Resources.Lang", typeof(EMR.Desktop.Plugins.EmrBusiness.UC_EmrBusiness).Assembly);
                this.lcBussinessCode.Text = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.lcBussinessCode.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcBussinessName.Text = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.lcBussinessName.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.btnEdit.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.btnAdd.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.btnSearch.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRest.Text = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.btnRest.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCoLSTT.Caption = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.grdCoLSTT.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdBussinessCode.Caption = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.grdBussinessCode.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBussinessName.Caption = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.grdColBussinessName.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.grdColCreateTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.grdColCreator.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsAction.Caption = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.grdColIsAction.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.grdColModifyTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.grdColModifier.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCoLSTT.ToolTip = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.grdCoLSTT.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdBussinessCode.ToolTip = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.grdBussinessCode.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBussinessName.ToolTip = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.grdColBussinessName.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsAction.ToolTip = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.grdColIsAction.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.grdColCreateTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.grdColCreator.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.grdColModifyTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("UC_EmrBusiness.grdColModifier.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region --Validate
        private void ValidateForm()
        {
            try
            {
                ValidateBussinessCode();
                ValidateBussinessName();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ValidateBussinessCode()
        {
            try
            {
                ValidateMaxlangth validate = new ValidateMaxlangth();
                validate.txtControl = txtBussinessCode;
                validate.Maxlength = 20;
                validate.message = ResourcesMassage.MaNghiepVuVuaQuaMaxLangth;
                validate.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtBussinessCode, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ValidateBussinessName()
        {
            try
            {
                ValidateMaxlangth validate = new ValidateMaxlangth();
                validate.txtControl = txtBussinessName;
                validate.Maxlength = 100;
                validate.message = ResourcesMassage.TenNghiepVuVuotQuaMaxLangth;
                validate.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtBussinessName, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    pagingSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    pagingSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPagin(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Start = rowdata;
                param.Count = dataTotal;
                ucPaging2.Init(LoadPagin, param, pagingSize, this.grdControlEmrBussiness);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        #endregion

        #region --Button click
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
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

                LogSystem.Warn(ex);
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

                LogSystem.Warn(ex);
            }
        }

        private void btnRest_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                this.positionHandler = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                RestFormData();
                txtBussinessCode.Focus();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            EMR_BUSINESS EmrBusiness = new EMR_BUSINESS();
            try
            {
                EMR_BUSINESS data = (EMR_BUSINESS)grdViewEmrBussiness.GetFocusedRow();
                if (MessageBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    EmrBusiness = new BackendAdapter(param).Post<EMR_BUSINESS>(EMR.URI.EmrBusiness.LOCK, ApiConsumers.EmrConsumer, data, param);
                    WaitingManager.Hide();
                    if (EmrBusiness != null) FillDataToGridControl();
                }
                success = true;
                BackendDataWorker.Reset<EMR_BUSINESS>();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnUnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            EMR_BUSINESS EmrBusiness = new EMR_BUSINESS();
            bool success = false;
            try
            {
                EMR_BUSINESS data = (EMR_BUSINESS)grdViewEmrBussiness.GetFocusedRow();
                if (MessageBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    EmrBusiness = new BackendAdapter(param).Post<EMR_BUSINESS>(EMR.URI.EmrBusiness.UNLOCK, ApiConsumers.EmrConsumer, data, param);
                    WaitingManager.Hide();
                    if (EmrBusiness != null)
                        FillDataToGridControl();
                }
                success = true;
                BackendDataWorker.Reset<EMR_BUSINESS>();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                CommonParam param = new CommonParam();
                bool succes = false;
                EMR_BUSINESS datarow = (EMR_BUSINESS)grdViewEmrBussiness.GetFocusedRow();
                if (MessageBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    succes = new BackendAdapter(param).Post<bool>(EMR.URI.EmrBusiness.DELETE, ApiConsumers.EmrConsumer, datarow.ID, null);
                    if (succes)
                    {
                        FillDataToGridControl();
                        btnRest_Click(null, null);
                        currentData = ((List<EMR_BUSINESS>)grdControlEmrBussiness.DataSource).FirstOrDefault();
                    }
                    MessageManager.Show(this.ParentForm, param, succes);
                }

            }
            catch (Exception ex)
            {

                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnEmrFlow_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (EMR_BUSINESS)grdViewEmrBussiness.GetFocusedRow();
                EMR_BUSINESS row = data as EMR_BUSINESS;
                if (row != null)
                {

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);

                    if (this.moduleData != null)
                    {
                        CallModule callModule = new CallModule(CallModule.EmrFlow, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.EmrFlow, 0, 0, listArgs);
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
        #region --ShotCut
        public void BtnSearch()
        {
            try
            {
                if (btnSearch.Enabled)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        public void BtnEdit()
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

                LogSystem.Warn(ex);
            }
        }
        public void BtnAdd()
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                {
                    btnAdd_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        public void BtnRest()
        {
            try
            {
                btnRest_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion

        #endregion

        #region --ChangeData
        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                EMR_BUSINESS data = new EMR_BUSINESS();
                UpdateDTOfromDataFrom(ref data);
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    var resultDat = new BackendAdapter(param).Post<EMR_BUSINESS>(EMR.URI.EmrBusiness.CREATE, ApiConsumers.EmrConsumer, data, param);
                    if (resultDat != null)
                    {
                        BackendDataWorker.Reset<EMR_BUSINESS>();
                        success = true;
                        FillDataToGridControl();
                        RestFormData();
                    }
                }
                else
                {
                    if (EmrBusinessID > 0)
                    {
                        data.ID = EmrBusinessID;
                        var resultData = new BackendAdapter(param).Post<EMR_BUSINESS>(EMR.URI.EmrBusiness.UPDATE, ApiConsumers.EmrConsumer, data, param);
                        if (resultData != null)
                        {
                            BackendDataWorker.Reset<EMR_BUSINESS>();
                            success = true;
                            FillDataToGridControl();
                        }
                    }
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void UpdateDTOfromDataFrom(ref EMR_BUSINESS data)
        {
            try
            {
                data.BUSINESS_CODE = txtBussinessCode.Text.Trim();
                data.BUSINESS_NAME = txtBussinessName.Text.Trim();
                data.DEPARTMENT_CODE = cboDepartment.EditValue.ToString(); // Lưu mã khoa                
                data.DEPARTMENT_NAME = cboDepartment.Text.Trim(); // Lưu tên khoa
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region --KeyDow
        private void txtBussinessCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBussinessName.Focus();
                    txtBussinessName.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtBussinessName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                    {
                        btnEdit.Focus();
                    }
                    else if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    else
                        btnRest.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region --even grid
        private void grdViewEmrBussiness_Click(object sender, EventArgs e)
        {
            try
            {
                EMR_BUSINESS RowData = (EMR_BUSINESS)grdViewEmrBussiness.GetFocusedRow();
                if (RowData != null)
                {
                    currentData = RowData;
                    ChangDataRow(RowData);
                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void FillDataToEditorControl(EMR_BUSINESS data)
        {
            try
            {
                if (data != null)
                {
                    EmrBusinessID = data.ID;
                    txtBussinessCode.Text = data.BUSINESS_CODE;
                    txtBussinessName.Text = data.BUSINESS_NAME;
                    cboDepartment.EditValue = data.DEPARTMENT_CODE; // Gán mã khoa để hiển thị tên khoa
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ChangDataRow(EMR_BUSINESS Data)
        {
            try
            {
                if (Data != null)
                {
                    FillDataToEditorControl(Data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    if (currentData != null)
                    {
                        btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    }
                    positionHandler = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void grdViewEmrBussiness_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    EMR_BUSINESS RowData = (EMR_BUSINESS)grdViewEmrBussiness.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        if (RowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else
                            e.Appearance.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void grdViewEmrBussiness_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    EMR_BUSINESS data = (EMR_BUSINESS)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startpage;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            e.Value = data.IS_ACTIVE == 1 ? "Hoạt động" : "Tạm khóa";
                        }
                    }
                }


            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void grdViewEmrBussiness_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    EMR_BUSINESS RowData = (EMR_BUSINESS)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (RowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnUnLock : btnLock);
                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (RowData.IS_ACTIVE == 1)
                            e.RepositoryItem = btnDelete;
                        else
                            e.RepositoryItem = btnEnableDelete;
                    }
                    if (e.Column.FieldName == "EmrFlow")
                    {
                        if (RowData.IS_ACTIVE == 1)
                            e.RepositoryItem = btnEmrFlow;
                        else
                            e.RepositoryItem = btnEnableEmrFlow;
                    }

                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void grdViewEmrBussiness_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var data = (EMR_BUSINESS)grdViewEmrBussiness.GetFocusedRow();
                    ChangDataRow(data);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void cboDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }

        #endregion

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            //try
            //{
            //    if (e.CloseMode == PopupCloseMode.Normal)
            //    {
            //        if (cboDepartment.EditValue != null)
            //        {
            //            var dataStore = listdata.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "").ToString()));
            //            if (dataStore != null)
            //            {
            //                cboDepartment.Properties.Buttons[1].Visible = true;
            //                if (btnAdd.Enabled)
            //                {
            //                    btnAdd.Focus();
            //                    btnAdd.Select();
            //                }
            //                else
            //                {
            //                    btnEdit.Focus();
            //                    btnEdit.Select();
            //                }
            //            }
            //            else
            //            {
            //                cboDepartment.Focus();
            //                cboDepartment.ShowPopup();
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void cboDepartment_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

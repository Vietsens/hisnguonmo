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
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.HisDepartment.Resources;
using HIS.Desktop.Plugins.HisFilmSize.Validate;

namespace HIS.Desktop.Plugins.HisFilmSize.HisFilmSize
{
    public partial class frmHisFilmSize : FormBase
    {
        #region Reclare variables
        PagingGrid pagingGrid;
        int ActionType = -1;
        int RowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int positionHandle = -1;
        long filmSizeID;
        int IS_ACTIVE_FALSE = 0;
        int IS_ACTIVE_TRUE = 1;


        Inventec.Desktop.Common.Modules.Module Moduledate;
        HIS_FILM_SIZE currentDate;
        #endregion

        public frmHisFilmSize(Inventec.Desktop.Common.Modules.Module modeleData)
            : base(modeleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.Moduledate = modeleData;
                grdCotrolHisFilmSize.ToolTipController = toolTipController1;

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
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

        private void frmHisFilmSize_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValues();
                EnableControlChanged(this.ActionType);
                Validate();
                FillDataToGridControl();
                SetCaptionByLanguageKey();

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #region ---Load data
        private void SetDefaultValues()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtFilmSizeCode.Text = "";
                txtFilmSizeName.Text = "";
                txtSearch.Text = "";
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int actionType)
        {
            try
            {
                btnEdit.Enabled = (actionType == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (actionType == GlobalVariables.ActionAdd);
                txtFilmSizeCode.ReadOnly = !(actionType == GlobalVariables.ActionAdd);
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
                if (this.Moduledate != null && !String.IsNullOrEmpty(this.Moduledate.text))
                {
                    this.Text = this.Moduledate.text;
                }
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisFilmSize.Resources.Lang", typeof(HIS.Desktop.Plugins.HisFilmSize.HisFilmSize.frmHisFilmSize).Assembly);
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisFilmSize.btnAdd.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisFilmSize.btnEdit.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRest.Text = Inventec.Common.Resource.Get.Value("frmHisFilmSize.btnRest.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisFilmSize.btnSearch.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcFilmSizeCode.Text = Inventec.Common.Resource.Get.Value("frmHisFilmSize.lcFilmSizeCode.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcFilmSizeName.Text = Inventec.Common.Resource.Get.Value("frmHisFilmSize.lcFilmSizeName.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisFilmSize.STT.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.ToolTip = Inventec.Common.Resource.Get.Value("frmHisFilmSize.STT.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFilmSizeCode.Caption = Inventec.Common.Resource.Get.Value("frmHisFilmSize.grdColFilmSizeCode.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFilmSizeCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisFilmSize.grdColFilmSizeCode.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFilmSizeName.Caption = Inventec.Common.Resource.Get.Value("frmHisFilmSize.grdColFilmSizeName.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFilmSizeName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisFilmSize.grdColFilmSizeName.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsActive.Caption = Inventec.Common.Resource.Get.Value("frmHisFilmSize.grdColIsActive.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsActive.ToolTip = Inventec.Common.Resource.Get.Value("frmHisFilmSize.grdColIsActive.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisFilmSize.grdColCreateTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisFilmSize.grdColCreateTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisFilmSize.grdColCreator.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisFilmSize.grdColCreator.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisFilmSize.grdColModifyTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisFilmSize.grdColModifyTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisFilmSize.grdColModifier.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisFilmSize.grdColModifier.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
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
                HIS_FILM_SIZE updateDTO = new HIS_FILM_SIZE();
                UpdataDTOFromDataForm(ref updateDTO);
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<HIS_FILM_SIZE>(HisRequestUriStore.HisFilmSize_Create, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        BackendDataWorker.Reset<HIS_FILM_SIZE>();
                        success = true;
                        FillDataToGridControl();
                        RestFromData();
                    }
                }
                else
                {
                    if (filmSizeID > 0)
                    {
                        updateDTO.ID = filmSizeID;
                        var resultData = new BackendAdapter(param).Post<HIS_FILM_SIZE>(HisRequestUriStore.HisFilmSize_Update, ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            BackendDataWorker.Reset<HIS_FILM_SIZE>();
                            success = true;
                            FillDataToGridControl();

                        }
                    }
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private void UpdataDTOFromDataForm(ref HIS_FILM_SIZE updateDTO)
        {
            try
            {
                updateDTO.FILM_SIZE_CODE = txtFilmSizeCode.Text.Trim();
                updateDTO.FILM_SIZE_NAME = txtFilmSizeName.Text.Trim();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        #region validate
        private void Validate()
        {
            try
            {
                ValidateTxtFilmSizeCode();
                ValidateTxtFilmSizeName();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ValidateTxtFilmSizeCode()
        {
            try
            {
                ValidateMaxLength validate = new ValidateMaxLength();
                validate.txtControl = txtFilmSizeCode;
                validate.MaxLength = 20;
                validate.messageErro = ResourcesMassage.MaVuotQuaMaxLength;
                validate.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtFilmSizeCode, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ValidateTxtFilmSizeName()
        {
            try
            {
                ValidateMaxLength validate = new ValidateMaxLength();
                validate.txtControl = txtFilmSizeName;
                validate.MaxLength = 50;
                validate.messageErro = ResourcesMassage.TenVuaQuaMaxLength;
                validate.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtFilmSizeName, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion

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
                LoadPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = RowCount;
                param.Count = dataTotal;
                ucPaging2.Init(LoadPaging, param, pagingSize, this.grdCotrolHisFilmSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object commonParam)
        {
            startPage = ((CommonParam)commonParam).Start ?? 0;
            int limit = ((CommonParam)commonParam).Limit ?? 0;
            CommonParam paramcommon = new CommonParam(startPage, limit);
            ApiResultObject<List<HIS_FILM_SIZE>> apiResult = null;
            HisFilmSizeFilter filter = new HisFilmSizeFilter();
            SetFilter(ref filter);
            grdCotrolHisFilmSize.DataSource = null;
            grdViewHisFilmSize.BeginUpdate();
            apiResult = new BackendAdapter(paramcommon).GetRO<List<HIS_FILM_SIZE>>(HisRequestUriStore.HisFilmSize_Get, ApiConsumers.MosConsumer, filter, paramcommon);
            if (apiResult != null)
            {
                var data = (List<HIS_FILM_SIZE>)apiResult.Data;
                if (data != null && data.Count > 0)
                {
                    grdCotrolHisFilmSize.DataSource = data;
                    RowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);

                }
                grdViewHisFilmSize.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramcommon);
                #endregion
            }

        }
        private void SetFilter(ref HisFilmSizeFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.KEY_WORD = txtSearch.Text;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void RestFromData()
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
                            txtFilmSizeCode.Focus();
                            txtFilmSizeCode.SelectAll();
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
        #endregion
        #region ---button

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnRest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnAdd_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnRestFocus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

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
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                RestFromData();
                txtFilmSizeCode.Focus();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                HIS_FILM_SIZE filmsize = new HIS_FILM_SIZE();
                bool success = false;
                try
                {
                    HIS_FILM_SIZE data = (HIS_FILM_SIZE)grdViewHisFilmSize.GetFocusedRow();
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        filmsize = new BackendAdapter(param).Post<HIS_FILM_SIZE>(HisRequestUriStore.HisFilmSize_ChangeLock, ApiConsumers.MosConsumer, data.ID, param);
                        WaitingManager.Hide();
                        if (filmsize != null) FillDataToGridControl();
                    }
                    success = true;
                    BackendDataWorker.Reset<HIS_FILM_SIZE>();
                    MessageManager.Show(this.ParentForm, param, success);

                }
                catch (Exception ex)
                {

                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnUnLock_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                HIS_FILM_SIZE filmSize = new HIS_FILM_SIZE();
                bool success = false;
                try
                {
                    HIS_FILM_SIZE data = (HIS_FILM_SIZE)grdViewHisFilmSize.GetFocusedRow();
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        filmSize = new BackendAdapter(param).Post<HIS_FILM_SIZE>(HisRequestUriStore.HisFilmSize_ChangeLock, ApiConsumers.MosConsumer, data.ID, param);
                        WaitingManager.Hide();
                        if (filmSize != null) FillDataToGridControl();
                    }
                    success = true;
                    BackendDataWorker.Reset<HIS_FILM_SIZE>();
                    MessageManager.Show(this.ParentForm, param, success);
                }
                catch (Exception ex)
                {

                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (HIS_FILM_SIZE)grdViewHisFilmSize.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HisFilmSize_Delete, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            btnRest_Click(null, null);
                            currentDate = ((List<HIS_FILM_SIZE>)grdCotrolHisFilmSize.DataSource).FirstOrDefault();
                            
                        }
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                }
            }
            catch (Exception ex)
            {

                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region ---KeyDown
        private void txtFilmSizeCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFilmSizeName.Focus();
                    txtFilmSizeName.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSession.Warn(ex);
            }
        }

        private void txtFilmSizeName_KeyDown(object sender, KeyEventArgs e)
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
                    {
                        btnRest.Focus();
                    }
                }
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
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
        #region --even gridview
        private void grdViewHisFilmSize_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (HIS_FILM_SIZE)grdViewHisFilmSize.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangeDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void ChangeDataRow(HIS_FILM_SIZE data)
        {
            try
            {
                if (data != null)
                {
                    FillDataEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    btnEdit.Enabled = (this.currentDate.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void FillDataEditorControl(HIS_FILM_SIZE data)
        {
            try
            {
                if (data != null)
                {
                    filmSizeID = data.ID;
                    txtFilmSizeCode.Text = data.FILM_SIZE_CODE;
                    txtFilmSizeName.Text = data.FILM_SIZE_NAME;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void grdViewHisFilmSize_Click(object sender, EventArgs e)
        {
            try
            {
                var rowdata = (HIS_FILM_SIZE)grdViewHisFilmSize.GetFocusedRow();
                if (rowdata != null)
                {
                    currentDate = rowdata;
                    ChangeDataRow(rowdata);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void grdViewHisFilmSize_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_FILM_SIZE data = (HIS_FILM_SIZE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IS_ACTIVE_FALSE ? btnUnLock : btnLock);

                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (data.IS_ACTIVE == IS_ACTIVE_TRUE)
                        {
                            e.RepositoryItem = btnDelete;
                        }
                        else e.RepositoryItem = btnEnableDelete;
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void grdViewHisFilmSize_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    HIS_FILM_SIZE data = (HIS_FILM_SIZE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
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

                LogSystem.Error(ex);
            }
        }

        private void grdViewHisFilmSize_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_FILM_SIZE data = (HIS_FILM_SIZE)grdViewHisFilmSize.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            e.Appearance.ForeColor = Color.Red;
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

        #endregion

        
    }
}

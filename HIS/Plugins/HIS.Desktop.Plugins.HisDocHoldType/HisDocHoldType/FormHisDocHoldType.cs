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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using Inventec.UC.Paging;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.HisDocHoldType.Validates;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisBidTest.Resources;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.HisDocHoldType.HisDocHoldType
{
    public partial class FormHisDocHoldType : FormBase
    {
        #region Reclare variables
        Inventec.Desktop.Common.Modules.Module moduleData;
        PagingGrid pagingGrid;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int ActionType = -1;
        int positionHandle = -1;
        long DocHodlTypeID;
        HIS_DOC_HOLD_TYPE currentData;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        #endregion
        #region Construct
        public FormHisDocHoldType(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlHisDocHoldType.ToolTipController = toolTipControllerGrid;

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
        #endregion
        #region Private Methor
        private void FormHisDocHoldType_Load(object sender, EventArgs e)
        {
            SetDefaultValue();
            EnableControlChanged(this.ActionType);
            FillDataToControl();
            ValidateForm();
            SetCaptionByLanguagekey();
        }

        private void SetCaptionByLanguagekey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisDocHoldType.Resources.Lang", typeof(HIS.Desktop.Plugins.HisDocHoldType.HisDocHoldType.FormHisDocHoldType).Assembly);
                this.lcDocHoldTypeCode.Text = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.lcDocHoldTypeCode.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcDocHoldName.Text = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.lcDocHoldName.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.btnEdit.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.btnAdd.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRest.Text = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.btnRest.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.btnSearch.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCloDocHoldTypeCode.Caption = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.grdCloDocHoldTypeCode.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCloDocHoldTypeCode.ToolTip = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.grdCloDocHoldTypeCode.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.STT.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.ToolTip = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.STT.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDelete.Caption = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.grdColDelete.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDelete.ToolTip = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.grdColDelete.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColLock.Caption = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.grdColLock.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColLock.ToolTip = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.grdColLock.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDocHoldTypeName.Caption = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.grdColDocHoldTypeName.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDocHoldTypeName.ToolTip = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.grdColDocHoldTypeName.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.grdColCreateTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.grdColCreateTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColCreator.Caption = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.gridColCreator.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColCreator.ToolTip = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.gridColCreator.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.grdColModifier.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.grdColModifier.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.grdColModifyTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("FormHisDocHoldType.grdColModifyTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtSearch.Text = "";
                txtDocHoldTypeCode.Text = "";
                txtDocHoldTypeName.Text = "";
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SetFocusEditor()
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
                txtDocHoldTypeCode.ReadOnly = !(action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void changeDataRow(HIS_DOC_HOLD_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    fillDataEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(ActionType);
                    if (currentData != null)
                    {
                        btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    }
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void fillDataEditorControl(HIS_DOC_HOLD_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    DocHodlTypeID = data.ID;
                    txtDocHoldTypeCode.Text = data.DOC_HOLD_TYPE_CODE;
                    txtDocHoldTypeName.Text = data.DOC_HOLD_TYPE_NAME;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void UpdataDTOFromDataFrom(ref HIS_DOC_HOLD_TYPE data)
        {
            try
            {
                data.DOC_HOLD_TYPE_CODE = txtDocHoldTypeCode.Text.Trim();
                data.DOC_HOLD_TYPE_NAME = txtDocHoldTypeName.Text.Trim();
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
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                HIS_DOC_HOLD_TYPE updateDTO = new HIS_DOC_HOLD_TYPE();
                UpdataDTOFromDataFrom(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<HIS_DOC_HOLD_TYPE>(HisRequestUriStore.MOSHIS_DOC_HOLD_TYPE_CREATE, ApiConsumer.ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        BackendDataWorker.Reset<HIS_DOC_HOLD_TYPE>();
                        success = true;
                        FillDataToControl();
                        RestFormData();
                    }
                   
                }
                else
                {
                    if (DocHodlTypeID > 0)
                    {
                        updateDTO.ID = DocHodlTypeID;
                        var ResultData = new BackendAdapter(param).Post<HIS_DOC_HOLD_TYPE>(HisRequestUriStore.MOSHIS_DOC_HOLD_TYPE_UPDATE, ApiConsumer.ApiConsumers.MosConsumer, updateDTO, param);
                        if (ResultData != null)
                        {
                            BackendDataWorker.Reset<HIS_DOC_HOLD_TYPE>();
                            success = true;
                            FillDataToControl();
                        }

                    }
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {

                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                //TODO
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
        #region Load data to grid
        private void FillDataToControl()
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
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(LoadPaging, param, pagingSize, this.gridControlHisDocHoldType);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {

            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                //limit=pageSize
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramcommon = new CommonParam(startPage, limit);
                ApiResultObject<List<HIS_DOC_HOLD_TYPE>> apiResult = null;
                MOS.Filter.HisDocHoldTypeFilter filter = new MOS.Filter.HisDocHoldTypeFilter();
                SetFilter(ref filter);
                gridControlHisDocHoldType.DataSource = null;
                gridViewHisDocHoldType.BeginUpdate();
                apiResult = new BackendAdapter(paramcommon).GetRO<List<HIS_DOC_HOLD_TYPE>>(HisRequestUriStore.MOSHIS_DOC_HOLD_TYPE_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, paramcommon);
                if (apiResult != null)
                {
                    var data = (List<HIS_DOC_HOLD_TYPE>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlHisDocHoldType.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }

                }
              
                gridViewHisDocHoldType.EndUpdate();
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramcommon);
                #endregion

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref MOS.Filter.HisDocHoldTypeFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.KEY_WORD = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #endregion
        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidatetxtDocHoldTypeCode();
                ValidatetxtDocHoldTypeName();
                ValidatetxtDocHoldTypeGroupCode();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void ValidatetxtDocHoldTypeCode()
        {
            try
            {
                ValiDateMaxLength_TypeCode validate = new ValiDateMaxLength_TypeCode();
                validate.txtcontrol = txtDocHoldTypeCode;
                validate.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtDocHoldTypeCode, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void ValidatetxtDocHoldTypeName()
        {
            try
            {
                ValidateMaxLength_TypeName validate = new ValidateMaxLength_TypeName();
                validate.txtEdit = txtDocHoldTypeName;
                validate.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtDocHoldTypeName, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void ValidatetxtDocHoldTypeGroupCode()
        {
            try
            {
                ValidateMaxLength_GroupCode validate = new ValidateMaxLength_GroupCode();
                validate.maxlength = 50;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion
        #region Gridview

        private void gridViewHisDocHoldType_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_DOC_HOLD_TYPE data = (HIS_DOC_HOLD_TYPE)gridViewHisDocHoldType.GetRow(e.RowHandle);
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

                LogSystem.Warn(ex);
            }
        }

        private void gridViewHisDocHoldType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowdata = (MOS.EFMODEL.DataModels.HIS_DOC_HOLD_TYPE)gridViewHisDocHoldType.GetFocusedRow();
                    if (rowdata != null)
                    {
                        changeDataRow(rowdata);
                        SetFocusEditor();

                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewHisDocHoldType_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (HIS_DOC_HOLD_TYPE)gridViewHisDocHoldType.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    changeDataRow(rowData);
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }


      
        private void gridViewHisDocHoldType_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    HIS_DOC_HOLD_TYPE data = (HIS_DOC_HOLD_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void gridViewHisDocHoldType_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_DOC_HOLD_TYPE data = (HIS_DOC_HOLD_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    
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
                        else
                            e.RepositoryItem = btnEnableDelete;
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #endregion
        #region button click

        private void btnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_DOC_HOLD_TYPE HisDocHoldType = new HIS_DOC_HOLD_TYPE();
            bool notHandler = false;
            try
            {
                HIS_DOC_HOLD_TYPE dataDocHoldType = (HIS_DOC_HOLD_TYPE)gridViewHisDocHoldType.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //HIS_DOC_HOLD_TYPE filter = new HIS_DOC_HOLD_TYPE();
                    //filter.ID = dataDocHoldType.ID;
                    WaitingManager.Show();
                    HisDocHoldType = new BackendAdapter(param).Post<HIS_DOC_HOLD_TYPE>(HisRequestUriStore.MOSHIS_DOC_HOLD_TYPE_ChangeLock, ApiConsumer.ApiConsumers.MosConsumer, dataDocHoldType.ID, param);
                    WaitingManager.Hide();
                    if (HisDocHoldType != null) FillDataToControl();
                }
                notHandler = true;
                BackendDataWorker.Reset<HIS_DOC_HOLD_TYPE>();
                MessageManager.Show(this.ParentForm, param, notHandler);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_DOC_HOLD_TYPE dataHisDocHoldType = new HIS_DOC_HOLD_TYPE();
            bool notHandler = false;
            try
            {
                HIS_DOC_HOLD_TYPE dataGridview = (HIS_DOC_HOLD_TYPE)gridViewHisDocHoldType.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //HIS_DOC_HOLD_TYPE Filter = new HIS_DOC_HOLD_TYPE();
                    //Filter.ID = dataGridview.ID;
                    WaitingManager.Show();
                    dataHisDocHoldType = new BackendAdapter(param).Post<HIS_DOC_HOLD_TYPE>(HisRequestUriStore.MOSHIS_DOC_HOLD_TYPE_ChangeLock, ApiConsumer.ApiConsumers.MosConsumer, dataGridview.ID, param);
                    WaitingManager.Hide();
                    if (dataHisDocHoldType != null) FillDataToControl();
                }
                notHandler = true;
                BackendDataWorker.Reset<HIS_DOC_HOLD_TYPE>();
                MessageManager.Show(this.ParentForm, param, notHandler);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (HIS_DOC_HOLD_TYPE)gridViewHisDocHoldType.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_DOC_HOLD_TYPE_DELETE, ApiConsumer.ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToControl();
                            currentData = ((List<HIS_DOC_HOLD_TYPE>)gridControlHisDocHoldType.DataSource).FirstOrDefault();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {

                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch_Click(null, null);
               
            }
            
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
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
                            txtDocHoldTypeCode.Focus();
                            txtDocHoldTypeCode.SelectAll();
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

        private void btnRest_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                RestFormData();
                txtDocHoldTypeCode.Focus();
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

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }
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
        #endregion
        #region textEdit Keydow

        private void txtDocHoldTypeCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDocHoldTypeName.Focus();
                    txtDocHoldTypeName.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }


        private void txtDocHoldTypeName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        btnAdd.Focus();
                    }
                    else
                        btnEdit.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        #endregion

        private void gridControlHisDocHoldType_Click(object sender, EventArgs e)
        {

        }

      
       
    }
}

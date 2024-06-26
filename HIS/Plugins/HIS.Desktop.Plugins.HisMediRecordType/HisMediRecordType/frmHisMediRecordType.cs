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
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors;
using HIS.Desktop.LibraryMessage;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.BackendData;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.HisBidTest.Resources;




namespace HIS.Desktop.Plugins.HisMediRecordType.HisMediRecordType
{
    public partial class frmHisMediRecordType : FormBase
    {

        #region Reclare variables 
        Inventec.Desktop.Common.Modules.Module moduleData;
        PagingGrid pagingGrid;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int ActionType = -1;
        int positionHandle = -1;
        long HisMediRecordTypeID;
        HIS_MEDI_RECORD_TYPE currentData;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;


        #endregion

        //private Inventec.Desktop.Common.Modules.Module moduleData;

        public frmHisMediRecordType()
        {
            InitializeComponent();
        }
        #region Construct 
        public frmHisMediRecordType(Inventec.Desktop.Common.Modules.Module moduleData): base(moduleData)
        {
            // TODO: Complete member initialization
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControl1.ToolTipController = toolTipController1;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }

                catch (Exception ex)
                {
                    
                    throw;
                }


            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        #endregion
        #region Private Methor
        private void frmHisMediRecordType_Load(object sender, EventArgs e)
        {
            SetDefaultValue();
            EnableControlChanged(this.ActionType);
            FillDataToControl();
            ValidateFrom();
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
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisMediRecordType.Resources.Lang", typeof(HIS.Desktop.Plugins.HisMediRecordType.HisMediRecordType.frmHisMediRecordType).Assembly);
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.layoutControlItem6.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.layoutControlItem7.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSua.Text = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.btnSua.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnThem.Text = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.btnThem.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnLamlai.Text = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.btnLamlai.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTimkiem.Text = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.btnTimkiem.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdMaloai.Caption = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.grdMaloai.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdMaloai.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.grdMaloai.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.STT.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.STT.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdDelete.Caption = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.grdDelete.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdDelete.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.grdDelete.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdLock.Caption = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.grdLock.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdLock.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.grdLock.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdTenloai.Caption = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.grdTenloai.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdTenloai.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.grdCTenloai.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdThoigiantao.Caption = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.grdThoigiantao.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdThoigiantao.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.grdThoigiantao.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdNguoitao.Caption = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.grdNguoitao.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdNguoitao.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.grdNguoitao.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdNguoisua.Caption = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.grdNguoisua.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdNguoisua.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.grdNguoisua.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdThoigiansua.Caption = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.grdThoigiansua.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdThoigiansua.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMediRecordType.grdThoigiansua.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);

            }
        }
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
                ucPaging2.Init(LoadPaging, param, pagingSize, this.gridControl1);
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
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramcommon = new CommonParam(startPage, limit);
                ApiResultObject<List<HIS_MEDI_RECORD_TYPE>> apiResult = null;
                MOS.Filter.HisMediRecordTypeFilter filter = new MOS.Filter.HisMediRecordTypeFilter();
                SetFilter(ref filter);
                gridControl1.DataSource = null;
                grdViewHisMediRecordType.BeginUpdate();
                apiResult = new BackendAdapter(paramcommon).GetRO<List<HIS_MEDI_RECORD_TYPE>>(HisRequestUriStore.MOSHIS_MEDI_RECORD_TYPE_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, paramcommon);
                if (apiResult != null)
                {
                    var data = (List<HIS_MEDI_RECORD_TYPE>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControl1.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);

                    }
                
                }
                grdViewHisMediRecordType.EndDataUpdate();
                #region
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramcommon);
                #endregion


            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetFilter( ref MOS.Filter.HisMediRecordTypeFilter filter)
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
        private void UpdataDTOFromDataFrom(ref HIS_MEDI_RECORD_TYPE data)
        {
            try
            {
                data.MEDI_RECORD_TYPE_CODE = txtMaloai.Text.Trim();
                data.MEDI_RECORD_TYPE_NAME = txtTenloai.Text.Trim();

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        

        }
        private void fillDataEditorControl(HIS_MEDI_RECORD_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    HisMediRecordTypeID = data.ID;
                    txtMaloai.Text = data.MEDI_RECORD_TYPE_CODE;
                    txtTenloai.Text = data.MEDI_RECORD_TYPE_NAME;

                }

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
                {
                    return;
                }
                WaitingManager.Show();
                HIS_MEDI_RECORD_TYPE updateDTO = new HIS_MEDI_RECORD_TYPE();
                UpdataDTOFromDataFrom(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<HIS_MEDI_RECORD_TYPE>(HisRequestUriStore.MOSHIS_MEDI_RECORD_TYPE_CRETAE, ApiConsumer.ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        BackendDataWorker.Reset<HIS_MEDI_RECORD_TYPE>();
                        success = true;
                        FillDataToControl();
                        RestFormData();
                        

                    }
                }
                else
                {
                    if (HisMediRecordTypeID > 0)
                    {
                        updateDTO.ID = HisMediRecordTypeID;
                        var ResultData = new BackendAdapter(param).Post<HIS_MEDI_RECORD_TYPE>(HisRequestUriStore.MOSHIS_MEDI_RECORD_TYPE_UPDATE,ApiConsumer.ApiConsumers.MosConsumer, updateDTO,param);
                        if (ResultData != null)
                        {
                            BackendDataWorker.Reset<HIS_MEDI_RECORD_TYPE>();
                            success = true;
                            FillDataToControl();
                        }

                    }
                }
                WaitingManager.Hide();
                #region  hien thi message thong bao 
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
        private void changeDataRow(HIS_MEDI_RECORD_TYPE data)
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
                        btnSua.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

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
        private void EnableControlChanged(int action)
        {
            try
            {
                btnSua.Enabled = (action == GlobalVariables.ActionEdit);
                btnThem.Enabled = (action == GlobalVariables.ActionAdd);
                txtMaloai.ReadOnly = !(action == GlobalVariables.ActionAdd);
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
                txtMaloai.Text = "";
                txtTenloai.Text = "";
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


        #endregion 

        #region validate

        private void ValidateFrom()
        {
            try
            {
                ValidetxtMaloai();
                ValidetxtTenloai();
                ValideGroup();

            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        private void ValidetxtMaloai()
        {
            try
            {

                Validates.ValiDateMaxLength_TypeCode validate = new Validates.ValiDateMaxLength_TypeCode();
                validate.txtcontrol = txtMaloai;
                validate.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtMaloai, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
            
        }
        private void ValidetxtTenloai()
        {
            try
            {
                Validates.ValidateMaxLength_TypeName validate = new Validates.ValidateMaxLength_TypeName();
                validate.txtEdit = txtTenloai;
                validate.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtTenloai, validate);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ValideGroup()
        {
            try
            {
                Validates.ValidateMaxLength_GroupCode validate = new Validates.ValidateMaxLength_GroupCode();
                validate.maxlength = 50;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        private void RestFormData()
        {
            try
            {
                if (!layoutControl3.IsInitialized)
                    return;
                layoutControl3.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl3.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            txtMaloai.Focus();
                            txtMaloai.SelectAll();
                        }
                    }

                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl3.EndUpdate();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewHisMediRecordType_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_MEDI_RECORD_TYPE data = (HIS_MEDI_RECORD_TYPE)grdViewHisMediRecordType.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "IS_ACTIVE_ACTIVE")
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

        private void grdViewHisMediRecordType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowdata = (MOS.EFMODEL.DataModels.HIS_MEDI_RECORD_TYPE)grdViewHisMediRecordType.GetFocusedRow();
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

        private void gridControl1_Click(object sender, EventArgs e)
        {

        }

        private void grdViewHisMediRecordType_Click(object sender, EventArgs e)
        {

            try
            {
                var rowData = (HIS_MEDI_RECORD_TYPE)grdViewHisMediRecordType.GetFocusedRow();
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

        private void grdViewHisMediRecordType_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {

            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    HIS_MEDI_RECORD_TYPE data = (HIS_MEDI_RECORD_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_CREATE")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_MODIFY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_ACTIVE")
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

        private void grdViewHisMediRecordType_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_MEDI_RECORD_TYPE data = (HIS_MEDI_RECORD_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];

                    if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IS_ACTIVE_FALSE ? btnUnlock : btnLock);

                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        if (data.IS_ACTIVE == IS_ACTIVE_TRUE)
                        {
                            e.RepositoryItem =  btnXoa;
                        }
                        else
                            e.RepositoryItem = btnXoaDisable;
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_MEDI_RECORD_TYPE HisMediRecordType = new HIS_MEDI_RECORD_TYPE();
            bool notHandler = false;
            try
            {
                HIS_MEDI_RECORD_TYPE dataMediRecordType = (HIS_MEDI_RECORD_TYPE)grdViewHisMediRecordType.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    HisMediRecordType = new BackendAdapter(param).Post<HIS_MEDI_RECORD_TYPE>(HisRequestUriStore.MOSHIS_MEDI_RECORD_TYPE_CHANGELOCK, ApiConsumer.ApiConsumers.MosConsumer, dataMediRecordType.ID, param);
                    WaitingManager.Hide();
                    if (HisMediRecordType != null)
                    FillDataToControl();
                    notHandler = true;
                    BackendDataWorker.Reset<HIS_MEDI_RECORD_TYPE>();
                    MessageManager.Show(this.ParentForm, param, notHandler);
                }
               

            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        private void btnUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_MEDI_RECORD_TYPE dataHisMediRecordType = new HIS_MEDI_RECORD_TYPE();
            bool notHandler = false;
            try
            {
                HIS_MEDI_RECORD_TYPE dataGridview = (HIS_MEDI_RECORD_TYPE)grdViewHisMediRecordType.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
              
                    WaitingManager.Show();
                   dataHisMediRecordType = new BackendAdapter(param).Post<HIS_MEDI_RECORD_TYPE>(HisRequestUriStore.MOSHIS_MEDI_RECORD_TYPE_CHANGELOCK, ApiConsumer.ApiConsumers.MosConsumer, dataGridview.ID, param);
                    WaitingManager.Hide();
                    if (dataHisMediRecordType != null)
                        FillDataToControl();
                }
                notHandler = true;
                BackendDataWorker.Reset<HIS_MEDI_RECORD_TYPE>();
                MessageManager.Show(this.ParentForm, param, notHandler);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnXoa_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (HIS_MEDI_RECORD_TYPE)grdViewHisMediRecordType.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_MEDI_RECORD_TYPE_DELETE, ApiConsumer.ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToControl();
                            currentData = ((List<HIS_MEDI_RECORD_TYPE>)grdViewHisMediRecordType.DataSource).FirstOrDefault();
                            btnLamlai_Click(null, null);


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
                btnTimkiem_Click(null, null);

            }
        }

        private void btnTimkiem_Click(object sender, EventArgs e)
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

        private void btnLamlai_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                RestFormData();
                txtMaloai.Focus();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
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

        private void btnThem_Click(object sender, EventArgs e)
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

        private void bbtnTim_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
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
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnSua_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnSua.Enabled)
                {
                    btnSua_Click_1(null, null);
                    //SaveProcess();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnThem.Enabled)
                {
                   
                    btnThem_Click(null, null);
                   // SaveProcess();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnLamlai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            try
            {
                btnLamlai_Click(null, null);

                //this.ActionType = GlobalVariables.ActionAdd;
                //EnableControlChanged(this.ActionType);
                //positionHandle = -1;
                //Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                //RestFormData();
                //txtMaloai.Focus();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtTenloai_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        btnThem.Focus();
                    }
                    else
                        btnSua.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtMaloai_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTenloai.Focus();
                    txtTenloai.SelectAll();

                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnSua_Click_1(object sender, EventArgs e)
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





    }
}

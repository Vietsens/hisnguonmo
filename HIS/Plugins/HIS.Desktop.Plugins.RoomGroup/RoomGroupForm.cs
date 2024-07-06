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
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.Common;
using Inventec.Common.Logging;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.ViewInfo;
using MOS.Filter;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.Desktop.Plugins.RoomGroup
{
    public partial class RoomGroupForm : FormBase
    { 
        #region Declare
            int rowCount = 0;
            int dataTotal = 0;
            int startPage = 0;
            int ActionType = -1;
            MOS.EFMODEL.DataModels.HIS_ROOM_GROUP currentData;
            MOS.EFMODEL.DataModels.HIS_ROOM_GROUP resultData;
            DelegateSelectData delegateSelect = null;
            Inventec.Desktop.Common.Modules.Module currentModule;
            Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
            int positionHandle = -1;

            #endregion

            public RoomGroupForm(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData delegateData)
                : base(module)

        {
           
            InitializeComponent();
            currentModule = module;
            this.delegateSelect = delegateData;
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

            public RoomGroupForm(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {

            try
            {
                InitializeComponent();
                //pagingGrid = new PagingGrid();
                currentModule = module;
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

            #region Loadform
            private void RoomGroupForm_Load(object sender, EventArgs e)
            {
                try
                {
                    MeShow();
                }
                catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
            }

            private void MeShow()
            {
                SetDefaultValue();

                EnableControlChanged(this.ActionType);

                FillDatagctFormList();

                SetCaptionByLanguageKey();

                InitTabIndex();

                ValidateForm();

                SetDefaultFocus();
            }

            private void SetDefaultFocus()
            {
                try
                {
                    this.ActionType = GlobalVariables.ActionAdd;

                    txtFind.Text = "";
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
                    ValidationSingleControl(txtName);
                    ValidationSingleControl(txtCode);


                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }

            private void ValidationSingleControl(BaseEdit control)
            {
                try
                {
                    ControlEditValidationRule validRule = new ControlEditValidationRule();
                    validRule.editor = control;
                    validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    validRule.ErrorType = ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(control, validRule);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }

            private void InitTabIndex()
            {
                try
                {
                    dicOrderTabIndexControl.Add("txtName", 1);
                    dicOrderTabIndexControl.Add("txtCode", 0);


                    if (dicOrderTabIndexControl != null)
                    {
                        foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                        {
                            SetTabIndexToControl(itemOrderTab, layoutControl1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }

            private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
            {
                bool success = false;
                try
                {
                    if (!layoutControlEditor.IsInitialized) return success;
                    layoutControlEditor.BeginUpdate();
                    try
                    {
                        foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                        {
                            DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                            if (lci != null && lci.Control != null)
                            {
                                BaseEdit be = lci.Control as BaseEdit;
                                if (be != null)
                                {
                                    if (itemOrderTab.Key.Contains(be.Name))
                                    {
                                        be.TabIndex = itemOrderTab.Value;
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        layoutControlEditor.EndUpdate();
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

                return success;
            }

            private void SetCaptionByLanguageKey()
            {
                try
                {  Resource.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RoomGroup.Resource.Lang", typeof(HIS.Desktop.Plugins.RoomGroup.RoomGroupForm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("RoomGroupForm.layoutControl1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("RoomGroupForm.layoutControl3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("RoomGroupForm.layoutControl4.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grlSTT.Caption = Inventec.Common.Resource.Get.Value("RoomGroupForm.grlSTT.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclCode.Caption = Inventec.Common.Resource.Get.Value("RoomGroupForm.gclCode.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclName.Caption = Inventec.Common.Resource.Get.Value("RoomGroupForm.gclName.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("RoomGroupForm.btnEdit.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("RoomGroupForm.btnReset.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("RoomGroupForm.btnAdd.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("RoomGroupForm.layoutControl2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("RoomGroupForm.btnFind.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("RoomGroupForm.layoutControlItem2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("RoomGroupForm.layoutControlItem3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("RoomGroupForm.bar1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("RoomGroupForm.bar2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("RoomGroupForm.bbtnAdd.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("RoomGroupForm.barButtonItem2.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("RoomGroupForm.barButtonItem3.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem4.Caption = Inventec.Common.Resource.Get.Value("RoomGroupForm.barButtonItem4.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("RoomGroupForm.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModule != null && !string.IsNullOrEmpty(currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
                }
                catch (Exception ex)
                {
                    
               
                }
               
            }

            private void FillDatagctFormList()
            {
                try
                {
                    WaitingManager.Show();

                    int numPageSize = 0;
                    if (ucPaging.pagingGrid != null)
                    {
                        numPageSize = ucPaging.pagingGrid.PageSize;
                    }
                    else
                    {
                        numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                    }

                    LoadPaging(new CommonParam(0, numPageSize));

                    CommonParam param = new CommonParam();
                    param.Limit = rowCount;
                    param.Count = dataTotal;
                    ucPaging.Init(LoadPaging, param, numPageSize);
                    WaitingManager.Hide();
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                    WaitingManager.Hide();
                }
            }

            private void LoadPaging(object param)
            {
                try
                {
                    startPage = ((CommonParam)param).Start ?? 0;
                    int limit = ((CommonParam)param).Limit ?? 0;
                    CommonParam paramCommon = new CommonParam(startPage, limit);
                    Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_ROOM_GROUP>> apiResult = null;
                    HisRoomGroupFilter filter = new HisRoomGroupFilter();
                    SetFilterNavBar(ref filter);
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    gridView1.BeginUpdate();
                    apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_ROOM_GROUP>>(HIS.Desktop.Plugins.RoomGroup.HisRequestUriStore.MOSHIS_ROOM_GROUP_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                    if (apiResult != null)
                    {
                        var data = (List<MOS.EFMODEL.DataModels.HIS_ROOM_GROUP>)apiResult.Data;
                        if (data != null)
                        {
                            gridView1.GridControl.DataSource = data;
                            rowCount = (data == null ? 0 : data.Count);
                            dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                        }
                    }
                    gridView1.EndUpdate();

                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                }
            }

            private void SetFilterNavBar(ref HisRoomGroupFilter filter)
            {
                try
                {
                    filter.KEY_WORD = txtFind.Text.Trim();
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
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

            private void SetDefaultValue()
            {
                try
                {
                    this.ActionType = GlobalVariables.ActionAdd;

                    txtFind.Text = "";
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            #endregion

            #region event
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

            private void btnEdit_Click(object sender, EventArgs e)
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
                MOS.EFMODEL.DataModels.HIS_ROOM_GROUP updateDTO = new MOS.EFMODEL.DataModels.HIS_ROOM_GROUP();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_ROOM_GROUP>(HIS.Desktop.Plugins.RoomGroup.HisRequestUriStore.MOSHIS_ROOM_GROUP_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {

                        success = true;
                        FillDatagctFormList();
                        txtCode.Text = "";
                        txtName.Text = "";
                        RefeshDataAfterSave(resultData);
                        ResetFormData();

                    }
                }
                else
                {
                   
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_ROOM_GROUP>(HIS.Desktop.Plugins.RoomGroup.HisRequestUriStore.MOSHIS_ROOM_GROUP_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDatagctFormList();
                        RefeshDataAfterSave(resultData);
                    }
                }

                if (success)
                {
                    SetFocusEditor();
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
}

            private void SetFocusEditor()
{
 	try
            {
                txtCode.Focus();
                txtCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
}

            private void ResetFormData()
            {
                try
                {
                    if (!layoutControl1.IsInitialized) return;
                    layoutControl1.BeginUpdate();
                    try
                    {
                        foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl1.Items)
                        {
                            DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                            if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                            {
                                DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                                fomatFrm.ResetText();
                                fomatFrm.EditValue = null;
                                txtCode.Focus();

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                    finally
                    {
                        layoutControl1.EndUpdate();


                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }

            private void RefeshDataAfterSave(MOS.EFMODEL.DataModels.HIS_ROOM_GROUP data)
{
 	try
            {
                if (this.delegateSelect != null)
                {
                    this.delegateSelect(data);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
}

            private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_ROOM_GROUP currentDTO)
{
 	try
            {
                currentDTO.ROOM_GROUP_CODE = txtCode.Text.Trim();
                currentDTO.ROOM_GROUP_NAME = txtName.Text.Trim();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
}

            private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_ROOM_GROUP currentDTO)
{
 	 try
            {
                CommonParam param = new CommonParam();
                HisRoomGroupFilter filter = new HisRoomGroupFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_ROOM_GROUP>>(HIS.Desktop.Plugins.RoomGroup.HisRequestUriStore.MOSHIS_ROOM_GROUP_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                txtCode.Text = "";
                txtName.Text = "";
                txtFind.Text = "";
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                ResetFormData();
                SetFocusEditor();
                FillDatagctFormList();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

            private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FillDatagctFormList();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

            private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_ROOM_GROUP pData = (MOS.EFMODEL.DataModels.HIS_ROOM_GROUP)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }

                   
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

            private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                MOS.EFMODEL.DataModels.HIS_ROOM_GROUP data = null;
                if (e.RowHandle > -1)
                {
                    data = (MOS.EFMODEL.DataModels.HIS_ROOM_GROUP)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnLock : btnUnLock);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnDeleteEnable : btnDeleteDisable);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

            private void gridControl1_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (MOS.EFMODEL.DataModels.HIS_ROOM_GROUP)gridView1.GetFocusedRow();
                if (this.currentData != null)
                {

                    ChangedDataRow(this.currentData);
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

            private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_ROOM_GROUP data)
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

            private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_ROOM_GROUP data)
{
 	try
            {
                if (data != null)
                {

                    txtCode.Text = data.ROOM_GROUP_CODE;
                    txtName.Text = data.ROOM_GROUP_NAME;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
}

            private void btnLock_Click(object sender, EventArgs e)
        {
         CommonParam param = new CommonParam();
            bool rs = false;
            MOS.EFMODEL.DataModels.HIS_ROOM_GROUP success = new MOS.EFMODEL.DataModels.HIS_ROOM_GROUP();
            //bool notHandler = false;
            try
            {

                MOS.EFMODEL.DataModels.HIS_ROOM_GROUP data = (MOS.EFMODEL.DataModels.HIS_ROOM_GROUP)gridView1.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MOS.EFMODEL.DataModels.HIS_ROOM_GROUP data1 = new MOS.EFMODEL.DataModels.HIS_ROOM_GROUP();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_ROOM_GROUP>(HIS.Desktop.Plugins.RoomGroup.HisRequestUriStore.MOSHIS_ROOM_GROUP_GROUP_CHANGE_LOCK, ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        rs = true;
                        FillDatagctFormList();
                    }
                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, rs);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                    btnReset_Click(null, null);
                }

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
            bool rs = false;
            MOS.EFMODEL.DataModels.HIS_ROOM_GROUP success = new MOS.EFMODEL.DataModels.HIS_ROOM_GROUP();
            //bool notHandler = false;

            try
            {

                MOS.EFMODEL.DataModels.HIS_ROOM_GROUP data = (MOS.EFMODEL.DataModels.HIS_ROOM_GROUP)gridView1.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_ROOM_GROUP>(HIS.Desktop.Plugins.RoomGroup.HisRequestUriStore.MOSHIS_ROOM_GROUP_GROUP_CHANGE_LOCK, ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        rs = true;
                        FillDatagctFormList();
                    }
                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, rs);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                    btnReset_Click(null, null);
                }

            }

            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

            private void btnDeleteEnable_Click(object sender, EventArgs e)
        {
          try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (MOS.EFMODEL.DataModels.HIS_ROOM_GROUP)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {

                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(HIS.Desktop.Plugins.RoomGroup.HisRequestUriStore.MOSHIS_ROOM_GROUP_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            this.ActionType = 1;
                            txtName.Text = "";
                            txtCode.Text = "";
                            EnableControlChanged(this.ActionType);
                            FillDatagctFormList();
                            currentData = ((List<MOS.EFMODEL.DataModels.HIS_ROOM_GROUP>)gridControl1.DataSource).FirstOrDefault();
                        }
                        MessageManager.Show(this, param, success);
                        btnReset_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

            private void btnDeleteDisable_Click(object sender, EventArgs e)
        {
        
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

            private void txtCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {
                txtName.Focus();
            }
        }

            private void txtName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnAdd.Focus();
            }
        }

            private void txtFind_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
            }
        }
            #endregion

            #region ShortCut
            private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
            {
                if (btnAdd.Enabled)
                {
                    btnAdd_Click(null, null);
                }
            }

            private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
            {
                if (btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }

            }

            private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
            {
                if (btnReset.Enabled)
                {
                    btnReset_Click(null, null);
                }
            }

            private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
            {
                btnFind_Click(null, null);
            }

            #endregion
       
    }
}
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
//using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
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
using HIS.Desktop.Utility;
using DevExpress.XtraLayout;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.IsAdmin;
namespace HIS.Desktop.Plugins.EkipTemp
{
    public partial class EkipTempForm : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int ActionType = -1;
        MOS.EFMODEL.DataModels.HIS_EKIP_TEMP currentData;
        MOS.EFMODEL.DataModels.HIS_EKIP_TEMP resultData;
        DelegateSelectData delegateSelect = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        int positionHandle = -1;
        List<ACS.EFMODEL.DataModels.ACS_USER> ListName = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
        List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> Execute = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>();
        List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP> ekipTemps { get; set; }
        #endregion

        #region construct

        public EkipTempForm(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData delegateData)
            : base(module)
        {

            try
            {
                InitializeComponent();
                //pagingGrid = new PagingGrid();
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        public EkipTempForm(Inventec.Desktop.Common.Modules.Module module)
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

        #endregion

        #region Private method

        private void EkipTempForm_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }

        }

        void RefeshDataAfterSave(MOS.EFMODEL.DataModels.HIS_EKIP_TEMP data)
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

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;

                txtKey.Text = "";
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
                // btnEdit.Enabled = (action == GlobalVariables.ActionEdit);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                ucPaging.Init(LoadPaging, param, numPageSize,gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resource.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.EkipTemp.Resource.Lang", typeof(HIS.Desktop.Plugins.EkipTemp.EkipTempForm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("EkipTempForm.layoutControl1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("EkipTempForm.layoutControl5.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("EkipTempForm.btnFind.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKey.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("EkipTempForm.txtKey.Properties.NullValuePrompt", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("EkipTempForm.layoutControl4.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("EkipTempForm.layoutControl6.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnReset.Text = Inventec.Common.Resource.Get.Value("EkipTempForm.btnReset.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnEdit.Text = Inventec.Common.Resource.Get.Value("EkipTempForm.btnEdit.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("EkipTempForm.layoutControlItem7.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("EkipTempForm.layoutControl2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grlSTT.Caption = Inventec.Common.Resource.Get.Value("EkipTempForm.grlSTT.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclName.Caption = Inventec.Common.Resource.Get.Value("EkipTempForm.gclName.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("EkipTempForm.bar1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("EkipTempForm.barButtonItem2.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("EkipTempForm.barButtonItem3.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem4.Caption = Inventec.Common.Resource.Get.Value("EkipTempForm.barButtonItem4.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("EkipTempForm.layoutControl3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("EkipTempForm.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("EkipTempForm.btnAdd.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("EkipTempForm.btnReset.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.currentModule != null && !string.IsNullOrEmpty(currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
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

        private void ValidateForm()
        {
            try
            {

                //ValidationSingleControl(txtEkipName);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDefaultFocus()
        {
            try
            {
                txtKey.Focus();
                txtKey.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP>> apiResult = null;
                HisEkipTempFilter filter = new HisEkipTempFilter();
                filter.IS_ACTIVE = 1;
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP>>(HIS.Desktop.Plugins.EkipTemp.HisRequestUriStore.MOSHIS_EKIP_TEMP_GET, ApiConsumers.MosConsumer, filter, paramCommon);

                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP>)apiResult.Data;
                    if (data != null)
                    {
                        //var data1 = from item in data
                        //            where item.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || item.IS_PUBLIC == 1
                        //            select item;
                        //if (!String.IsNullOrEmpty(txtKey.Text))
                        //{
                        //    data1 = data1.Where(o => o.EKIP_TEMP_NAME.ToUpper().Contains(txtKey.Text.ToUpper())).ToList();
                        //}

                        gridView1.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView1.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisEkipTempFilter filter)
        {
            try
            {
                //filter.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                //if (cboIsPublic.SelectedIndex==0)
                //{
                //    filter.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                //}
                //else if (cboIsPublic.SelectedIndex==1)
                //{
                   
                //    filter.CAN_VIEW = true;
                //}
                filter.KEY_WORD = txtKey.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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
                                //Cac control dac biet can fix khong co thay doi thuoc tinh enable
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
        
        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_EKIP_TEMP currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisEkipTempFilter filter = new HisEkipTempFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP>>(HIS.Desktop.Plugins.EkipTemp.HisRequestUriStore.MOSHIS_EKIP_TEMP_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_EKIP_TEMP currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                //  currentDTO.EKIP_TEMP_NAME = txtEkipName.Text.Trim();
                HisEkipTempUserFilter filter = new HisEkipTempUserFilter();
                filter.EKIP_TEMP_ID = currentDTO.ID;
                List<HIS_EKIP_TEMP_USER> ekipTempUsers = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_EKIP_TEMP_USER>>("api/HisEkipTempUser/Get", ApiConsumers.MosConsumer, filter, param);
                if (ekipTempUsers != null && ekipTempUsers.Count > 0)
                {
                    foreach (var item in ekipTempUsers)
                    {
                        currentDTO.HIS_EKIP_TEMP_USER.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                            //txtEkipName.Focus();

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

        private void SetFocusEditor()
        {
            try
            {
                //txtEkipName.Focus();
                //txtEkipName.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_EKIP_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    // btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_EKIP_TEMP data)
        {
            try
            {
                if (data != null)
                {

                    //txtEkipName.Text = data.EKIP_TEMP_NAME;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
      
        private void gridView2_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                HIS_EKIP_TEMP_USER data = null;
                if (e.RowHandle > -1)
                {
                    data = (HIS_EKIP_TEMP_USER)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "Delete2")
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

        private void refeshData()
        {
            btnReset_Click_1(null, null);
        }

        private void gridView2_CustomUnboundColumnData_1(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {


                    MOS.EFMODEL.DataModels.V_HIS_EKIP_TEMP_USER pData = (V_HIS_EKIP_TEMP_USER)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        private void gridView2_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {


                    MOS.EFMODEL.DataModels.HIS_EKIP_TEMP_USER pData = (HIS_EKIP_TEMP_USER)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "Name1")
                    {
                        foreach (var item in ListName)
                        {
                            if (item.USERNAME == pData.USERNAME)
                            {
                                e.Value = item.LOGINNAME;
                            }
                        }
                    }
                    else if (e.Column.FieldName == "Excute")
                    {
                        foreach (var item in Execute)
                        {
                            if (item.ID == pData.EXECUTE_ROLE_ID)
                            {
                                e.Value = item.EXECUTE_ROLE_NAME;
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
        #endregion

        #region Public method

        public void MeShow()
        {
            //cboIsPublic.SelectedIndex = 2;

            SetDefaultValue();

            EnableControlChanged(this.ActionType);

            FillDatagctFormList();

            SetCaptionByLanguageKey();

            InitTabIndex();

            ValidateForm();

            SetDefaultFocus();
        }
        #endregion

        #region event

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {


                    MOS.EFMODEL.DataModels.HIS_EKIP_TEMP pData = (HIS_EKIP_TEMP)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR") {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
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

                HIS_EKIP_TEMP data = null;
                if (e.RowHandle > -1)
                {
                    data = (HIS_EKIP_TEMP)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && data.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()) ? btnDeleteEnable : btnDeleteDisable);
                    }
                    if (e.Column.FieldName == "gclEdit")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && data.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() || CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()) ? btnEditEkip : btndisableEdit);
                    }

                }
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

        #region event click

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FillDatagctFormList();               
                    this.currentData = (MOS.EFMODEL.DataModels.HIS_EKIP_TEMP)gridView1.GetFocusedRow();
                    if (this.currentData != null)
                    {
                        FillDatatoEkipTempUser(this.currentData);
                        ChangedDataRow(this.currentData);
                        SetFocusEditor();
                    }
                
               
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }   

        private void gridControl1_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (MOS.EFMODEL.DataModels.HIS_EKIP_TEMP)gridView1.GetFocusedRow();
                if (this.currentData != null)
                {
                    FillDatatoEkipTempUser(this.currentData);
                    ChangedDataRow(this.currentData);
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDatatoEkipTempUser(MOS.EFMODEL.DataModels.HIS_EKIP_TEMP data)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisEkipTempUserFilter filter = new HisEkipTempUserFilter();
                filter.EKIP_TEMP_ID = data.ID;
                List<MOS.EFMODEL.DataModels.V_HIS_EKIP_TEMP_USER> ekipTemUsers = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_EKIP_TEMP_USER>>("api/HisEkipTempUser/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                List<ACS.EFMODEL.DataModels.ACS_USER> acsUsers = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                if (ekipTemUsers != null && ekipTemUsers.Count > 0)
                {
                    foreach (var item in ekipTemUsers)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER acsUser = acsUsers.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                        item.USERNAME = acsUser.USERNAME;
                    }
                }
                

                gridView2.GridControl.DataSource = ekipTemUsers;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }


        }

        private void btnDeleteEnable1_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteEnable_Click(object sender, EventArgs e)
        {

            try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (MOS.EFMODEL.DataModels.HIS_EKIP_TEMP)gridView1.GetFocusedRow();
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(HIS.Desktop.Plugins.EkipTemp.HisRequestUriStore.MOSHIS_EKIP_TEMP_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            this.ActionType = 1;
                            //txtEkipName.Text = "";
                            EnableControlChanged(this.ActionType);
                            FillDatagctFormList();
                            currentData = ((List<HIS_EKIP_TEMP>)gridControl1.DataSource).FirstOrDefault();
                        }
                        MessageManager.Show(this, param, success);
                        btnReset_Click_1(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnEditEkip_Click(object sender, EventArgs e)
        {
            //var rowData = (MOS.EFMODEL.DataModels.HIS_EKIP_TEMP)gridView1.GetFocusedRow();
            //if (rowData != null && rowData.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
            //{
            this.currentData = (MOS.EFMODEL.DataModels.HIS_EKIP_TEMP)gridView1.GetFocusedRow();
            frmEdit.frmEditEkipTemp a = new frmEdit.frmEditEkipTemp(this.currentData, refeshData, currentModule);
            a.ShowDialog();
            //}
            //else
            //{
            //    DevExpress.XtraEditors.XtraMessageBox.Show("Chỉ người tạo mới có thể sửa", "Thông báo");  
            //}

        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            frmEdit.frmEditEkipTemp a = new frmEdit.frmEditEkipTemp(refeshData, currentModule);
            a.ShowDialog();
        }

        private void btnReset_Click_1(object sender, EventArgs e)
        {
            //cboIsPublic.SelectedIndex = 2;

            txtKey.Text = "";
            txtKey.Focus();
            FillDatagctFormList();
            this.currentData = (MOS.EFMODEL.DataModels.HIS_EKIP_TEMP)gridView1.GetFocusedRow();
            if (this.currentData != null)
            {
                FillDatatoEkipTempUser(this.currentData);
                ChangedDataRow(this.currentData);
                SetFocusEditor();
            }
        }
               
        #region lock
        //private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{

        //    CommonParam param = new CommonParam();
        //    bool rs = false;
        //    HIS_EKIP_TEMP success = new HIS_EKIP_TEMP();
        //    //bool notHandler = false;
        //    try
        //    {

        //        HIS_EKIP_TEMP data = (HIS_EKIP_TEMP)gridView1.GetFocusedRow();
        //        //UpdateDTOFromDataForm(ref data);
        //        if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        //        {
        //            HIS_EKIP_TEMP data1 = new HIS_EKIP_TEMP();
        //            data1.ID = data.ID;
        //            WaitingManager.Show();
        //            success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EKIP_TEMP>(HIS.Desktop.Plugins.EkipTemp.HisRequestUriStore.MOSHIS_EKIP_TEMP_GROUP_CHANGE_LOCK, ApiConsumers.MosConsumer, data, param);
        //            WaitingManager.Hide();
        //            if (success != null)
        //            {
        //                rs = true;
        //                FillDatagctFormList();
        //            }
        //            #region Hien thi message thong bao
        //            MessageManager.Show(this, param, rs);
        //            #endregion

        //            #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
        //            SessionManager.ProcessTokenLost(param);
        //            #endregion
        //            btnReset_Click(null, null);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void btnUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        //{
        //    CommonParam param = new CommonParam();
        //    bool rs = false;
        //    MOS.EFMODEL.DataModels.HIS_EKIP_TEMP success = new MOS.EFMODEL.DataModels.HIS_EKIP_TEMP();
        //    //bool notHandler = false;

        //    try
        //    {

        //        MOS.EFMODEL.DataModels.HIS_EKIP_TEMP data = (HIS_EKIP_TEMP)gridView1.GetFocusedRow();
        //        if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        //        {
        //            WaitingManager.Show();
        //            success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EKIP_TEMP>(HIS.Desktop.Plugins.EkipTemp.HisRequestUriStore.MOSHIS_EKIP_TEMP_GROUP_CHANGE_LOCK, ApiConsumers.MosConsumer, data, param);
        //            WaitingManager.Hide();
        //            if (success != null)
        //            {
        //                rs = true;
        //                FillDatagctFormList();
        //            }
        //            #region Hien thi message thong bao
        //            MessageManager.Show(this, param, rs);
        //            #endregion

        //            #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
        //            SessionManager.ProcessTokenLost(param);
        //            #endregion
        //            btnReset_Click(null, null);
        //        }

        //    }

        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }

        //}
        #endregion

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            txtKey.Text = "";
            txtKey.Focus();
            FillDatagctFormList();
            this.currentData = (MOS.EFMODEL.DataModels.HIS_EKIP_TEMP)gridView1.GetFocusedRow();
            if (this.currentData != null)
            {
                FillDatatoEkipTempUser(this.currentData);
                ChangedDataRow(this.currentData);
                SetFocusEditor();
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            btnAdd_Click_1(null, null);
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnFind_Click(null, null);
        }

        #endregion

        #region envet keyup
        private void txtKey_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);

                }
                if (e.KeyCode == Keys.Down)
                {
                    gridView1.Focus();
                    gridView1.FocusedRowHandle = 0;
                    var rowData = (HIS_EKIP_TEMP)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);

                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridView1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {

                    var rowData = (HIS_EKIP_TEMP)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
                if (e.KeyCode == Keys.Up)
                {

                    var rowData = (HIS_EKIP_TEMP)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        #endregion
        private void txtKey_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
                gridControl1.Focus();
            }
            else if (e.KeyCode == Keys.Down)
            {
                btnFind_Click(null, null);
                gridControl1.Focus();
            }

        }

        private void gridControl1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                try
                {
                    this.currentData = (MOS.EFMODEL.DataModels.HIS_EKIP_TEMP)gridView1.GetFocusedRow();
                    if (this.currentData != null)
                    {
                        FillDatatoEkipTempUser(this.currentData);
                        ChangedDataRow(this.currentData);
                        SetFocusEditor();
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
        }
        #endregion   
      
    }
}

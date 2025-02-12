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
//using HIS.Desktop.Plugins.ServiceReqType.Properties;

namespace HIS.Desktop.Plugins.ServiceReqType
{
    public partial class ServiceReqTypeForm : FormBase
    { 
        #region Declare
            int rowCount = 0;
            int dataTotal = 0;
            int startPage = 0;
            MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE currentData;
            MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE resultData;
            DelegateSelectData delegateSelect = null;
            Inventec.Desktop.Common.Modules.Module currentModule;
            Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
            int positionHandle = -1;

            #endregion

            public ServiceReqTypeForm(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData delegateData)
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

            public ServiceReqTypeForm(Inventec.Desktop.Common.Modules.Module module)
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
            private void ServiceReqTypeForm_Load(object sender, EventArgs e)
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
                {    ////Khoi tao doi tuong resource
                    ////Khoi tao doi tuong resource
                    Resource.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ServiceReqType.Resource.Lang", typeof(HIS.Desktop.Plugins.ServiceReqType.ServiceReqTypeForm).Assembly);

                    ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                    this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.layoutControl1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.layoutControl5.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.layoutControl3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.layoutControl4.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.grlSTT.Caption = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.grlSTT.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.gclCode.Caption = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.gclCode.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.gclName.Caption = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.gclName.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.btnReset.Text = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.btnReset.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.layoutControl2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.btnFind.Text = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.btnFind.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.layoutControlItem2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.layoutControlItem3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.bar2.Text = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.bar2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.barButtonItem3.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.barButtonItem1.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.bar1.Text = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.bar1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.Text = Inventec.Common.Resource.Get.Value("ServiceReqTypeForm.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                    Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE>> apiResult = null;
                   HisServiceReqTypeFilter filter = new HisServiceReqTypeFilter();
                  //  SetFilterNavBar(ref filter);
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    gridView1.BeginUpdate();
                    apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE>>(HIS.Desktop.Plugins.ServiceReqType.HisRequestUriStore.MOSHIS_SERVICE_REQ_TYPE_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                    if (apiResult != null)
                    {
                        var data = (List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE>)apiResult.Data;
                        List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE> data1=new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE>();
                        if (data != null)
                        {
                            foreach (var item in data)
                            {
                                if (item.SERVICE_REQ_TYPE_NAME.ToLower().Contains(txtFind.Text.ToLower()) || item.SERVICE_REQ_TYPE_CODE.Contains(txtFind.Text))
                                {
                                    data1.Add(item);
                                }
                            }
                            gridView1.GridControl.DataSource = data1;
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

            private void SetDefaultValue()
            {
                try
                {
                   

                    txtFind.Text = "";
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            #endregion

            #region event
 
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

            private void RefeshDataAfterSave(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE data)
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

            private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE currentDTO)
{
 	try
            {
                //currentDTO.MACHINE_CODE = txtCode.Text.Trim();
                //currentDTO.MACHINE_NAME = txtName.Text.Trim();
                //currentDTO.SERIAL_NUMBER = txtSeri.Text.Trim();
                //currentDTO.MACHINE_GROUP_CODE = txtMachineGroupCode.Text.Trim();
                //if (cboSource.SelectedIndex==0)
                //{
                //    currentDTO.SOURCE_CODE = "1";
                //}
                //else if (cboSource.SelectedIndex==1)
                //{
                //    currentDTO.SOURCE_CODE = "2";
                //}
                //else if (cboSource.SelectedIndex==2)
                //{
                //       currentDTO.SOURCE_CODE = "3";
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
}

            private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE currentDTO)
{
 	 try
            {
                CommonParam param = new CommonParam();
                HisServiceReqTypeFilter filter = new HisServiceReqTypeFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE>>(HIS.Desktop.Plugins.ServiceReqType.HisRequestUriStore.MOSHIS_SERVICE_REQ_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                    MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE pData = (MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    //else if (e.Column.FieldName=="SOURCE")
                    //{
                        //if (pData.SOURCE_CODE=="1")
                        //{
                        //    e.Value = "Ngân sách";
                        //}
                        //else if (pData.SOURCE_CODE=="2")
                        //{
                        //    e.Value = "Xã hội hóa";
                        //}
                        //else if (pData.SOURCE_CODE=="3")
                        //{
                        //    e.Value = "Khác";
                        //}
                   // }
                   
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
                MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE data = null;
                if (e.RowHandle > -1)
                {
                    data = (MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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
                this.currentData = (MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE)gridView1.GetFocusedRow();
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

            private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE data)
{
 	 try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
}

            private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE data)
{
 	try
            {
                if (data != null)
                {

                    txtCode.Text = data.SERVICE_REQ_TYPE_CODE;
                    txtName.Text = data.SERVICE_REQ_TYPE_NAME;
                    //txtSeri.Text = data.SERIAL_NUMBER;
                    //txtMachineGroupCode.Text = data.MACHINE_GROUP_CODE;
                    //if (data.SOURCE_CODE == "1")
                    //{
                    //    cboSource.SelectedIndex = 0;
                    //}
                    //else if (data.SOURCE_CODE=="2")
                    //{
                    //    cboSource.SelectedIndex = 1;
                    //}
                    //else if (data.SOURCE_CODE=="3")
                    //{
                    //    cboSource.SelectedIndex = 2;
                    //}
                }
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
                btnReset.Focus();
            }
        }

            private void txtFind_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
                gridView1.Focus();
            }
        }
            #endregion

            #region ShortCut


            private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
            {
                if (btnReset.Enabled)
                {
                    btnReset_Click(null, null);
                }
            }


            #endregion



            private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
            {
                btnFind_Click(null,null);
            }
       
    }
}

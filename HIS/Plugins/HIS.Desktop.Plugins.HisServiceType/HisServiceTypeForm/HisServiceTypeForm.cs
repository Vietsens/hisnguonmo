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
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using Inventec.UC.Paging;
using DevExpress.XtraEditors.ViewInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using MOS.SDO;
using HIS.Desktop.Plugins.HisServiceType.Properties;

namespace HIS.Desktop.Plugins.HisServiceType.HisServiceTypeForm
{
    public partial class HisServiceTypeForm : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int ActionType = -1;
        PagingGrid pagingGrid;
        MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE currentData;
        MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE resultData;
        DelegateSelectData delegateSelect = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        int positionHandle = -1;
        List<HIS_EXE_SERVICE_MODULE> listExeServiceModule = new List<HIS_EXE_SERVICE_MODULE>();

        #endregion

        public HisServiceTypeForm(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData delegateData)
            : base(module)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
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

        public HisServiceTypeForm(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {

            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
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

        #region loadform
        private void HisServiceTypeForm_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void MeShow()
        {
            try
            {
                //Gán mặc định
                setDefaultValue();
                //Gán lại mặc định nút
                enableControlChanged(this.ActionType);

                //load dữ liệu vào datagctFormList
                LoadDatagctFormList();
                //----load ngôn ngữ
                SetCaptionByLanguageKey();

                //Set tabindex control//gán tabindex cho control
                InitTabIndex();
                //gán quy định bắt buộc
                ValidateForm();
                //Focus default
                SetDefaultFocus();
                //InitCombo
                InitCombo();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitCombo()
        {
            InitComboServiceCode();
        }
        private void InitComboServiceCode()
        {
            try
            {
                if (listExeServiceModule == null || listExeServiceModule.Count <= 0)
                {
                    CommonParam param = new CommonParam();
                    HisExeServiceModuleFilter filter = new HisExeServiceModuleFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    listExeServiceModule = new BackendAdapter(param).Get<List<HIS_EXE_SERVICE_MODULE>>("api/HisExeServiceModule/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXE_SERVICE_MODULE_NAME", "", 350, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXE_SERVICE_MODULE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboExeServiceModule, listExeServiceModule, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void setDefaultValue()
        {
            try
            {
                //this.ActionType = GlobalVariables.ActionAdd;//Mới mở form cho phép thêm actionType=1
                // ResetFormData();               
                txtKey.Text = "";

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void enableControlChanged(int action)
        {
            try
            {
                //btnEdit.Enabled = (action == GlobalVariables.ActionEdit);//action=actionEdit->hiện btnEdit
                //btnAdd.Enabled = (action == GlobalVariables.ActionAdd);//...
                txtServiceCode.Enabled = false;
                txtServiceName.Enabled = false;
                cboExeServiceModule.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDatagctFormList()
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
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControl1);
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE>> apiResult = null;
                HisServiceTypeFilter filter = new HisServiceTypeFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE>>(HISRequestUriStore.MOSHIS_SERVICE_TYPE_TYPE_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE>)apiResult.Data;
                    if (data != null)
                    {

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

        private void SetFilterNavBar(ref HisServiceTypeFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKey.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }



        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisServiceType.Resources.Lang", typeof(HisServiceTypeForm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsNotDisplayAssign.Properties.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.chkIsNotDisplayAssign.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.barButtonItem3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFind.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.bbtnFind.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsAutoSplitReq.Properties.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.chkIsAutoSplitReq.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExeServiceModule.Properties.NullText = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.cboExeServiceModule.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclPatientType.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.gclPatientType.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclExeServiceModule.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.grclExeServiceModule.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.layoutControlItem3.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.layoutControlItem6.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grlSTT.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.grlSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsSplitReqBySampleType.Properties.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.chkIsSplitReqBySampleType.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsSplitReqBySampleType.ToolTip = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.chkIsSplitReqBySampleType.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsRequiredSampleType.Properties.Caption = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.chkIsRequiredSampleType.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsRequiredSampleType.ToolTip = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.chkIsRequiredSampleType.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("HisServiceTypeForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
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
                dicOrderTabIndexControl.Add("txtKey", 0);
                dicOrderTabIndexControl.Add("cbbStock", 1);
                dicOrderTabIndexControl.Add("cbbPatient", 2);
                //dicOrderTabIndexControl.Add("comboBox1", 3);

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

        private void ValidateForm()
        {
            try
            {
                //ValidationSingleControl(cbbStock);
                //ValidationSingleControl(cbbPatient);


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
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultFocus()
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

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                HIS_SERVICE_TYPE data = null;
                if (e.RowHandle > -1)
                {
                    data = (HIS_SERVICE_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (listExeServiceModule == null || listExeServiceModule.Count <= 0)
                    {
                        CommonParam param = new CommonParam();
                        HisExeServiceModuleFilter filter = new HisExeServiceModuleFilter();
                        filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        listExeServiceModule = new BackendAdapter(param).Get<List<HIS_EXE_SERVICE_MODULE>>("api/HisExeServiceModule/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                    }
                    MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE pData = (MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "EXE_SERVICE_MODULE_Str")
                    {
                        if (pData.EXE_SERVICE_MODULE_ID != null)
                        {
                            e.Value = listExeServiceModule.FirstOrDefault(o => o.ID == pData.EXE_SERVICE_MODULE_ID).EXE_SERVICE_MODULE_NAME;
                        }
                    }
                    else if (e.Column.FieldName == "MODULE_LINK")
                    {
                        if (pData.EXE_SERVICE_MODULE_ID != null)
                        {
                            e.Value = listExeServiceModule.FirstOrDefault(o => o.ID == pData.EXE_SERVICE_MODULE_ID).MODULE_LINK;
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Convert.ToInt64(pData.CREATE_TIME));
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Convert.ToInt64(pData.MODIFY_TIME));
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region event


        void RefeshDataAfterSave(MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE data)
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
                            txtKey.Focus();

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

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                enableControlChanged(this.ActionType);
                positionHandle = -1;
                spinNumOrder.EditValue = null;
                txtServiceCode.Text = "";
                txtServiceName.Text = "";
                chkIsAutoSplitReq.Checked = false;
                chkIsNotDisplayAssign.Checked = false;
                chkIsSplitReqBySampleType.Checked = false;
                chkIsSplitReqBySampleType.Enabled = false;
                chkIsRequiredSampleType.Checked = false;
                chkIsRequiredSampleType.Enabled = false;
                txtKey.Text = "";
                cboExeServiceModule.EditValue = null;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                ResetFormData();
                SetDefaultFocus();
                LoadDatagctFormList();
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
                LoadDatagctFormList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    enableControlChanged(this.ActionType);

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

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    txtServiceCode.Text = data.SERVICE_TYPE_CODE;
                    txtServiceName.Text = data.SERVICE_TYPE_NAME;
                    spinNumOrder.EditValue = data.NUM_ORDER;
                    cboExeServiceModule.EditValue = data.EXE_SERVICE_MODULE_ID;
                    chkIsAutoSplitReq.Checked = data.IS_AUTO_SPLIT_REQ == (short)1 ? true : false;
                    chkIsNotDisplayAssign.Checked = data.IS_NOT_DISPLAY_ASSIGN == (short)1 ? true : false;
                    if (data.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                    {
                        chkIsSplitReqBySampleType.Enabled = true;
                        chkIsSplitReqBySampleType.Checked = data.IS_SPLIT_REQ_BY_SAMPLE_TYPE == (short)1 ? true : false;
                    }
                    else
                    {
                        chkIsSplitReqBySampleType.Enabled = false;
                        chkIsSplitReqBySampleType.Checked = false;
                    }

                    if ((data.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN && chkIsSplitReqBySampleType.Checked == true) || (data.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL))
                    {
                        chkIsRequiredSampleType.Enabled = true;
                        chkIsRequiredSampleType.Checked = data.IS_REQUIRED_SAMPLE_TYPE == (short)1 ? true : false;
                    }
                    else
                    {
                        chkIsRequiredSampleType.Enabled = false;
                        chkIsRequiredSampleType.Checked = false;
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
            try
            {
                this.currentData = (MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE)gridView1.GetFocusedRow();
                if (this.currentData != null)
                {

                    ChangedDataRow(this.currentData);
                    SetDefaultFocus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void btnDeleteEnable_Click(object sender, EventArgs e)
        //{

        //    try
        //    {
        //        if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        //        {
        //            var rowData = (MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE)gridView1.GetFocusedRow();
        //            if (rowData != null)
        //            {

        //                bool success = false;
        //                CommonParam param = new CommonParam();
        //                success = new BackendAdapter(param).Post<bool>(HISRequestUriStore.MOSHIS_SERVICE_TYPE_TYPE_DELETE, ApiConsumers.MosConsumer, rowData, param);
        //                if (success)
        //                {
        //                    this.ActionType = 1;
        //                    txt.Text = "";
        //                    cbbStock.Text = "";
        //                    enableControlChanged(this.ActionType);
        //                    LoadDatagctFormList();
        //                    currentData = ((List<HIS_SERVICE_TYPE>)gridControl1.DataSource).FirstOrDefault();
        //                    BackendDataWorker.Reset<HIS_SERVICE_TYPE>();
        //                }
        //                MessageManager.Show(this, param, success);
        //                btnReset_Click(null, null);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void btnLock_Click(object sender, EventArgs e)
        //{
        //    CommonParam param = new CommonParam();
        //    HIS_SERVICE_TYPE hisDepertments = new HIS_SERVICE_TYPE();
        //    bool notHandler = false;
        //    try
        //    {
        //        HIS_SERVICE_TYPE dataDepartment = (HIS_SERVICE_TYPE)gridView1.GetFocusedRow();
        //        if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        //        {
        //            HIS_SERVICE_TYPE data1 = new HIS_SERVICE_TYPE();
        //            data1.ID = dataDepartment.ID;
        //            WaitingManager.Show();
        //            hisDepertments = new BackendAdapter(param).Post<HIS_SERVICE_TYPE>(HISRequestUriStore.MOSHIS_SERVICE_TYPE_TYPE_CHANGE_LOCK, ApiConsumers.MosConsumer, data1, param);
        //            WaitingManager.Hide();
        //            if (hisDepertments != null) LoadDatagctFormList();
        //            btnEdit.Enabled = false;
        //            BackendDataWorker.Reset<HIS_SERVICE_TYPE>();
        //        }
        //        notHandler = true;
        //        MessageManager.Show(this.ParentForm, param, notHandler);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void btnUnLock_Click(object sender, EventArgs e)
        //{
        //    CommonParam param = new CommonParam();
        //    HIS_SERVICE_TYPE hisDepertments = new HIS_SERVICE_TYPE();
        //    bool notHandler = false;
        //    try
        //    {
        //        HIS_SERVICE_TYPE dataDepartment = (HIS_SERVICE_TYPE)gridView1.GetFocusedRow();
        //        if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        //        {
        //            HIS_SERVICE_TYPE data1 = new HIS_SERVICE_TYPE();
        //            data1.ID = dataDepartment.ID;
        //            WaitingManager.Show();
        //            hisDepertments = new BackendAdapter(param).Post<HIS_SERVICE_TYPE>(HISRequestUriStore.MOSHIS_SERVICE_TYPE_CHANGE_LOCK, ApiConsumers.MosConsumer, data1, param);
        //            WaitingManager.Hide();
        //            if (hisDepertments != null) LoadDatagctFormList();
        //            btnEdit.Enabled = true;
        //            BackendDataWorker.Reset<HIS_SERVICE_TYPE>();
        //        }
        //        notHandler = true;
        //        MessageManager.Show(this.ParentForm, param, notHandler);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

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

        private void txtKey_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
                gridView1.SelectAll();
                gridView1.Focus();
            }
            if (e.KeyCode == Keys.Down)
            {
                gridView1.SelectAll();
                gridView1.Focus();

            }
        }

        private void gridControl1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {

                    var rowData = (HIS_SERVICE_TYPE)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
                if (e.KeyCode == Keys.Up)
                {

                    var rowData = (HIS_SERVICE_TYPE)gridView1.GetFocusedRow();
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

        #region shortcut


        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnReset.Enabled)
            {
                btnReset_Click(null, null);
            }
        }

        private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnFind.Enabled)
            {
                btnFind_Click(null, null);
            }
        }
        #endregion


        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnEdit.Enabled)
                    return;

                positionHandle = -1;
                WaitingManager.Show();
                MOS.SDO.ServiceTypeUpdateSDO updateDTO = new MOS.SDO.ServiceTypeUpdateSDO();
                UpdateDTOFromDataForm(ref updateDTO);
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));
                var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE>("api/HisServiceType/UpdateSdo", ApiConsumers.MosConsumer, updateDTO, param);
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultData), resultData));
                if (resultData != null)
                {
                    success = true;
                    LoadDatagctFormList();
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

        private void UpdateDTOFromDataForm(ref ServiceTypeUpdateSDO currentDTO)
        {
            try
            {
                currentDTO.ServiceTypeId = this.currentData.ID;
                if (chkIsAutoSplitReq.Checked)
                {
                    currentDTO.IsAutoSplitReq = true;
                }
                else
                {
                    currentDTO.IsAutoSplitReq = null;
                }

                if (chkIsNotDisplayAssign.Checked)
                {
                    currentDTO.IsNotDisplayAssign = true;
                }
                else
                {
                    currentDTO.IsNotDisplayAssign = false;
                }

                if (spinNumOrder.EditValue != null)
                {
                    currentDTO.NumOrder = (long)spinNumOrder.Value;
                }
                else
                {
                    currentDTO.NumOrder = null;
                }
                if (chkIsSplitReqBySampleType.Checked)
                {
                    currentDTO.IsSplitReqBySampleType = true;
                }
                else
                {
                    currentDTO.IsSplitReqBySampleType = false;
                }
                if (chkIsRequiredSampleType.Checked)
                {
                    currentDTO.IsRequiredSampleType = true;
                }
                else
                {
                    currentDTO.IsRequiredSampleType = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                //dynamic filter = new System.Dynamic.ExpandoObject();
                HisServiceTypeFilter filter = new HisServiceTypeFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE>>("api/HisServiceType/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            SaveProcess();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnEdit.Enabled)
            {
                btnEdit_Click(null, null);
            }
        }

        private void chkIsSplitReqBySampleType_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                if (chkIsSplitReqBySampleType.Checked)
                {
                    chkIsRequiredSampleType.Enabled = true;
                }
                else
                {
                    chkIsRequiredSampleType.Enabled = false;
                    chkIsRequiredSampleType.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

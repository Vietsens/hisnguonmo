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
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.EmrDocumentGroup
{
    public partial class EmrDocumentGroupForm : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int ActionType = -1;
        EMR_DOCUMENT_GROUP currentData;
        EMR_DOCUMENT_GROUP resultData;
        DelegateSelectData delegateSelect = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        int positionHandle = -1;
        List<EMR_DOCUMENT_GROUP> listParentService;
        List<EMR_DOCUMENT_GROUP> currentDataStore;

        #endregion

        public EmrDocumentGroupForm(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData delegateData)
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

        public EmrDocumentGroupForm(Inventec.Desktop.Common.Modules.Module module)
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
        private void EmrDocumentGroupForm_Load(object sender, EventArgs e)
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

            LoaddataToTreeList();

            //SetCaptionByLanguageKey();

            //InitTabIndex();

            ValidateForm();

            SetDefaultFocus();

            this.InItComboParent(null);
        }


        public void LoaddataToTreeList()
        {
            try
            {
                CommonParam param = new CommonParam();
                EmrDocumentGroupFilter filter = new EmrDocumentGroupFilter();
                //filter.IS_LEAF = true;
                filter.KEY_WORD = txtFind.Text.Trim();

                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                WaitingManager.Show();
                currentDataStore = new BackendAdapter(param).Get<List<EMR_DOCUMENT_GROUP>>(HIS.Desktop.Plugins.EmrDocumentGroup.HisRequestUriStore.EMR_DOCUMENT_GROUP_GET, ApiConsumers.EmrConsumer, filter, param);


                treeList1.KeyFieldName = "ID";
                treeList1.ParentFieldName = "PARENT_ID";
                treeList1.DataSource = currentDataStore;
                treeList1.CollapseAll();
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboParentID(List<EMR_DOCUMENT_GROUP> listParentService)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOCUMENT_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DOCUMENT_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOCUMENT_GROUP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboParent, listParentService, controlEditorADO);
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
                ValidationSingleControl(txtEmrDocumentGroupCode);
                ValidationSingleControl(txtEmrDocumentGroupName);
                ValidationControlMaxLength(txtEmrDocumentGroupCode, 6);
                ValidationControlMaxLength(txtEmrDocumentGroupName, 200);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationControlMaxLength(BaseEdit control, int? maxLength)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.IsRequired = true;
            validate.ErrorText = "Nhập quá kí tự cho phép";
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            dxValidationProvider1.SetValidationRule(control, validate);
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

        //private void InitTabIndex()
        //{
        //    try
        //    {
        //        dicOrderTabIndexControl.Add("txtName", 1);
        //        dicOrderTabIndexControl.Add("txtCode", 0);


        //        if (dicOrderTabIndexControl != null)
        //        {
        //            foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
        //            {
        //                SetTabIndexToControl(itemOrderTab, layoutControl1);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}



        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resource.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.EmrDocumentGroupForm.Resource.Lang", typeof(HIS.Desktop.Plugins.EmrDocumentGroup.EmrDocumentGroupForm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("EmrDocumentGroupForm.layoutControl1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("EmrDocumentGroupForm.layoutControl3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("EmrDocumentGroupForm.layoutControl4.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grlSTT.Caption = Inventec.Common.Resource.Get.Value("HisExeServiceModuleForm.grlSTT.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gclCode.Caption = Inventec.Common.Resource.Get.Value("HisExeServiceModuleForm.gclCode.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.gclName.Caption = Inventec.Common.Resource.Get.Value("HisExeServiceModuleForm.gclName.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("EmrDocumentGroupForm.btnEdit.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("EmrDocumentGroupForm.btnReset.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnAdd.Text = Inventec.Common.Resource.Get.Value("HisExeServiceModuleForm.btnAdd.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("EmrDocumentGroupForm.layoutControl2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("EmrDocumentGroupForm.btnFind.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("EmrDocumentGroupForm.layoutControlItem3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.bar2.Text = Inventec.Common.Resource.Get.Value("HisExeServiceModuleForm.bar2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("HisExeServiceModuleForm.bbtnAdd.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("HisExeServiceModuleForm.barButtonItem2.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("HisExeServiceModuleForm.barButtonItem3.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.barButtonItem4.Caption = Inventec.Common.Resource.Get.Value("HisExeServiceModuleForm.barButtonItem4.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.bar1.Text = Inventec.Common.Resource.Get.Value("HisExeServiceModuleForm.bar1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("EmrDocumentGroupForm.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                if (this.currentModule != null && !string.IsNullOrEmpty(currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {


            }

        }

        private void SetFilterNavBar(ref EmrDocumentGroupFilter filter)
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
                //txtModuleLink.Enabled = true;
                //txtModuleLink.ReadOnly = true;
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
                //if (!btnEdit.Enabled)
                //    return;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                WaitingManager.Show();
                EMR_DOCUMENT_GROUP updateDTO = new EMR_DOCUMENT_GROUP();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                    var resultData = new BackendAdapter(param).Post<EMR_DOCUMENT_GROUP>(HIS.Desktop.Plugins.EmrDocumentGroup.HisRequestUriStore.EMR_DOCUMENT_GROUP_CREATE, ApiConsumers.EmrConsumer, updateDTO, param);
                    //Inventec.Common.Logging.LogSystem.Info(resultData.DOCUMENT_GROUP_CODE);
                    if (resultData != null)
                    {
                        success = true;
                        LoaddataToTreeList();
                        txtEmrDocumentGroupCode.Text = "";
                        txtEmrDocumentGroupName.Text = "";
                        txtSTT.Text = "";
                        cboParent.EditValue = null;
                        RefeshDataAfterSave(resultData);
                        ResetFormData();
                    }
                }
                else
                {

                    var resultData = new BackendAdapter(param).Post<EMR_DOCUMENT_GROUP>(HIS.Desktop.Plugins.EmrDocumentGroup.HisRequestUriStore.EMR_DOCUMENT_GROUP_UPDATE, ApiConsumers.EmrConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        LoaddataToTreeList();
                        RefeshDataAfterSave(resultData);
                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<EMR_DOCUMENT>();
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
                txtEmrDocumentGroupCode.Focus();
                txtEmrDocumentGroupCode.SelectAll();
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
                            txtEmrDocumentGroupCode.Focus();

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

        private void RefeshDataAfterSave(EMR_DOCUMENT_GROUP data)
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

        private void UpdateDTOFromDataForm(ref EMR_DOCUMENT_GROUP currentDTO)
        {
            try
            {
                currentDTO.DOCUMENT_GROUP_CODE = txtEmrDocumentGroupCode.Text.Trim();
                currentDTO.DOCUMENT_GROUP_NAME = txtEmrDocumentGroupName.Text.Trim();
                if (cboParent.EditValue != null)
                {
                    currentDTO.PARENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboParent.EditValue.ToString());
                }
                else
                {
                    currentDTO.PARENT_ID = null;
                }

                if (!String.IsNullOrWhiteSpace(txtSTT.Text))
                {
                    currentDTO.NUM_ORDER = long.Parse(txtSTT.Text);
                }
                else
                {
                    currentDTO.NUM_ORDER = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref EMR_DOCUMENT_GROUP currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                EmrDocumentGroupFilter filter = new EmrDocumentGroupFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<EMR_DOCUMENT_GROUP>>(HIS.Desktop.Plugins.EmrDocumentGroup.HisRequestUriStore.EMR_DOCUMENT_GROUP_GET, ApiConsumers.EmrConsumer, filter, param).FirstOrDefault();
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
                this.currentData = null;
                this.ActionType = GlobalVariables.ActionAdd;
                InItComboParent(null);
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                txtEmrDocumentGroupCode.Text = "";
                txtEmrDocumentGroupName.Text = "";
                txtFind.Text = "";
                txtSTT.Text = "";
                cboParent.EditValue = null;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                ResetFormData();
                SetFocusEditor();
                LoaddataToTreeList();

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
                LoaddataToTreeList();
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
                    EMR_DOCUMENT_GROUP pData = (EMR_DOCUMENT_GROUP)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.MODIFY_TIME.ToString()));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.CREATE_TIME.ToString()));
                        }
                        catch (Exception ex) { }
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
                EMR_DOCUMENT_GROUP data = null;
                if (e.RowHandle > -1)
                {
                    data = (EMR_DOCUMENT_GROUP)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGUnLock : btnGLock);
                    }
                    if (e.Column.FieldName == "DELETE")
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

        private void ChangedDataRow(EMR_DOCUMENT_GROUP data)
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

        private void FillDataToEditorControl(EMR_DOCUMENT_GROUP data)
        {
            try
            {
                if (data != null)
                {
                    txtEmrDocumentGroupCode.Text = data.DOCUMENT_GROUP_CODE;
                    txtEmrDocumentGroupName.Text = data.DOCUMENT_GROUP_NAME;
                    txtSTT.Text = data.NUM_ORDER.ToString();
                    cboParent.EditValue = data.PARENT_ID;
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
            EMR_DOCUMENT_GROUP success = new EMR_DOCUMENT_GROUP();
            //bool notHandler = false;
            try
            {
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                EMR_DOCUMENT_GROUP rowData = data as EMR_DOCUMENT_GROUP;
                if (rowData != null && MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    EMR_DOCUMENT_GROUP data1 = new EMR_DOCUMENT_GROUP();
                    data1.ID = rowData.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<EMR_DOCUMENT_GROUP>(EMR.URI.EmrDocumentGroup.LOCK, ApiConsumers.EmrConsumer, data, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<EMR_DOCUMENT_GROUP>();
                        BackendDataWorker.Reset<EMR_DOCUMENT>();
                        rs = true;
                        LoaddataToTreeList();
                        currentData = success;
                        InItComboParent(currentData);
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

        private void InItComboParent(EMR_DOCUMENT_GROUP currentDocument)
        {
            try
            {
                if (currentDocument != null)
                {
                    listParentService = currentDataStore.Where(o => o.IS_ACTIVE == 1
                          && (o.ID != currentDocument.ID && !("/" + o.VIR_PATH + "/").Contains(("/" + currentDocument.ID + "/")))).ToList();
                }
                else
                {
                    listParentService = currentDataStore.Where(o => o.IS_ACTIVE == 1).ToList();
                }

                InitComboParentID(listParentService);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool rs = false;

            EMR_DOCUMENT_GROUP success = new EMR_DOCUMENT_GROUP();

            try
            {
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                EMR_DOCUMENT_GROUP rowData = data as EMR_DOCUMENT_GROUP;
                if (rowData != null && MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<EMR_DOCUMENT_GROUP>(EMR.URI.EmrDocumentGroup.UNLOCK, ApiConsumers.EmrConsumer, data, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<EMR_DOCUMENT_GROUP>();
                        BackendDataWorker.Reset<EMR_DOCUMENT>();
                        rs = true;
                        LoaddataToTreeList();
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
                    var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                    EMR_DOCUMENT_GROUP rowData = data as EMR_DOCUMENT_GROUP;
                    if (rowData != null)
                    {

                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(HIS.Desktop.Plugins.EmrDocumentGroup.HisRequestUriStore.EMR_DOCUMENT_GROUP_DELETE, ApiConsumers.EmrConsumer, rowData.ID, param);
                        if (success)
                        {
                            this.ActionType = 1;
                            txtEmrDocumentGroupName.Text = "";
                            txtEmrDocumentGroupCode.Text = "";
                            EnableControlChanged(this.ActionType);
                            LoaddataToTreeList();
                            currentData = ((List<EMR_DOCUMENT_GROUP>)treeList1.DataSource).FirstOrDefault();
                            BackendDataWorker.Reset<EMR_DOCUMENT_GROUP>();
                            BackendDataWorker.Reset<EMR_DOCUMENT>();
                            InItComboParent(currentData);
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




        private void txtFind_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
                treeList1.Focus();
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

        private void txtSTT_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    cboParent.Focus();
                }
                else if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtEmrDocumentGroupCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtEmrDocumentGroupName.Focus();
                txtEmrDocumentGroupName.SelectAll();
            }
        }

        private void txtEmrDocumentGroupName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtSTT.Focus();
                txtSTT.SelectAll();
            }
        }

        private void txtSTT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cboParent.Focus();
                //cboParent.ShowPopup();
            }
        }

        private void treeList1_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = treeList1.GetDataRecordByNode(e.Node);
                if (data != null && data is EMR_DOCUMENT_GROUP)
                {
                    EMR_DOCUMENT_GROUP rowData = data as EMR_DOCUMENT_GROUP;
                    if (rowData == null) return;

                    else if (e.Column.FieldName == "IS_LOCK")
                    {
                        e.RepositoryItem = (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGLock : btnGUnLock);

                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        try
                        {
                            if (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.RepositoryItem = btnDeleteEnable;
                            else
                                e.RepositoryItem = btnDeleteDisable;
                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    EMR_DOCUMENT_GROUP pData = (EMR_DOCUMENT_GROUP)e.Row;
                    //var pData = data as V_HIS_SERVICE;
                    if (pData == null || this.treeList1 == null) return;

                    if (e.Column.FieldName == "CREATE_TIME_STR")
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_Click(object sender, EventArgs e)
        {
            try
            {
                //InitComboParentID();
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                EMR_DOCUMENT_GROUP rowData = data as EMR_DOCUMENT_GROUP;
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);
                }

                if (currentDataStore != null && currentDataStore.Count > 0 && currentData != null)
                {
                    InItComboParent(currentData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeList1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                    EMR_DOCUMENT_GROUP rowData = data as EMR_DOCUMENT_GROUP;
                    if (rowData != null)
                    {
                        currentData = rowData;
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParent_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboParent.ShowPopup();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParent_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
            {
                cboParent.EditValue = null;
            }
        }

        private void cboParent_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
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

    }
}

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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using EMR.Filter;
using EMR.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EmrDocumentType
{
    public partial class EmrDocumentTypeForm : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int ActionType = -1;
        EMR_DOCUMENT_TYPE currentData;
        EMR_DOCUMENT_TYPE resultData;
        DelegateSelectData delegateSelect = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        int positionHandle = -1;

        #endregion

        public EmrDocumentTypeForm(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData delegateData)
            : base(module)
        {
            InitializeComponent();
            try
            {
                currentModule = module;
                this.delegateSelect = delegateData;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public EmrDocumentTypeForm(Inventec.Desktop.Common.Modules.Module module)
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
        private void EmrDocumentTypeForm_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();

                EnableControlChanged(this.ActionType);

                FillDatagctFormList();

                SetCaptionByLanguageKey();

                InitTabIndex();

                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //private void SetToolTip
        //{ 
        //    toolTipController1.
        //    toolTipController.SetToolTip(spinEdit1, "Đây là tooltip cho SpinEdit.")
        //}

        private void ValidateForm()
        {
            try
            {
                //ValidationSingleControl(txtName);
                //ValidationSingleControl(txtCode);
                ValidationControlMaxLength(txtName, 100);
                ValidationControlMaxLength(txtCode, 2);
                //ValidationControlMaxLength(txtSTT, 19);
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

        private void InitTabIndex()
        {
            try
            {
                dicOrderTabIndexControl.Add("txtName", 1);
                dicOrderTabIndexControl.Add("txtCode", 0);
                dicOrderTabIndexControl.Add("txtSTT", 2);

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
            {////Khoi tao doi tuong resource
                Resource.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.EmrDocumentType.Resource.Lang", typeof(HIS.Desktop.Plugins.EmrDocumentType.EmrDocumentTypeForm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.layoutControl1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.layoutControl3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.layoutControl4.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grlSTT.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.grlSTT.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclCode.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.gclCode.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclName.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.gclName.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.btnEdit.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.btnReset.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.btnAdd.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.layoutControl2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.btnFind.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.layoutControlItem2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.layoutControlItem3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.bar2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.bbtnAdd.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.barButtonItem2.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.barButtonItem3.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem4.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.barButtonItem4.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.bar1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclCreateTime.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.grclCreateTime.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclCreator.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.grclCreator.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclModifyTime.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.grclModifyTime.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclModifiter.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.grclModifiter.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsAllowPatientIssue.Text = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.lciIsAllowPatientIssue.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gvIsAllowPatientIssue.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.gvAllowPatientIssue.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsMedicalPaymentEvidence.Text = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.lciIsMedicalPaymentEvidence.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclIsMedicalPaymentEvidence.Caption = Inventec.Common.Resource.Get.Value("EmrDocumentTypeForm.IsMedicalPaymentEvidence.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModule != null && !string.IsNullOrEmpty(currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Core.ApiResultObject<List<EMR_DOCUMENT_TYPE>> apiResult = null;
                EmrDocumentTypeFilter filter = new EmrDocumentTypeFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<EMR_DOCUMENT_TYPE>>(HIS.Desktop.Plugins.EmrDocumentType.EmrRequestUriStore.EMR_DOCUMENT_TYPE_GET, ApiConsumers.EmrConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<EMR_DOCUMENT_TYPE>)apiResult.Data;
                    if (data != null)
                    {
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("EMR_DOCUMENT_TYPE data ", data));
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

        private void SetFilterNavBar(ref EmrDocumentTypeFilter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtFind.Text.Trim()))
                {
                    filter.KEY_WORD = txtFind.Text.Trim();
                }
                else
                    filter.KEY_WORD = null;
                
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
                numFixZoom.EditValue = null;
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
                EMR_DOCUMENT_TYPE updateDTO = new EMR_DOCUMENT_TYPE();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                    var resultData = new BackendAdapter(param).Post<EMR_DOCUMENT_TYPE>(HIS.Desktop.Plugins.EmrDocumentType.EmrRequestUriStore.EMR_DOCUMENT_TYPE_CREATE, ApiConsumers.EmrConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDatagctFormList();

                        RefeshDataAfterSave(resultData);
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<EMR_DOCUMENT_TYPE>(HIS.Desktop.Plugins.EmrDocumentType.EmrRequestUriStore.EMR_DOCUMENT_TYPE_UPDATE, ApiConsumers.EmrConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDatagctFormList();
                        RefeshDataAfterSave(resultData);
                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<HIS_MACHINE>();
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
                txtCode.Text = "";
                txtName.Text = "";
                txtSTT.Text = "";
                txtKDK.Text = "";
                chkRequire.EditValue = false;
                chkIS_HAS_ONE.EditValue = false;
                chkIsSignParallel.EditValue = false;
                chkIS_MULTI_SIGN.EditValue = false;
                chkAllowDuplicateHisCode.EditValue = false;
                chkIsAllowPatientIssue.EditValue = false;
                chkIsMedicalPaymentEvidence.EditValue = false;
                chkCancelSign.EditValue = false;
                chkIsRequiredToComplete.Checked = false;
                chkPatientMustSign.Checked = false;
                cboPatientSignatureDisplayType.SelectedIndex = -1;
                numFixZoom.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshDataAfterSave(EMR_DOCUMENT_TYPE data)
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

        private void UpdateDTOFromDataForm(ref EMR_DOCUMENT_TYPE currentDTO)
        {
            try
            {
                if (chkRequire.Checked)
                    currentDTO.IS_MANDATORY = (short)1;
                else
                    currentDTO.IS_MANDATORY = null;

                if (chkIS_HAS_ONE.Checked)
                    currentDTO.IS_HAS_ONE = (short)1;
                else
                    currentDTO.IS_HAS_ONE = null;

                if (chkIS_MULTI_SIGN.Checked)
                    currentDTO.IS_MULTI_SIGN = (short)1;
                else
                    currentDTO.IS_MULTI_SIGN = null;

                if (chkIsSignParallel.Checked)
                    currentDTO.IS_SIGN_PARALLEL = (short)1;
                else
                    currentDTO.IS_SIGN_PARALLEL = null;

                if (chkAllowDuplicateHisCode.Checked)
                    currentDTO.IS_ALLOW_DUPLICATE_HIS_CODE = (short)1;
                else
                    currentDTO.IS_ALLOW_DUPLICATE_HIS_CODE = null;

                if (chkIsAllowPatientIssue.Checked)
                    currentDTO.IS_ALLOW_PATIENT_ISSUE = (short)1;
                else
                    currentDTO.IS_ALLOW_PATIENT_ISSUE = null;


                if (chkIsMedicalPaymentEvidence.Checked)
                    currentDTO.IS_MEDICAL_PAYMENT_EVIDENCE = (short)1;
                else
                    currentDTO.IS_MEDICAL_PAYMENT_EVIDENCE = null;

                if (chkCancelSign.Checked)
                    currentDTO.MUST_CANCEL_BEFORE_DELETE = (short)1;
                else
                    currentDTO.MUST_CANCEL_BEFORE_DELETE = null;

                if (chkIsRequiredToComplete.Checked)
                    currentDTO.IS_REQUIRED_TO_COMPLETE = (short)1;
                else
                    currentDTO.IS_REQUIRED_TO_COMPLETE = null;
                if (chkPatientMustSign.Checked)
                    currentDTO.PATIENT_MUST_SIGN = (short)1;
                else
                    currentDTO.PATIENT_MUST_SIGN = null;

                currentDTO.DOCUMENT_TYPE_CODE = txtCode.Text.Trim();
                currentDTO.DOCUMENT_TYPE_NAME = txtName.Text.Trim();

                if (!string.IsNullOrEmpty(txtSTT.Text))
                    currentDTO.NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt32(txtSTT.Text.Trim());

                currentDTO.SIGNER_EXCEPTION_LOGINNAME = txtKDK.Text != null ? txtKDK.Text : null;
                currentDTO.PATIENT_SIGNATURE_DISPLAY_TYPE = cboPatientSignatureDisplayType.SelectedIndex != -1 ? (long?)cboPatientSignatureDisplayType.SelectedIndex : null;
                if (numFixZoom.EditValue == null)
                {
                    currentDTO.VIEW_ZOOM_PERCENT = null;
                }
                else
                {
                    long zoomValueLong = (long)numFixZoom.Value;
                    currentDTO.VIEW_ZOOM_PERCENT = zoomValueLong;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref EMR_DOCUMENT_TYPE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                EmrDocumentTypeFilter filter = new EmrDocumentTypeFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<EMR_DOCUMENT_TYPE>>(HIS.Desktop.Plugins.EmrDocumentType.EmrRequestUriStore.EMR_DOCUMENT_TYPE_GET, ApiConsumers.EmrConsumer, filter, param).FirstOrDefault();
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
                txtFind.Text = "";
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                ResetFormData();
                SetFocusEditor();
                FillDatagctFormList();
                txtCode.Enabled = true;
                txtName.Enabled = true;
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
                    EMR_DOCUMENT_TYPE pData = (EMR_DOCUMENT_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.MODIFY_TIME.ToString()));
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.CREATE_TIME.ToString()));
                    }
                    else if (e.Column.FieldName == "IS_MANDATORY_ch")
                    {
                        e.Value = pData.IS_MANDATORY == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_MULTI_SIGN_ch")
                    {
                        e.Value = pData.IS_MULTI_SIGN == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_HAS_ONE_ch")
                    {
                        e.Value = pData.IS_HAS_ONE == 1 ? true : false;
                    } 
                    else if (e.Column.FieldName == "IS_SIGN_PARALLEL_CHK")
                    {
                        e.Value = pData.IS_SIGN_PARALLEL == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_ALLOW_DUPLICATE_HIS_CODE")
                    {
                        e.Value = pData.IS_ALLOW_DUPLICATE_HIS_CODE == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "GV_IS_ALLOW_PATIENT_ISSUE_ch")
                    {
                        e.Value = pData.IS_ALLOW_PATIENT_ISSUE == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_MEDICAL_PAYMENT_EVIDENCE_ch")
                    {
                        e.Value = pData.IS_MEDICAL_PAYMENT_EVIDENCE == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_REQUIRED_TO_COMPLETE_ch")
                    {
                        e.Value = pData.IS_REQUIRED_TO_COMPLETE == 1 ? true : false;
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
                EMR_DOCUMENT_TYPE data = null;
                if (e.RowHandle > -1)
                {
                    data = (EMR_DOCUMENT_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnLock : btnUnLock);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = ((data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && data.IS_SYSTEM != 1) ? btnDeleteEnable : btnDeleteDisable);
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
                this.currentData = (EMR_DOCUMENT_TYPE)gridView1.GetFocusedRow();
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

        private void ChangedDataRow(EMR_DOCUMENT_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    txtCode.Enabled = data.IS_SYSTEM != 1 ? true : false;
                    txtName.Enabled = data.IS_SYSTEM != 1 ? true : false;
                    txtKDK.Text = data.SIGNER_EXCEPTION_LOGINNAME;
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

        private void FillDataToEditorControl(EMR_DOCUMENT_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    chkRequire.EditValue = data.IS_MANDATORY == 1 ? true : false;
                    chkIS_HAS_ONE.EditValue = data.IS_HAS_ONE == 1 ? true : false;
                    chkIS_MULTI_SIGN.EditValue = data.IS_MULTI_SIGN == 1 ? true : false;
                    chkIsSignParallel.EditValue = data.IS_SIGN_PARALLEL == 1 ? true : false;
                    txtCode.Text = data.DOCUMENT_TYPE_CODE;
                    txtName.Text = data.DOCUMENT_TYPE_NAME;
                    txtSTT.Text = data.NUM_ORDER !=null ?data.NUM_ORDER.ToString():null;
                    txtKDK.Text = data.SIGNER_EXCEPTION_LOGINNAME;
                    chkAllowDuplicateHisCode.EditValue = data.IS_ALLOW_DUPLICATE_HIS_CODE == 1 ? true : false;
                    chkIsAllowPatientIssue.EditValue = data.IS_ALLOW_PATIENT_ISSUE == 1 ? true : false;
                    decimal? zoomValueNullable = data.VIEW_ZOOM_PERCENT;
                    if (zoomValueNullable.HasValue)
                    {
                        decimal zoomValue = zoomValueNullable.Value;
                        numFixZoom.Value = zoomValue;
                    }
                    else
                    {
                        //numFixZoom.Value = 0;
                        numFixZoom.EditValue = null;
                    }

                    //chkIsMedicalPaymentEvidence.EditValue = data.IS_MEDICAL_PAYMENT_EVIDENCE == 1 ? true : false;
                    chkIsMedicalPaymentEvidence.Checked = data.IS_MEDICAL_PAYMENT_EVIDENCE == 1 ? true : false;
                    chkCancelSign.Checked = data.MUST_CANCEL_BEFORE_DELETE == 1 ? true : false;
                    chkIsRequiredToComplete.Checked = data.IS_REQUIRED_TO_COMPLETE == 1 ? true : false;
                    chkPatientMustSign.Checked = data.PATIENT_MUST_SIGN == 1 ? true : false;
                    cboPatientSignatureDisplayType.SelectedIndex = data.PATIENT_SIGNATURE_DISPLAY_TYPE.HasValue ? (int)data.PATIENT_SIGNATURE_DISPLAY_TYPE : -1;
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
            EMR_DOCUMENT_TYPE success = new EMR_DOCUMENT_TYPE();
            //bool notHandler = false;
            try
            {
                EMR_DOCUMENT_TYPE data = (EMR_DOCUMENT_TYPE)gridView1.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    EMR_DOCUMENT_TYPE data1 = new EMR_DOCUMENT_TYPE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<EMR_DOCUMENT_TYPE>(EMR.URI.EmrDocumentType.UNLOCK, ApiConsumers.EmrConsumer, data, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<HIS_MACHINE>();
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
            EMR_DOCUMENT_TYPE success = new EMR_DOCUMENT_TYPE();
            //bool notHandler = false;

            try
            {
                EMR_DOCUMENT_TYPE data = (EMR_DOCUMENT_TYPE)gridView1.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<EMR_DOCUMENT_TYPE>(EMR.URI.EmrDocumentType.LOCK, ApiConsumers.EmrConsumer, data, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<HIS_MACHINE>();
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
                    var rowData = (EMR_DOCUMENT_TYPE)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {

                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(HIS.Desktop.Plugins.EmrDocumentType.EmrRequestUriStore.EMR_DOCUMENT_TYPE_DELETE, ApiConsumers.EmrConsumer, rowData.ID, param);
                        if (success)
                        {
                            this.ActionType = 1;
                            txtName.Text = "";
                            txtCode.Text = "";
                            txtSTT.Text = "";
                            txtKDK.Text = "";
                            //numFixZoom.Value = 0;
                            numFixZoom.EditValue = null;
                            cboPatientSignatureDisplayType.SelectedIndex = -1;
                            EnableControlChanged(this.ActionType);
                            FillDatagctFormList();
                            currentData = ((List<EMR_DOCUMENT_TYPE>)gridControl1.DataSource).FirstOrDefault();
                            BackendDataWorker.Reset<HIS_MACHINE>();
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
            if (e.KeyCode == Keys.Enter)
            {
                txtName.Focus();
                txtName.SelectAll();
            }
        }

        private void txtName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtSTT.Focus();
                txtSTT.SelectAll();
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

        private void chkIS_HAS_ONE_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                chkIsSignParallel.Focus();
            }
        }

        private void chkIS_MULTI_SIGN_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                chkIS_HAS_ONE.Focus();
            }
        }

        private void chkIS_HAS_ONE_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtSTT_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;

        }

        private void txtSTT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtKDK.Focus();
                txtKDK.SelectAll();
            }
        }

        private void chkIsSignParallel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                chkRequire.Focus();
            }
        }

        private void chkRequire_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAllowDuplicateHisCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAllowDuplicateHisCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsAllowPatientIssue.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsAllowPatientIssue_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsMedicalPaymentEvidence.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void chkIsMedicalPaymentEvidence_KeyDown(object sender, KeyEventArgs e)
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
                    {
                        btnEdit.Focus();
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetDoNotAllowLoginNames(string delegateLoginNames)
        {
            try
            {
                if (!string.IsNullOrEmpty(delegateLoginNames))
                {
                    txtKDK.Text = delegateLoginNames;
                }
                else
                {
                    txtKDK.Text = "";
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKDK_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    WaitingManager.Show();
                    // string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    //CallModule callModule = new CallModule(CallModule.SecondaryIcd, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    frmAcsUser frm = new frmAcsUser(GetDoNotAllowLoginNames, txtKDK.Text);
                    frm.ShowDialog();
                    WaitingManager.Hide();
                }
                else
                    if (e.KeyCode == Keys.Enter)
                    {
                        chkIS_MULTI_SIGN.Focus();
                    }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIS_HAS_ONE_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkIsMedicalPaymentEvidence_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCancelSign.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCancelSign_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsRequiredToComplete.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsRequiredToComplete_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkPatientMustSign.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientSignatureDisplayType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cboPatientSignatureDisplayType.Properties.Buttons[1].Visible = cboPatientSignatureDisplayType.SelectedIndex != -1 ? true : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPatientMustSign_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void gridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "VIEW_ZOOM_PERCENT")
                {
                    if (e.Value != null)
                    {
                        e.DisplayText = e.Value.ToString() + "%";
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

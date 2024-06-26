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
using HIS.Desktop.Plugins.EmrSigner.Properties;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using EMR.Filter;
using EMR.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using ACS.EFMODEL.DataModels;
using System.IO;
using EMR.SDO;
using HIS.Desktop.Plugins.EmrSigner.ValitionRule;
using System.Drawing.Drawing2D;
using DevExpress.XtraEditors.Controls;
using System.Diagnostics;
using DevExpress.Utils.Menu;

namespace HIS.Desktop.Plugins.EmrSigner
{
    public partial class EmrSignerForm : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int ActionType = -1;
        EMR_SIGNER currentData;
        EMR_SIGNER resultData;
        DelegateSelectData delegateSelect = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        int positionHandle = -1;
        List<HIS_DEPARTMENT> ListDepartment = new List<HIS_DEPARTMENT>();
        List<ACS_USER> ListUsser = new List<ACS_USER>();
        bool isNotLoadWhileChangeControlStateInFirst = false;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        const string moduleLink = "HIS.Desktop.Plugins.EmrSigner";
        string deviceName = "";
        #endregion

        public EmrSignerForm(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData delegateData)
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

        public EmrSignerForm(Inventec.Desktop.Common.Modules.Module module)
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
        private void EmrSignerForm_Load(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.LocalStorage.EmrConfig.ConfigLoader.Refresh();
                Config.ConfigKey.GetConfigKey();
                MeShow();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstan.DeviceName)
                        {
                            this.deviceName = item.VALUE;
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.deviceName), this.deviceName));
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void MeShow()
        {
            SetDefaultValue();

            EnableControlChanged(this.ActionType);

            FillDatagctFormList();

            LoadDatacboThongTinKy();

            SetCaptionByLanguageKey();

            InitTabIndex();

            InitCombo();

            ValidateForm();

            InitControlState();

            SetDefaultFocus();
        }

        private void InitCombo()
        {
            InitComboDepartment();
            InitComboUser();
        }
        private void InitComboUser()
        {
            ListUsser = BackendDataWorker.Get<ACS_USER>() ?? new List<ACS_USER>();
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
            columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 350);
            ControlEditorLoader.Load(cboUser, ListUsser.Where(o => !string.IsNullOrEmpty(o.LOGINNAME) && !string.IsNullOrEmpty(o.USERNAME)).ToList(), controlEditorADO);
        }

        private void InitComboDepartment()
        {

            ListDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>();
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
            columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "DEPARTMENT_CODE", columnInfos, false, 350);
            ControlEditorLoader.Load(cboDepartment, ListDepartment.Where(o => !string.IsNullOrEmpty(o.DEPARTMENT_CODE) && !string.IsNullOrEmpty(o.DEPARTMENT_NAME)).ToList(), controlEditorADO);
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
                ValidationSingleControl(cboDepartment);
                ValidationSingleControl(cboUser);
                ValidationControlMaxLength(txtTitle, 100);
                ValidationControlMaxLength(txtEmail, 100);
                // Check Length vượt quá cho phép
                ValidationControlMaxLengthNoRequest(txtSCA_Serial, 50);
                ValidationControlMaxLengthNoRequest(txtPhone, 20);
                ValidationControlMaxLengthNoRequest(txtPcaSerial, 50);
                ValidationControlMaxLengthNoRequest(txtSignerCode, 50);
                //ValidationControlPic(picSignImage);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationControlPic(PictureEdit control)
        {
            SIGN_IMAGE_Validation validate = new SIGN_IMAGE_Validation();
            validate.picturEdit = control;
            validate.ErrorText = "Kích thước ảnh phải trong giới hạn kích thước quy định dài 600px, cao 600px";
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            dxValidationProvider1.SetValidationRule(control, validate);
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

        private void ValidationControlMaxLengthNoRequest(BaseEdit control, int? maxLength)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.IsRequired = false;
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
            {
                HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.EmrSigner.Resource.Lang", typeof(HIS.Desktop.Plugins.EmrSigner.EmrSignerForm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.layoutControl1.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("EmrSignerForm.cboDepartment.Properties.NullText", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.bar2.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("EmrSignerForm.bbtnAdd.Caption", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("EmrSignerForm.barButtonItem2.Caption", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("EmrSignerForm.barButtonItem3.Caption", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem4.Caption = Inventec.Common.Resource.Get.Value("EmrSignerForm.barButtonItem4.Caption", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.layoutControl3.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.layoutControl4.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grlSTT.Caption = Inventec.Common.Resource.Get.Value("EmrSignerForm.grlSTT.Caption", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclCode.Caption = Inventec.Common.Resource.Get.Value("EmrSignerForm.gclCode.Caption", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclName.Caption = Inventec.Common.Resource.Get.Value("EmrSignerForm.gclName.Caption", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("EmrSignerForm.gridColumn1.Caption", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("EmrSignerForm.gridColumn2.Caption", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("EmrSignerForm.gridColumn3.Caption", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclIntegrateAddress.Caption = Inventec.Common.Resource.Get.Value("EmrSignerForm.grclIntegrateAddress.Caption", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColEmail.Caption = Inventec.Common.Resource.Get.Value("EmrSignerForm.grdColEmail.Caption", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.btnEdit.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.btnReset.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.btnAdd.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.layoutControl2.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.btnFind.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.layoutControlItem13.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.layoutControlItem14.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.bar1.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.layoutControlItem15.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboUser.Properties.NullText = Inventec.Common.Resource.Get.Value("EmrSignerForm.cboUser.Properties.NullText", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.layoutControlItem16.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEmail.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.lcEmail.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciSerialSCA.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.lciSerialSCA.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciPhone.Text = Inventec.Common.Resource.Get.Value("EmrSignerForm.lciPhone.Text", HIS.Desktop.Plugins.EmrConsumer.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

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
                Inventec.Core.ApiResultObject<List<EMR_SIGNER>> apiResult = null;
                EmrSignerFilter filter = new EmrSignerFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<EMR_SIGNER>>(HIS.Desktop.Plugins.EmrSigner.HisRequestUriStore.EMR_SIGNER_GET, ApiConsumers.EmrConsumer, filter, paramCommon);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                if (apiResult != null)
                {
                    var data = (List<EMR_SIGNER>)apiResult.Data;
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

        private void SetFilterNavBar(ref EmrSignerFilter filter)
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

                if (Inventec.Common.TypeConvert.Parse.ToInt32(Config.ConfigKey.intergrateOption) == (int)EmrSignerCFG.EmrHsmIntegrateOption.USING_EasySign)
                    btnUploadImageUsingSigCertificate.Enabled = true;
                else
                    btnUploadImageUsingSigCertificate.Enabled = false;
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
                EMR_SIGNER updateDTO = new EMR_SIGNER();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                EmrSignerSDO ndata = new EmrSignerSDO();
                ndata.EmrSigner = updateDTO;
                if (picSignImage.Image != null)
                {
                    Bitmap bImage = ResizeSignImage();
                    ndata.ImgBase64Data = Convert.ToBase64String(imageToByteArray(bImage));
                    picSignImage.Image = bImage;
                }
                else
                {
                    ndata.ImgBase64Data = null;
                    ndata.EmrSigner.SIGN_IMAGE = null;
                }
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<EMR_SIGNER>(HIS.Desktop.Plugins.EmrSigner.HisRequestUriStore.EMR_SIGNER_CREATE, ApiConsumers.EmrConsumer, ndata, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDatagctFormList();
                        cboUser.EditValue = null;
                        txtTitle.Text = "";
                        cboDepartment.EditValue = null;
                        txtNumOrder.EditValue = 0;
                        txtEmail.Text = "";
                        RefeshDataAfterSave(resultData);
                        ResetFormData();
                        txtPhone.Text = "";
                        txtSCA_Serial.Text = "";

                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<EMR_SIGNER>(HIS.Desktop.Plugins.EmrSigner.HisRequestUriStore.EMR_SIGNER_UPDATE, ApiConsumers.EmrConsumer, ndata, param);
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
                cboUser.Focus();
                cboUser.SelectAll();
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
                            cboUser.Focus();
                            txtCmndNumber.Text = "";
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

        private void RefeshDataAfterSave(EMR_SIGNER data)
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

        private void UpdateDTOFromDataForm(ref EMR_SIGNER currentDTO)
        {
            try
            {
                //MemoryStream ms = new MemoryStream();
                //picSignImage.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                //var SignImage = ms.ToArray();
                currentDTO.LOGINNAME = cboUser.EditValue.ToString();
                currentDTO.USERNAME = ListUsser.Where(o => o.LOGINNAME == cboUser.EditValue.ToString()).FirstOrDefault().USERNAME;
                currentDTO.TITLE = txtTitle.Text.Trim();
                if (txtNumOrder.Text != null && txtNumOrder.Text != "")
                {
                    currentDTO.NUM_ORDER = Convert.ToInt16(txtNumOrder.Text.Trim());
                }
                else
                {
                    currentDTO.NUM_ORDER = null;
                }
                currentDTO.EMAIL = txtEmail.Text;
                currentDTO.PCA_SERIAL = txtPcaSerial.Text.Trim();
                currentDTO.HSM_USER_CODE = txtSignerCode.Text.Trim();
                currentDTO.PHONE = txtPhone.Text;
                currentDTO.SCA_SERIAL = txtSCA_Serial.Text;
                if (cboDepartment.EditValue != null)
                {
                    currentDTO.DEPARTMENT_CODE = cboDepartment.EditValue.ToString();
                    currentDTO.DEPARTMENT_NAME = ListDepartment.Where(o => o.DEPARTMENT_CODE == cboDepartment.EditValue.ToString()).FirstOrDefault().DEPARTMENT_NAME;
                }
                else
                {
                    currentDTO.DEPARTMENT_CODE = null;
                    currentDTO.DEPARTMENT_NAME = null;
                }

                if (cboThongTinKy.EditValue != null)
                {
                    currentDTO.SIGNATURE_DISPLAY_TYPE = long.Parse(cboThongTinKy.EditValue.ToString() ?? "0");
                }
                else
                {
                    currentDTO.SIGNATURE_DISPLAY_TYPE = null;
                }

                decimal n;
                bool isNumeric = decimal.TryParse(txtKichThuoc.Text, out n);
                if (isNumeric == true)
                {
                    currentDTO.SIGNALTURE_IMAGE_WIDTH = n;
                }
                if (string.IsNullOrEmpty(txtKichThuoc.Text))
                {
                    currentDTO.SIGNALTURE_IMAGE_WIDTH = null;
                }

                currentDTO.CMND_NUMBER = txtCmndNumber.Text;
                currentDTO.SECRET_KEY = txtSecretKey.Text;
                currentDTO.PASSWORD = txtPassword.Text;
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("currentDTO: ", currentDTO));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref EMR_SIGNER currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                EmrSignerFilter filter = new EmrSignerFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<EMR_SIGNER>>(HIS.Desktop.Plugins.EmrSigner.HisRequestUriStore.EMR_SIGNER_GET, ApiConsumers.EmrConsumer, filter, param).FirstOrDefault();
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
                txtTitle.Text = "";
                txtNumOrder.EditValue = 0;
                cboUser.EditValue = null;
                txtFind.Text = "";
                cboDepartment.EditValue = null;
                txtEmail.Text = "";
                txtSCA_Serial.Text = "";
                txtPhone.Text = "";
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
                    EMR_SIGNER pData = (EMR_SIGNER)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    if (e.Column.FieldName == "SIGNATURE_DISPLAY_TYPE_STR")
                    {
                        if (pData.SIGNATURE_DISPLAY_TYPE == 0)
                        {
                            e.Value = "Không hiểu thị chữ ký";
                        }
                        if (pData.SIGNATURE_DISPLAY_TYPE == 1)
                        {
                            e.Value = "Chỉ hiển thị thông tin ký";
                        }
                        if (pData.SIGNATURE_DISPLAY_TYPE == 2)
                        {
                            e.Value = "Chỉ hiển thị ảnh chữ ký";
                        }
                        if (pData.SIGNATURE_DISPLAY_TYPE == 3)
                        {
                            e.Value = "Hiển thị cả ảnh chữ ký & thông tin ký";
                        }
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
                EMR_SIGNER data = null;
                if (e.RowHandle > -1)
                {
                    data = (EMR_SIGNER)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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

        }

        private void ChangedDataRow(EMR_SIGNER data)
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

        private void FillDataToEditorControl(EMR_SIGNER data)
        {
            try
            {
                if (data != null)
                {
                    ResetFormData();
                    txtPcaSerial.EditValue = data.PCA_SERIAL;
                    txtSignerCode.EditValue = data.HSM_USER_CODE;
                    txtNumOrder.EditValue = data.NUM_ORDER;
                    if (ListUsser.Exists(o => o.LOGINNAME == data.LOGINNAME && !string.IsNullOrEmpty(o.LOGINNAME) && !string.IsNullOrEmpty(o.USERNAME)))
                    {
                        cboUser.EditValue = data.LOGINNAME;
                    }
                    else
                    {
                        cboUser.EditValue = null;
                    }
                    txtTitle.Text = data.TITLE;
                    if (ListDepartment.Exists(o => o.DEPARTMENT_CODE == data.DEPARTMENT_CODE && !string.IsNullOrEmpty(o.DEPARTMENT_CODE) && !string.IsNullOrEmpty(o.DEPARTMENT_NAME)))
                    {
                        cboDepartment.EditValue = data.DEPARTMENT_CODE;
                    }
                    else
                    {
                        cboDepartment.EditValue = null;
                    }
                    if (data.SIGN_IMAGE != null)
                    {
                        picSignImage.EditValue = byteArrayToImage(data.SIGN_IMAGE);
                    }
                    if (data.SIGNATURE_DISPLAY_TYPE != null)
                    {
                        cboThongTinKy.EditValue = data.SIGNATURE_DISPLAY_TYPE;
                    }
                    txtCmndNumber.Text = data.CMND_NUMBER ?? "";
                    txtEmail.Text = data.EMAIL ?? "";
                    txtPhone.Text = data.PHONE ?? "";
                    txtSCA_Serial.Text = data.SCA_SERIAL ?? "";
                    //data.DEPARTMENT_CODE.ToString("0.#####");
                    if (data.SIGNALTURE_IMAGE_WIDTH.HasValue)
                        txtKichThuoc.Text = data.SIGNALTURE_IMAGE_WIDTH.Value.ToString("0.###########");
                    
                    txtSecretKey.Text = data.SECRET_KEY;
                    txtPassword.Text = data.PASSWORD;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public byte[] imageToByteArray(Bitmap imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }
        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool rs = false;
            EMR_SIGNER success = new EMR_SIGNER();
            //bool notHandler = false;
            try
            {

                EMR_SIGNER data = (EMR_SIGNER)gridView1.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    EMR_SIGNER data1 = new EMR_SIGNER();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<EMR_SIGNER>(HIS.Desktop.Plugins.EmrSigner.HisRequestUriStore.EMR_SIGNER_UNLOCK, ApiConsumers.EmrConsumer, data, param);
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
            EMR_SIGNER success = new EMR_SIGNER();
            //bool notHandler = false;

            try
            {

                EMR_SIGNER data = (EMR_SIGNER)gridView1.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<EMR_SIGNER>(HIS.Desktop.Plugins.EmrSigner.HisRequestUriStore.EMR_SIGNER_LOCK, ApiConsumers.EmrConsumer, data, param);
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
                    var rowData = (EMR_SIGNER)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {

                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(HIS.Desktop.Plugins.EmrSigner.HisRequestUriStore.EMR_SIGNER_DELETE, ApiConsumers.EmrConsumer, rowData.ID, param);
                        if (success)
                        {
                            this.ActionType = 1;
                            cboUser.EditValue = null;
                            EnableControlChanged(this.ActionType);
                            FillDatagctFormList();
                            currentData = ((List<EMR_SIGNER>)gridControl1.DataSource).FirstOrDefault();
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

        private void txtMachineGroupCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnAdd.Focus();
            }
        }

        private void txtNumOrder_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtNumOrder_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtEmail.Focus();
                    txtEmail.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void picSignImage_DockChanged(object sender, EventArgs e)
        {
            try
            {
                if (picSignImage.Width < 14000)
                {

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void txtPcaSerial_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCmndNumber.Focus();
                    txtCmndNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void txtPcaSerial_KeyUp(object sender, KeyEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        txtCmndNumber.Focus();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Debug(ex);
            //}
        }

        private void txtCmndNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNumOrder.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void txtEmail_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSCA_Serial.Focus();
                    txtSCA_Serial.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSCA_Serial.Focus();
                    txtSCA_Serial.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnUploadImageFromPC_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Filter = "*.png|*.png|*.jpg|*.jpg|*.jpeg|*.jpeg|*.bmp|*.bmp|*.gif|*.gif|*.ico|*.ico|All file|*.*";
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bImage = ResizeSignImage(openFile.FileName);
                    picSignImage.Image = bImage;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #region Thay đổi kích thước ảnh
        //cách 1: nhập vào tỷ lệ phần trăm kích thước của ảnh mới so với ảnh ban đầu cần thay đổi. 
        //Để ảnh còn lại 70% thì ta cần nhập vào con số 0.7F
        // ví dụ:
        // var ifirst = Image.FromFile("abc.jpg");
        // var iresize = Resize(ifirst, 0.7F);
        // iresize.Save(ifirst);
        // Created by Dungnv2
        //public Image Resize(Image img, float percentage)
        //{
        //    //lấy kích thước ban đầu của bức ảnh
        //    int originalW = img.Width;
        //    int originalH = img.Height;

        //    //tính kích thước cho ảnh mới theo tỷ lệ đưa vào
        //    int resizedW = (int)(originalW * percentage);
        //    int resizedH = (int)(originalH * percentage);

        //    //tạo 1 ảnh Bitmap mới theo kích thước trên
        //    Bitmap bmp = new Bitmap(resizedW, resizedH);
        //    //tạo 1 graphic mới từ Bitmap
        //    Graphics graphic = Graphics.FromImage((Image)bmp);
        //    //vẽ lại ảnh ban đầu lên bmp theo kích thước mới
        //    graphic.DrawImage(img, 0, 0, resizedW, resizedH);
        //    //giải phóng tài nguyên mà graphic đang giữ
        //    graphic.Dispose();
        //    //return the image
        //    return (Image)bmp;
        //}

        //cách 2: nhập vào kích thước của ảnh
        //ví dụ:
        //var reSizeImage = resizeImage(picSignImage.Image, new Size(200, 74));
        //picSignImage.Image = reSizeImage;
        private Image resizeImage(Image imgToResize, Size size)
        {
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => imgToResize.Size.Width), imgToResize.Size.Width)
                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => imgToResize.Size.Height), imgToResize.Size.Height));
            //if (imgToResize.Size.Width > 600 || imgToResize.Size.Height > 200)
            //{
            int destWidth = size.Width;
            int destHeight = size.Height;
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            b.MakeTransparent(Color.Black);
            imgToResize = b;

            //}

            return (Image)imgToResize;
        }

        private Bitmap ResizeSignImage(string imageFile = "")
        {
            Size size = new Size();
            if (picSignImage.Image != null)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => picSignImage.Image.Size.Width), picSignImage.Image.Size.Width)
                     + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => picSignImage.Image.Size.Height), picSignImage.Image.Size.Height)
                     + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => imageFile), imageFile));

                size = picSignImage.Image.Size;
                if (picSignImage.Image.Size.Width > 600 || picSignImage.Image.Size.Height > 600)
                {
                    int heightD = (int)((double)((double)600 / (double)(picSignImage.Image.Size.Width)) * (double)(picSignImage.Image.Size.Height));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => heightD), heightD));
                    size = new Size(600, heightD);
                    Inventec.Common.Logging.LogSystem.Debug("Anh chu ky qua lon sẽ bi resize lai ve kich thuoc (" + size.Width + ", " + size.Height + ")" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => size), size));
                }
            }

            Bitmap b1 = !String.IsNullOrEmpty(imageFile) ? (Bitmap)Image.FromFile(imageFile) : (Bitmap)picSignImage.Image.Clone();
            if (!String.IsNullOrEmpty(imageFile) && b1 != null)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => imageFile), imageFile));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("b1.Size", b1.Size));
                int heightD = b1.Size.Height;
                int wightD = b1.Size.Width;
                if (b1.Size.Width > 600 || b1.Size.Height > 600)
                {
                    wightD = 600;
                    heightD = (int)((double)((double)600 / (double)(b1.Size.Width)) * (double)(b1.Size.Height));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => heightD), heightD));
                }
                size = new Size(wightD, heightD);
                Inventec.Common.Logging.LogSystem.Debug("Anh chu ky qua lon sẽ bi resize lai ve kich thuoc (" + size.Width + ", " + size.Height + ")" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => size), size));
            }

            int destWidth = size.Width;
            int destHeight = size.Height;
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(b1, 0, 0, destWidth, destHeight);
            g.Dispose();
            b.MakeTransparent();

            //}

            return b;
        }
        #endregion

        System.Windows.Forms.Timer timerCheckFileSign;
        private void btnUploadImageUsingSigDevice_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsProcessOpen("Inventec.SignPadManager"))
                {
                    string pathSaveFolder = Path.Combine(Path.Combine(Application.StartupPath, "temp"), DateTime.Now.ToString("ddMMyyyy"), "STPadLibFile");
                    if (!Directory.Exists(pathSaveFolder))
                    {
                        Directory.CreateDirectory(pathSaveFolder);
                    }
                    DirectoryInfo dicInfo = new DirectoryInfo(pathSaveFolder);

                    string[] fileImage = Directory.GetFiles(dicInfo.FullName, "*");
                    if (fileImage != null && fileImage.Length > 0)
                    {
                        try
                        {
                            dicInfo.Delete(true);
                        }
                        catch (Exception exx)
                        {
                            LogSystem.Error(exx);
                        }
                    }

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = Application.StartupPath + @"\Inventec.SignPadManager.exe";
                    Process.Start(startInfo);

                    while (true)
                    {
                        if (IsProcessOpen("Inventec.SignPadManager"))
                        {
                            //Nothing...
                            Inventec.Common.Logging.LogSystem.Info("btnUploadImageUsingSigDevice_Click.1");
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Info("btnUploadImageUsingSigDevice_Click.2");
                            fileImage = Directory.GetFiles(dicInfo.FullName, "*");
                            if (fileImage != null && fileImage.Length > 0)
                            {
                                //TODO
                                picSignImage.Image = Image.FromFile(fileImage[0]);
                                Inventec.Common.Logging.LogSystem.Info("btnUploadImageUsingSigDevice_Click.3");
                                break;
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Info("btnUploadImageUsingSigDevice_Click.4");
                                break;
                            }
                        }
                    }
                }

                //Code cũ
                //Inventec.SignViewLib.SignViewInputADO signViewInputADO = new Inventec.SignViewLib.SignViewInputADO();
                //signViewInputADO.ActGetSignImageFile = GetSignImageFIle;
                //signViewInputADO.ActSelectDevice = ActSelectDevice;
                //signViewInputADO.DriverName = this.deviceName;
                //Inventec.SignViewLib.MainWindow MainWindow = new Inventec.SignViewLib.MainWindow(signViewInputADO);
                //MainWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void CheckImageFile(object sender, EventArgs e)
        {
            try
            {
                string pathSaveFolder = Path.Combine(Path.Combine(Application.StartupPath, "temp"), DateTime.Now.ToString("ddMMyyyy"), "STPadLibFile");
                DirectoryInfo dicInfo = new DirectoryInfo(pathSaveFolder);

                string[] fileImage = Directory.GetFiles(dicInfo.FullName, "*");
                if (fileImage != null && fileImage.Length > 0)
                {
                    //TODO
                    picSignImage.Image = Image.FromFile(fileImage[0]);
                    timerCheckFileSign.Enabled = false;
                    timerCheckFileSign.Stop();
                    //try
                    //{
                    //    dicInfo.Delete(true);
                    //}
                    //catch (Exception exx)
                    //{
                    //    LogSystem.Error(exx);
                    //}                    
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        bool IsProcessOpen(string name)
        {
            try
            {
                foreach (Process clsProcess in Process.GetProcesses())
                {
                    if (clsProcess.ProcessName == name || clsProcess.ProcessName == String.Format("{0}.exe", name) || clsProcess.ProcessName == String.Format("{0} (32 bit)", name) || clsProcess.ProcessName == String.Format("{0}.exe (32 bit)", name))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                LogSystem.Debug(String.Format("Xảy ra lỗi khi kiểm tra ứng dụng {0}.", name), ex);
            }

            return false;
        }

        private void ActSelectDevice(string deviceName)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.DeviceName && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = deviceName;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.DeviceName;
                    csAddOrUpdate.VALUE = deviceName;
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                this.deviceName = deviceName;
                Inventec.Common.Logging.LogSystem.Debug("ActSelectDevice____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => deviceName), deviceName)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void GetSignImageFIle(Bitmap bmpSignImage)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("GetSignImageFIle__");
                if (bmpSignImage != null)
                {
                    picSignImage.Image = bmpSignImage;
                }
                //Inventec.Common.Logging.LogSystem.Debug("GetSignImageFIle__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileImage), fileImage));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtPhone_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    cboThongTinKy.Focus();
                    cboThongTinKy.ShowPopup();
                }
                //if (e.KeyCode == Keys.Enter)
                //{
                //    if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                //    {
                //        btnAdd.Focus();
                //    }
                //    else if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                //    {
                //        btnEdit.Focus();
                //    }
                //}
                // e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtSCA_Serial_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPhone.Focus();
                    txtPhone.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (EMR_SIGNER)gridView1.GetFocusedRow();
                if (this.currentData != null)
                {

                    ChangedDataRow(this.currentData);
                    //SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSignerCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPcaSerial.Focus();
                    txtPcaSerial.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboUser_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtTitle.Focus();
            }
        }

        private void txtTitle_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cboDepartment.Focus();
                cboDepartment.ShowPopup();
            }
        }

        private void cboDepartment_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtSignerCode.Focus();
                txtSignerCode.SelectAll();
            }
        }

        private void LoadDatacboThongTinKy()
        {
            try
            {
                List<FilterAnhKy> listFilterType = new List<FilterAnhKy>();
                listFilterType.Add(new FilterAnhKy(0, "Không hiển thị chữ ký"));
                listFilterType.Add(new FilterAnhKy(1, "Chỉ hiển thị thông tin ký"));
                listFilterType.Add(new FilterAnhKy(2, "Chỉ hiển thị ảnh chữ ký"));
                listFilterType.Add(new FilterAnhKy(3, "Hiển thị cả ảnh chữ ký & thông tin ký"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("ID", "", 30, 1));
                columnInfos.Add(new ColumnInfo("FilterTypeName", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("FilterTypeName", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboThongTinKy, listFilterType.ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboThongTinKy_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    txtKichThuoc.Focus();
                    //if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    //{
                    //    btnAdd.Focus();
                    //}
                    //else if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                    //{
                    //    btnEdit.Focus();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboThongTinKy_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {

                txtKichThuoc.SelectAll();
                txtKichThuoc.Focus();
                //if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                //{
                //    btnAdd.Focus();
                //}
                //else if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                //{
                //    btnEdit.Focus();
                //}

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboThongTinKy_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboThongTinKy.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtKichThuoc_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    else if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                    {
                        btnEdit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKichThuoc_KeyPress(object sender, KeyPressEventArgs e)
        {

            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnUploadImageUsingSigCertificate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtPcaSerial.Text))
                {
                    Inventec.Sign.EasySignca.SignProcessor processor = new Inventec.Sign.EasySignca.SignProcessor();
                    if (processor.GetImage(txtPcaSerial.Text).Base64Image != null)
                    {
                        picSignImage.Image = Base64ToImage(processor.GetImage(txtPcaSerial.Text).Base64Image);
                    }
                    else
                        MessageBox.Show("Không có thông tin ảnh chữ ký vui lòng sử dụng chức năng tải ảnh khác", "Thông báo");
                }
                else
                {
                    MessageBox.Show("Vui lòng điền serial chứng thư số để tải ảnh", "Thông báo");
                }

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        public Image Base64ToImage(string base64String)
        {

            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image im = System.Drawing.Image.FromStream(ms, true);
            return im;
        }

        private void txtSecretKey_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSecretKey.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }
       
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyData == Keys.C ||
                e.Control && e.KeyData == Keys.X)
            {
                
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void txtPassword_Properties_BeforeShowMenu(object sender, BeforeShowMenuEventArgs e)
        {
            foreach (DXMenuItem item in e.Menu.Items)
            {
                if (item.Tag != null && (item.Tag.Equals(DevExpress.XtraEditors.Controls.StringId.TextEditMenuCopy)
                    || item.Tag.Equals(DevExpress.XtraEditors.Controls.StringId.TextEditMenuPaste)))
                {
                    item.Visible = false;
                }
            }  
        }

        private void btnEyePassword_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                txtPassword.Properties.PasswordChar = new char();
                txtPassword.Properties.UseSystemPasswordChar = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void btnEyePassword_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                txtPassword.Properties.PasswordChar = '*';
                txtPassword.Properties.UseSystemPasswordChar = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

       
    }
}

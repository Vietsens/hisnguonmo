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
using HIS.Desktop.Utility;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using Inventec.Common.Logging;
using SDA.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using SDA.EFMODEL.DataModels;
using DevExpress.Data;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
//using DevExpress.XtraTreeList.Data;

namespace HIS.Desktop.Plugins.SdaTranslate.SdaTranslateForm
{
    public partial class SdaTranslateForm : FormBase
    {

        #region declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int ActionType = -1;
        SDA.EFMODEL.DataModels.SDA_TRANSLATE currentData;
        SDA.EFMODEL.DataModels.SDA_TRANSLATE resultData;
        DelegateSelectData delegateSelect = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        int positionHandle = -1;
        List<SDA_LANGUAGE> listSdaLanguage;
        List<SDA_LANGUAGE> listSdaLanguageNotBlock;
        List<SDA_LANGUAGE> listSdaLanguageClick;
        #endregion

        #region construct
        public SdaTranslateForm(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            try
            {
                InitializeComponent();
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

        #region Public method

        public void MeShow()
        {

            try
            {
                GetLanguage();
                SetDefaultValue();
                EnableControlChanged(this.ActionType);
                LoadLanguage();
                LoadLanguageNotBlock();
                FillDatagctFormList();
                SetCaptionByLanguageKey();
                InitTabIndex();
                ValidateForm();
                SetDefaultFocus();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region private Method

        private void SdaTranslateForm_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }

        }

        void RefeshDataAfterSave()
        {
            try
            {
                if (this.delegateSelect != null)
                {
                    this.delegateSelect(resultData);
                }

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

                textEdit1.Text = "";


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
                ValidationSingleControl(txtSchema);
                ValidationSingleControl(txtTableName);
                ValidationSingleControl(txtColumnName1);
                ValidationSingleControl(txtCode1);
                ValidationSingleControl(cboLanguage);
                ValidationSingleControl(txtValue);
                ValidationSingleControl(textEdit2);

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
                dicOrderTabIndexControl.Add("txtSchema", 0);
                dicOrderTabIndexControl.Add("txtCode1", 2);
                dicOrderTabIndexControl.Add("txtCode2", 4);
                dicOrderTabIndexControl.Add("txtTableName", 5);
                dicOrderTabIndexControl.Add("txtValue", 7);
                dicOrderTabIndexControl.Add("lueLanguage", 6);
                dicOrderTabIndexControl.Add("txtColumnName2", 3);
                dicOrderTabIndexControl.Add("txtColumnName1", 1);


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

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SdaTranslate.Resources.Lang", typeof(HIS.Desktop.Plugins.SdaTranslate.SdaTranslateForm.SdaTranslateForm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("SdaTranslateForm.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("SdaTranslateForm.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.textEdit1.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("SdaTranslateForm.textEdit1.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("SdaTranslateForm.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboLanguage.Properties.NullText = Inventec.Common.Resource.Get.Value("SdaTranslateForm.cboLanguage.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("SdaTranslateForm.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("SdaTranslateForm.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("SdaTranslateForm.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("SdaTranslateForm.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("SdaTranslateForm.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("SdaTranslateForm.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("SdaTranslateForm.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("SdaTranslateForm.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("SdaTranslateForm.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("SdaTranslateForm.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("SdaTranslateForm.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("SdaTranslateForm.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcoSTT.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gcoSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclSchema.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclSchema.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclTableName.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclTableName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclColumnName01.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclColumnName01.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclCode1.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclCode1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclColumnName02.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclColumnName02.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclCode2.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclCode2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclLanguage2.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclLanguage2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclUpdatecolumn.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclUpdatecolumn.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclValue1.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclValue1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclSTT.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclColumnName.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclColumnName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclColumnName1.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclColumnName1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclDatacode1.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclDatacode1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclColumnName2.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclColumnName2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclDataCode2.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclDataCode2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclLanguage.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclLanguage.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclValue.Caption = Inventec.Common.Resource.Get.Value("SdaTranslateForm.gclValue.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
               
                //this.Text = Inventec.Common.Resource.Get.Value("SdaTranslateForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        private void FillDatagctFormList()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, numPageSize);
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

            startPage = ((CommonParam)param).Start ?? 0;
            int limit = ((CommonParam)param).Limit ?? 0;
            CommonParam paramCommon = new CommonParam(startPage, limit);
            Inventec.Core.ApiResultObject<List<SDA.EFMODEL.DataModels.SDA_TRANSLATE>> apiResult = null;
            SdaTranslateFilter filter = new SdaTranslateFilter();
            SetFilterNavBar(ref filter);
            filter.ORDER_DIRECTION = "DESC";
            filter.ORDER_FIELD = "MODIFY_TIME";

            gridView1.BeginUpdate();
            apiResult = new BackendAdapter(paramCommon).GetRO<List<SDA.EFMODEL.DataModels.SDA_TRANSLATE>>(SdaRequestUriStore.SDA_TRANSLATE_GET, ApiConsumers.SdaConsumer, filter, paramCommon);
            if (apiResult != null)
            {
                var data = (List<SDA.EFMODEL.DataModels.SDA_TRANSLATE>)apiResult.Data;
                if (data != null)
                {
                    gridView1.GridControl.DataSource = data;
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
            }
            gridView1.EndUpdate();


            SessionManager.ProcessTokenLost(paramCommon);
        }

        private void SetFilterNavBar(ref SdaTranslateFilter filter)
        {

            try
            {
                filter.KEY_WORD = textEdit1.Text.Trim();
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

        private void ChangedDataRow(SDA.EFMODEL.DataModels.SDA_TRANSLATE data)
        {
            try
            {
                if (data != null)
                {
                    GetLanguage();
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

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

        private void FillDataToEditorControl(SDA.EFMODEL.DataModels.SDA_TRANSLATE data)
        {
            try
            {
                if (data != null)
                {

                    txtSchema.Text = data.SCHEMA;
                    txtTableName.Text = data.TABLE_NAME;
                    txtColumnName1.Text = data.FIND_COLUMN_NAME_ONE;
                    txtCode1.Text = data.FIND_DATA_CODE_ONE;
                    txtColumnName2.Text = data.FIND_COLUMN_NAME_TWO;
                    txtCode2.Text = data.FIND_DATA_CODE_TWO;
                    txtValue.Text = data.UPDATE_COLUMN_NAME;
                    cboLanguage.EditValue = data.LANGUAGE_ID;
                    txtValue.Text = data.UPDATE_DATA_VALUE;
                    textEdit2.Text = data.UPDATE_COLUMN_NAME;

                    var listLanguage = listSdaLanguage.Where(o => o.ID == data.LANGUAGE_ID || o.IS_ACTIVE == 1).ToList();
                    InitComboLanguage(listLanguage);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetLanguage()
        {
            try
            {
                CommonParam param = new CommonParam();
                SdaLanguageFilter filter = new SdaLanguageFilter();
                listSdaLanguageClick = new BackendAdapter(param).Get<List<SDA_LANGUAGE>>("api/SdaLanguage/Get", ApiConsumers.SdaConsumer, filter, null).ToList();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadLanguageNotBlock()
        {
            try
            {
                if (listSdaLanguage == null || listSdaLanguage.Count == 0)
                { 
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay ngon ngu nao");
                    return;
                }

                var listLanguageNotBlock = listSdaLanguage.Where(o => o.IS_ACTIVE == 1).ToList();
                InitComboLanguage(listLanguageNotBlock);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboLanguage(object data)
        {
            try
            {
                
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LANGUAGE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("LANGUAGE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("LANGUAGE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboLanguage, data, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadLanguage()
        {
            try
            {
                CommonParam param = new CommonParam();
                SdaLanguageFilter filter = new SdaLanguageFilter();
                listSdaLanguage = new BackendAdapter(param).Get<List<SDA_LANGUAGE>>(SdaRequestUriStore.SDA_LANGUAGE_GET, ApiConsumers.SdaConsumer, filter, null).ToList();
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

                textEdit1.Text = "";
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
                SDA.EFMODEL.DataModels.SDA_TRANSLATE updateDTO = new SDA.EFMODEL.DataModels.SDA_TRANSLATE();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<SDA.EFMODEL.DataModels.SDA_TRANSLATE>(SdaRequestUriStore.SDA_TRANSLATE_CREATE, ApiConsumers.SdaConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        //if (resultData.IS_BASE == 1)
                        //{
                        //    SDA.EFMODEL.DataModels.SDA_LANGUAGE updateDTO2 = new SDA.EFMODEL.DataModels.SDA_LANGUAGE();
                        //    LoadCurrent2(ref updateDTO2);
                        //    var resultData2 = new BackendAdapter(param).Post<SDA.EFMODEL.DataModels.SDA_LANGUAGE>(SdaRequestUriStore.SDA_LANGUAGE_CREATE, ApiConsumers.SdaConsumer, updateDTO2, param);

                        //}
                        //if (byte.Parse(resultData.LANGUAGE_CODE)>2)
                        //{
                        //    MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBDuLieuNhapVaoKhongHopLe));
                        //}
                        success = true;
                        FillDatagctFormList();

                        ResetFormData();

                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<SDA.EFMODEL.DataModels.SDA_TRANSLATE>(SdaRequestUriStore.SDA_TRANSLATE_UPDATE, ApiConsumers.SdaConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDatagctFormList();

                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<SDA_TRANSLATE>();
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

        private void ResetFormData()
        {
            try
            {
                txtCode1.Text = "";
                txtCode2.Text = "";
                txtColumnName1.Text = "";
                txtColumnName2.Text = "";
                txtSchema.Text = "";
                txtTableName.Text = "";
                txtValue.Text = "";
                textEdit2.Text = "";
                textEdit1.Text = "";
                cboLanguage.EditValue = null;
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
                            txtSchema.Focus();

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

        private void UpdateDTOFromDataForm(ref SDA.EFMODEL.DataModels.SDA_TRANSLATE currentDTO)
        {
            try
            {
                currentDTO.FIND_COLUMN_NAME_ONE = txtColumnName1.Text.Trim();
                currentDTO.FIND_COLUMN_NAME_TWO = txtColumnName2.Text.Trim();
                currentDTO.FIND_DATA_CODE_ONE = txtCode1.Text.Trim();
                currentDTO.FIND_DATA_CODE_TWO = txtCode2.Text.Trim();
                currentDTO.SCHEMA = txtSchema.Text.Trim();
                currentDTO.TABLE_NAME = txtTableName.Text.Trim();
                currentDTO.UPDATE_DATA_VALUE = txtValue.Text.Trim();
                currentDTO.UPDATE_COLUMN_NAME = textEdit2.Text.Trim();
                currentDTO.LANGUAGE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboLanguage.EditValue.ToString() ?? "0");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref SDA.EFMODEL.DataModels.SDA_TRANSLATE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                SdaTranslateFilter filter = new SdaTranslateFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<SDA.EFMODEL.DataModels.SDA_TRANSLATE>>(SdaRequestUriStore.SDA_TRANSLATE_GET, ApiConsumers.SdaConsumer, filter, param).FirstOrDefault();
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
                txtSchema.Focus();
                txtSchema.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        #endregion

        #region event
        #region evntGridview
        private void gridView1_CustomUnboundColumnData_1(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SDA.EFMODEL.DataModels.SDA_TRANSLATE pData = (SDA_TRANSLATE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "LANGUAGE_NAME_STR")
                    {
                        try
                        {
                            e.Value = listSdaLanguageClick.FirstOrDefault(o => o.ID == pData.LANGUAGE_ID).LANGUAGE_NAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomRowCellEdit_1(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                SDA_TRANSLATE data = null;
                if (e.RowHandle > -1)
                {
                    data = (SDA_TRANSLATE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__FALSE ? btnLock : btnUnLock);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE ? btnDeleteEnable : btnDeleteDisable);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region envetClick
        private void gridControl1_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (SDA.EFMODEL.DataModels.SDA_TRANSLATE)gridView1.GetFocusedRow();
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnAdd.Enabled)
                    return;
               
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
                if (!btnEdit.Enabled)
                    return;

                SaveProcess();


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
                EnableControlChanged(this.ActionType);
                LoadLanguageNotBlock();
                positionHandle = -1;

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

        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool rs = false;
            SDA_TRANSLATE success = new SDA_TRANSLATE();
            //bool notHandler = false;
            try
            {

                SDA_TRANSLATE data = (SDA_TRANSLATE)gridView1.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SDA_TRANSLATE data1 = new SDA_TRANSLATE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<SDA_TRANSLATE>(SdaRequestUriStore.SDA_TRANSLATE_GROUP_CHANGE_LOCK, ApiConsumers.SdaConsumer, data, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<SDA_TRANSLATE>();
                        rs = true;
                        FillDatagctFormList();
                    }
                    btnReset_Click(null, null);
                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, rs);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool rs = false;
            SDA.EFMODEL.DataModels.SDA_TRANSLATE success = new SDA.EFMODEL.DataModels.SDA_TRANSLATE();
            try
            {

                SDA.EFMODEL.DataModels.SDA_TRANSLATE data = (SDA_TRANSLATE)gridView1.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<SDA_TRANSLATE>(SdaRequestUriStore.SDA_TRANSLATE_GROUP_CHANGE_LOCK, ApiConsumers.SdaConsumer, data, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<SDA_TRANSLATE>();
                        rs = true;
                        FillDatagctFormList();
                    }
                    btnReset_Click(null, null);
                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, rs);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDeleteEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (SDA.EFMODEL.DataModels.SDA_TRANSLATE)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {

                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(SdaRequestUriStore.SDA_TRANSLATE_DELETE, ApiConsumers.SdaConsumer, rowData.ID, param);
                        if (success)
                        {
                            this.ActionType = 1;
                            ResetFormData();
                            BackendDataWorker.Reset<SDA_TRANSLATE>();
                           EnableControlChanged(this.ActionType);
                            FillDatagctFormList();
                            currentData = ((List<SDA_TRANSLATE>)gridControl1.DataSource).FirstOrDefault();

                        }
                        MessageManager.Show(this, param, success);
                        btnReset_Click(null,null);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDeleteDisable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }
        #endregion

        #region envetKeyup
        private void textEdit1_KeyUp(object sender, KeyEventArgs e)
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
                    var rowData = (SDA_TRANSLATE)gridView1.GetFocusedRow();
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

        private void txtSchema_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTableName.Focus();
                    txtTableName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTableName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtColumnName1.Focus();
                    txtColumnName1.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtColumnName1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCode1.Focus();
                    txtCode1.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtCode1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtColumnName2.Focus();
                    txtColumnName2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtColumnName2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCode2.Focus();
                    txtCode2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCode2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboLanguage.Focus();
                    cboLanguage.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLanguage_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    textEdit2.Focus();
                    textEdit2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtValue_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    else
                        btnEdit.Focus();
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

                    var rowData = (SDA_TRANSLATE)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
                if (e.KeyCode == Keys.Up)
                {

                    var rowData = (SDA_TRANSLATE)gridView1.GetFocusedRow();
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

        private void textEdit2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtValue.Focus();
                    txtValue.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region shortcut

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnFind_Click(null, null);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnEdit_Click(null, null);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnAdd_Click(null, null);

        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnReset_Click(null, null);
        }

        #endregion
        #endregion

    }
}

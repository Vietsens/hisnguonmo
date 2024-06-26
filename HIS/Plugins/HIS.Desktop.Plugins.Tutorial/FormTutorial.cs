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
using DevExpress.Spreadsheet;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.FlexCellExport;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using Inventec.Fss.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Tutorial
{
    public partial class FormTutorial : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private System.Globalization.CultureInfo cultureLang;
        private int rowCount = 0;
        private int dataTotal = 0;
        private int startPage = 0;
        private int ActionType = -1;
        private int positionHandle = -1;

        private SDA.EFMODEL.DataModels.SDA_TUTORIAL currentData;
        private List<ACS.EFMODEL.DataModels.V_ACS_MODULE> listModule;
        private string currentUrl;
        private string currentFileName;
        private MemoryStream resultStream;
        private const string REPORT_EXTENSION = ".pdf";
        #endregion

        #region Ctor
        public FormTutorial()
        {
            InitializeComponent();
        }

        public FormTutorial(Inventec.Desktop.Common.Modules.Module moduleData)
            :base(moduleData)
        {
            try
            {
				InitializeComponent();
                this.moduleData = moduleData;
                this.Text = moduleData.text;
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region private method
        private void FormTutorial_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                SetCaptionByLanguageKey();

                LoadDataModule();

                SetDefaultValue();

                InitComboModule();

                LoadDataToGrid();

                ValidateForm();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(CboModule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboModule()
        {
            try
            {
                CboModule.Properties.DataSource = listModule;
                CboModule.Properties.DisplayMember = "MODULE_NAME";
                CboModule.Properties.ValueMember = "MODULE_LINK";
                CboModule.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                CboModule.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                CboModule.Properties.ImmediatePopup = true;
                CboModule.ForceInitialize();
                CboModule.Properties.View.Columns.Clear();
                CboModule.Properties.PopupFormSize = new System.Drawing.Size(300, 250);

                GridColumn aColumnName = CboModule.Properties.View.Columns.AddField("MODULE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataModule()
        {
            try
            {
                var param = new CommonParam();
                ACS.Filter.AcsModuleViewFilter filter = new ACS.Filter.AcsModuleViewFilter();
                filter.APPLICATION_CODE = GlobalVariables.APPLICATION_CODE;
                var apiResult = new BackendAdapter(param).Get<List<ACS.EFMODEL.DataModels.V_ACS_MODULE>>("api/AcsModule/GetView", ApiConsumer.ApiConsumers.AcsConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    listModule = apiResult;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGrid()
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPaging(object commonParam)
        {
            try
            {
                startPage = ((CommonParam)commonParam).Start ?? 0;
                int limit = ((CommonParam)commonParam).Limit ?? 0;

                CommonParam param = new CommonParam(startPage, limit);

                Inventec.Core.ApiResultObject<List<SDA.EFMODEL.DataModels.SDA_TUTORIAL>> apiResult = null;

                SDA.Filter.SdaTutorialFilter filter = new SDA.Filter.SdaTutorialFilter();

                SetFilterNavbar(ref filter);
                gridViewTutorial.BeginUpdate();

                apiResult = new BackendAdapter(param).GetRO<List<SDA.EFMODEL.DataModels.SDA_TUTORIAL>>("api/SdaTutorial/Get", ApiConsumer.ApiConsumers.SdaConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (apiResult != null)
                {
                    var data = (List<SDA.EFMODEL.DataModels.SDA_TUTORIAL>)apiResult.Data;
                    if (data != null)
                    {
                        gridControlTutorial.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }

                gridViewTutorial.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilterNavbar(ref SDA.Filter.SdaTutorialFilter filter)
        {
            try
            {
                filter.KEY_WORD = this.txtKeyWord.Text.Trim();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChange(this.ActionType);
                ResetFormData();
                this.txtKeyWord.ResetText();
                this.CboModule.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.BtnAdd.Text = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__BTN_NEW");
                this.BtnChooseFile.ToolTip = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__BTN__CHOOSE_FILE");
                this.BtnEdit.Text = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__BTN_UPDATE");
                this.BtnRefresh.Text = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__BTN_REFRESH");
                this.CREATE_TIME.Caption = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__GC_CREATE_TIME");
                this.CREATOR.Caption = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__GC_CREATOR");
                this.gridColumnCode.Caption = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__GC_MODULE_NAME");
                this.gridColumnName.Caption = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__GC_MODULE_LINK");
                this.gridColumnUrl.Caption = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__GC_URL");
                this.lciModuleName.Text = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__LCI_MODULE_NAME");
                this.lciUrl.Text = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__LCI_URL");
                this.MODIFIER.Caption = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__GC_MODIFIER");
                this.MODIFY_TIME.Caption = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__GC_MODIFY_TIME");
                this.STT.Caption = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__GC_STT");
                this.repositoryItemButtonDelete.Buttons[0].ToolTip = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__BTN_DELETE");
                this.repositoryItemButtonDeleteDisable.Buttons[0].ToolTip = this.repositoryItemButtonDelete.Buttons[0].ToolTip;
                this.repositoryItemButtonLock.Buttons[0].ToolTip = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__BTN_LOCK");
                this.repositoryItemButtonUnLock.Buttons[0].ToolTip = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__BTL_UNLOCK");
                this.simpleButton1.Text = setKey("IVT_LANGUAGE_KEY__FORM_TUTORIAL__BTN_SEARCH");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string setKey(string key)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Resource.Get.Value(
                    key, Resources.ResourceLanguageManager.LanguageFormTutorial, cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void EnableControlChange(int action)
        {
            try
            {
                BtnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                BtnAdd.Enabled = (action == GlobalVariables.ActionAdd);
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
                validRule.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderInfo_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void gridViewTutorial_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var pData = (SDA.EFMODEL.DataModels.SDA_TUTORIAL)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)pData.CREATE_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)pData.MODIFY_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else if (e.Column.FieldName == "URL_STR")
                    {
                        try
                        {
                            var url = pData.URL.Split('\\');
                            var fileName = (url != null && url.Count() > 0) ? url.LastOrDefault() : "";
                            e.Value = !String.IsNullOrEmpty(fileName) ? fileName.Split('.').FirstOrDefault() : "";
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTutorial_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (SDA.EFMODEL.DataModels.SDA_TUTORIAL)gridViewTutorial.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;

                    if (rowData.IS_ACTIVE == 0)
                    {
                        BtnEdit.Enabled = false;
                        BtnAdd.Enabled = false;
                    }
                    else
                    {
                        ChangeDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangeDataRow(SDA.EFMODEL.DataModels.SDA_TUTORIAL rowData)
        {
            try
            {
                if (rowData != null)
                {
                    FillDataToEditorControl(rowData);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChange(this.ActionType);

                    dxValidationProviderInfo.RemoveControlError(CboModule);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToEditorControl(SDA.EFMODEL.DataModels.SDA_TUTORIAL rowData)
        {
            try
            {
                if (rowData != null)
                {
                    this.CboModule.EditValue = rowData.MODULE_LINK;
                    if (!String.IsNullOrEmpty(rowData.URL))
                    {
                        var filename = rowData.URL.Split('\\').LastOrDefault();
                        this.txtUrl.Text = !String.IsNullOrEmpty(filename) ? filename.Split('.').FirstOrDefault() : "";
                    }
                    else
                        this.txtUrl.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTutorial_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    long is_active = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewTutorial.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                    if (e.Column.FieldName == "isLock")
                    {
                        e.RepositoryItem = (is_active == 0 ? repositoryItemButtonUnLock : repositoryItemButtonLock);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (is_active == 1 ? repositoryItemButtonDelete : repositoryItemButtonDeleteDisable);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                dxValidationProviderInfo.RemoveControlError(CboModule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveProcess()
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                if (!BtnEdit.Enabled && !BtnAdd.Enabled)
                    return;

                if (!dxValidationProviderInfo.Validate())
                    return;

                SDA.EFMODEL.DataModels.SDA_TUTORIAL updateDTO = new SDA.EFMODEL.DataModels.SDA_TUTORIAL();

                if (ProcessUrl())
                {
                    return;
                }

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrentData(this.currentData.ID, ref updateDTO);
                }
                else
                {
                    UpdateDTOFromDataForm(ref updateDTO);
                }

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<SDA.EFMODEL.DataModels.SDA_TUTORIAL>("api/SdaTutorial/Create", ApiConsumer.ApiConsumers.SdaConsumer, updateDTO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    if (resultData != null)
                    {
                        success = true;

                        LoadDataToGrid();
                        ResetFormData();
                    }
                }
                else
                {
                    //Hàm Update
                    var resultData = new BackendAdapter(param).Post<SDA.EFMODEL.DataModels.SDA_TUTORIAL>("api/SdaTutorial/Update", ApiConsumer.ApiConsumers.SdaConsumer, updateDTO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    if (resultData != null)
                    {
                        success = true;

                        //Nếu cần Update tại vị trí đang Focus
                        LoadDataToGrid();
                    }
                }

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessUrl()
        {
            var result = true;
            try
            {
                if (this.resultStream == null)
                {
                    this.currentUrl = "";
                    MessageManager.Show(this, new Inventec.Core.CommonParam(), false);
                }
                else
                {
                    var fileUploadInfo = new Inventec.Fss.Utility.FileUploadInfo();

                    if (this.resultStream != null)
                    {
                        fileUploadInfo = FileUpload.UploadFile("SDA", "TUTORIAL", this.resultStream, currentFileName + REPORT_EXTENSION, true);
                    }

                    if (fileUploadInfo == null)
                    {
                        if (ActionType == GlobalVariables.ActionAdd)
                        {
                            this.currentUrl = "";
                            MessageBox.Show(Resources.ResourceMessage.LoiUploadFile);
                            throw new Exception("loi upload file");
                        }
                        else
                        {
                            this.currentUrl = this.currentData != null ? this.currentData.URL : "";
                        }
                    }
                    else
                    {
                        this.currentUrl = fileUploadInfo.Url;
                    }
                }

                result = String.IsNullOrEmpty(this.currentUrl);
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ResetFormData()
        {
            try
            {
                currentData = null;
                resultStream = null;
                currentFileName = "";
                currentUrl = "";
                this.CboModule.EditValue = null;
                this.txtUrl.Text = "";
                this.CboModule.Focus();
                this.CboModule.Properties.Buttons[1].Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref SDA.EFMODEL.DataModels.SDA_TUTORIAL updateDTO)
        {
            try
            {
                var module = listModule.FirstOrDefault(o => o.MODULE_LINK == (string)(this.CboModule.EditValue ?? ""));
                if (module != null)
                {
                    updateDTO.APP_CODE = GlobalVariables.APPLICATION_CODE;
                    updateDTO.MODULE_LINK = module.MODULE_LINK;
                    updateDTO.MODULE_NAME = module.MODULE_NAME;
                    updateDTO.URL = currentUrl;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCurrentData(long p, ref SDA.EFMODEL.DataModels.SDA_TUTORIAL updateDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                SDA.Filter.SdaTutorialFilter filter = new SDA.Filter.SdaTutorialFilter();
                filter.ID = p;

                updateDTO = new BackendAdapter(param).Get<List<SDA.EFMODEL.DataModels.SDA_TUTORIAL>>("api/SdaTutorial/Get",
                    ApiConsumer.ApiConsumers.SdaConsumer, filter, param).FirstOrDefault();

                //Gán lại giá trị cần thay đổi cho đối tượng Update để tiến hành Update
                var module = listModule.FirstOrDefault(o => o.MODULE_LINK == (string)(this.CboModule.EditValue ?? ""));
                if (module != null)
                {
                    updateDTO.MODULE_LINK = module.MODULE_LINK;
                    updateDTO.MODULE_NAME = module.MODULE_NAME;
                    updateDTO.URL = currentUrl;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                simpleButton1_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnChooseFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.AddExtension = true;
                ofd.CheckFileExists = true;
                ofd.Filter = "Tutorial Files (*.pdf)|*.pdf";
                ofd.Multiselect = false;
                ofd.RestoreDirectory = true;
                ofd.Title = "Select a Tutorial file";
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                string taptin = ofd.FileName;
                if (!string.IsNullOrEmpty(taptin))
                {
                    var name = taptin.Split('\\').LastOrDefault();
                    if (!String.IsNullOrEmpty(name))
                        currentFileName = name.Split('.').FirstOrDefault();
                    else
                        currentFileName = "";

                    txtUrl.Text = !String.IsNullOrEmpty(currentFileName) ? currentFileName : name;

                    //doc file de gui len
                    if (!String.IsNullOrEmpty(currentFileName))
                    {
                        this.resultStream = new MemoryStream();
                        DevExpress.XtraPdfViewer.PdfViewer pdfViewr = new DevExpress.XtraPdfViewer.PdfViewer();
                        pdfViewr.LoadDocument(taptin);
                        pdfViewr.SaveDocument(resultStream);
                        resultStream.Position = 0;
                    }
                    else
                        this.resultStream = null;
                }
            }
            catch (Exception ex)
            {
                this.resultStream = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var success = new SDA.EFMODEL.DataModels.SDA_TUTORIAL();
                var hisCareTypeFocus = (SDA.EFMODEL.DataModels.SDA_TUTORIAL)gridViewTutorial.GetFocusedRow();
                DialogResult dialog = new DialogResult();

                dialog = MessageBox.Show
                    (Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong,
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialog == DialogResult.Yes)
                {
                    WaitingManager.Show();

                    success = new BackendAdapter(param).Post<SDA.EFMODEL.DataModels.SDA_TUTORIAL>
                        ("api/SdaTutorial/ChangeLock", ApiConsumer.ApiConsumers.SdaConsumer, hisCareTypeFocus, param);

                    WaitingManager.Hide();

                    if (success != null)
                    {
                        if (hisCareTypeFocus.IS_ACTIVE == 1)
                        {
                            BtnEdit.Enabled = false;
                        }
                        else
                        {
                            FillDataToEditorControl(hisCareTypeFocus);
                            ActionType = GlobalVariables.ActionEdit;
                            EnableControlChange(ActionType);
                        }

                        LoadDataToGrid();

                        MessageManager.Show(this, param, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var success = new SDA.EFMODEL.DataModels.SDA_TUTORIAL();
                var hisCareTypeFocus = (SDA.EFMODEL.DataModels.SDA_TUTORIAL)gridViewTutorial.GetFocusedRow();
                DialogResult dialog = new DialogResult();

                dialog = MessageBox.Show
                    (Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong,
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialog == DialogResult.Yes)
                {
                    WaitingManager.Show();

                    success = new BackendAdapter(param).Post<SDA.EFMODEL.DataModels.SDA_TUTORIAL>
                        ("api/SdaTutorial/ChangeLock", ApiConsumer.ApiConsumers.SdaConsumer, hisCareTypeFocus, param);

                    WaitingManager.Hide();

                    if (success != null)
                    {
                        if (hisCareTypeFocus.IS_ACTIVE == 1)
                        {
                            BtnEdit.Enabled = false;
                        }
                        else
                        {
                            FillDataToEditorControl(hisCareTypeFocus);
                            ActionType = GlobalVariables.ActionEdit;
                            EnableControlChange(ActionType);
                        }

                        LoadDataToGrid();

                        MessageManager.Show(this, param, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowdata = (SDA.EFMODEL.DataModels.SDA_TUTORIAL)gridViewTutorial.GetFocusedRow();

                DialogResult dialog = new DialogResult();

                dialog = MessageBox.Show
                    (Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong,
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (rowdata != null && dialog == DialogResult.Yes)
                {
                    bool success = false;
                    success = new BackendAdapter(param).Post<bool>
                        ("api/SdaTutorial/Delete", ApiConsumer.ApiConsumers.SdaConsumer, rowdata.ID, param);
                    if (success)
                    {
                        LoadDataToGrid();
                        currentData = ((List<SDA.EFMODEL.DataModels.SDA_TUTORIAL>)gridControlTutorial.DataSource).FirstOrDefault();
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboModule_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (CboModule.EditValue != null)
                    {
                        CboModule.Properties.Buttons[1].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboModule_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.currentData = null;
                    this.currentFileName = "";
                    this.currentUrl = "";
                    this.CboModule.EditValue = null;
                    this.CboModule.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

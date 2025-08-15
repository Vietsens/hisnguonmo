using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using static DevExpress.XtraBars.Customization.Helpers.DesignTimeManager.DesignTimeCreateItemMenu;
//using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
//using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using System.IO;
using MOS.EFMODEL.DataModels;
using Inventec.Common.SignLibrary;

namespace HIS.Desktop.Plugins.HisRegimenTemp.HisReigimenTemp
{
    public partial class frmReigimenTemp : HIS.Desktop.Utility.FormBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP currentData;
        Inventec.Desktop.Common.Modules.Module moduleData;

        string creator = "";
        string fileName = "";
        string filePath = "";
        string fileExtension = "";
        string url = "";
        private string tempDocFilePathPDF = null;
        private string tempDocFilePathDoc = null;

        HIS.Desktop.Plugins.HisRegimenTemp.UC.UCDocument UcDocument = new UC.UCDocument();
        HIS.Desktop.Plugins.HisRegimenTemp.UC.UCPDF UCPdf = new UC.UCPDF();


        public frmReigimenTemp(Inventec.Desktop.Common.Modules.Module moduleData) : base(moduleData)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                try
                {
                    string iconPath = System.IO.Path.Combine
                    (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings
                    ["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                    creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
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

        private void frmReigimenTemp_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
                SetDefaultFocus();
                EnableControlChange(this.ActionType);
                SetDefaultValue();
                SetResourcesByLanguage();
                ValidateForm();
                this.UcDocument.SetRtfText("");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }


        private void ValidateForm()
        {
            try
            {
                ValidationControlTextEditLanguageCode();
                ValidationControlTextEditLanguageName();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlTextEditLanguageName()
        {
            try
            {
                HIS.Desktop.Plugins.HisRegimenTemp.Validation.ValidationTextEditRegimenTempName validRule = new HIS.Desktop.Plugins.HisRegimenTemp.Validation.ValidationTextEditRegimenTempName();
                validRule.txtTen = txtTen;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtTen, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlTextEditLanguageCode()
        {
            try
            {
                HIS.Desktop.Plugins.HisRegimenTemp.Validation.ValidationTextEditRegimenTempCode validRule = new HIS.Desktop.Plugins.HisRegimenTemp.Validation.ValidationTextEditRegimenTempCode();
                validRule.txtMa = txtMa;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtMa, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetResourcesByLanguage()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResources = new ResourceManager("HIS.Desktop.Plugins.HisRegimenTemp.Resources.Lang", typeof
                (HIS.Desktop.Plugins.HisRegimenTemp.HisReigimenTemp.frmReigimenTemp).Assembly);
                //layout
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmReigimenTemp.LayoutControl1.Text", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmReigimenTemp.LayoutControl2.Text", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmReigimenTemp.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                //button
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmReigimenTemp.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmReigimenTemp.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmReigimenTemp.btnReset.Text", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmReigimenTemp.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.chkTheoKhoa.Text = Inventec.Common.Resource.Get.Value("frmReigimenTemp.chkTheoKhoa.Text", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.chkToanVien.Text = Inventec.Common.Resource.Get.Value("frmReigimenTemp.chkToanVien.Text", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                //this.btnImport.Image = Inventec.Common.Resource.Get.("frmReigimenTemp.btnImport.Image", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                //bar button
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmReigimenTemp.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmReigimenTemp.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmReigimenTemp.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmReigimenTemp.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());

                //column
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmReigimenTemp.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmReigimenTemp.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmReigimenTemp.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmReigimenTemp.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmReigimenTemp.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmReigimenTemp.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmReigimenTemp.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmReigimenTemp.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmReigimenTemp.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmReigimenTemp.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());
                //input box
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmReigimenTemp.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResources, LanguageManager.GetCulture());

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
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
                EnableControlChange(this.ActionType);
                txtSearch.Text = "";
                txtMa.Text = "";
                txtTen.Text = "";
                //this.UcDocument.SetRtfText("");
                this.UcDocument.SetFont(new Font("Times New Roman", 14));
                label2.Text = "";
                UCPdf.CloseDocument();
                this.panelControl1.Controls.Clear();
                fileName = "";
                //zoomFactor();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void EnableControlChange(int actionType)
        {
            btnAdd.Enabled = (actionType == GlobalVariables.ActionAdd);
            btnEdit.Enabled = (actionType == GlobalVariables.ActionEdit);
        }

        private void SetDefaultFocus()
        {
            try
            {
                txtSearch.Focus();
                txtSearch.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(Object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP>> apiResult = null;
                HisRegimenTempFilter filter = new HisRegimenTempFilter();
                filter.KEY_WORD = txtSearch.Text.Trim();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridView1.BeginDataUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP>>(RegimenTempUriStore.REGIMEN_TEMP_GET,
                   ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP>)apiResult.Data;
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
                LogSystem.Warn(ex);
            }
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SaveStreamToDocFile(Stream streamSource, string outputFilePath)
        {
            try
            {
                using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                {
                    streamSource.CopyTo(fileStream);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                MessageBox.Show("Error saving stream to .doc file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    FillDataEditorControl(data);
                    if (label2.Text == "") this.panelControl1.Controls.Clear();
                    if (data.IS_PUBLIC == 1)
                        chkToanVien.Checked = true;
                    else
                        chkToanVien.Checked = false;

                    if (data.IS_PUBLIC_IN_DEPARTMENT == 1)
                        chkTheoKhoa.Checked = true;
                    else
                        chkTheoKhoa.Checked = false;
                    // Xóa file tạm cũ nếu có
                    if ((!string.IsNullOrEmpty(tempDocFilePathDoc) && File.Exists(tempDocFilePathDoc)) ||
                            (!string.IsNullOrEmpty(tempDocFilePathPDF) && File.Exists(tempDocFilePathPDF)))
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(tempDocFilePathDoc) && File.Exists(tempDocFilePathDoc))
                                File.Delete(tempDocFilePathDoc);
                            if (!string.IsNullOrEmpty(tempDocFilePathPDF) && File.Exists(tempDocFilePathPDF))
                            {
                                UCPdf.CloseDocument();
                                File.Delete(tempDocFilePathPDF);
                            }
                        }
                        catch { }
                        tempDocFilePathDoc = null;
                        tempDocFilePathPDF = null;
                    }
                    string output = Utils.GenerateTempFileWithin();
                    string path = Utils.GenerateTempFolderWithin();
                    var streamSource = Inventec.Fss.Client.FileDownload.GetFile(data.URL);
                    streamSource.Position = 0;
                    string extension = ".doc";
                    if (!string.IsNullOrEmpty(data.FILE_NAME))
                    {
                        extension = Path.GetExtension(data.FILE_NAME).ToLower();
                        if (extension != ".doc" && extension != ".docx")
                            extension = ".doc";
                    }
                    string outputFilePath = Path.Combine(path, Guid.NewGuid().ToString() + extension);

                    if (data.FILE_TYPE == 2)
                    {
                        Utils.ByteToFile(Utils.StreamToByte(streamSource), output);
                        InitPdf();
                        UCPdf.LoadDocument(output);
                    }
                    else if (data.FILE_TYPE == 3)
                    {
                        SaveStreamToDocFile(streamSource, outputFilePath);
                        InitDocument();
                        UcDocument.LoadDocument(outputFilePath);
                    }                
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChange(this.ActionType);
                    btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    positionHandle = -1;
                    tempDocFilePathDoc = outputFilePath;
                    tempDocFilePathPDF = output;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void FillDataEditorControl(MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    txtMa.Text = data.REGIMEN_TEMP_CODE;
                    txtTen.Text = data.REGIMEN_TEMP_NAME;
                    label2.Text = data.FILE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP)gridView1.GetFocusedRow();
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
                MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP updateDTO = new MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP();
                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (chkToanVien.Checked)
                    updateDTO.IS_PUBLIC = 1;
                else
                    updateDTO.IS_PUBLIC = null;
                if (chkTheoKhoa.Checked)
                    updateDTO.IS_PUBLIC_IN_DEPARTMENT = 1;
                else
                    updateDTO.IS_PUBLIC_IN_DEPARTMENT = null;
                updateDTO.URL = url;
                updateDTO.FILE_NAME = fileName;
                if (fileExtension == ".pdf")
                    updateDTO.FILE_TYPE = 2;
                else if (fileExtension == ".doc" || fileExtension == ".docx")
                    updateDTO.FILE_TYPE = 3;
                else
                    updateDTO.FILE_TYPE = 0;
                HisEmployeeFilter filter = new HisEmployeeFilter();
                if (ActionType == GlobalVariables.ActionAdd)
                {                    
                    filter.LOGINNAME__EXACT = creator;
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var employees = new Inventec.Common.Adapter.BackendAdapter
                        (param).Get<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>>
                        (RegimenTempUriStore.HIS_EMPLOYEE, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (employees != null && employees.Count > 0)
                    {
                        var departmentId = employees.FirstOrDefault().DEPARTMENT_ID;
                        updateDTO.DEPARTMENT_ID = departmentId;
                    }
                    else
                    {
                        updateDTO.DEPARTMENT_ID = null;
                    }
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP>(RegimenTempUriStore.REGIMEN_TEMP_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToControl();
                        SetDefaultValue();
                    }
                }
                else
                {
                    filter.LOGINNAME__EXACT = creator;
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    if (fileName == "")
                    {
                        updateDTO.URL = currentData.URL;
                        updateDTO.FILE_NAME = currentData.FILE_NAME;
                        updateDTO.FILE_TYPE = currentData.FILE_TYPE;
                    }
                    var employees = new Inventec.Common.Adapter.BackendAdapter
                        (param).Get<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>>
                        (RegimenTempUriStore.HIS_EMPLOYEE, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                    if (employees != null && employees.Count > 0)
                    {
                        var departmentId = employees.FirstOrDefault().DEPARTMENT_ID;
                        updateDTO.DEPARTMENT_ID = departmentId;
                    }
                    else
                    {
                        updateDTO.DEPARTMENT_ID = null;
                    }
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP>(RegimenTempUriStore.REGIMEN_TEMP_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToControl();
                        SetDefaultValue();
                    }
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

        private void UpdateDTOFromDataForm(ref HIS_REGIMEN_TEMP currentDTO)
        {
            try
            {
                currentDTO.REGIMEN_TEMP_CODE = txtMa.Text.Trim();
                currentDTO.REGIMEN_TEMP_NAME = txtTen.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentID, ref HIS_REGIMEN_TEMP currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisRegimenTempFilter filter = new HisRegimenTempFilter();
                filter.ID = currentID;
                currentDTO = new BackendAdapter(param).Get<List<HIS_REGIMEN_TEMP>>(RegimenTempUriStore.REGIMEN_TEMP_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChange(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridView1.Focus();
                    gridView1.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP)gridView1.GetFocusedRow();
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

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridView1.Focus();
                    gridView1.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP)gridView1.GetFocusedRow();
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

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP pData = (MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "IS_PUBLIC_STR")
                    {
                        e.Value = pData.IS_PUBLIC == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "IS_PUBLIC_IN_DEPARTMENT_STR")
                    {
                        e.Value = pData.IS_PUBLIC_IN_DEPARTMENT == 1 ? true : false;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                    btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            btnEdit.Enabled = false;
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (MOS.EFMODEL.DataModels.HIS_REGIMEN_TEMP)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                    DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(RegimenTempUriStore.REGIMEN_TEMP_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToControl();
                            currentData = ((List<HIS_REGIMEN_TEMP>)gridControl1.DataSource).FirstOrDefault();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDocument()
        {
            try
            {
                this.panelControl1.Controls.Clear();

                if (this.UcDocument != null)
                {
                    this.UcDocument.Dock = DockStyle.Fill;
                    this.panelControl1.Controls.Add(this.UcDocument);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitPdf()
        {
            try
            {
                this.panelControl1.Controls.Clear();

                if (this.UCPdf != null)
                {
                    this.UCPdf.Dock = DockStyle.Fill;
                    this.panelControl1.Controls.Add(this.UCPdf);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Tất cả(*.pdf;*.doc;*.docx)|*.pdf;*.doc;*.docx|PDF (*.pdf)|*.pdf|Word (*.doc;*.docx)|*.doc;*.docx";
                    openFileDialog.Title = "Chọn tệp mẫu (PDF hoặc Word)";
                    openFileDialog.Multiselect = false;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        filePath = openFileDialog.FileName;
                        fileName = System.IO.Path.GetFileName(filePath);
                        fileExtension = System.IO.Path.GetExtension(filePath).ToLower();

                        // Kiểm tra loại file và khởi tạo control phù hợp
                        if (fileExtension == ".doc" || fileExtension == ".docx")
                        {
                            InitDocument();
                            UcDocument.LoadDocument(filePath);
                        }
                        if (fileExtension == ".pdf")
                        {
                            InitPdf();
                            UCPdf.LoadDocument(filePath);
                        }

                        byte[] buff = System.IO.File.ReadAllBytes(filePath);

                        MemoryStream stream = new MemoryStream(buff);
                        string clientCode = GlobalVariables.APPLICATION_CODE;
                        string fileStoreLocation = txtMa.Text.Trim();
                        url = Inventec.Fss.Client.FileUpload.UploadFile(
                            clientCode,
                            fileStoreLocation,
                            stream,
                            fileName
                        ).Url;

                        label2.Text = fileName;
                        //MessageBox.Show("Tệp đã được tải lên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //MessageBox.Show("Có lỗi khi tải lên tệp mẫu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmReigimenTemp_FormClosed(object sender, FormClosedEventArgs e)
        {
            base.OnFormClosing(new FormClosingEventArgs(CloseReason.UserClosing, false));
            if ((!string.IsNullOrEmpty(tempDocFilePathDoc) && File.Exists(tempDocFilePathDoc)))
            {
                try
                {
                    File.Delete(tempDocFilePathDoc);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
                finally
                {
                    tempDocFilePathDoc = null;
                }
            }
            if (!string.IsNullOrEmpty(tempDocFilePathPDF) && File.Exists(tempDocFilePathPDF))
            {
                try
                {
                    UCPdf.CloseDocument(); // Đảm bảo đã đóng file PDF
                    File.Delete(tempDocFilePathPDF);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
                finally
                {
                    tempDocFilePathPDF = null;
                }

            }
        }
    }
}

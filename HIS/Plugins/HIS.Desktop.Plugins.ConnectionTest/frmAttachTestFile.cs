using DevExpress.XtraEditors;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.ConnectionTest.ADO;
using HIS.Desktop.Plugins.ConnectionTest.Config;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Controls.EditorLoader;
using iTextSharp.text;
using iTextSharp.text.pdf;
using EMR.TDO;

namespace HIS.Desktop.Plugins.ConnectionTest
{
    public partial class frmAttachTestFile : Form
    {
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string treatmentCode = null;
        string serviceReqCode = null;
        string barCode = null;
        string[] selectedFileNames;
        AttachFileADO currentAttachmentFile;
        AttachFileADO attachmentFiles;
        List<AttachFileADO> listAttachmentFiles = new List<AttachFileADO>();

        public frmAttachTestFile(string TREATMENT_CODE, string SERVICE_REQ_CODE, string BAR_CODE)
        {
            treatmentCode = TREATMENT_CODE;
            serviceReqCode = SERVICE_REQ_CODE;
            barCode = BAR_CODE;

            InitializeComponent();
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmFileAttachment_Load(object sender, EventArgs e)
        {
            try
            {
                Config.ConfigKey.GetConfigKey();
                this.SetCaptionByLanguageKey();
                InitializegridLookUpEditDocType();
                InitializeControlState();
                textEditDocName.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmAttachTestFile
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ConnectionTest.Resources.Lang", typeof(frmAttachTestFile).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAttachTestFile.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl2.Text = Inventec.Common.Resource.Get.Value("frmAttachTestFile.labelControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("frmAttachTestFile.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmAttachTestFile.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.simpleButtonSave.Text = Inventec.Common.Resource.Get.Value("frmAttachTestFile.simpleButtonSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmAttachTestFile.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSTT.Caption = Inventec.Common.Resource.Get.Value("frmAttachTestFile.gridColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnFileName.Caption = Inventec.Common.Resource.Get.Value("frmAttachTestFile.gridColumnFileName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmAttachTestFile.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitializeControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitializegridLookUpEditDocType()
        {
            try
            {
                if (!Config.ConfigKey.IsHasConnectionEmr)
                    return;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOCUMENT_TYPE_CODE", "", 80, 1));
                columnInfos.Add(new ColumnInfo("DOCUMENT_TYPE_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOCUMENT_TYPE_NAME", "ID", columnInfos, false, 230);
                ControlEditorLoader.Load(gridLookUpEditDocType, GetDocumentTypes(), controlEditorADO);
                gridLookUpEditDocType.EditValue = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<EMR_DOCUMENT_TYPE> GetDocumentTypes()
        {
            List<EMR_DOCUMENT_TYPE> result = new List<EMR_DOCUMENT_TYPE>();

            try
            {
                CommonParam param = new CommonParam();
                EMR.Filter.EmrDocumentTypeFilter filter = new EMR.Filter.EmrDocumentTypeFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_FIELD = "DOCUMENT_TYPE_ID";
                filter.ORDER_DIRECTION = "ASC";

                result = new BackendAdapter(param).Get<List<EMR_DOCUMENT_TYPE>>(
                    "api/EmrDocumentType/Get",
                    ApiConsumers.EmrConsumer,
                    filter,
                    param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        private void SimpleButtonFindFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "*.pdf|*.pdf";
                openFileDialog.DefaultExt = ".pdf";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFileNames = openFileDialog.FileNames;

                    if (selectedFileNames != null && selectedFileNames.Length > 0)
                    {
                        foreach (var item in this.selectedFileNames)
                        {
                            int lIndex = item.LastIndexOf("\\");
                            int lIndex1 = item.LastIndexOf(".");
                            this.attachmentFiles = new AttachFileADO();
                            this.attachmentFiles.FILE_NAME = item.Substring(lIndex > 0 ? lIndex + 1 : lIndex);
                            this.attachmentFiles.EXTENSION = item.Substring(lIndex1 > 0 ? lIndex1 + 1 : lIndex1);
                            this.attachmentFiles.Base64Data = Inventec.Common.SignLibrary.Utils.FileToBase64String(item);
                            this.attachmentFiles.FullName = item;
                            string extension = System.IO.Path.GetExtension(item);
                            if ((extension ?? "").ToLower() != ".pdf")
                            {
                                this.attachmentFiles.image = System.Drawing.Image.FromFile(item);
                            }

                            listAttachmentFiles.Add(attachmentFiles);
                        }
                    }

                    // Update grid data source
                    gridControl1.BeginUpdate();
                    gridControl1.DataSource = listAttachmentFiles;
                    gridControl1.EndUpdate();

                    // Update document name if it's empty
                    if (string.IsNullOrEmpty(textEditDocName.Text) && listAttachmentFiles.Count > 0)
                    {
                        textEditDocName.Text = Path.GetFileNameWithoutExtension(listAttachmentFiles[0].FILE_NAME);
                    }
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
                currentAttachmentFile = (AttachFileADO)gridView1.GetFocusedRow();

                if (currentAttachmentFile != null)
                {
                    string fileExtension = Path.GetExtension(currentAttachmentFile.FullName).ToLower();
                    // Display pdf viewer
                    Root.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    pdfViewer1.LoadDocument(currentAttachmentFile.FullName);
                    textEditDocName.Text = currentAttachmentFile.FILE_NAME;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string CombinePdfFiles()
        {
            string outputPath = @"FSS\Upload\EMR\";
            Document document = new Document();

            try
            {
                PdfCopy writer = new PdfCopy(document, new FileStream(outputPath, FileMode.Create));
                document.Open();

                foreach (var item in listAttachmentFiles)
                {
                    string extension = Path.GetExtension(item.FullName).ToLower();

                    PdfReader reader = new PdfReader(item.FullName);
                    reader.ConsolidateNamedDestinations();

                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        document.NewPage();
                        writer.NewPage();
                        writer.AddPage(page);
                    }

                    PRAcroForm form = reader.AcroForm;
                    if (form != null)
                    {
                        try
                        {
                            writer.CopyDocumentFields(reader);
                        }
                        catch (Exception ex)
                        {
                            LogSystem.Error(ex);
                        }
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                document.Close();
            }

            return outputPath;
        }

        private MemoryStream GetMemoryStreamFromFile(string filePath)
        {
            MemoryStream memoryStream = null;

            try
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    memoryStream = new MemoryStream();

                    using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[fileStream.Length];
                        fileStream.Read(buffer, 0, (int)fileStream.Length);
                        memoryStream.Write(buffer, 0, (int)fileStream.Length);
                    }

                    memoryStream.Position = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                memoryStream = null;
            }

            return memoryStream;
        }

        private void simpleButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Config.ConfigKey.IsHasConnectionEmr)
                    return;

                if (listAttachmentFiles != null && listAttachmentFiles.Count > 0)
                {
                    // Create document data object
                    DocumentTDO document = new DocumentTDO();
                    Validation.ValidateMaxLength valid = new Validation.ValidateMaxLength();
                    valid.memoEdit = textEditDocName;
                    valid.maxLength = 2000;
                    valid.ErrorText = "Trường dữ liệu vượt quá 2000 ký tự";
                    valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(textEditDocName, valid);
                    if (!dxValidationProvider1.Validate())  return;
                    WaitingManager.Show();
                    document.DocumentName = textEditDocName.Text.Trim();
                    document.DocumentTypeId = (long)gridLookUpEditDocType.EditValue;
                    document.DocumentTypeId = 22;
                    document.HisCode = "TREATMENT_CODE:" + treatmentCode + " SERVICE_REQ_CODE:" + serviceReqCode + " BAR_CODE:" + barCode;
                    document.IsCapture = true;
                    // Generate combined PDF from all files
                    string combinedPdfPath = CombinePdfFiles();

                    // Create file holder
                    Inventec.Core.FileHolder fileHolder = new Inventec.Core.FileHolder();
                    fileHolder.FileName = combinedPdfPath;
                    fileHolder.Content = GetMemoryStreamFromFile(combinedPdfPath);

                    // Save document to EMR system
                    if (document != null)
                    {
                        CommonParam param = new CommonParam();

                        var result = new BackendAdapter(param).PostWithFile<DocumentTDO>(
                            EMR.URI.EmrDocument.CREATE_BY_TDO,
                            ApiConsumers.EmrConsumer,
                            document,
                            new List<Inventec.Core.FileHolder>() { fileHolder },
                            param);

                        MessageManager.Show(this, param, result != null);

                        if (result != null)
                        {
                            try
                            {
                                if (File.Exists(combinedPdfPath))
                                {
                                    File.Delete(combinedPdfPath);
                                }
                            }
                            catch { }

                            this.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một file để đính kèm.");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                AttachFileADO selectedFile = gridView1.GetFocusedRow() as AttachFileADO;

                if (selectedFile != null)
                {
                    if (MessageBox.Show(
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                            HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
                        "",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        listAttachmentFiles.Remove(selectedFile);

                        gridControl1.BeginUpdate();
                        gridControl1.DataSource = (this.listAttachmentFiles != null ? this.listAttachmentFiles.ToList() : null);
                        gridControl1.EndUpdate();
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

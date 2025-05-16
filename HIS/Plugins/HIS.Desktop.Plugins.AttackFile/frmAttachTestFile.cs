using DevExpress.XtraEditors;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.AttackFile.ADO;
using HIS.Desktop.Plugins.AttackFile.Config;
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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using HIS.Desktop.ADO;
using AttachFileADO = HIS.Desktop.Plugins.AttackFile.ADO.AttachFileADO;
using EMR.Filter;

namespace HIS.Desktop.Plugins.AttackFile
{
    public partial class frmAttachTestFile : HIS.Desktop.Utility.FormBase
    {
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        string[] selectedFileNames;
        AttachFileADO currentAttachmentFile;
        AttachFileADO attachmentFiles;
        List<AttachFileADO> listAttachmentFiles = new List<AttachFileADO>();
        Inventec.Desktop.Common.Modules.Module currentModule;
        EmrAttackFileADO inputADO;

        public frmAttachTestFile(Inventec.Desktop.Common.Modules.Module currentModule, EmrAttackFileADO inputADO) :base(currentModule)
        {
         
            InitializeComponent();
            try
            {
                this.inputADO = inputADO;
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
                InitializegridLookUpEditDocType();
                txtDocName.Focus();
                this.layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                EMR_TREATMENT treatment = GetTreatmentById(this.inputADO.TreatmentCode);
                if(treatment == null)
                {
                    XtraMessageBox.Show("Hồ sơ hiện chưa có trên hệ thống văn bản. Vui lòng sử dụng chức năng 'Đồng bộ lại EMR'");
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private EMR_TREATMENT GetTreatmentById(string treatmentCode)
        {
            try
            {
                if (String.IsNullOrEmpty(treatmentCode))
                {
                    return null;
                }
                CommonParam paramCommon = new CommonParam();
                EmrTreatmentFilter filter = new EmrTreatmentFilter();
                filter.TREATMENT_CODE__EXACT = treatmentCode;
                return new BackendAdapter(paramCommon).Get<List<EMR_TREATMENT>>("api/EmrTreatment/Get", ApiConsumers.EmrConsumer, filter, paramCommon).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
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
                ControlEditorLoader.Load(cboDocType, GetDocumentTypes(), controlEditorADO);
                cboDocType.EditValue = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;
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
                openFileDialog.Filter = "Ảnh(*.jpg, *.Png, *.jpeg, *.bmp,*.gif,*.pdf)|*.jpg;*.png;*.jpeg;*.bmp;*.gif;*.pdf";
                openFileDialog.DefaultExt = ".jpg;.png;.jpeg;.bmp;.gif;.pdf";


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
                    this.gridControl1.DataSource = listAttachmentFiles;
                    gridControl1.EndUpdate();

                    txtDocName.Text = Path.GetFileNameWithoutExtension(listAttachmentFiles[0].FILE_NAME);
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
                    if (fileExtension == ".pdf")
                    {
                        // Display pdf viewer
                        this.layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        pdfViewer1.LoadDocument(currentAttachmentFile.FullName);
                    }
                    else
                    {
                        //Display picture viewer
                        this.layoutControlItem9.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        this.layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        pictureEdit1.Image = System.Drawing.Image.FromFile(currentAttachmentFile.FullName);
                        pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                    }
                    txtDocName.Text = currentAttachmentFile.FILE_NAME;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pictureEdit1.Image.Tag), pictureEdit1.Image.Tag));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GeneratePdfFileFromImages()
        {
            string output = Path.GetTempFileName();
            try
            {

                using (FileStream fs = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (Document doc = new Document())
                    {
                        try
                        {
                            PdfWriter.GetInstance(doc, fs);

                            doc.Open();
                            foreach (var item in this.listAttachmentFiles)
                            {
                                string extensionc = System.IO.Path.GetExtension(item.FullName);
                                if ((extensionc ?? "").ToLower() != ".pdf")
                                {
                                    iTextSharp.text.Image image;
                                    image = iTextSharp.text.Image.GetInstance(item.image, BaseColor.BLACK);
                                    if (image.Height > image.Width)
                                    {
                                        float percentage = 0.0f;
                                        percentage = doc.PageSize.Height / image.Height;
                                        image.ScalePercent(percentage * 90);
                                    }
                                    else
                                    {
                                        float percentage = 0.0f;
                                        percentage = doc.PageSize.Width / image.Width;
                                        image.ScalePercent(percentage * 90);
                                    }
                                    doc.NewPage();
                                    doc.Add(image);
                                }
                                else
                                {
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                        finally
                        {
                            doc.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return output;
        }

        private string CombineMultiplePDFs()
        {
            Document document = new Document();
            string outFile = Path.GetTempFileName();
            PdfCopy writer = new PdfCopy(document, new FileStream(outFile, FileMode.Create));

            try
            {
                document.Open();


                foreach (var item in this.listAttachmentFiles)
                {
                    string extensionc = System.IO.Path.GetExtension(item.FullName);
                    if ((extensionc ?? "").ToLower() == ".pdf")
                    {
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
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }

                        reader.Close();
                    }
                    else
                    {
                        string outputss = Path.GetTempFileName();
                        FileStream fss = new FileStream(outputss, FileMode.Create, FileAccess.Write, FileShare.None);
                        Document docc = new Document();
                        PdfWriter.GetInstance(docc, fss);
                        docc.Open();
                        iTextSharp.text.Image image;
                        image = iTextSharp.text.Image.GetInstance(item.image, BaseColor.BLACK);
                        if (image.Height > image.Width)
                        {
                            float percentage = 0.0f;
                            percentage = document.PageSize.Height / image.Height;
                            image.ScalePercent(percentage * 90);
                        }
                        else
                        {
                            float percentage = 0.0f;
                            percentage = document.PageSize.Width / image.Width;
                            image.ScalePercent(percentage * 90);
                        }
                        docc.NewPage();
                        docc.Add(image);
                        docc.Close();
                        PdfReader reader = new PdfReader(outputss);
                        reader.ConsolidateNamedDestinations();
                        for (int i = 1; i <= reader.NumberOfPages; i++)
                        {
                            PdfImportedPage page = writer.GetImportedPage(reader, i);
                            document.NewPage();
                            writer.NewPage();
                            page.Height = document.PageSize.Height;
                            writer.AddPage(page);
                        }

                    }
                }

                writer.Close();
                document.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return outFile;
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
                currentAttachmentFile = (AttachFileADO)gridView1.GetFocusedRow();
                currentAttachmentFile.FILE_NAME = txtDocName.Text;
                if (!Config.ConfigKey.IsHasConnectionEmr)
                    return;

                if (listAttachmentFiles != null && listAttachmentFiles.Count > 0)
                {
                    // Create document data object
                    DocumentTDO document = new DocumentTDO();
                    document.DocumentName = txtDocName.Text.Trim();
                    document.DocumentTypeId = (long)cboDocType.EditValue;
                    document.TreatmentCode = inputADO.TreatmentCode;
                    document.HisCode = inputADO.HisCode;
                    // Generate combined PDF from all files
                    GeneratePdfFileFromImages();
                    string combinedPdfPath = CombineMultiplePDFs();
                    
                    document.OriginalVersion = new VersionTDO();
                    document.OriginalVersion.Base64Data = Convert.ToBase64String(GetMemoryStreamFromFile(combinedPdfPath).ToArray());
                    document.FileType = EMR.TDO.FileType.PDF;
                    // Save document to EMR system
                    if (document != null)
                    {
                        CommonParam param = new CommonParam();

                        var result = new BackendAdapter(param).Post<DocumentTDO>(
                            EMR.URI.EmrDocument.CREATE_BY_TDO,
                            ApiConsumers.EmrConsumer,
                            document,
                            param);
                        MessageManager.Show(this, param, result != null);

                        if (result != null)
                        {
                            this.Close();
                        }
                        try
                        {
                            if (File.Exists(combinedPdfPath))
                            {
                                File.Delete(combinedPdfPath);
                            }
                        }
                        catch { }
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một file để đính kèm.");
                }
                this.attachmentFiles = null;
                this.selectedFileNames = null;
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
                        pictureEdit1.Image = null;
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
                    AttachFileADO AttackTDO = (AttachFileADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (AttackTDO != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void textEditDocName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Validation.ValidateMaxLength valid = new Validation.ValidateMaxLength();
                    valid.memoEdit = txtDocName;
                    valid.maxLength = 2000;
                    valid.ErrorText = "Trường dữ liệu vượt quá 2000 ký tự";
                    valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(txtDocName, valid);
                    if (!dxValidationProvider1.Validate()) return;
                }
                currentAttachmentFile = (AttachFileADO)gridView1.GetFocusedRow();
                currentAttachmentFile.FILE_NAME = txtDocName.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void pictureEdit1_ImageChanged(object sender, EventArgs e)
        {
            try
            {
                var rowData = (AttachFileADO)gridView1.GetFocusedRow();
                if (rowData != null)
                {

                    PictureEdit pedit = sender as PictureEdit;
                    string imageLocal = pedit.GetLoadedImageLocation();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => imageLocal), imageLocal));
                    if (!String.IsNullOrEmpty(imageLocal))
                    {
                        int lIndex = imageLocal.LastIndexOf("\\");
                        //this.fileNameAttack = imageLocal.Substring(lIndex > 0 ? lIndex + 1 : lIndex);
                        rowData.FILE_NAME = imageLocal.Substring(lIndex > 0 ? lIndex + 1 : lIndex);
                    }
                    else if (!String.IsNullOrEmpty(rowData.FullName))
                    {
                        int lIndex = rowData.FullName.LastIndexOf("\\");
                        rowData.FILE_NAME = rowData.FullName.Substring(lIndex > 0 ? lIndex + 1 : lIndex);
                    }
                    //txtDocumentName.Text = this.fileNameAttack;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

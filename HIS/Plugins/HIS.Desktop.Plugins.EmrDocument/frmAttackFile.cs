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
using Aspose.Pdf.Devices;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.TDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.EmrDocument.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Runtime.InteropServices;
using WIA;
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
using System.Globalization;
using System.Drawing.Imaging;
using Inventec.Common.SignLibrary.DTO;
using System.Drawing.Drawing2D;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.Desktop.Plugins.EmrDocument
{
    public partial class frmAttackFile : Form
    {
        #region Reclare

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;


        V_EMR_DOCUMENT curentDocument;
        DelegateSelectData dlgGetImageFromModuleCamera;
        DelegateReturnMutilObject lstdlgGetImageFromModuleCamera;
        long _TreatmentId = 0;
        string loginName = null;
        string[] fullfileNameAttack;
        AttackADO fileNameAttack;
        AttackADO currentFileAttack;
        List<AttackADO> ListfileNameAttack = new List<AttackADO>();
        Action actRefesh;

        private List<string> _tempFilesToDelete = new List<string>();
        private PdfDocument _doc;
        private const string formatJpeg = "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}";
        internal string _deviceId;
        private string _lastItem;
        #endregion

        #region Construct
        public frmAttackFile(long treatmentId, string loginName, Action _actRefesh)
            : this(null, treatmentId, loginName, _actRefesh)
        {
        }

        public frmAttackFile(V_EMR_DOCUMENT document, long treatmentId, string loginName, Action _actRefesh)
        {
            InitializeComponent();
            this.curentDocument = document;
            this._TreatmentId = treatmentId;
            this.loginName = loginName;
            this.actRefesh = _actRefesh;
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
        #endregion

        #region Private Method
        private void frmAttackFile_Load(object sender, EventArgs e)
        {
            try
            {
                Config.ConfigKey.GetConfigKey();
                this.SetCaptionByLanguageKey();
                this.imageview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.pdfview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                InitComboDocumentType();

                InitControlState();
                txtDocumentName.Focus();
                txtDocumentName.SelectAll();
                LoadCboTextGroup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkPrintDupicate.Name)
                        {
                            chkPrintDupicate.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCboTextGroup()
        {
            try
            {
                if (!Config.ConfigKey.IsHasConnectionEmr)
                    return;
                EmrDocumentGroupFilter filter = new EmrDocumentGroupFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.IS_LEAF = true;
                var datas = new BackendAdapter(new CommonParam()).Get<List<EMR_DOCUMENT_GROUP>>("api/EmrDocumentGroup/Get", ApiConsumers.EmrConsumer, filter, null);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOCUMENT_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DOCUMENT_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOCUMENT_GROUP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.CboDocumentGroup, datas, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        //private string GeneratePdfFileFromImage()
        //{
        //    string output = Path.GetTempFileName();
        //    try
        //    {
        //        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(this.pteAnhChupFileDinhKem.Image, BaseColor.BLACK);
        //        using (FileStream fs = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None))
        //        {
        //            using (Document doc = new Document(image))
        //            {
        //                using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
        //                {
        //                    doc.Open();
        //                    image.SetAbsolutePosition(0, 0);
        //                    writer.DirectContent.AddImage(image);
        //                    doc.Close();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return output;
        //}

        /// <summary>
        /// nối nhiếu file ảnh thành 1 file pdf
        /// </summary>
        /// <returns></returns>
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
                            foreach (var item in this.ListfileNameAttack)
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

            document.Open();


            foreach (var item in this.ListfileNameAttack)
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
                    //FileInfo fi = new FileInfo(GeneratePdfFileFromImages());
                    //if (!string.IsNullOrEmpty(GeneratePdfFileFromImages()) && File.Exists(GeneratePdfFileFromImages()) && fi.Length > 0)
                    //{
                    //    PdfReader reader = new PdfReader(GeneratePdfFileFromImages());
                    //    reader.ConsolidateNamedDestinations();
                    //    for (int i = 1; i <= reader.NumberOfPages; i++)
                    //    {
                    //        PdfImportedPage page = writer.GetImportedPage(reader, i);
                    //        document.NewPage();
                    //        writer.NewPage();
                    //        writer.AddPage(page);

                    //    }
                    //} 

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
                        writer.AddPage(page);
                    }

                }
            }

            writer.Close();
            document.Close();
            return outFile;
        }
        private string MultiplePDF()
        {
            CombineMultiplePDFs();
            GeneratePdfFileFromImages();
            Document document = new Document();
            List<string> file = new List<string>();
            if (!string.IsNullOrEmpty(CombineMultiplePDFs()))
            {
                file.Add(CombineMultiplePDFs());
            }
            if (!string.IsNullOrEmpty(GeneratePdfFileFromImages()))
            {
                file.Add(GeneratePdfFileFromImages());
            }

            string outFiles = Path.GetTempFileName();
            PdfCopy writer = new PdfCopy(document, new FileStream(outFiles, FileMode.Create));

            document.Open();

            foreach (var item in file)
            {

                PdfReader reader = new PdfReader(item);
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
                    writer.CopyDocumentFields(reader);
                }

                reader.Close();


            }
            writer.Close();
            document.Close();
            return outFiles;
        }
        private string GetBase64FileData(string outFile)
        {
            string b64Data = "";
            try
            {
                MemoryStream streamData = new MemoryStream();
                if (!String.IsNullOrEmpty(outFile))
                {
                    using (FileStream file = new FileStream(outFile, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        streamData.Write(bytes, 0, (int)file.Length);
                    }
                }

                streamData.Position = 0;
                b64Data = Convert.ToBase64String(streamData.ToArray());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return b64Data;
        }

        private MemoryStream GetMemoryStreamFileData(string outFile)
        {
            MemoryStream streamData = null;
            try
            {
                if (!String.IsNullOrEmpty(outFile))
                {
                    streamData = new MemoryStream();
                    using (FileStream file = new FileStream(outFile, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        streamData.Write(bytes, 0, (int)file.Length);
                    }
                    streamData.Position = 0;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                streamData = null;
            }
            return streamData;
        }

        private void InitComboDocumentType()
        {
            try
            {
                if (!Config.ConfigKey.IsHasConnectionEmr)
                    return;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOCUMENT_TYPE_CODE", "", 80, 1));
                columnInfos.Add(new ColumnInfo("DOCUMENT_TYPE_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOCUMENT_TYPE_NAME", "ID", columnInfos, false, 230);
                ControlEditorLoader.Load(cboDocumentType, GetDocumentType(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE> GetDocumentType()
        {
            List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE> result = new List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE>();
            try
            {
                CommonParam param = new CommonParam();
                EMR.Filter.EmrDocumentTypeFilter filter = new EMR.Filter.EmrDocumentTypeFilter();
                filter.IS_ACTIVE = 1;
                filter.ORDER_FIELD = "DOCUMENT_TYPE_CODE";
                filter.ORDER_DIRECTION = "ASC";

                result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE>>("api/EmrDocumentType/Get", ApiConsumers.EmrConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private EMR_TREATMENT GetTreatmentById(long treatmentId)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                EmrTreatmentFilter filter = new EmrTreatmentFilter();
                filter.ID = treatmentId;
                return new BackendAdapter(paramCommon).Get<List<EMR_TREATMENT>>(EmrRequestUriStore.EMR_TREATMENT_GET, ApiConsumers.EmrConsumer, filter, paramCommon).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        private void FillImageFromModuleCamereToUC(object dataImage)
        {
            try
            {
                if (dataImage != null)
                {
                    Inventec.Common.Logging.LogSystem.Info("dataImage: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataImage), dataImage));
                    pteAnhChupFileDinhKem.Image = (System.Drawing.Image)dataImage;
                    var check = this.ListfileNameAttack.OrderByDescending(o => o.Dem).FirstOrDefault();

                    Inventec.Common.Logging.LogSystem.Info("dem max: " + check);
                    int dem = 0;
                    if (check == null || check.Dem == 0)
                    {
                        dem = 1;
                    }
                    else
                    {
                        dem = check.Dem + 1;
                    }
                    fileNameAttack = new AttackADO();
                    this.fileNameAttack.FILE_NAME = "Ảnh chụp " + dem.ToString() + ".jpg";
                    this.fileNameAttack.FullName = "Ảnh chụp " + dem.ToString() + ".jpg";
                    this.fileNameAttack.image = (System.Drawing.Image)dataImage;
                    this.fileNameAttack.Dem = dem;

                    pteAnhChupFileDinhKem.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                    this.ListfileNameAttack.Add(this.fileNameAttack);
                    Inventec.Common.Logging.LogSystem.Info("dữ liệu ảnh chụp: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListfileNameAttack), this.ListfileNameAttack));

                    gridView2.BeginUpdate();
                    gridView2.GridControl.DataSource = this.ListfileNameAttack;
                    gridView2.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnOpenFileInComputer_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                //openFile.Filter = "Ảnh jpg|*.jpg|Ảnh Png|*.png|Ảnh jpeg|*.jpeg|Ảnh bmp|*.bmp|Ảnh gif|*.gif";
                //openFile.DefaultExt = ".jpg";

                openFile.Multiselect = true;
                openFile.Filter = "Ảnh(*.jpg, *.Png, *.jpeg, *.bmp,*.gif,*.pdf)|*.jpg;*.png;*.jpeg;*.bmp;*.gif;*.pdf";
                openFile.DefaultExt = ".jpg;.png;.jpeg;.bmp;.gif;.pdf";

                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    // pteAnhChupFileDinhKem.Image = System.Drawing.Image.FromFile(openFile.FileName);
                    this.fullfileNameAttack = openFile.FileNames;
                    // pteAnhChupFileDinhKem.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                    // Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pteAnhChupFileDinhKem.Image.Tag), pteAnhChupFileDinhKem.Image.Tag));

                    if (this.fullfileNameAttack != null)
                    {
                        foreach (var item in this.fullfileNameAttack)
                        {
                            int lIndex = item.LastIndexOf("\\");
                            int lIndex1 = item.LastIndexOf(".");
                            this.fileNameAttack = new AttackADO();
                            this.fileNameAttack.FILE_NAME = item.Substring(lIndex > 0 ? lIndex + 1 : lIndex);
                            this.fileNameAttack.EXTENSION = item.Substring(lIndex1 > 0 ? lIndex1 + 1 : lIndex1);
                            this.fileNameAttack.Base64Data = Inventec.Common.SignLibrary.Utils.FileToBase64String(item);
                            this.fileNameAttack.FullName = item;
                            string extension = System.IO.Path.GetExtension(item);
                            if ((extension ?? "").ToLower() != ".pdf")
                            {
                                //int largestEdgeLength = 10;
                                // DevExpress.XtraPdfViewer.PdfViewer pdf = new DevExpress.XtraPdfViewer.PdfViewer();
                                //pdf.LoadDocument(item);
                                //for (int i = 1; i <= pdf.PageCount; i++)
                                //{
                                //FileStream st = new FileStream(item, FileMode.Open);

                                //    // Export all pages to bitmaps
                                //  Bitmap image = pdf.CreateBitmap(i, largestEdgeLength);
                                //    // Save the resulting images
                                //   this.fileNameAttack.image = System.Drawing.Image.FromStream(st);
                                //}
                                this.fileNameAttack.image = System.Drawing.Image.FromFile(item);
                            }
                            //else
                            //{
                            //    
                            //}
                            this.ListfileNameAttack.Add(fileNameAttack);
                        }
                    }
                }
                gridView2.BeginUpdate();
                this.gridView2.GridControl.DataSource = ListfileNameAttack;
                gridView2.EndUpdate();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fullfileNameAttack), fullfileNameAttack));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListfileNameAttack), ListfileNameAttack));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnOpenCamera_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Camera").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Camera");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    this.dlgGetImageFromModuleCamera = this.FillImageFromModuleCamereToUC;
                    listArgs.Add(this.dlgGetImageFromModuleCamera);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAttackFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Config.ConfigKey.IsHasConnectionEmr)
                    return;
                //if (pteAnhChupFileDinhKem.Image != null)
                if (this.ListfileNameAttack != null && this.ListfileNameAttack.Count > 0)
                {


                    DocumentTDO docCreate = new DocumentTDO();
                    docCreate.DocumentName = txtDocumentName.Text;
                    docCreate.DocumentTypeId = cboDocumentType.EditValue != null ? (long?)cboDocumentType.EditValue : null;
                    docCreate.DocumentGroupId = CboDocumentGroup.EditValue != null ? (long?)CboDocumentGroup.EditValue : null;
                    if (this.curentDocument != null)
                    {
                        docCreate.TreatmentCode = this.curentDocument.TREATMENT_CODE;
                    }
                    else
                    {
                        EMR_TREATMENT treatemnt = GetTreatmentById(this._TreatmentId);
                        docCreate.TreatmentCode = treatemnt != null ? treatemnt.TREATMENT_CODE : "";
                    }
                    docCreate.HisCode = "";//TODO
                    docCreate.IsCapture = true;
                    //string output = GeneratePdfFileFromImage();

                    //docCreate.OriginalVersion = new VersionTDO();
                    //docCreate.OriginalVersion.Base64Data = GetBase64FileData(output);

                    List<Inventec.Core.FileHolder> files = new List<Inventec.Core.FileHolder>();
                    //CombineMultiplePDFs();
                    //GeneratePdfFileFromImages();
                    //MultiplePDF();
                    string output = CombineMultiplePDFs();
                    Inventec.Core.FileHolder file = new Inventec.Core.FileHolder();
                    file.FileName = output;
                    file.Content = GetMemoryStreamFileData(output);

                    //TODO

                    if (docCreate != null)
                    {
                        CommonParam param = new CommonParam();
                        var resultData = new BackendAdapter(param).PostWithFile<DocumentTDO>(EMR.URI.EmrDocument.CREATE_WITH_FILE, ApiConsumers.EmrConsumer, docCreate, new List<Inventec.Core.FileHolder>() { file }, param);
                        //var resultData = new BackendAdapter(param).PostWithFile<DocumentTDO>(EMR.URI.EmrDocument.CREATE_WITH_FILE, ApiConsumers.EmrConsumer, docCreate, files, param);

                        Inventec.Common.Logging.LogSystem.Debug("Goi api tao van ban " + (resultData != null ? "thanh cong" : "that bai") + "____Du lieu dau vao:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => docCreate), docCreate) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => output), output) + "____Ket qua tra ve:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultData), resultData) + "___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                        MessageManager.Show(this, param, resultData != null);
                        if (resultData != null)
                        {
                            if (this.actRefesh != null) this.actRefesh();
                            try
                            {
                                if (File.Exists(output))
                                {
                                    File.Delete(output);
                                }
                            }
                            catch { }
                            this.Close();
                        }
                        else
                        {
                            //Inventec.Common.Logging.LogSystem.Debug("Goi api tao van ban that bai____Du lieu dau vao:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => docCreate), docCreate) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => output), output) + "____Ket qua tra ve:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultData), resultData));
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn file ảnh từ máy tính hoặc chụp ảnh từ camera");
                }

                this.fileNameAttack = null;
                this.fullfileNameAttack = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void pteAnhChupFileDinhKem_ImageChanged(object sender, EventArgs e)
        {
            try
            {
                var rowData = (AttackADO)gridView2.GetFocusedRow();
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

        private void cboDocumentType_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDocumentType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocumentType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboDocumentType.Properties.Buttons[1].Visible = cboDocumentType.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocumentType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboDocumentType.EditValue != null)
                    {
                        txtDocumentName.Focus();
                        txtDocumentName.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocumentType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDocumentType.ClosePopup();
                    if (cboDocumentType.EditValue != null)
                    {
                        txtDocumentName.Focus();
                        txtDocumentName.SelectAll();
                    }
                }
                else
                    cboDocumentType.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDocumentName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CboDocumentGroup.Focus();
                    CboDocumentGroup.ShowPopup();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        #endregion

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                AttackADO data = (AttackADO)gridView2.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.ListfileNameAttack.Remove(data);

                    gridView2.BeginUpdate();
                    gridView2.GridControl.DataSource = (this.ListfileNameAttack != null ? this.ListfileNameAttack.ToList() : null);
                    gridView2.EndUpdate();

                    pteAnhChupFileDinhKem.Image = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    AttackADO AttackTDO = (AttackADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                LogSystem.Warn(ex);
            }
        }

        private void gridView2_Click(object sender, EventArgs e)
        {
            try
            {
                currentFileAttack = (AttackADO)gridView2.GetFocusedRow();
                if (currentFileAttack != null)
                {
                    if ((System.IO.Path.GetExtension(currentFileAttack.FullName) ?? "").ToLower() == ".pdf")
                    {
                        // DevExpress.XtraPdfViewer.PdfViewer pdf = new DevExpress.XtraPdfViewer.PdfViewer();

                        //pdf.LoadDocument(data.FullName);
                        this.imageview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.pdfview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        pdfViewer1.LoadDocument(currentFileAttack.FullName);
                        btnRotateLeft.Enabled = false;
                        btnRotateRight.Enabled = false;

                    }
                    else
                    {
                        this.imageview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        this.pdfview.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        pteAnhChupFileDinhKem.Image = currentFileAttack.image;
                        btnRotateLeft.Enabled = true;
                        btnRotateRight.Enabled = true;
                    }
                    pteAnhChupFileDinhKem.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pteAnhChupFileDinhKem.Image.Tag), pteAnhChupFileDinhKem.Image.Tag));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void CboDocumentGroup_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAttackFile.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboDocumentGroup_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (CboDocumentGroup.EditValue != null)
                    {
                        btnAttackFile.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void CboDocumentGroup_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CboDocumentGroup.Properties.Buttons[1].Visible = CboDocumentGroup.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboDocumentGroup_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    CboDocumentGroup.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnAttackFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAttackFile_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            try
            {
                ShowScan();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        class WIA_DPS_DOCUMENT_HANDLING_SELECT
        {
            public const uint FEEDER = 0x00000001;
            public const uint FLATBED = 0x00000002;
            public const uint DUPLEX = 0x004;
        }
        class WIA_DPS_DOCUMENT_HANDLING_STATUS
        {
            public const uint FEED_READY = 0x00000001;
        }
        class WIA_PROPERTIES
        {
            public const uint WIA_RESERVED_FOR_NEW_PROPS = 1024;
            public const uint WIA_DIP_FIRST = 2;
            public const uint WIA_DPA_FIRST = WIA_DIP_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            public const uint WIA_DPC_FIRST = WIA_DPA_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            //
            // Scanner only device properties (DPS)
            //
            public const uint WIA_DPS_FIRST = WIA_DPC_FIRST + WIA_RESERVED_FOR_NEW_PROPS;
            public const uint WIA_DPS_DOCUMENT_HANDLING_STATUS = WIA_DPS_FIRST + 13;
            public const uint WIA_DPS_DOCUMENT_HANDLING_SELECT = WIA_DPS_FIRST + 14;
        }

        public static List<StreamToPdfADO> ScanDuplex(WIA.Device device)
        {
            try
            {
                List<StreamToPdfADO> ret = new List<StreamToPdfADO>();
                device.Properties["3088"].set_Value(5);
                //SetDeviceProperty(ref device, 3096,1);
                WIA.Item items = device.Items[1];
                //items.Properties["6146"].set_Value(2);
                AdjustScannerSettings(items, 150, 0, 0, 1250, 1700, 0, 0, 1);

                ICommonDialog dlg = new WIA.CommonDialog();
                while (true)
                {
                    try
                    {
                        WIA.ImageFile image = (WIA.ImageFile)dlg.ShowTransfer(items);
                        if (image != null && image.FileData != null)
                        {
                            StreamToPdfADO ado = new StreamToPdfADO();
                            string fileName = Path.GetTempFileName();
                            File.Delete(fileName);
                            image.SaveFile(fileName);
                            ado.Url = fileName;
                            ret.Add(ado);
                        }
                    }
                    catch
                    {
                        break;
                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("Exception from HRESULT: 0x80210067"))
                {
                    MessageBox.Show("Máy scan có thể không hỗ trợ in 2 mặt, vui lòng kiểm tra lại.");
                }
                return null;
            }
        }
        public static List<StreamToPdfADO> Scan(WIA.Device device)
        {
            List<StreamToPdfADO> ret = new List<StreamToPdfADO>();
            try
            {

                var scannerItem = device.Items[1];
                AdjustScannerSettings(scannerItem, 150, 0, 0, 1250, 1700, 0, 0, 1);
                ICommonDialog dlg = new WIA.CommonDialog();

                try
                {
                    WIA.ImageFile imageFile = (WIA.ImageFile)dlg.ShowTransfer(scannerItem, formatJpeg, false);
                    if (imageFile != null && imageFile.FileData != null)
                    {
                        StreamToPdfADO ado = new StreamToPdfADO();
                        string fileName = Path.GetTempFileName();
                        File.Delete(fileName);
                        imageFile.SaveFile(fileName);
                        //
                        ado.Url = fileName;
                        ret.Add(ado);
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return ret;
        }
        public void ShowScan()
        {
            try
            {
                DeviceManager deviceManager = new DeviceManager();
                DeviceInfo firstScannerAvailable = null;
                if (deviceManager.DeviceInfos.Count == 0)
                {
                    MessageBox.Show("Vui lòng kết nối đến máy Scan với máy tính");
                    return;
                }

                for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
                {
                    if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType)
                        continue;
                    firstScannerAvailable = deviceManager.DeviceInfos[i];
                    break;
                }
                var device = firstScannerAvailable.Connect();

                List<StreamToPdfADO> streams = new List<StreamToPdfADO>();
                if (chkPrintDupicate.Checked)
                {
                    streams = ScanDuplex(device);
                }
                else
                {
                    streams = Scan(device);
                }

                if (streams != null && streams.Count() > 0)
                {
                    string fileName = Path.GetTempFileName();


                    var check = this.ListfileNameAttack.OrderByDescending(o => o.Dem).FirstOrDefault();

                    Inventec.Common.Logging.LogSystem.Info("dem max: " + check);
                    int dem = 0;
                    if (check == null || check.Dem == 0)
                    {
                        dem = 1;
                    }
                    else
                    {
                        dem = check.Dem + 1;
                    }
                    fileNameAttack = new AttackADO();
                    this.fileNameAttack.FILE_NAME = "Ảnh " + dem.ToString() + ".png";
                    if (streams.Count == 1)
                    {
                        pteAnhChupFileDinhKem.Image = System.Drawing.Image.FromFile(streams.First().Url);
                        this.fileNameAttack.image = System.Drawing.Image.FromFile(streams.First().Url);

                    }
                    else if (streams.Count == 2)
                    {
                        System.Drawing.Image image1 = Resize(System.Drawing.Image.FromFile(streams[0].Url), 0.5F, true);
                        System.Drawing.Image image2 = Resize(System.Drawing.Image.FromFile(streams[1].Url), 0.5F, true);
                        pteAnhChupFileDinhKem.Image = System.Drawing.Image.FromFile(MergeImages(image1, image2, fileName, 10));
                        this.fileNameAttack.image = System.Drawing.Image.FromFile(fileName);
                    }

                    this.fileNameAttack.Dem = dem;

                    pteAnhChupFileDinhKem.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
                    this.ListfileNameAttack.Add(this.fileNameAttack);

                    gridView2.BeginUpdate();
                    gridView2.GridControl.DataSource = this.ListfileNameAttack;
                    gridView2.EndUpdate();
                }

                // convert image To Pdf
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private Bitmap RotateBitmap(Bitmap bm, float angle)
        {

            Matrix rotate_at_origin = new Matrix();
            rotate_at_origin.Rotate(angle);


            PointF[] points =
            {
                new PointF(0, 0),
                new PointF(bm.Width, 0),
                new PointF(bm.Width, bm.Height),
                new PointF(0, bm.Height),
            };
            rotate_at_origin.TransformPoints(points);
            float xmin, xmax, ymin, ymax;
            GetPointBounds(points, out xmin, out xmax, out ymin, out ymax);

            int wid = (int)Math.Round(xmax - xmin);
            int hgt = (int)Math.Round(ymax - ymin);
            Bitmap result = new Bitmap(wid, hgt);

            Matrix rotate_at_center = new Matrix();
            rotate_at_center.RotateAt(angle,
                new PointF(wid / 2f, hgt / 2f));

            using (Graphics gr = Graphics.FromImage(result))
            {
                gr.InterpolationMode = InterpolationMode.High;

                gr.Clear(bm.GetPixel(0, 0));
                gr.Transform = rotate_at_center;

                int x = (wid - bm.Width) / 2;
                int y = (hgt - bm.Height) / 2;
                gr.DrawImage(bm, x, y);
            }

            return result;
        }

        private void GetPointBounds(PointF[] points, out float xmin, out float xmax, out float ymin, out float ymax)
        {
            xmin = points[0].X;
            xmax = xmin;
            ymin = points[0].Y;
            ymax = ymin;
            foreach (PointF point in points)
            {
                if (xmin > point.X) xmin = point.X;
                if (xmax < point.X) xmax = point.X;
                if (ymin > point.Y) ymin = point.Y;
                if (ymax < point.Y) ymax = point.Y;
            }
        }

        public System.Drawing.Image Resize(System.Drawing.Image img, float percentage, bool isRotate)
        {
            //lấy kích thước ban đầu của bức ảnh
            int originalW = img.Width;
            int originalH = img.Height;

            //tính kích thước cho ảnh mới theo tỷ lệ đưa vào
            int resizedW = (int)(originalW * percentage);
            int resizedH = (int)(originalH * percentage);

            //tạo 1 ảnh Bitmap mới theo kích thước trên
            Bitmap bmp = new Bitmap(resizedW, resizedH);
            //tạo 1 graphic mới từ Bitmap
            Graphics graphic = Graphics.FromImage((System.Drawing.Image)bmp);
            //vẽ lại ảnh ban đầu lên bmp theo kích thước mới
            graphic.DrawImage(img, 0, 0, resizedW, resizedH);
            //giải phóng tài nguyên mà graphic đang giữ
            graphic.Dispose();

            // đổi lại chiều của ảnh thứ 2 do máy scan quét từ trên xuống
            if (isRotate) bmp = RotateBitmap(bmp, 180);

            //return the image
            return (System.Drawing.Image)bmp;
        }

        private string MergeImages(System.Drawing.Image image1, System.Drawing.Image image2, string urlToSave, int space)
        {

            Bitmap bitmap = new Bitmap(Math.Max(image1.Width, image2.Width), image1.Height + image2.Height + space);
            bitmap.SetResolution(72, 72);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                Bitmap bm1 = (Bitmap)image1;
                Bitmap bm2 = (Bitmap)image2;
                bm1.SetResolution(72, 72); // <-- Set resolution equal to bitmap2
                bm2.SetResolution(72, 72); // <-- Set resolution equal to bitmap2
                g.Clear(Color.White);
                g.DrawImage(bm2, 0, 0);
                g.DrawImage(bm1, 0, image1.Height + space);
            }
            System.Drawing.Image img = bitmap;

            img.Save(urlToSave);
            img.Dispose();
            return urlToSave;
        }

        public static List<string> GetDevices()
        {
            List<string> devices = new List<string>();
            WIA.DeviceManager manager = new WIA.DeviceManager();
            foreach (WIA.DeviceInfo info in manager.DeviceInfos)
            {
                devices.Add(info.DeviceID);
            }
            return devices;
        }
        private static void SetDeviceProperty(ref WIA.Device device, int propertyID, int propertyValue)
        {
            foreach (Property p in device.Properties)
            {
                if (p.PropertyID == propertyID)
                {
                    object value = propertyValue;
                    p.set_Value(ref value);
                    break;
                }
            }
        }

        private static void AdjustScannerSettings(IItem scannnerItem, int scanResolutionDPI, int scanStartLeftPixel, int scanStartTopPixel, int scanWidthPixels, int scanHeightPixels, int brightnessPercents, int contrastPercents, int colorMode)
        {
            const string WIA_SCAN_COLOR_MODE = "6146";
            const string WIA_HORIZONTAL_SCAN_RESOLUTION_DPI = "6147";
            const string WIA_VERTICAL_SCAN_RESOLUTION_DPI = "6148";
            const string WIA_HORIZONTAL_SCAN_START_PIXEL = "6149";
            const string WIA_VERTICAL_SCAN_START_PIXEL = "6150";
            const string WIA_HORIZONTAL_SCAN_SIZE_PIXELS = "6151";
            const string WIA_VERTICAL_SCAN_SIZE_PIXELS = "6152";
            const string WIA_SCAN_BRIGHTNESS_PERCENTS = "6154";
            const string WIA_SCAN_CONTRAST_PERCENTS = "6155";
            try
            {
                SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
                SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
                SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_START_PIXEL, scanStartLeftPixel);
                SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_START_PIXEL, scanStartTopPixel);
                SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_SIZE_PIXELS, scanWidthPixels);
                SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_SIZE_PIXELS, scanHeightPixels);
                SetWIAProperty(scannnerItem.Properties, WIA_SCAN_BRIGHTNESS_PERCENTS, brightnessPercents);
                SetWIAProperty(scannnerItem.Properties, WIA_SCAN_CONTRAST_PERCENTS, contrastPercents);
                SetWIAProperty(scannnerItem.Properties, WIA_SCAN_COLOR_MODE, colorMode);
            }
            catch (Exception ex)
            {
                try
                {

                    Inventec.Common.Logging.LogSystem.Error(String.Format("Gắn lại giá trị theo máy scan: \r\n WIA_HORIZONTAL_SCAN_RESOLUTION_DPI {0} \r\n WIA_VERTICAL_SCAN_RESOLUTION_DPI {1} \r\n WIA_HORIZONTAL_SCAN_SIZE_PIXELS {2}\r\n WIA_VERTICAL_SCAN_SIZE_PIXELS {3}____ {4} ", scannnerItem.Properties[WIA_HORIZONTAL_SCAN_RESOLUTION_DPI].get_Value(), scannnerItem.Properties[WIA_VERTICAL_SCAN_RESOLUTION_DPI].get_Value(), scannnerItem.Properties[WIA_HORIZONTAL_SCAN_SIZE_PIXELS].get_Value(), scannnerItem.Properties[WIA_VERTICAL_SCAN_SIZE_PIXELS].get_Value(), ex));
                    SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_RESOLUTION_DPI, scannnerItem.Properties[WIA_HORIZONTAL_SCAN_RESOLUTION_DPI].get_Value());
                    SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_RESOLUTION_DPI, scannnerItem.Properties[WIA_HORIZONTAL_SCAN_RESOLUTION_DPI].get_Value());
                    SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_START_PIXEL, scanStartLeftPixel);
                    SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_START_PIXEL, scanStartTopPixel);

                    SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_SIZE_PIXELS, (int)(scanWidthPixels * ((int)scannnerItem.Properties[WIA_HORIZONTAL_SCAN_RESOLUTION_DPI].get_Value() / scanResolutionDPI)) + 50);
                    SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_SIZE_PIXELS, (int)(scanHeightPixels * ((int)scannnerItem.Properties[WIA_HORIZONTAL_SCAN_RESOLUTION_DPI].get_Value() / scanResolutionDPI)) + 50);
                    SetWIAProperty(scannnerItem.Properties, WIA_SCAN_BRIGHTNESS_PERCENTS, brightnessPercents);
                    SetWIAProperty(scannnerItem.Properties, WIA_SCAN_CONTRAST_PERCENTS, contrastPercents);
                    SetWIAProperty(scannnerItem.Properties, WIA_SCAN_COLOR_MODE, colorMode);
                }
                catch (Exception exE)
                {
                    Inventec.Common.Logging.LogSystem.Error(exE);
                }

            }
        }

        private static void SetWIAProperty(IProperties properties, object propName, object propValue)
        {
            Property prop = properties.get_Item(ref propName);
            prop.set_Value(ref propValue);
        }

        private void chkPrintDupicate_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (currentControlStateRDO != null && currentControlStateRDO.Count > 0) ? currentControlStateRDO.Where(o => o.KEY == chkPrintDupicate.Name && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintDupicate.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintDupicate.Name;
                    csAddOrUpdate.VALUE = (chkPrintDupicate.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmAttackFile
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__frmAttackFile = new ResourceManager("HIS.Desktop.Plugins.EmrDocument.Resources.Lang", typeof(frmAttackFile).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.toolTipItem2.Text = Inventec.Common.Resource.Get.Value("toolTipItem2.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAttackFile.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.chkPrintDupicate.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAttackFile.chkPrintDupicate.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmAttackFile.bar1.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.bbtnAttackFile.Caption = Inventec.Common.Resource.Get.Value("frmAttackFile.bbtnAttackFile.Caption", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.btnScan.ToolTip = Inventec.Common.Resource.Get.Value("frmAttackFile.btnScan.ToolTip", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.CboDocumentGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAttackFile.CboDocumentGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmAttackFile.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmAttackFile.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmAttackFile.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.cboDocumentType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAttackFile.cboDocumentType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.btnAttackFile.Text = Inventec.Common.Resource.Get.Value("frmAttackFile.btnAttackFile.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.pteAnhChupFileDinhKem.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAttackFile.pteAnhChupFileDinhKem.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.toolTipItem1.Text = Inventec.Common.Resource.Get.Value("toolTipItem1.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.btnCapture.ToolTip = Inventec.Common.Resource.Get.Value("frmAttackFile.btnCapture.ToolTip", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.lciFortxtDocumentName.Text = Inventec.Common.Resource.Get.Value("frmAttackFile.lciFortxtDocumentName.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmAttackFile.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmAttackFile.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmAttackFile.Text", Resources.ResourceLanguageManager.LanguageResource__frmAttackFile, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        System.Drawing.Image Rotate90(System.Drawing.Image image, bool isLeft)
        {
            System.Drawing.Image rotatedImage = null;
            if (image != null)
            {
                rotatedImage = new Bitmap(image);
                rotatedImage.RotateFlip(isLeft ? RotateFlipType.Rotate270FlipNone : RotateFlipType.Rotate90FlipNone);
            }
            return rotatedImage;
        }

        private void btnRotateLeft_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentFileAttack == null || !btnRotateLeft.Enabled) return;
                currentFileAttack.image = Rotate90(currentFileAttack.image, true);
                pteAnhChupFileDinhKem.Image = currentFileAttack.image;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRotateRight_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentFileAttack == null || !btnRotateRight.Enabled) return;
                currentFileAttack.image = Rotate90(currentFileAttack.image, false);
                pteAnhChupFileDinhKem.Image = currentFileAttack.image;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

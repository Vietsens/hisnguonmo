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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.CacheClient;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Inventec.Common.SignLibrary.DTO;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraRichEdit;
using DevExpress.Spreadsheet;
using HIS.Desktop.Plugins.HisTrackingList.ADO;
using HIS.Desktop.Plugins.HisTrackingList.Config;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.Utils.Menu;
using DevExpress.XtraSpreadsheet.UI;

namespace HIS.Desktop.Plugins.HisTrackingList.UcEmrDocument
{
    public partial class UcViewEmrDocument : UserControl
    {
        public bool ShowBar;
        List<DocumentTrackingADO> streamFiles;
        DXPopupMenu menuTemplate = new DXPopupMenu();
        public UcViewEmrDocument()
        {
            InitializeComponent();
            pdfCommandBar1.Visible = ShowBar;
        }
        public UcViewEmrDocument(bool visibleMaximize)
        {
            InitializeComponent();
            bbtnMaximize.Visibility = visibleMaximize ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
        }
        public void SetMenu(DXPopupMenu menuTemplate)
        {
            this.menuTemplate = menuTemplate;
        }

        bool isViewOnly;

        /// <summary>
        /// Read only viewer: keeps page navigation and zoom only, no print/save/open/maximize,
        /// pages are always shown as images and content cannot be selected or copied.
        /// </summary>
        internal void SetViewOnlyMode()
        {
            try
            {
                this.isViewOnly = true;
                pdfCommandBar1.Visible = true;
                foreach (DevExpress.XtraBars.BarItem item in new DevExpress.XtraBars.BarItem[] { pdfFilePrintBarItem1, bbtnMaximize, pdfFileOpenBarItem1, pdfFileSaveAsBarItem1, pdfFindTextBarItem1, pdfExportFormDataBarItem1, pdfImportFormDataBarItem1 })
                {
                    item.Enabled = false;
                    item.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
                this.pdfViewer1.ReadOnly = true;
                this.pdfViewer1.AllowDrop = false;
                this.pdfViewer1.PopupMenuShowing += pdfViewer1_ViewOnly_PopupMenuShowing;
                this.pdfViewer1.SelectionStarted += pdfViewer1_ViewOnly_Selection;
                this.pdfViewer1.SelectionContinued += pdfViewer1_ViewOnly_Selection;
                this.pdfViewer1.KeyDown += pdfViewer1_ViewOnly_KeyDown;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void pdfViewer1_ViewOnly_PopupMenuShowing(object sender, DevExpress.XtraPdfViewer.PdfPopupMenuShowingEventArgs e)
        {
            try
            {
                // Context menu has copy/select all/print commands: not shown in read only mode
                e.ItemLinks.Clear();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void pdfViewer1_ViewOnly_Selection(object sender, DevExpress.XtraPdfViewer.PdfSelectionEventArgs e)
        {
            try
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    try
                    {
                        if (!this.IsDisposed && this.pdfViewer1.HasSelection)
                            this.pdfViewer1.ClearSelection();
                    }
                    catch (Exception exClear)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(exClear);
                    }
                }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void pdfViewer1_ViewOnly_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.Insert || e.KeyCode == Keys.A || e.KeyCode == Keys.X
                    || e.KeyCode == Keys.P || e.KeyCode == Keys.S || e.KeyCode == Keys.O || e.KeyCode == Keys.F))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void UcViewEmrDocument_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //public void ReloadDocument(List<V_EMR_DOCUMENT> listData, bool showBar = false)
        //{
        //    try
        //    {
        //        pdfCommandBar1.Visible = showBar;
        //        List<MemoryStream> documentData = new List<MemoryStream>();
        //        if (listData != null && listData.Count > 0)
        //        {
        //            CommonParam paramCommon = new CommonParam();
        //            EmrVersionFilter verFilter = new EmrVersionFilter();
        //            verFilter.DOCUMENT_IDs = listData.Select(s => s.ID).Distinct().ToList();
        //            var version = new BackendAdapter(paramCommon).Get<List<EMR_VERSION>>("api/EmrVersion/Get", ApiConsumers.EmrConsumer, verFilter, paramCommon);
        //            if (version != null && version.Count > 0)
        //            {
        //                List<EMR_VERSION> groupDoc = version.OrderBy(o => o.DOCUMENT_ID).ThenByDescending(o => o.ID).GroupBy(o => o.DOCUMENT_ID).Select(s => s.First()).ToList();
        //                foreach (var item in groupDoc)
        //                {
        //                    try
        //                    {
        //                        documentData.Add(Inventec.Fss.Client.FileDownload.GetFile(item.URL));
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Inventec.Common.Logging.LogSystem.Error(ex);
        //                    }
        //                }
        //            }
        //        }

        //        ProcessJoinDocument(documentData);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //public void ReloadDocument(MemoryStream streamFile, string extension)
        //{
        //    WaitingManager.Show();
        //    string templateFile = Application.StartupPath + "\\Temp\\Mps000062__Temp__" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".pdf";
        //    string excelFile = Application.StartupPath + "\\Temp\\Mps000062__Temp__" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
        //    try
        //    {
        //        if (streamFile != null && !String.IsNullOrWhiteSpace(extension))
        //        {
        //            if (extension.ToLower().EndsWith("xlsx"))
        //            {
        //                File.WriteAllBytes(excelFile, streamFile.ToArray());
        //                MemoryStream pfdFile = new MemoryStream();
        //                if (ConvertExcelToPdfByFlexCel(excelFile, pfdFile))
        //                {
        //                    //File.WriteAllBytes(templateFile, Inventec.Common.SignLibrary.Utils.StreamToByte(pfdFile));
        //                    this.pdfViewer1.DetachStreamAfterLoadComplete = true;
        //                    this.pdfViewer1.LoadDocument(pfdFile);
        //                }
        //            }
        //            else if (extension.ToLower().EndsWith("docx"))
        //            {
        //                if (DocToPdf(streamFile, templateFile, extension))
        //                {
        //                    this.pdfViewer1.DetachStreamAfterLoadComplete = true;
        //                    this.pdfViewer1.LoadDocument(templateFile);
        //                }
        //            }
        //            else
        //            {
        //                //repx out stream PDF
        //                this.pdfViewer1.DetachStreamAfterLoadComplete = true;
        //                this.pdfViewer1.LoadDocument(streamFile);
        //            }
        //        }
        //        else
        //        {
        //            this.pdfViewer1.CloseDocument();
        //        }

        //        WaitingManager.Hide();
        //    }
        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        this.pdfViewer1.CloseDocument();
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //    finally
        //    {
        //        if (File.Exists(templateFile))
        //        {
        //            try
        //            {
        //                File.Delete(templateFile);
        //            }
        //            catch { }
        //        }
        //        if (File.Exists(excelFile))
        //        {
        //            try
        //            {
        //                File.Delete(excelFile);
        //            }
        //            catch { }
        //        }
        //    }
        //}

        /// <summary>
        /// Shows the documents joined into one file, ordered as described in BuildViewFile.
        /// </summary>
        public void ReloadDocument(List<DocumentTrackingADO> streamFiles, bool showBar = false)
        {
            try
            {
                bbtnMaximize.Enabled = !this.isViewOnly;
                this.streamFiles = streamFiles;
                pdfCommandBar1.Visible = showBar;
                WaitingManager.Show();
                string temFile = BuildViewFile(streamFiles);
                ShowViewFile(temFile);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Converts the documents to PDF and joins them into a temporary PDF file: in the given order in view only mode
        /// or when IsViewTrackingNewestFirst is on, otherwise sorted by tracking time ascending as before.
        /// Does not touch any control so it can run on a background thread. Returns null when there is nothing to show.
        /// </summary>
        internal string BuildViewFile(List<DocumentTrackingADO> streamFiles)
        {
            string result = null;
            try
            {
                if (streamFiles == null || streamFiles.Count == 0)
                    return null;
                List<DocumentTrackingADO> documentData = ConvertToPdfDocuments(streamFiles);
                if (!this.isViewOnly && !HisConfigCFG.IsViewTrackingNewestFirst)
                    documentData = documentData.OrderBy(p => p.TRACKING_TIME).ToList();
                result = JoinDocuments(documentData, this.isViewOnly);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Loads the file built by BuildViewFile into the viewer (UI thread), then deletes the file.
        /// </summary>
        internal void ShowViewFile(string temFile)
        {
            try
            {
                if (!String.IsNullOrEmpty(temFile) && File.Exists(temFile))
                {
                    this.pdfViewer1.DetachStreamAfterLoadComplete = true;
                    this.pdfViewer1.LoadDocument(temFile);
                    try
                    {
                        System.IO.File.Delete(temFile);
                    }
                    catch (Exception exDelete)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(exDelete);
                    }
                }
                else
                {
                    this.pdfViewer1.CloseDocument();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<DocumentTrackingADO> ConvertToPdfDocuments(List<DocumentTrackingADO> streamFiles)
        {
            List<DocumentTrackingADO> documentData = new List<DocumentTrackingADO>();
            try
            {
                foreach (var item in streamFiles)
                {
                    if (String.IsNullOrEmpty(item.FullTemplateFileName) || item.FullTemplateFileName.ToLower().EndsWith(".pdf") || item.FullTemplateFileName.ToLower().EndsWith(".repx"))
                    {
                        //Inventec.Common.Logging.LogSystem.Debug("ReloadDocument.2");
                        documentData.Add(item);
//Inventec.Common.Logging.LogSystem.Debug("ReloadDocument.3");
                    }
                    else
                    {
                        try
                        {
                            //Inventec.Common.Logging.LogSystem.Debug("ReloadDocument.4");
                            if (item.DocumentFile != null && !String.IsNullOrEmpty(item.FullTemplateFileName))
                            {
                                MemoryStream pfdFile = new MemoryStream();
                                if (item.FullTemplateFileName.ToLower().EndsWith(".xlsx"))
                                {
                                    //Inventec.Common.Logging.LogSystem.Debug("ReloadDocument.5");
                                    if (Inventec.Common.FileConvert.Convert.ExcelToPdfUsingFlex(item.DocumentFile, "", pfdFile, ""))
                                    {
                                        //Inventec.Common.Logging.LogSystem.Debug("ReloadDocument.6");
                                        pfdFile.Position = 0;
                                        documentData.Add(new DocumentTrackingADO()
                                        {
                                            DocumentFile = pfdFile,
                                            FullTemplateFileName = item.FullTemplateFileName,
                                            TRACKING_TIME = item.TRACKING_TIME
                                        });

                                        //Inventec.Common.Logging.LogSystem.Debug("ReloadDocument.7");
                                        //File.WriteAllBytes(templateFile, Inventec.Common.SignLibrary.Utils.StreamToByte(pfdFile));
                                        //this.pdfViewer1.DetachStreamAfterLoadComplete = true;
                                        //this.pdfViewer1.LoadDocument(pfdFile);
                                    }
                                }
                                else if (item.FullTemplateFileName.ToLower().EndsWith(".doc") || item.FullTemplateFileName.ToLower().EndsWith(".docx"))
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("ReloadDocument.8");
                                    Inventec.Common.Logging.LogSystem.Debug("item.saveFilePath: " + item.saveFilePath);
                                    if (Inventec.Common.FileConvert.Convert.DocToPdf(item.DocumentFile, item.saveFilePath, pfdFile, ""))
                                    {
                                        Inventec.Common.Logging.LogSystem.Debug("ReloadDocument.9");
                                        pfdFile.Position = 0;
                                        documentData.Add(new DocumentTrackingADO()
                                        {
                                            DocumentFile = pfdFile,
                                            FullTemplateFileName = item.FullTemplateFileName,
                                            TRACKING_TIME = item.TRACKING_TIME
                                        });
                                        Inventec.Common.Logging.LogSystem.Debug("ReloadDocument.10");
                                    }
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("ReloadDocument.11");
                                    if (Inventec.Common.FileConvert.Convert.DocToPdf(item.DocumentFile, "", pfdFile, ""))
                                    {
                                        Inventec.Common.Logging.LogSystem.Debug("ReloadDocument.12");
                                        pfdFile.Position = 0;
                                        documentData.Add(new DocumentTrackingADO()
                                        {
                                            DocumentFile = pfdFile,
                                            FullTemplateFileName = item.FullTemplateFileName,
                                            TRACKING_TIME = item.TRACKING_TIME
                                        });
                                        Inventec.Common.Logging.LogSystem.Debug("ReloadDocument.13");
                                    }
                                }
                            }
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
            return documentData;
        }

        /// <summary>
        /// Joins the PDF documents into one temporary PDF file. When forceImage is false and there is only one document,
        /// the document is used as is; otherwise every page is converted into an image.
        /// </summary>
        private string JoinDocuments(List<DocumentTrackingADO> streamFiles, bool forceImage)
        {
            string result = null;
            try
            {
                if (streamFiles != null && streamFiles.Count > 0)
                {
                    string temFile = System.IO.Path.GetTempFileName();
                    temFile = temFile.Replace(".tmp", ".pdf");
                    if (streamFiles.Count == 1 && !forceImage)
                    {
                        //Inventec.Common.Logging.LogSystem.Debug("ProcessJoinDocument.3");
                        using (var fileStream = System.IO.File.Create(temFile))
                        {
                            streamFiles.First().DocumentFile.Position = 0;
                            streamFiles.First().DocumentFile.CopyTo(fileStream);
                            fileStream.Position = 0;
                        }
                        //Inventec.Common.Logging.LogSystem.Debug("ProcessJoinDocument.4");
                    }
                    else
                    {
                        ///Inventec.Common.Logging.LogSystem.Debug("ProcessJoinDocument.5");
                        //Nối văn bản
                        float oginalHeight = 0;
                        using (MemoryStream str = new MemoryStream())
                        {
                            streamFiles.First().DocumentFile.Position = 0;
                            streamFiles.First().DocumentFile.CopyTo(str);
                            str.Position = 0;
                            oginalHeight = GetOginalHeight(str);
                        }
                        //Inventec.Common.Logging.LogSystem.Debug("ProcessJoinDocument.6");
                        iTextSharp.text.pdf.PdfReader readerWorking = null;

                        using (MemoryStream str = new MemoryStream())
                        {
                            streamFiles.First().DocumentFile.Position = 0;
                            streamFiles.First().DocumentFile.CopyTo(str);
                            str.Position = 0;
                            readerWorking = new iTextSharp.text.pdf.PdfReader(str);
                        }
                        //Inventec.Common.Logging.LogSystem.Debug("ProcessJoinDocument.7");
                        if (readerWorking == null)
                            return null;

                        int pageCount = readerWorking.NumberOfPages;
                        iTextSharp.text.Rectangle pageSize = readerWorking.GetPageSizeWithRotation(readerWorking.NumberOfPages);
                        iTextSharp.text.Rectangle pageSize1 = new iTextSharp.text.Rectangle(pageSize.Left, pageSize.Bottom, pageSize.Right, (pageSize.Bottom + oginalHeight), pageSize.Rotation);
                        pageSize1.BorderColor = pageSize.BorderColor;
                        pageSize1.BackgroundColor = pageSize.BackgroundColor;
                        pageSize1.Rotation = pageSize.Rotation;
                        pageSize1.Border = pageSize.Border;
                        pageSize1.BorderWidth = pageSize.BorderWidth;
                        pageSize1.BorderColor = pageSize.BorderColor;
                        pageSize1.BackgroundColor = pageSize.BackgroundColor;
                        pageSize1.BorderColorLeft = pageSize.BorderColorLeft;
                        pageSize1.BorderColorRight = pageSize.BorderColorRight;
                        pageSize1.BorderColorTop = pageSize.BorderColorTop;
                        pageSize1.BorderColorBottom = pageSize.BorderColorBottom;
                        pageSize1.BorderWidthLeft = pageSize.BorderWidthLeft;
                        pageSize1.BorderWidthRight = pageSize.BorderWidthRight;
                        pageSize1.BorderWidthTop = pageSize.BorderWidthTop;
                        pageSize1.BorderWidthBottom = pageSize.BorderWidthBottom;
                        pageSize1.UseVariableBorders = pageSize.UseVariableBorders;
                        //Inventec.Common.Logging.LogSystem.Debug("ProcessJoinDocument.8");
                        PdfReader readerWorkingSource = GetTempReader(pageSize1);
                        using (FileStream fs_ = File.Open(temFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            //Inventec.Common.Logging.LogSystem.Debug("ProcessJoinDocument.9");
                            using (PdfStamper stam = new PdfStamper(readerWorkingSource, fs_))
                            {
                                List<ImageOfPageDTO> imageFiles = new List<ImageOfPageDTO>();

                                foreach (var item in streamFiles)
                                {
                                    item.DocumentFile.Position = 0;
                                    imageFiles.AddRange(ConvertPdfPageToListImage(item.DocumentFile));
                                }

                                int pageIndex = 1;
                                float currentPageHeight = 0;

                                foreach (var item in imageFiles)
                                {
                                    float pageXPosition = 0, pageYPosition = 0;
                                    if (currentPageHeight + item.Height > oginalHeight)
                                    {
                                        pageIndex += 1;
                                        stam.InsertPage(
                                            pageIndex,
                                            pageSize1
                                        );
                                        currentPageHeight = item.Height;
                                        pageXPosition = 0;
                                        pageYPosition = oginalHeight - currentPageHeight;
                                    }
                                    else
                                    {
                                        currentPageHeight += item.Height;
                                        pageXPosition = 0;
                                        pageYPosition = oginalHeight - currentPageHeight;
                                    }

                                    float percentage = 0.0f;
                                    //Inventec.Common.Logging.LogSystem.Debug("PageNUm=" + item.PageNumber + "__Path=" + item.Path);
                                    iTextSharp.text.Image img = !String.IsNullOrEmpty(item.Path) ? iTextSharp.text.Image.GetInstance(item.Path) : iTextSharp.text.Image.GetInstance(item.ImageContent);
                                    img.SetAbsolutePosition(pageXPosition, pageYPosition);

                                    percentage = pageSize.Width / img.Width;
                                    img.ScalePercent(percentage * 100);

                                    stam.GetOverContent(pageIndex).AddImage(img);
                                }
                            }
                        }
                    }
                    result = temFile;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private float GetOginalHeight(MemoryStream memoryStream)
        {
            using (Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(memoryStream))
            {
                Aspose.Pdf.Page page = pdfDocument.Pages[1];

                return (float)page.GetPageRect(false).Height;
            }
        }

        private static List<ImageOfPageDTO> ConvertPdfPageToListImage(Stream output_file)
        {
            List<ImageOfPageDTO> imageOfPages = new List<ImageOfPageDTO>();
            try
            {
                //Inventec.Common.Logging.LogSystem.Debug("JoinAllPdfPageToOne.1");
                LicenceProcess.SetLicenseForAspose();

                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(output_file);
                //Inventec.Common.Logging.LogSystem.Debug("JoinAllPdfPageToOne.2");
                for (int pageCount = 1; pageCount <= pdfDocument.Pages.Count; pageCount++)
                {
                    //Inventec.Common.Logging.LogSystem.Debug("JoinAllPdfPageToOne.3." + pageCount + ".1");
                    using (MemoryStream imageStream = new MemoryStream())
                    {
                        // Create JPEG device with specified attributes
                        // Width, Height, Resolution, Quality
                        // Quality [0-100], 100 is Maximum
                        // Create Resolution object
                        Aspose.Pdf.Devices.Resolution resolution = new Aspose.Pdf.Devices.Resolution(300);

                        // JpegDevice jpegDevice = new JpegDevice(500, 700, resolution, 100);
                        Aspose.Pdf.Devices.JpegDevice jpegDevice = new Aspose.Pdf.Devices.JpegDevice(resolution, 100);

                        // Convert a particular page and save the image to stream
                        jpegDevice.Process(pdfDocument.Pages[pageCount], imageStream);

                        imageOfPages.Add(new ImageOfPageDTO()
                        {
                            //Path = output_filePath,
                            ImageContent = imageStream.ToArray(),
                            PageNumber = pdfDocument.Pages[pageCount].Number,
                            Width = (float)pdfDocument.Pages[pageCount].Rect.Width,
                            Height = (float)pdfDocument.Pages[pageCount].Rect.Height,
                        });
                        //Inventec.Common.Logging.LogSystem.Debug("JoinAllPdfPageToOne.3." + pageCount + ".2");
                    }
                    //Inventec.Common.Logging.LogSystem.Debug("JoinAllPdfPageToOne.4 => pageCount = " + pageCount + ", imageOfPages.Count=" + imageOfPages.Count);
                }
            }
            catch (Exception ex)
            {
                imageOfPages = new List<ImageOfPageDTO>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return imageOfPages;
        }

        private static PdfReader GetTempReader(iTextSharp.text.Rectangle pageSize = null)
        {
            var stream = new MemoryStream();
            {
                if (pageSize != null)
                {
                    using (Document document = new Document(pageSize))
                    {
                        PdfWriter.GetInstance(document, stream);
                        document.Open();
                        document.Add(new Phrase("123"));
                    }
                }
                else
                {
                    using (Document document = new Document())
                    {
                        PdfWriter.GetInstance(document, stream);
                        document.Open();
                        document.Add(new Phrase("123"));
                    }
                }
                return new PdfReader(stream.ToArray());
            }
        }

        private void bbtnResetChkState_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //CacheClientWorker.ChangeValue("");
                //MessageBox.Show(ResourceMessage.ResetTrangThaiNguoiDungDaLuuTaiMayTram);

                //GlobalStore.EmrConfigs = null;
                //GlobalStore.EmrBusiness = null;
                //var rsCfg = GlobalStore.EmrConfigs;
                //var rsBus = GlobalStore.EmrBusiness;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ClearDocument()
        {
            try
            {
                bbtnMaximize.Enabled = false;
                this.pdfViewer1.CloseDocument();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ExportExcelToPdfUsingApose(MemoryStream sourceFile, string pdfFile)
        {
            try
            {
                // If either required string is null or empty, stop and bail out
                if (sourceFile == null || sourceFile.Length == 0 || string.IsNullOrEmpty(pdfFile))
                {
                    return false;
                }

                string fulldir = Path.GetDirectoryName(pdfFile);
                if (!Directory.Exists(fulldir))
                {
                    Directory.CreateDirectory(fulldir);
                }

                LicenceProcess.SetLicenseForAsposeCell();

                // Open the template excel file
                Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook(sourceFile);

                // Save the pdf file.
                wb.Save(pdfFile, Aspose.Cells.SaveFormat.Pdf);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }

            return true;
        }

        private bool ConvertExcelToPdfByFlexCel(string excelPath, MemoryStream pdfStream)
        {
            bool result = false;
            try
            {
                //Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.1");
                FlexCel.Render.FlexCelPdfExport flexCelPdfExport1 = new FlexCel.Render.FlexCelPdfExport();
                flexCelPdfExport1.FontEmbed = FlexCel.Pdf.TFontEmbed.Embed;
                flexCelPdfExport1.PageLayout = FlexCel.Pdf.TPageLayout.None;
                flexCelPdfExport1.PageSize = null;
                FlexCel.Pdf.TPdfProperties tPdfProperties1 = new FlexCel.Pdf.TPdfProperties();
                tPdfProperties1.Author = null;
                tPdfProperties1.Creator = null;
                tPdfProperties1.Keywords = null;
                tPdfProperties1.Subject = null;
                tPdfProperties1.Title = null;
                flexCelPdfExport1.Properties = tPdfProperties1;
                flexCelPdfExport1.Workbook = new FlexCel.XlsAdapter.XlsFile();
                flexCelPdfExport1.Workbook.Open(excelPath);

                //Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.2");
                int SaveSheet = flexCelPdfExport1.Workbook.ActiveSheet;
                try
                {
                    flexCelPdfExport1.BeginExport(pdfStream);

                    flexCelPdfExport1.PageLayout = FlexCel.Pdf.TPageLayout.None;
                    flexCelPdfExport1.ExportSheet();

                    flexCelPdfExport1.EndExport();
                }
                finally
                {
                    flexCelPdfExport1.Workbook.ActiveSheet = SaveSheet;
                }
                pdfStream.Position = 0;
                result = true;
                //Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.3");
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        private bool DocToPdf(MemoryStream inputStream, string outputFile, string extension = "")
        {
            bool success = false;
            try
            {
                if (inputStream == null || inputStream.Length == 0)
                    throw new ArgumentNullException("inStream & inFile is null");

                string ext = "";
                RichEditDocumentServer server = new RichEditDocumentServer();
                inputStream.Position = 0;
                ext = extension;
                if (ext == ".doc")
                {
                    server.LoadDocument(inputStream, DevExpress.XtraRichEdit.DocumentFormat.Doc);
                }
                else if (ext == ".html")
                {
                    server.LoadDocument(inputStream, DevExpress.XtraRichEdit.DocumentFormat.Html);
                }
                else if (ext == ".mht")
                {
                    server.LoadDocument(inputStream, DevExpress.XtraRichEdit.DocumentFormat.Mht);
                }
                else if (ext == ".txt")
                {
                    server.LoadDocument(inputStream, DevExpress.XtraRichEdit.DocumentFormat.PlainText);
                }
                else if (ext == ".rtf")
                {
                    server.LoadDocument(inputStream, DevExpress.XtraRichEdit.DocumentFormat.Rtf);
                }
                else
                {
                    server.LoadDocument(inputStream, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                }

                if (!String.IsNullOrEmpty(outputFile))
                {
                    DevExpress.XtraPrinting.PdfExportOptions options = new DevExpress.XtraPrinting.PdfExportOptions();
                    options.Compressed = false;
                    options.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                    using (FileStream pdfFileStream = new FileStream(outputFile, FileMode.Create))
                    {
                        server.ExportToPdf(pdfFileStream, options);
                    }
                }

                //server.Dispose();
                success = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UcViewEmrDocument
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__UcViewEmrDocument = new ResourceManager("HIS.Desktop.Plugins.HisTrackingList.Resources.Lang", typeof(UcViewEmrDocument).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UcViewEmrDocument.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__UcViewEmrDocument, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public frmEmrDocument frmEmr { get; set; }
        private void bbtnMaximize_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmEmr = new frmEmrDocument(streamFiles, menuTemplate);
            frmEmr.ShowDialog();
        }
    }
}

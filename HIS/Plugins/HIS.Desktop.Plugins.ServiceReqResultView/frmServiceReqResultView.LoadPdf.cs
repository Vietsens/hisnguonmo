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
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ServiceReqResultView.ADO;
using HIS.Desktop.Plugins.ServiceReqResultView.Helpers;
using Inventec.Common.Adapter;
using Inventec.Common.SignLibrary.DTO;
using Inventec.Core;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceReqResultView
{
    public partial class frmServiceReqResultView
    {
        private bool isShowEmrDocument = false;
        private List<V_EMR_DOCUMENT> listEmrDocument;
        private List<MemoryStream> documentData = new List<MemoryStream>();

        private void KiemTraThongTinPhieuKetQuaDienTu()
        {
            try
            {
                if (this.sereServ == null)
                    return;
                CommonParam paramCommon = new CommonParam();
                EmrDocumentViewFilter filter = new EmrDocumentViewFilter();
                filter.TREATMENT_CODE__EXACT = this.sereServ.TDL_TREATMENT_CODE;
                filter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;
                filter.IS_DELETE = false;
                var apiResult = new BackendAdapter(paramCommon).Get<List<V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, filter, paramCommon);
                if (apiResult != null && apiResult.Count > 0 )
                {
                    if (String.IsNullOrWhiteSpace(this.sereServ.TDL_SERVICE_REQ_CODE ))
                        return;
                    string hisCode = "SERVICE_REQ_CODE:" + this.sereServ.TDL_SERVICE_REQ_CODE;
                    string id = "SER_SERV_ID:" + this.sereServ.ID;
                    listEmrDocument = apiResult.Where(o => o.HIS_CODE != null && ((o.HIS_CODE.Contains(hisCode) && o.HIS_CODE.Contains(id)) || (o.HIS_CODE == this.sereServ.ID.ToString()))).ToList();
                    if (listEmrDocument != null && listEmrDocument.Count > 0)
                    {
                        ReloadDocument(listEmrDocument);
                        this.isShowEmrDocument = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ReloadDocument(List<V_EMR_DOCUMENT> listData)
        {
            try
            {
                this.documentData = new List<MemoryStream>();
                if (listData != null && listData.Count > 0)
                {
                    CommonParam paramCommon = new CommonParam();
                    EmrVersionFilter verFilter = new EmrVersionFilter();
                    verFilter.DOCUMENT_IDs = listData.Select(s => s.ID).Distinct().ToList();
                    var version = new BackendAdapter(paramCommon).Get<List<EMR_VERSION>>("api/EmrVersion/Get", ApiConsumers.EmrConsumer, verFilter, paramCommon);
                    if (version != null && version.Count > 0)
                    {
                        List<EMR_VERSION> groupDoc = version.OrderBy(o => o.DOCUMENT_ID).ThenByDescending(o => o.ID).GroupBy(o => o.DOCUMENT_ID).Select(s => s.First()).ToList();

                        List<EmrVersionADO> listDoc = new List<EmrVersionADO>();
                        foreach (var item in groupDoc)
                        {
                            EmrVersionADO addData = new EmrVersionADO(item);
                            var document = listData.FirstOrDefault(o => o.ID == item.DOCUMENT_ID);
                            addData.DOCUMENT_CODE_ForOrder = document != null ? document.DOCUMENT_CODE : "";
                            listDoc.Add(addData);
                        }
                        listDoc = listDoc.OrderBy(o => o.DOCUMENT_CODE_ForOrder).ToList();

                        foreach (var item in listDoc)
                        {
                            try
                            {
                                this.documentData.Add(Inventec.Fss.Client.FileDownload.GetFile(item.URL));
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessJoinDocument(List<MemoryStream> documentData)
        {
            try
            {
                //pdfCommandBar1.Visible = ShowBar;
                if (documentData != null && documentData.Count > 0)
                {
                    string temFile = System.IO.Path.GetTempFileName();
                    temFile = temFile.Replace(".tmp", ".pdf");
                    if (documentData.Count == 1)
                    {
                        using (var fileStream = System.IO.File.Create(temFile))
                        {
                            documentData.First().CopyTo(fileStream);
                        }
                    }
                    else
                    {
                        //Nối văn bản
                        float oginalHeight = 0;
                        using (MemoryStream str = new MemoryStream())
                        {
                            documentData.First().Position = 0;
                            documentData.First().CopyTo(str);
                            str.Position = 0;
                            oginalHeight = GetOginalHeight(str);
                        }

                        iTextSharp.text.pdf.PdfReader readerWorking = null;

                        using (MemoryStream str = new MemoryStream())
                        {
                            documentData.First().Position = 0;
                            documentData.First().CopyTo(str);
                            str.Position = 0;
                            readerWorking = new iTextSharp.text.pdf.PdfReader(str);
                        }

                        if (readerWorking == null)
                            return;

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

                        PdfReader readerWorkingSource = GetTempReader(pageSize1);
                        using (FileStream fs_ = File.Open(temFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            using (PdfStamper stam = new PdfStamper(readerWorkingSource, fs_))
                            {
                                List<ImageOfPageDTO> imageFiles = new List<ImageOfPageDTO>();

                                foreach (var item in documentData)
                                {
                                    imageFiles.AddRange(ConvertPdfPageToListImage(item));
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
                                    Inventec.Common.Logging.LogSystem.Debug("PageNUm=" + item.PageNumber + "__Path=" + item.Path);
                                    iTextSharp.text.Image img = !String.IsNullOrEmpty(item.Path) ? iTextSharp.text.Image.GetInstance(item.Path) : iTextSharp.text.Image.GetInstance(item.ImageContent);
                                    img.SetAbsolutePosition(pageXPosition, pageYPosition);

                                    percentage = pageSize.Width / img.Width;
                                    img.ScalePercent(percentage * 100);

                                    stam.GetOverContent(pageIndex).AddImage(img);
                                }
                            }
                        }
                    }

                    this.pdfViewer1.DetachStreamAfterLoadComplete = true;
                    this.pdfViewer1.LoadDocument(temFile);
                    try
                    {
                        System.IO.File.Delete(temFile);
                    }
                    catch (Exception) { }
                }
                else
                {
                    this.pdfViewer1.CloseDocument();// = new DevExpress.XtraPdfViewer.PdfViewer();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                Inventec.Common.Logging.LogSystem.Debug("JoinAllPdfPageToOne.1");
                LicenceProcess.SetLicenseForAspose();

                Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(output_file);
                Inventec.Common.Logging.LogSystem.Debug("JoinAllPdfPageToOne.2");
                for (int pageCount = 1; pageCount <= pdfDocument.Pages.Count; pageCount++)
                {
                    Inventec.Common.Logging.LogSystem.Debug("JoinAllPdfPageToOne.3." + pageCount + ".1");
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
                        Inventec.Common.Logging.LogSystem.Debug("JoinAllPdfPageToOne.3." + pageCount + ".2");
                    }
                    Inventec.Common.Logging.LogSystem.Debug("JoinAllPdfPageToOne.4 => pageCount = " + pageCount + ", imageOfPages.Count=" + imageOfPages.Count);
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

    }
}

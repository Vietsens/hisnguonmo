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
using FlexCel.Report;
using Inventec.Common.Logging;
using MRS.MANAGER.Core.MrsReport.RDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.FlexCellExport
{
    public class Store
    {
        public string TemplatePath { get; set; }
        private string SavePath { get; set; }
        public FlexCelReport flexCel;
        public MemoryStream TemplateStream { get; set; }
        private MemoryStream SaveStream { get; set; }
        const string TFlexCelUFRowNumberName = "FlFuncRowNumber";
        const string TFlexCelUFSubStringName = "FlFuncSubString";
        const string TFlexCelUFSpeechNumberToStringName = "FlFuncSpeechNumberToString";
        const string TFlexCelUFCalculateAgeName = "FlFuncCalculateAge";
        const string TFlexCelUFDecToFracName = "FlFuncDecToFrac";
        const string TFlexCelUFTimeNumberToDateStringName = "FlFuncTimeNumberToDateString";
        const string TFlexCelUFTimeNumberToTimeStringName = "FlFuncTimeNumberToTimeString";
        const string TFlexCelUFTimeNumberToDateStringSeparateStringName = "FlFuncTimeNumberToDateStringSeparateString";
        const string TFlexCelUFConvertNumberToString = "FlexCelUFConvertNumberToString";
        const string TFlexCelUFConvertNumberToString1 = "FlFuncConvertNumberToString";
        const string TFlexCelUFNumberToString = "FlFuncNumberToString";
        const string TFlexCelUFCreateQR = "FlFuncCreateQR";
        const string TFlexCelUFCreateBarcode = "FlFuncCreateBarcode";
        const string TFlexCelUFFormatIcd = "FlFuncFormatIcd";
        public Dictionary<string, object> DictionaryTemplateKey { get; set; }
        public bool IsUseCommentKey = true;//TODO
        private MemoryStream ResultStream { get; set; }
        private System.Drawing.Printing.PaperSize PaperSizeDefault { get; set; }

        public Store()
        {
            flexCel = new FlexCelReport();
        }

        //Determines if FlexCel will automatically delete existing files or not
        public Store(bool aAllowOverwritingFiles)
        {
            flexCel = new FlexCelReport(aAllowOverwritingFiles);
        }

        public bool ReadTemplate(string path)
        {
            bool result = false;
            try
            {
                TemplatePath = path;

                byte[] byteArray = File.ReadAllBytes(TemplatePath);
                if (byteArray.Length > 0)
                {
                    TemplateStream = new MemoryStream();
                    TemplateStream.Write(byteArray, 0, (int)byteArray.Length);
                    TemplateStream.Position = 0;
                    result = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
                flexCel = null;
                TemplatePath = "";
            }
            return result;
        }

        public bool ReadTemplate(MemoryStream inputStream)
        {
            bool result = false;
            try
            {
                TemplateStream = new MemoryStream();
                inputStream.CopyTo(TemplateStream);
                TemplateStream.Position = 0;
                result = (TemplateStream != null && TemplateStream.Length > 0);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
                flexCel = null;
                TemplatePath = "";
            }
            return result;
        }

        public bool SetCommonFunctions()
        {
            bool result = false;
            try
            {
                if (flexCel == null) throw new ArgumentNullException("flexCel");

                flexCel.SetUserFunction(TFlexCelUFRowNumberName, new TFlexCelUFRowNumber());
                flexCel.SetUserFunction(TFlexCelUFSubStringName, new TFlexCelUFSubString());
                flexCel.SetUserFunction(TFlexCelUFSpeechNumberToStringName, new TFlexCelUFSpeechNumberToString());
                flexCel.SetUserFunction(TFlexCelUFCalculateAgeName, new TFlexCelUFCalculateAge());
                flexCel.SetUserFunction(TFlexCelUFDecToFracName, new TFlexCelUFDecToFrac());
                flexCel.SetUserFunction(TFlexCelUFTimeNumberToDateStringName, new TFlexCelUFTimeNumberToDateString());
                flexCel.SetUserFunction(TFlexCelUFTimeNumberToTimeStringName, new TFlexCelUFTimeNumberToTimeString());
                flexCel.SetUserFunction(TFlexCelUFTimeNumberToDateStringSeparateStringName, new TFlexCelUFTimeNumberToDateStringSeparateString());
                flexCel.SetUserFunction(TFlexCelUFConvertNumberToString, new TFlexCelUFConvertNumberToString());
                flexCel.SetUserFunction(TFlexCelUFConvertNumberToString1, new TFlexCelUFConvertNumberToString());
                flexCel.SetUserFunction(TFlexCelUFNumberToString, new TFlexCelUFNumberToString());
                flexCel.SetUserFunction(TFlexCelUFCreateQR, new TFlexCelUFCreateQR());
                flexCel.SetUserFunction(TFlexCelUFCreateBarcode, new TFlexCelUFCreateBarcode());
                flexCel.SetUserFunction(TFlexCelUFFormatIcd, new TFlexCelUFFormatIcd());
                result = true;
            }
            catch (ArgumentNullException ex)
            {
                LogSystem.Warn(ex);
                result = false;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public void SetIsComment(bool isUseCommentKey)
        {
            this.IsUseCommentKey = isUseCommentKey;
        }

        void ProcessComment(FlexCel.XlsAdapter.XlsFile xls)
        {
            try
            {
                string SINGLE_KEY__COMMENT_SIGN_FIRSTKEY = "<SINGLE_KEY__COMMENT_SIGN";
                string SINGLE_KEY__COMMENT_SIGN_CLOSE_TAG = ">";
                //<SINGLE_KEY__COMMENT_SIGN__$3__u:phuongdt|d:4|p:5|f:3|w:50|h:30>
                {
                    if (this.IsUseCommentKey)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Store.ProcessComment. 1");
                        for (int col = 1; col <= xls.ColCount; col++)
                        {
                            for (int row = 1; row <= xls.RowCount; row++)
                            {
                                try
                                {
                                    var dataCell = xls.GetCellValue(row, col);
                                    string cellValue = (dataCell != null ? dataCell : "").ToString();
                                    if (!System.String.IsNullOrEmpty(cellValue) && cellValue.Contains(SINGLE_KEY__COMMENT_SIGN_FIRSTKEY))
                                    {
                                        Inventec.Common.Logging.LogSystem.Debug(
                                        Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => col), col)
                                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => row), row)
                                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => cellValue), cellValue));

                                        int comOpenTagIndex = cellValue.IndexOf(SINGLE_KEY__COMMENT_SIGN_FIRSTKEY);
                                        string fullCellValue = cellValue.Substring(comOpenTagIndex);
                                        int comCloseTagIndex = fullCellValue.IndexOf(SINGLE_KEY__COMMENT_SIGN_CLOSE_TAG);

                                        string fullcommentTagValue = fullCellValue.Substring(0, comCloseTagIndex + 1);
                                        int fullcommentTagValueIndexOfS = fullcommentTagValue.IndexOf("$") - 1;
                                        string commentValue = fullcommentTagValue.Substring(fullcommentTagValueIndexOfS + 1, fullcommentTagValue.Length - 2 - fullcommentTagValueIndexOfS);

                                        cellValue = cellValue.Replace(fullcommentTagValue, "");
                                        if (!System.String.IsNullOrEmpty(commentValue))
                                        {
                                            xls.SetComment(row, col, commentValue);
                                        }

                                        xls.SetCellValue(row, col, new FlexCel.Core.TRichString(cellValue));

                                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => cellValue), cellValue)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => cellValue), cellValue)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fullCellValue), fullCellValue)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => comCloseTagIndex), comCloseTagIndex)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fullcommentTagValue), fullcommentTagValue)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fullcommentTagValueIndexOfS), fullcommentTagValueIndexOfS)
                                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => commentValue), commentValue));

                                    }
                                }
                                catch (Exception exx)
                                {
                                    LogSystem.Warn(exx);
                                }
                            }
                        }
                        Inventec.Common.Logging.LogSystem.Debug("Store.ProcessComment. 2");
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void GetRowColIndex(FlexCel.XlsAdapter.XlsFile xls)
        {
            try
            {
                //int row = 0, col = 0;
                //GetRowColIndex(xls, item.Key, ref row, ref col);
                //if (row > 0 && col > 0 && !System.String.IsNullOrEmpty(item.Value))
                //{
                //    xls.SetComment(row, col, item.Value);
                //    xls.SetCellValue(row, col, "");
                //}


                //for (int i = 1; i <= xls.ColCount; i++)
                //{
                //    for (int j = 1; j <= xls.RowCount; j++)
                //    {
                //        if ((string)xls.GetCellValue(j, i) == key)
                //        {
                //            row = j;
                //            col = i;
                //            return;
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public System.Drawing.Printing.PaperSize GetPaperSizeDefault()
        {
            return PaperSizeDefault;
        }

        public bool OutFile(string path)
        {
            bool result = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("OutFile. 1");
                if (flexCel != null && !System.String.IsNullOrWhiteSpace(path))
                {
                    SavePath = path;
                    flexCel.Run(TemplatePath, SavePath);

                    FlexCel.XlsAdapter.XlsFile xls = new FlexCel.XlsAdapter.XlsFile(true);
                    xls.Open(SavePath);
                    xls.ConvertFormulasToValues(false);
                    this.ProcessComment(xls);
                    this.ProcessImageHeaderFooter(xls);
                    if (DictionaryTemplateKey != null)
                    {
                        int SheetCount = 0;
                        int activesheet = xls.ActiveSheet;
                        // them 1 sheet an de chua cac key//kiểm tra đã có sheet nào có tên Template_Key
                        for (int i = 1; i <= xls.SheetCount; i++)
                        {
                            if (xls.GetSheetName(i) == "Template_Key")
                            {
                                SheetCount = i;
                                break;
                            }
                        }

                        if (SheetCount == 0)
                        {
                            xls.AddSheet();
                            SheetCount = xls.SheetCount;
                        }

                        var count = DictionaryTemplateKey.Count;
                        List<string> keys = DictionaryTemplateKey.Keys.ToList();
                        List<object> values = DictionaryTemplateKey.Values.ToList();
                        xls.SetCellValue(SheetCount, 1, 2, count, -1);
                        for (int i = 1; i < count + 1; i++)
                        {
                            xls.SetCellValue(SheetCount, i + 1, 1, keys[i - 1], -1);
                            xls.SetCellValue(SheetCount, i + 1, 2, values[i - 1], -1);
                        }
                        xls.ActiveSheet = SheetCount;
                        xls.SheetName = "Template_Key";

                        xls.SheetVisible = FlexCel.Core.TXlsSheetVisible.Hidden;
                        xls.ActiveSheet = activesheet;
                        xls.SheetVisible = FlexCel.Core.TXlsSheetVisible.Visible;
                    }
                    LoadPaperSizes(xls);
                    xls.Save(SavePath);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => SavePath), SavePath));
                    using (FileStream checkStream = System.IO.File.OpenRead(SavePath))
                    {
                        if (checkStream != null && checkStream.Length > 0)
                        {
                            result = true;
                        }
                        else
                        {
                            LogSystem.Error("Su dung FileStream de tao file khong thanh cong." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => SavePath), SavePath));
                        }
                    }
                }
                else
                {
                    LogSystem.Error("Khong out duoc do workSheet null hoac path null/whitespace." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => path), path));
                }
                Inventec.Common.Logging.LogSystem.Debug("OutFile. 2");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                SavePath = "";
                result = false;
            }
            return result;
        }

        private void ProcessImageHeaderFooter(FlexCel.XlsAdapter.XlsFile xls)
        {
            try
            {
                if (DictionaryTemplateKey != null)
                {
                    int SheetCount = 0;
                    int activesheet = xls.ActiveSheet;
                    //kiểm tra đã có sheet nào có tên Config_Image chưa
                    for (int i = 1; i <= xls.SheetCount; i++)
                    {
                        if (xls.GetSheetName(i) == "Config_Image")
                        {
                            SheetCount = i;
                            break;
                        }
                    }

                    if (SheetCount == 0)
                    {
                        return;
                    }

                    int XF = 0;
                    int j = 1;
                    //lấy dữ liệu từ ô A2 trở đi. ô A1 mô tả dữ liệu
                    object key = xls.GetCellValue(SheetCount, ++j, 1, ref XF);
                    object pos = xls.GetCellValue(SheetCount, j, 2, ref XF);

                    while (key != null && pos != null)
                    {
                        string checkKey = key.ToString();
                        checkKey = checkKey.Trim('<', '#', ';', '>');

                        if (DictionaryTemplateKey.ContainsKey(checkKey))
                        {
                            object value = DictionaryTemplateKey[checkKey];

                            if (value.GetType() == typeof(byte[]))
                            {
                                byte[] imageValue = ObjectToByteArray(value);
                                if (imageValue == null)
                                {
                                    continue;
                                }

                                FlexCel.Core.THeaderAndFooterPos imagePosition = FlexCel.Core.THeaderAndFooterPos.FooterCenter;

                                if (pos.ToString().ToLower() == "headerleft")
                                {
                                    imagePosition = FlexCel.Core.THeaderAndFooterPos.HeaderLeft;
                                    if (string.IsNullOrWhiteSpace(xls.PageHeader) || !xls.PageHeader.Contains("&G"))
                                    {
                                        xls.PageHeader += "\n\r&G";
                                    }
                                }
                                else if (pos.ToString().ToLower() == "headercenter")
                                {
                                    imagePosition = FlexCel.Core.THeaderAndFooterPos.HeaderCenter;
                                    if (string.IsNullOrWhiteSpace(xls.PageHeader) || !xls.PageHeader.Contains("&G"))
                                    {
                                        xls.PageHeader += "\n\r&G";
                                    }
                                }
                                else if (pos.ToString().ToLower() == "headerright")
                                {
                                    imagePosition = FlexCel.Core.THeaderAndFooterPos.HeaderRight;
                                    if (string.IsNullOrWhiteSpace(xls.PageHeader) || !xls.PageHeader.Contains("&G"))
                                    {
                                        xls.PageHeader += "\n\r&G";
                                    }
                                }
                                else if (pos.ToString().ToLower() == "footerleft")
                                {
                                    imagePosition = FlexCel.Core.THeaderAndFooterPos.FooterLeft;
                                    if (string.IsNullOrWhiteSpace(xls.PageFooter) || !xls.PageFooter.Contains("&G"))
                                    {
                                        xls.PageFooter += "\n\r&G";
                                    }
                                }
                                else if (pos.ToString().ToLower() == "footercenter")
                                {
                                    imagePosition = FlexCel.Core.THeaderAndFooterPos.FooterCenter;
                                    if (string.IsNullOrWhiteSpace(xls.PageFooter) || !xls.PageFooter.Contains("&G"))
                                    {
                                        xls.PageFooter += "\n\r&G";
                                    }
                                }
                                else if (pos.ToString().ToLower() == "footerright")
                                {
                                    imagePosition = FlexCel.Core.THeaderAndFooterPos.FooterRight;
                                    if (string.IsNullOrWhiteSpace(xls.PageFooter) || !xls.PageFooter.Contains("&G"))
                                    {
                                        xls.PageFooter += "\n\r&G";
                                    }
                                }

                                var propeties = xls.GetHeaderOrFooterImageProperties(FlexCel.Core.THeaderAndFooterKind.Default, imagePosition);
                                if (propeties == null)
                                {
                                    propeties = new FlexCel.Core.THeaderOrFooterImageProperties();
                                    propeties.Anchor = new FlexCel.Core.THeaderOrFooterAnchor(120, 120);
                                }

                                xls.SetHeaderOrFooterImage(FlexCel.Core.THeaderAndFooterKind.Default, imagePosition, imageValue, propeties);
                            }
                        }

                        key = xls.GetCellValue(SheetCount, ++j, 1, ref XF);
                        pos = xls.GetCellValue(SheetCount, j, 2, ref XF);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        byte[] ObjectToByteArray(object obj)
        {
            try
            {
                if (obj == null)
                    return null;
                byte[] byteArrayIn = (byte[])obj;

                using (var ms = new MemoryStream(byteArrayIn))
                {
                    System.Drawing.Image imageIn = System.Drawing.Image.FromStream(ms);
                    using (var ms2 = new MemoryStream())
                    {
                        imageIn.Save(ms2, imageIn.RawFormat);
                        return ms2.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        public MemoryStream OutStream()
        {
            Inventec.Common.Logging.LogSystem.Debug("OutStream. 1");
            MemoryStream result = new MemoryStream();
            try
            {
                if (flexCel != null)
                {
                    TemplateStream.Position = 0;
                    flexCel.Run(TemplateStream, result);
                    if (result != null)
                    {
                        result.Position = 0;
                    }
                    ResultStream = result;
                    FlexCel.XlsAdapter.XlsFile xls = new FlexCel.XlsAdapter.XlsFile(true);
                    xls.Open(result);
                    xls.ConvertFormulasToValues(false);
                    this.ProcessComment(xls);
                    this.ProcessImageHeaderFooter(xls);
                    if (DictionaryTemplateKey != null)
                    {
                        // them 1 sheet an de chua cac key
                        int SheetCount = 0;
                        int activesheet = xls.ActiveSheet;
                        // them 1 sheet an de chua cac key//kiểm tra đã có sheet nào có tên Template_Key
                        for (int i = 1; i <= xls.SheetCount; i++)
                        {
                            if (xls.GetSheetName(i) == "Template_Key")
                            {
                                SheetCount = i;
                                break;
                            }
                        }

                        if (SheetCount == 0)
                        {
                            xls.AddSheet();
                            SheetCount = xls.SheetCount;
                        }
                        var count = DictionaryTemplateKey.Count;
                        List<string> keys = DictionaryTemplateKey.Keys.ToList();
                        List<object> values = DictionaryTemplateKey.Values.ToList();
                        xls.SetCellValue(SheetCount, 1, 2, count, -1);
                        for (int i = 1; i < count + 1; i++)
                        {
                            xls.SetCellValue(SheetCount, i + 1, 1, keys[i - 1], -1);
                            xls.SetCellValue(SheetCount, i + 1, 2, values[i - 1], -1);
                        }
                        xls.ActiveSheet = SheetCount;
                        xls.SheetName = "Template_Key";
                        xls.SheetVisible = FlexCel.Core.TXlsSheetVisible.Hidden;
                        xls.ActiveSheet = activesheet;
                        xls.SheetVisible = FlexCel.Core.TXlsSheetVisible.Visible;
                    }
                    LoadPaperSizes(xls);
                    xls.Save(result);
                }
                else
                {
                    LogSystem.Error("Khong out duoc do workSheet null.");
                }
                Inventec.Common.Logging.LogSystem.Debug("OutStream. 2");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = new MemoryStream();
            }

            return result;
        }

        public MemoryStream OutStreamPDF()
        {
            MemoryStream resultPdfStream = new MemoryStream();
            MemoryStream result = new MemoryStream();
            try
            {
                if (flexCel != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("OutStreamPDF.1");
                    var tmpStream = new MemoryStream();
                    TemplateStream.Position = 0;
                    flexCel.Run(TemplateStream, tmpStream);
                    Inventec.Common.Logging.LogSystem.Debug("OutStreamPDF.2");
                    if (tmpStream != null)
                    {
                        tmpStream.Position = 0;
                    }

                    FlexCel.XlsAdapter.XlsFile xls = new FlexCel.XlsAdapter.XlsFile(true);
                    xls.Open(tmpStream);
                    xls.ConvertFormulasToValues(false);
                    this.ProcessComment(xls);
                    this.ProcessImageHeaderFooter(xls);

                    LoadPaperSizes(xls);

                    xls.Save(result);

                    result.Position = 0;
                    ConvertExcelToPdfByFlexCel(result, resultPdfStream);

                    Inventec.Common.Logging.LogSystem.Debug("OutStreamPDF.3");
                }
                else
                {
                    LogSystem.Error("Khong out duoc do workSheet null.");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                resultPdfStream = new MemoryStream();
            }
            return resultPdfStream;
        }

        private void LoadPaperSizes(FlexCel.Core.ExcelFile Xls)
        {
            try
            {
                PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Xls.PrintPaperDimensions), Xls.PrintPaperDimensions) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Xls.PrintPaperSize), Xls.PrintPaperSize.ToString()));
                //Get all paper sizes and add them to the combo box list
                foreach (PaperSize size in printerSettings.PaperSizes)
                {
                    if (size.RawKind.ToString() == Xls.PrintPaperSize.ToString())
                    {
                        PaperSizeDefault = size;
                        break;
                    }
                    else
                    {
                        if (size.Kind.ToString() == Xls.PrintPaperSize.ToString())
                        {
                            PaperSizeDefault = size;
                            break;
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("LoadPaperSizes____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PaperSizeDefault), PaperSizeDefault));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ConvertExcelToPdfByFlexCel(MemoryStream excelStream, MemoryStream pdfStream)
        {
            Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.2.1");
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
            flexCelPdfExport1.Workbook.Open(excelStream);

            Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.2.2");
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
            Inventec.Common.Logging.LogSystem.Debug("ConvertExcelToPdfByFlexCel.2.3");
        }

        private void ConvertExcelToPdfByOther(MemoryStream pdfStream, MemoryStream result)
        {
            //Inventec.Common.Logging.LogSystem.Debug("OutStreamPDFByGemBox.2.1");
            //EvoExcelToPdf.ExcelToPdfConverter excelToPdfConverter = new EvoExcelToPdf.ExcelToPdfConverter();
            //// Set license key received after purchase to use the converter in licensed mode
            //// Leave it not set to use the converter in demo mode
            //excelToPdfConverter.LicenseKey = "gA4eDxofD2YCeE5dSlVwHR8eGA8YARsPHB4BHh0BFhYWFg8e";

            //excelToPdfConverter.ConvertExcelStreamToStream(pdfStream, result);
            //Inventec.Common.Logging.LogSystem.Debug("OutStreamPDFByGemBox.2.3");
        }
    }
}

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
using DevExpress.XtraRichEdit;
using DocumentFormat.OpenXml.Spreadsheet;
using Inventec.Common.Logging;
using Newtonsoft.Json;
using NGS.Templater;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.TemplaterExport
{
    public class Store
    {
        string templatePath;
        public Dictionary<string, object> DictionaryTemplateKey { get; set; }
        internal ITemplateDocument templateDoc;


        public Store()
        {
            ProcessClearAllFileInTempFolder();
        }

        public bool ReadTemplate(string path)
        {
            bool result = false;
            try
            {
                string extension = Path.GetExtension(path);
                if (extension == ".doc")
                {
                    Inventec.Common.Logging.LogSystem.Debug("ReadTemplate.1");
                    this.templatePath = Utils.GenerateTempFileWithin("", ".docx");
                    Inventec.Common.Logging.LogSystem.Debug("ReadTemplate.2");
                    Utils.DocToDocx(null, path, null, templatePath);
                    Inventec.Common.Logging.LogSystem.Debug("ReadTemplate.3");
                }
                else
                {
                    this.templatePath = Utils.GenerateTempFileWithin(path);
                    File.Copy(path, this.templatePath, true);
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => path), path) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => templatePath), templatePath));

                Action<string, ITemplater, IEnumerable<string>, object> handleUnprocessed = (prefix, templater, tags, value) =>
                {
                    foreach (var t in tags)
                    {
                        var md = templater.GetMetadata(t, false);
                        var missing = md.FirstOrDefault(it => it.StartsWith("missing("));
                        if (missing != null)
                            templater.Replace(t, missing.Substring("missing(".Length, missing.Length - 1 - "missing(".Length));
                    }
                };

                var factory = Configuration.Builder.Include(Commonfuction)
                        .OnUnprocessed(handleUnprocessed)
                        .Build();
                this.templateDoc = factory.Open(this.templatePath);
                result = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
                this.templatePath = "";
                this.templateDoc = null;
            }
            return result;
        }

        public bool ReadTemplate(MemoryStream inputStream, TemplateType templateType)
        {
            bool result = false;
            try
            {
                this.templatePath = Utils.GenerateTempFileWithin(templateType);
                Utils.ByteToFile(Utils.StreamToByte(inputStream), this.templatePath);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => templatePath), templatePath));
                if (File.Exists(this.templatePath))
                {
                    Action<string, ITemplater, IEnumerable<string>, object> handleUnprocessed = (prefix, templater, tags, value) =>
                    {
                        foreach (var t in tags)
                        {
                            var md = templater.GetMetadata(t, false);
                            var missing = md.FirstOrDefault(it => it.StartsWith("missing("));
                            if (missing != null)
                                templater.Replace(t, missing.Substring("missing(".Length, missing.Length - 1 - "missing(".Length));
                        }
                    };

                    var factory = Configuration.Builder.Include(Commonfuction)
                        .OnUnprocessed(handleUnprocessed)
                        .Build();
                    this.templateDoc = factory.Open(this.templatePath);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
                this.templatePath = "";
                this.templateDoc = null;
            }
            return result;
        }

        public string OutFile()
        {
            try
            {
                this.RemoveTagsWithMissing(this.templateDoc.Templater);
                this.templateDoc.Dispose();
                this.ProcessLisence();
                return this.templatePath;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return System.String.Empty;
        }

        void ProcessLisence()
        {
            try
            {
                using (var doc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(this.templatePath, true))
                {
                    string docText = null;
                    using (StreamReader sr = new StreamReader(doc.MainDocumentPart.GetStream()))
                    {
                        docText = sr.ReadToEnd();
                    }
                    //Inventec.Common.Logging.LogSystem.Debug("ProcessLisence:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => docText), docText));
                    System.Text.RegularExpressions.Regex regexText = new System.Text.RegularExpressions.Regex("Unlicensed version. Please register @ templater.info");
                    System.Text.RegularExpressions.Regex regexText1 = new System.Text.RegularExpressions.Regex("<w:p><w:r><w:rPr><w:b /><w:color w:val=\"FF0000\" /></w:rPr><w:t>Unlicensed version. Please register @ templater.info</w:t></w:r></w:p>");
                    docText = regexText1.Replace(docText, "");
                    docText = regexText.Replace(docText, "");

                    using (StreamWriter sw = new StreamWriter(doc.MainDocumentPart.GetStream(FileMode.Create)))
                    {
                        sw.Write(docText);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        void ProcessLisenceExt()
        {
            try
            {
                // For complete examples and data files, please go to https://github.com/aspose-pdf/Aspose.PDF-for-.NET
                // The path to the documents directory.
                // Open document
                if (System.IO.File.Exists(this.templatePath))
                {
                    License.LicenceProcess.SetLicenseForAspose();
                    Aspose.Words.Document pdfDocument = new Aspose.Words.Document(this.templatePath);
                    string docText = pdfDocument.Range.Text;
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("pdfDocument.Range.Text", docText));
                    var arrDoc = (docText.Contains("[[") && docText.Contains("]]")) ? docText.Split(new string[] { "[[" }, StringSplitOptions.RemoveEmptyEntries) : null;
                    if (arrDoc != null && arrDoc.Length > 0)
                    {
                        foreach (var item in arrDoc)
                        {
                            if (item.Contains("]]"))
                            {
                                try
                                {
                                    string strReplace = "[[" + item.Substring(0, item.IndexOf("]]") + 2);
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strReplace), strReplace)
                                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                                    pdfDocument.Range.Replace(strReplace, "", false, false);
                                }
                                catch (Exception exx)
                                {
                                    Inventec.Common.Logging.LogSystem.Warn("Replace key in docx file error____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item), exx);
                                }
                            }
                        }

                        System.Text.RegularExpressions.Regex regexText = new System.Text.RegularExpressions.Regex("Unlicensed version. Please register @ templater.info");
                        System.Text.RegularExpressions.Regex regexText1 = new System.Text.RegularExpressions.Regex("<w:p><w:r><w:rPr><w:b /><w:color w:val=\"FF0000\" /></w:rPr><w:t>Unlicensed version. Please register @ templater.info</w:t></w:r></w:p>");

                        pdfDocument.Range.Replace(regexText, "");

                        // Save resulting PDF document.
                        pdfDocument.Save(this.templatePath);//outFile
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public MemoryStream OutStream()
        {
            MemoryStream result = null;
            try
            {
                this.RemoveTagsWithMissing(this.templateDoc.Templater);
                this.templateDoc.Dispose();
                this.ProcessLisence();
                result = Utils.GetStreamFromFile(this.templatePath);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public bool SetCommonFunctions()
        {
            return true;
        }

        void ProcessClearAllFileInTempFolder()
        {
            try
            {
                string tempFolderParent = Utils.ParentTempFolder();
                string tempFolder = Utils.GenerateTempFolderWithin();
                System.IO.DirectoryInfo di = new DirectoryInfo(tempFolderParent);

                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception exx1)
                    {
                        Logging.LogSystem.Warn(exx1);
                    }
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    try
                    {
                        if (dir.FullName != tempFolder)
                        {
                            dir.Delete(true);
                        }
                    }
                    catch (Exception exx1)
                    {
                        Logging.LogSystem.Warn(exx1);
                    }
                }
            }
            catch (Exception ex1)
            {
                Logging.LogSystem.Warn(ex1);
            }
        }

        private object Commonfuction(object arg1, string arg2)
        {
            object result = arg1;
            try
            {
                if (arg1 != null && arg2 != null)
                {
                    if (arg2 == "FuncPathImage")
                    {
                        result = PathImage(arg1, arg2);
                    }
                    else if (arg2 == "FuncUrlImage")
                    {
                        result = UrlImage(arg1, arg2);
                    }
                    else if (arg2 == "FuncByteImage")
                    {
                        result = ByteImage(arg1, arg2);
                    }
                    else if (arg2 == "FuncBase64Image")
                    {
                        result = Base64Image(arg1, arg2);
                    }
                    else if (arg2 == "xml")
                    {
                        result = Xml(arg1, arg2);
                    }
                    else if (arg2.StartsWith("FuncSpeechNumberToString"))
                    {
                        result = SpeechNumberToString(arg1, arg2);
                    }
                    else if (arg2.StartsWith("FuncSubString-"))
                    {
                        result = SubString(arg1, arg2);
                    }
                    else if (arg2.StartsWith("FuncNumberToString"))
                    {
                        result = NumberToString(arg1, arg2);
                    }
                    else if (arg2.Contains("FuncIfElseNotEmpty("))
                    {
                        result = IfElseNotEmpty(arg1, arg2);
                    }
                    else if (arg2.StartsWith("FuncCalculateAge"))
                    {
                        result = CalculateAge(arg1, arg2);
                    }
                    else if (arg2.StartsWith("FuncTimeNumberToDateStringSeparateString"))
                    {
                        result = TimeNumberToDateStringSeparateString(arg1, arg2);
                    }
                    else if (arg2.StartsWith("FuncTimeNumberToDateString"))
                    {
                        result = TimeNumberToDateString(arg1, arg2);
                    }
                    else if (arg2.StartsWith("FuncTimeNumberToTimeString"))
                    {
                        result = TimeNumberToTimeString(arg1, arg2);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        static object NumberToString(object argument, string metadata)
        {
            try
            {
                string result = "";

                string uiGSep = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator;
                string uiDSep = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => uiDSep), uiDSep)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => uiDSep), uiDSep));

                string strvalue = argument.ToString().Replace(".", uiDSep).Replace(",", uiDSep);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => argument), argument)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => metadata), metadata)
                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strvalue), strvalue));
                var value = System.Convert.ToDecimal(strvalue);

                var parameters = metadata.Split(new string['-'], StringSplitOptions.RemoveEmptyEntries);
                int length = parameters.Length;

                int numberDigit = 4;
                int convert = 1;

                switch (length)
                {
                    case 1:
                        numberDigit = 4;
                        break;
                    case 2:
                        numberDigit = Convert.ToInt32(parameters[1]);
                        break;
                    case 3:
                        numberDigit = Convert.ToInt32(parameters[1]);
                        convert = Convert.ToInt32(parameters[2]);
                        break;
                    default:
                        break;
                }

                result = Inventec.Common.Number.Convert.NumberToStringRoundMax4(value);
                if (convert == 1)
                {
                    result = result.Replace(",", "_");
                    result = result.Replace(".", ",");
                    result = result.Replace("_", ".");
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => value), value) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                return result;
            }
            catch (Exception exx1)
            {
                Logging.LogSystem.Warn(exx1);
            }

            return argument;
        }

        static object SubString(object argument, string metadata)
        {
            try
            {
                if (metadata.StartsWith("FuncSubString-") && argument != null)
                {
                    string result = "";
                    var value = argument.ToString();

                    var parameters = metadata.Split(new string['-'], StringSplitOptions.RemoveEmptyEntries);
                    int length = parameters.Length;
                    int lengthRaw = value.Length;
                    int startPosition = 0;
                    int lenghtTo = 0;

                    switch (length)
                    {
                        case 1:
                            result = value;
                            break;
                        case 2:
                            startPosition = Convert.ToInt32(parameters[1]);
                            result = value.Substring(startPosition);
                            break;
                        case 3:
                            startPosition = Convert.ToInt32(parameters[1]);
                            lenghtTo = Convert.ToInt32(parameters[2]);

                            if (lenghtTo < lengthRaw)
                            {
                                result = value.Substring(startPosition, lenghtTo);
                            }
                            break;
                        default:
                            break;
                    }

                    return result;
                }
            }
            catch (Exception exx1)
            {
                Logging.LogSystem.Warn(exx1);
            }

            return argument;
        }

        static object IfElseNotEmpty(object argument, string metadata)
        {
            try
            {
                if (metadata.Contains("FuncIfElseNotEmpty(") && argument != null)
                {
                    var expression = metadata.Substring("FuncIfElseNotEmpty(".Length, metadata.Length - "FuncIfElseNotEmpty(".Length - 1);
                    var parameters = expression.Split(new string[] { "," }, StringSplitOptions.None);
                    if (parameters.Count() > 1)
                    {
                        var json = JsonConvert.SerializeObject(argument);
                        var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var current = dictionary.ContainsKey(parameters[i].Trim()) ? dictionary[parameters[i].Trim()] : null;
                            if (current != null && !System.String.IsNullOrEmpty(current.ToString()))
                            {
                                return current;
                            }
                        }
                    }
                }
            }
            catch (Exception exx1)
            {
                Logging.LogSystem.Warn(exx1);
            }

            return null;
        }

        static object SpeechNumberToString(object argument, string metadata)
        {
            try
            {
                var vString = argument.ToString();

                string uiGSep = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator;
                string uiDSep = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                vString = vString.Replace(uiGSep, "");
                string temp = vString.Split(new System.String[] { uiDSep }, StringSplitOptions.None)[0];
                string values = Inventec.Common.String.Convert.CurrencyToVneseString(temp);
                return values;
            }
            catch (Exception exx1)
            {
                Logging.LogSystem.Warn(exx1);
            }

            return argument;
        }

        static object UrlImage(object argument, string metadata)
        {
            try
            {
                if (metadata == "FuncUrlImage" && argument is string)
                {
                    var urlImage = argument as string;
                    using (WebClient webClient = new WebClient())
                    {
                        byte[] data = webClient.DownloadData(urlImage);

                        using (MemoryStream mem = new MemoryStream(data))
                        {
                            using (var image = Image.FromStream(mem))
                            {
                                return new ImageInfo(mem, "png", image.Width, image.HorizontalResolution, image.Height, image.VerticalResolution);
                            }
                        }

                    }
                }
            }
            catch (Exception exx1)
            {
                Logging.LogSystem.Warn(exx1);
            }

            return argument;
        }

        static object PathImage(object argument, string metadata)
        {
            try
            {
                if (metadata == "FuncPathImage" && argument is string)
                    return System.Drawing.Image.FromFile(argument.ToString());
            }
            catch (Exception exx1)
            {
                Logging.LogSystem.Warn(exx1);
            }

            return argument;
        }

        static object Base64Image(object value, string metadata)
        {
            try
            {
                var str = value as string;
                if (metadata != "FuncBase64Image" || str == null) return value;
                var image = System.Drawing.Image.FromStream(new MemoryStream(System.Convert.FromBase64String(str)));
                //if we did not disable builtin plugins we could just return it now, but lets convert into Templater specific image
                var ms = new MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                return new ImageInfo(ms, "png", image.Width, image.HorizontalResolution, image.Height, image.VerticalResolution);
            }
            catch (Exception exx1)
            {
                Logging.LogSystem.Warn(exx1);
            }
            return value;
        }

        static object ByteImage(object value, string metadata)
        {
            try
            {
                var bValue = value as byte[];
                if (metadata != "FuncByteImage" || bValue == null || bValue.Length == 0) return value;
                var image = System.Drawing.Image.FromStream(new MemoryStream(bValue));
                //if we did not disable builtin plugins we could just return it now, but lets convert into Templater specific image
                var ms = new MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                return new ImageInfo(ms, "png", image.Width, image.HorizontalResolution, image.Height, image.VerticalResolution);
            }
            catch (Exception exx1)
            {
                Logging.LogSystem.Warn(exx1);
            }
            return value;
        }

        static object Xml(object value, string metadata)
        {
            try
            {
                var str = value as string;
                if (metadata != "xml" || str == null) return value;
                return System.Xml.Linq.XElement.Parse(str);
            }
            catch (Exception exx1)
            {
                Logging.LogSystem.Warn(exx1);
            }
            return value;
        }

        void RemoveTagsWithMissing(ITemplater templater)
        {
            try
            {
                foreach (var tag in templater.Tags.ToList())
                {
                    int i = 0;
                    string[] md;
                    //metadata will return null when a tag does not exist at that index
                    while ((md = templater.GetMetadata(tag, i)) != null)
                    {
                        var missing = md.FirstOrDefault(it => it.StartsWith("missing("));
                        if (missing != null)
                        {
                            var description = missing.Substring(8, missing.Length - 9);
                            //Replace tag at specific index, not just the first tag
                            templater.Replace(tag, i, description);
                        }
                        else i++;
                    }
                }
            }
            catch (Exception exx1)
            {
                Logging.LogSystem.Warn(exx1);
            }
        }

        static object CalculateAge(object argument, string metadata)
        {
            string result = System.String.Empty;
            try
            {
                long dob = 0;// Convert.ToInt64(argument);
                if (!long.TryParse(argument.ToString(), out dob))
                {
                    return argument;
                }

                long TimeTo = 0;
                string caption__Tuoi = "tuổi";
                string caption__ThangTuoi = "tháng tuổi";
                string caption__NgayTuoi = "ngày tuổi";
                string caption__GioTuoi = "giờ tuổi";
                var parameters = metadata.Split(new string[] { "FuncCalculateAge", ";", "(", ")", "," }, StringSplitOptions.RemoveEmptyEntries);

                if (parameters.Length > 0)
                {
                    caption__Tuoi = Convert.ToString(parameters[0]);
                }
                if (parameters.Length > 1)
                {
                    caption__ThangTuoi = Convert.ToString(parameters[1]);
                }
                if (parameters.Length > 2)
                {
                    caption__NgayTuoi = Convert.ToString(parameters[2]);
                }
                if (parameters.Length > 3)
                {
                    caption__GioTuoi = Convert.ToString(parameters[3]);
                }
                if (parameters.Length > 4)
                {
                    try
                    {
                        string timeToStr = parameters[4].ToString().Trim();
                        if (timeToStr.Length >= 14)
                        {
                            long.TryParse(timeToStr.Substring(0, 14), out TimeTo);
                        }

                    }
                    catch (Exception ex)
                    {
                        TimeTo = 0;
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }

                if (dob > 0)
                {
                    System.DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob).Value;
                    if (dtNgSinh == System.DateTime.MinValue) throw new ArgumentNullException("dtNgSinh");

                    TimeSpan diff__hour = (System.DateTime.Now - dtNgSinh);
                    TimeSpan diff__month = (System.DateTime.Now.Date - dtNgSinh.Date);

                    int year = System.DateTime.Now.Year - dtNgSinh.Year;

                    if (TimeTo > 0)
                    {
                        System.DateTime dtTimeTo = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(TimeTo).Value;
                        if (dtTimeTo == System.DateTime.MinValue) throw new ArgumentNullException("dtTimeTo");

                        diff__hour = (dtTimeTo - dtNgSinh);
                        diff__month = (dtTimeTo.Date - dtNgSinh.Date);

                        year = dtTimeTo.Year - dtNgSinh.Year;
                    }

                    //- Dưới 24h: tính chính xác đến giờ.
                    double hour = diff__hour.TotalHours;

                    if (hour < 24)
                    {
                        result = ((int)hour + " " + caption__GioTuoi);
                    }
                    else
                    {
                        long tongsogiay__hour = diff__hour.Ticks;
                        System.DateTime newDate__hour = new System.DateTime(tongsogiay__hour);
                        int month__hour = ((newDate__hour.Year - 1) * 12 + newDate__hour.Month - 1);
                        if (parameters.Count() == 5 && month__hour == 0)
                        {
                            //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                            result = ((int)diff__month.TotalDays + " " + caption__NgayTuoi);
                        }
                        else
                        {
                            long tongsogiay = diff__month.Ticks;
                            System.DateTime newDate = new System.DateTime(tongsogiay);
                            int month = ((newDate.Year - 1) * 12 + newDate.Month - 1);
                            if (month == 0)
                            {
                                //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                                result = ((int)diff__month.TotalDays + " " + caption__NgayTuoi);
                            }
                            else
                            {
                                //- Dưới 72 tháng tuổi: tính chính xác đến tháng như hiện tại
                                if (month < 72)
                                {
                                    result = (month + " " + caption__ThangTuoi);
                                }
                                //- Trên 72 tháng tuổi: tính chính xác đến năm: tuổi= năm hiện tại - năm sinh
                                else
                                {
                                    result = (year + " " + caption__Tuoi);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exx1)
            {
                Logging.LogSystem.Warn(exx1);
            }

            return result;
        }

        private object TimeNumberToDateStringSeparateString(object arg1, string arg2)
        {
            object result = null;
            try
            {
                result = Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(long.Parse(arg1.ToString()));
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private object TimeNumberToTimeString(object arg1, string arg2)
        {
            object result = null;
            try
            {
                result = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(long.Parse(arg1.ToString()));
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private object TimeNumberToDateString(object arg1, string arg2)
        {
            object result = null;
            try
            {
                result = Inventec.Common.DateTime.Convert.TimeNumberToDateString(long.Parse(arg1.ToString()));
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}

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

                var factory = Configuration.Builder
                        .Include(PathImage)
                        .Include(UrlImage)
                        .Include(Base64Image)
                        .Include(ByteImage)
                        .Include(Xml)
                        .Include(SpeechNumberToString)
                        .Include(SubString)
                        .Include(NumberToString)
                        .Include(IfElseNotEmpty)
                        .Include((value, metadata, tag, position, templater) =>
                        {
                            if ("leaveIfEmpty" == metadata && value is IList)
                            {
                                var list = (IList)value;
                                if (list.Count == 0)
                                {
                                    //when list is empty we want to leave the default message
                                    templater.Replace(tag, "");
                                }
                                else
                                {
                                    //when list is not empty, we will remove the default message
                                    templater.Resize(new[] { tag }, 0);
                                }
                                //indicates that only this tag was handled,
                                //so Templater will either duplicate or remove other tags from this collection
                                return Handled.ThisTag;
                            }
                            return Handled.Nothing;
                        })
                        .Include((value, metadata, pathTemper, position, templater) =>
                        {
                            var str = value as string;
                            if (str != null && metadata.StartsWith("collapseIf("))
                            {
                                //Extract the matching expression
                                var expression = metadata.Substring("collapseIf(".Length, metadata.Length - "collapseIf(".Length - 1);
                                if (str == expression)
                                {
                                    //remove the context around the specific property
                                    if (position == -1)
                                    {
                                        //when position is -1 it means non sharing tag is being used, in which case we can resize that region via "standard" API
                                        templater.Resize(new[] { pathTemper }, 0);
                                    }
                                    else
                                    {
                                        //otherwise we need to use "advanced" resize API to specify which exact tag to replace
                                        templater.Resize(new[] { new TagPosition(pathTemper, position) }, 0);
                                    }
                                    return Handled.NestedTags;
                                }
                            }
                            return Handled.Nothing;
                        }).Include((value, metadata, tag, position, templater) =>
                        {
                            if (value is IList && ("collapseNonEmpty" == metadata || "collapseEmpty" == metadata))
                            {
                                var list = (IList)value;
                                //loop until all tags with the same name are processed
                                do
                                {
                                    var md = templater.GetMetadata(tag, false);
                                    var collapseOnEmpty = md.Contains("collapseEmpty");
                                    var collapseNonEmpty = md.Contains("collapseNonEmpty");
                                    if (list.Count == 0)
                                    {
                                        if (collapseOnEmpty)
                                        {
                                            //when position is -1 it means non sharing tag is being used, in which case we can resize that region via "standard" API
                                            //otherwise we need to use "advanced" resize API to specify which exact tag to replace
                                            if (position == -1)
                                                templater.Resize(new[] { tag }, 0);
                                            else
                                                templater.Resize(new[] { new TagPosition(tag, position) }, 0);
                                        }
                                        else
                                        {
                                            //when position is -1 it means non sharing tag is being used, in which case we can just replace the first tag
                                            //otherwise we can replace that exact tag via position API
                                            //replacing the first tag is the same as calling replace(tag, 0, value)
                                            if (position == -1)
                                                templater.Replace(tag, "");
                                            else
                                                templater.Replace(tag, position, "");
                                        }
                                    }
                                    else
                                    {
                                        if (collapseNonEmpty)
                                        {
                                            if (position == -1)
                                                templater.Resize(new[] { tag }, 0);
                                            else
                                                templater.Resize(new[] { new TagPosition(tag, position) }, 0);
                                        }
                                        else
                                        {
                                            if (position == -1)
                                                templater.Replace(tag, "");
                                            else
                                                templater.Replace(tag, position, "");
                                        }
                                    }
                                } while (templater.Tags.Contains(tag));
                                //we want to stop further processing if list is empty
                                //otherwise we want to continue resizing list and processing it's elements
                                return list.Count == 0 ? Handled.NestedTags : Handled.Nothing;
                            }
                            return Handled.Nothing;
                        })
                        .OnUnprocessed(handleUnprocessed)
                        //.BuiltInLowLevelPlugins(false)
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

                    var factory = Configuration.Builder
                        .Include(PathImage)
                        .Include(UrlImage)
                        .Include(ByteImage)
                        .Include(Base64Image)
                        .Include(Xml)
                        .Include(SpeechNumberToString)
                        .Include(SubString)
                        .Include(NumberToString)
                        .Include(IfElseNotEmpty)
                        .Include((value, metadata, tag, position, templater) =>
                        {
                            if ("leaveIfEmpty" == metadata && value is IList)
                            {
                                var list = (IList)value;
                                if (list.Count == 0)
                                {
                                    //when list is empty we want to leave the default message
                                    templater.Replace(tag, "");
                                }
                                else
                                {
                                    //when list is not empty, we will remove the default message
                                    templater.Resize(new[] { tag }, 0);
                                }
                                //indicates that only this tag was handled,
                                //so Templater will either duplicate or remove other tags from this collection
                                return Handled.ThisTag;
                            }
                            return Handled.Nothing;
                        })
                        .Include((value, metadata, pathTemper, position, templater) =>
                        {
                            var str = value as string;
                            if (str != null && metadata.StartsWith("collapseIf("))
                            {
                                //Extract the matching expression
                                var expression = metadata.Substring("collapseIf(".Length, metadata.Length - "collapseIf(".Length - 1);
                                if (str == expression)
                                {
                                    //remove the context around the specific property
                                    if (position == -1)
                                    {
                                        //when position is -1 it means non sharing tag is being used, in which case we can resize that region via "standard" API
                                        templater.Resize(new[] { pathTemper }, 0);
                                    }
                                    else
                                    {
                                        //otherwise we need to use "advanced" resize API to specify which exact tag to replace
                                        templater.Resize(new[] { new TagPosition(pathTemper, position) }, 0);
                                    }
                                    return Handled.NestedTags;
                                }
                            }
                            return Handled.Nothing;
                        }).Include((value, metadata, tag, position, templater) =>
                        {
                            if (value is IList && ("collapseNonEmpty" == metadata || "collapseEmpty" == metadata))
                            {
                                var list = (IList)value;
                                //loop until all tags with the same name are processed
                                do
                                {
                                    var md = templater.GetMetadata(tag, false);
                                    var collapseOnEmpty = md.Contains("collapseEmpty");
                                    var collapseNonEmpty = md.Contains("collapseNonEmpty");
                                    if (list.Count == 0)
                                    {
                                        if (collapseOnEmpty)
                                        {
                                            //when position is -1 it means non sharing tag is being used, in which case we can resize that region via "standard" API
                                            //otherwise we need to use "advanced" resize API to specify which exact tag to replace
                                            if (position == -1)
                                                templater.Resize(new[] { tag }, 0);
                                            else
                                                templater.Resize(new[] { new TagPosition(tag, position) }, 0);
                                        }
                                        else
                                        {
                                            //when position is -1 it means non sharing tag is being used, in which case we can just replace the first tag
                                            //otherwise we can replace that exact tag via position API
                                            //replacing the first tag is the same as calling replace(tag, 0, value)
                                            if (position == -1)
                                                templater.Replace(tag, "");
                                            else
                                                templater.Replace(tag, position, "");
                                        }
                                    }
                                    else
                                    {
                                        if (collapseNonEmpty)
                                        {
                                            if (position == -1)
                                                templater.Resize(new[] { tag }, 0);
                                            else
                                                templater.Resize(new[] { new TagPosition(tag, position) }, 0);
                                        }
                                        else
                                        {
                                            if (position == -1)
                                                templater.Replace(tag, "");
                                            else
                                                templater.Replace(tag, position, "");
                                        }
                                    }
                                } while (templater.Tags.Contains(tag));
                                //we want to stop further processing if list is empty
                                //otherwise we want to continue resizing list and processing it's elements
                                return list.Count == 0 ? Handled.NestedTags : Handled.Nothing;
                            }
                            return Handled.Nothing;
                        })
                        .OnUnprocessed(handleUnprocessed)
                        //.BuiltInLowLevelPlugins(false)
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

        static object NumberToString(object argument, string metadata)
        {
            try
            {
                if (metadata.Contains("FuncNumberToString") && argument != null)
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
                if (metadata.Contains("FuncSpeechNumberToString") && argument != null)
                {
                    var vString = argument.ToString();

                    string uiGSep = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator;
                    string uiDSep = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    vString = vString.Replace(uiGSep, "");
                    string temp = vString.Split(new System.String[] { uiDSep }, StringSplitOptions.None)[0];
                    return Inventec.Common.String.Convert.CurrencyToVneseString(temp);
                }
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
    }
}

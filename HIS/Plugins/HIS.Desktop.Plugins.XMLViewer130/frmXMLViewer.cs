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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;
using His.Bhyt.ExportXml.CheckIn.XML;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace HIS.Desktop.Plugins.XMLViewer130
{
    public partial class frmXMLViewer130 : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        string FilePath;
        string directoryPathTemp = "";
        List<string> directoryPathTempList = new List<string>();
        MemoryStream mmStream;
        long? dataType; //1 - XML 130; 2 - XML check-in
        bool isFirstLoadForm = true;
        List<UCXml130> listUCXml130 = new List<UCXml130>();
        UCXml130 ucXmlCheckIn = new UCXml130();
        public frmXMLViewer130()
        {
            InitializeComponent();
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmXMLViewer130(Inventec.Desktop.Common.Modules.Module module, string data)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.FilePath = data;
                this.currentModule = module;
                this.directoryPathTempList = new List<string>();
                this.directoryPathTempList.Add(System.IO.Path.GetDirectoryName(Application.ExecutablePath));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmXMLViewer130(Inventec.Desktop.Common.Modules.Module module, MemoryStream _mmStream)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.mmStream = _mmStream;
                this.currentModule = module;
                this.directoryPathTempList = new List<string>();
                this.directoryPathTempList.Add(System.IO.Path.GetDirectoryName(Application.ExecutablePath));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public frmXMLViewer130(Inventec.Desktop.Common.Modules.Module module, string data, long? dataType)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.FilePath = data;
                this.currentModule = module;
                this.dataType = dataType;
                this.directoryPathTempList = new List<string>();
                this.directoryPathTempList.Add(System.IO.Path.GetDirectoryName(Application.ExecutablePath));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmXMLViewer130(Inventec.Desktop.Common.Modules.Module module, MemoryStream _mmStream, long? dataType)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.mmStream = _mmStream;
                this.currentModule = module;
                this.dataType = dataType;
                this.directoryPathTempList = new List<string>();
                this.directoryPathTempList.Add(System.IO.Path.GetDirectoryName(Application.ExecutablePath));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmXMLViewer130_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetIcon();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                chkViewXml130.Checked = true;
                chkViewXmlCheckIn.Checked = dataType == 2;
                ViewXml();
                isFirstLoadForm = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ViewXml()
        {
            try
            {
                if (chkViewXml130.Checked)
                {
                    if (!String.IsNullOrWhiteSpace(FilePath))
                    {
                        string[] filePaths = FilePath.Split(';');
                        if (filePaths == null || filePaths.Count() == 0)
                        {
                            return;
                        }
                        string firstFilePath = filePaths.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o));
                        DisplayXml(null, firstFilePath);
                    }
                    else if (mmStream != null)
                    {
                        DisplayXml(mmStream, null);
                    }
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(FilePath))
                    {
                        string[] filePaths = FilePath.Split(';');
                        if (filePaths == null || filePaths.Count() == 0)
                        {
                            return;
                        }
                        string firstFilePath = filePaths.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o));
                        DisplayXmlCheckIn(null, firstFilePath);
                    }
                    else if (mmStream != null)
                    {
                        DisplayXmlCheckIn(mmStream, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void DisplayXml(MemoryStream memoryStream, string filePath)
        {
            try
            {
                listUCXml130 = new List<UCXml130>();
                xtraTabControl1.TabPages.Clear();
                His.Bhyt.ExportXml.XML130.CreateXmlProcessor xmlProcessor = new His.Bhyt.ExportXml.XML130.CreateXmlProcessor(null);
                string xmlFile = "";

                if (!string.IsNullOrEmpty(filePath))
                {
                    xmlFile = File.ReadAllText(filePath);
                }
                else if (memoryStream != null)
                {
                    MemoryStream stream = memoryStream;
                    // Đọc dữ liệu từ MemoryStream và chuyển thành chuỗi
                    stream.Seek(0, SeekOrigin.Begin); // Đặt con trỏ về đầu của MemoryStream
                    StreamReader reader = new StreamReader(stream);
                    xmlFile = reader.ReadToEnd();
                }
                if (string.IsNullOrEmpty(xmlFile))
                    return;
                var hoso = xmlProcessor.GetDataFromString(xmlFile);

                foreach (var hoSo in hoso.THONGTINHOSO.DANHSACHHOSO.HOSO)
                {
                    foreach (var fileHoSo in hoSo.FILEHOSO)
                    {
                        List<object> listObj = new List<object>();
                        switch (fileHoSo.LOAIHOSO)
                        {
                            case "XML1":
                                His.Bhyt.ExportXml.XML130.XML1.CreateXmlMain xmlMain1 = new His.Bhyt.ExportXml.XML130.XML1.CreateXmlMain();
                                listObj.Add(xmlMain1.RunXml1Data(fileHoSo.NOIDUNGFILE));
                                break;
                            case "XML2":
                                His.Bhyt.ExportXml.XML130.XML2.CreateXmlMain xmlMain2 = new His.Bhyt.ExportXml.XML130.XML2.CreateXmlMain();
                                var xml2Data = xmlMain2.RunXml2Data(fileHoSo.NOIDUNGFILE);
                                if (xml2Data != null)
                                    listObj.AddRange(xml2Data.DSACH_CHI_TIET_THUOC.CHI_TIET_THUOC);
                                break;
                            case "XML3":
                                var xml3Data = His.Bhyt.ExportXml.XML130.XML3.XML3Data.LoadFromXMLString(fileHoSo.NOIDUNGFILE);
                                if (xml3Data != null)
                                {
                                    listObj.AddRange(xml3Data.DSACH_CHI_TIET_DVKT.CHI_TIET_DVKT);
                                }
                                break;
                            case "XML4":
                                His.Bhyt.ExportXml.XML130.XML4.CreateXmlMain xmlMain4 = new His.Bhyt.ExportXml.XML130.XML4.CreateXmlMain();
                                listObj.AddRange(xmlMain4.RunXml4DetailData(fileHoSo.NOIDUNGFILE));
                                break;
                            case "XML5":
                                His.Bhyt.ExportXml.XML130.XML5.CreateXmlMain xmlMain5 = new His.Bhyt.ExportXml.XML130.XML5.CreateXmlMain();
                                listObj.AddRange(xmlMain5.RunXml5DetailData(fileHoSo.NOIDUNGFILE));
                                break;
                            case "XML6":
                                His.Bhyt.ExportXml.XML130.XML6.CreateXmlMain xmlMain6 = new His.Bhyt.ExportXml.XML130.XML6.CreateXmlMain();
                                var xml6Data = xmlMain6.RunXml6Data(fileHoSo.NOIDUNGFILE);
                                if (xml6Data != null && xml6Data.DSACH_HO_SO_BENH_AN_CHAM_SOC_VA_DIEU_TRI_HIV_AIDS != null)
                                    listObj.AddRange(xml6Data.DSACH_HO_SO_BENH_AN_CHAM_SOC_VA_DIEU_TRI_HIV_AIDS.HO_SO_BENH_AN_CHAM_SOC_VA_DIEU_TRI_HIV_AIDS);
                                break;
                            case "XML7":
                                listObj.Add(His.Bhyt.ExportXml.XML130.XML7.XML7Data.LoadFromXMLString(fileHoSo.NOIDUNGFILE));
                                break;
                            case "XML8":
                                His.Bhyt.ExportXml.XML130.XML8.CreateXmlMain xmlMain8 = new His.Bhyt.ExportXml.XML130.XML8.CreateXmlMain();
                                var xml8Data = xmlMain8.RunXml8Data(fileHoSo.NOIDUNGFILE);
                                if (xml8Data != null)
                                    listObj.Add(xml8Data);
                                break;
                            case "XML9":
                                His.Bhyt.ExportXml.XML130.XML9.CreateXmlMain xmlMain9 = new His.Bhyt.ExportXml.XML130.XML9.CreateXmlMain();
                                listObj.AddRange(xmlMain9.RunXml9DetailData(fileHoSo.NOIDUNGFILE));
                                break;
                            case "XML10":
                                His.Bhyt.ExportXml.XML130.XML10.CreateXmlMain xmlMain10 = new His.Bhyt.ExportXml.XML130.XML10.CreateXmlMain();
                                var xml10Data = xmlMain10.RunXml10DetailData(fileHoSo.NOIDUNGFILE);
                                if (xml10Data != null)
                                    listObj.Add(xml10Data);
                                break;
                            case "XML11":
                                His.Bhyt.ExportXml.XML130.XML11.CreateXmlMain xmlMain11 = new His.Bhyt.ExportXml.XML130.XML11.CreateXmlMain();
                                var xml11Data = xmlMain11.RunXml11Data(fileHoSo.NOIDUNGFILE);
                                if (xml11Data != null)
                                    listObj.Add(xml11Data);
                                break;
                            case "XML12":
                                His.Bhyt.ExportXml.XML130.XML12.CreateXmlMain xmlMain12 = new His.Bhyt.ExportXml.XML130.XML12.CreateXmlMain();
                                listObj.AddRange(xmlMain12.RunXml12DetailsData(fileHoSo.NOIDUNGFILE));
                                break;
                            case "XML13":
                                His.Bhyt.ExportXml.XML130.XML13.CreateXmlMain xmlMain13 = new His.Bhyt.ExportXml.XML130.XML13.CreateXmlMain();
                                if(xmlMain13 != null)
                                listObj.Add(xmlMain13.RunXML13DetailsData(fileHoSo.NOIDUNGFILE));
                                break;
                            case "XML14":
                                His.Bhyt.ExportXml.XML130.XML14.CreateXmlMain xmlMain14 = new His.Bhyt.ExportXml.XML130.XML14.CreateXmlMain();
                                if (xmlMain14 != null)
                                    listObj.Add(xmlMain14.RunXML14DetailsData(fileHoSo.NOIDUNGFILE));
                                break;
                            case "XML15":
                                His.Bhyt.ExportXml.XML130.XML15.CreateXmlMain xmlMain15 = new His.Bhyt.ExportXml.XML130.XML15.CreateXmlMain();
                                if (xmlMain15 != null)
                                    listObj.AddRange(xmlMain15.RunXML15DetailsData(fileHoSo.NOIDUNGFILE));
                                break;

                        }
                        UCXml130 uc = new UCXml130();
                        uc.Tag = fileHoSo.LOAIHOSO;
                        if (listUCXml130 != null && listUCXml130.Count > 0)
                        {
                            var ucExist = listUCXml130.FirstOrDefault(o => o.Tag.ToString() == uc.Tag.ToString());
                            if (ucExist != null)
                            {
                                ucExist.LoadList(listObj);
                            }
                            else
                            {
                                uc.LoadList(listObj);
                                listUCXml130.Add(uc);
                            }
                        }
                        else
                        {
                            uc.LoadList(listObj);
                            listUCXml130.Add(uc);
                        }
                    }
                }

                listUCXml130 = listUCXml130.OrderBy(o => Convert.ToInt16(o.Tag.ToString().Substring(3))).ToList();
                foreach (var uc in listUCXml130)
                {
                    XtraTabPage xtraTabPage = new XtraTabPage();
                    uc.Dock = DockStyle.Fill;
                    xtraTabPage.Controls.Add(uc);
                    xtraTabPage.Text = uc.Tag.ToString();
                    xtraTabControl1.TabPages.Add(xtraTabPage);
                }
                if (!string.IsNullOrEmpty(filePath))
                {
                    StartExecuteFile(filePath, directoryPathTempList.FirstOrDefault());
                }
                else if (memoryStream != null)
                {
                    StartExecuteFile(memoryStream, directoryPathTempList.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void DisplayXmlCheckIn(MemoryStream memoryStream, string filePath)
        {
            try
            {
                xtraTabControl1.TabPages.Clear();
                His.Bhyt.ExportXml.CreateXmlMain createXmlMain = new His.Bhyt.ExportXml.CreateXmlMain(new His.Bhyt.ExportXml.Base.InputADO());
                string xmlFile = "";

                if (!string.IsNullOrEmpty(filePath))
                {
                    xmlFile = File.ReadAllText(filePath);
                }
                else if (memoryStream != null)
                {
                    // Đọc dữ liệu từ MemoryStream và chuyển thành chuỗi
                    memoryStream.Seek(0, SeekOrigin.Begin); // Đặt con trỏ về đầu của MemoryStream
                    StreamReader reader = new StreamReader(memoryStream);
                    xmlFile = reader.ReadToEnd();
                }
                if (string.IsNullOrEmpty(xmlFile))
                    return;

                List<object> listObj = new List<object>();
                DataQd130CheckIn dataCheckIn = createXmlMain.GetXmlQd130CheckInData(xmlFile);
                if (dataCheckIn != null && dataCheckIn.DSACH_TRANG_THAI_KCB != null && dataCheckIn.DSACH_TRANG_THAI_KCB.TRANG_THAI_KCB != null)
                    listObj.AddRange(dataCheckIn.DSACH_TRANG_THAI_KCB.TRANG_THAI_KCB);
                ucXmlCheckIn = new UCXml130();
                ucXmlCheckIn.Tag = "DATA CHECK-IN";
                if (listObj != null && listObj.Count > 0)
                    ucXmlCheckIn.LoadList(listObj);
                XtraTabPage xtraTabPage = new XtraTabPage();
                ucXmlCheckIn.Dock = DockStyle.Fill;
                xtraTabPage.Controls.Add(ucXmlCheckIn);
                xtraTabPage.Text = ucXmlCheckIn.Tag.ToString();
                xtraTabControl1.TabPages.Add(xtraTabPage);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetAllFileName(string directory)
        {
            string fileName = "";
            try
            {
                DirectoryInfo d = new DirectoryInfo(@directory);//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles("*.xml"); //Getting Text files
                fileName = Files.FirstOrDefault().Name;

            }
            catch (Exception ex)
            {
                fileName = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return fileName;
        }

        private void StartExecuteFile(string directoryFile, string directoryPath)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(directoryFile);

                DecodeAndReplaceWithNode(xmldoc, directoryPath);

                if (!String.IsNullOrWhiteSpace(directoryPathTemp))
                {
                    string fileName = GetAllFileName(directoryPathTemp);
                    DisplayXml(directoryPathTemp + "/" + fileName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void StartExecuteFile(MemoryStream mmStream, string directoryPath)
        {
            try
            {
                mmStream.Position = 0;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(mmStream);

                DecodeAndReplaceWithNode(xmlDoc, directoryPath);

                if (!String.IsNullOrWhiteSpace(directoryPathTemp))
                {
                    string fileName = GetAllFileName(directoryPathTemp);
                    DisplayXml(directoryPathTemp + "/" + fileName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private string RemoveFistLine(string input)
        {
            string output = "";
            try
            {
                if (String.IsNullOrWhiteSpace(input) || input.Length == 0)
                {
                    return output;
                }
                int n = 1;
                string[] lines = input
                    .Split(Environment.NewLine.ToCharArray())
                    .Skip(n)
                    .ToArray();

                output = string.Join(Environment.NewLine, lines);
                output = RemoveEmptyLines(output);
            }
            catch (Exception ex)
            {
                output = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return output;
        }

        private string RemoveEmptyLines(string lines)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(lines) || lines.Length == 0)
                {
                    return "";
                }
                return Regex.Replace(lines, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        public static void sample84(XslCompiledTransform objXslTrans)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(XLSDefault.xlsFormat) && XLSDefault.xlsFormat.Length > 0 && objXslTrans != null)
                {
                    objXslTrans.Load(new XmlTextReader(new StringReader(XLSDefault.xlsFormat)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void DisplayXml(string directoryViewFile)
        {
            try
            {
                if (chkViewXml130.Checked)
                {
                    xtraTabControl1.TabPages.Add(xtraTabPage__XMLfull);
                }
                string xmlString = File.ReadAllText(@directoryViewFile);

                // Load the xslt used by IE to render the xml
                XslCompiledTransform xTrans = new XslCompiledTransform();
                sample84(xTrans);

                // Read the xml string.
                StringReader sr = new StringReader(xmlString);
                XmlReader xReader = XmlReader.Create(sr);

                // Transform the XML data
                MemoryStream ms = new MemoryStream();
                xTrans.Transform(xReader, null, ms);

                ms.Position = 0;

                // Set to the document stream
                webBrowser1.DocumentStream = ms;

                clearFolder(this.directoryPathTemp);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void clearFolder(string FolderName)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(FolderName);
                if (dir == null)
                {
                    return;
                }
                if (dir.GetFiles() != null)
                {
                    foreach (FileInfo fi in dir.GetFiles())
                    {
                        fi.Delete();
                    }
                }
                if (dir.GetDirectories() != null)
                {
                    foreach (DirectoryInfo di in dir.GetDirectories())
                    {
                        clearFolder(di.FullName);
                        di.Delete();
                    }
                }

                if (Directory.Exists(FolderName))
                {
                    Directory.Delete(FolderName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateIfMissing(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (Directory.Exists(path))
                {
                    File.SetAttributes(path, FileAttributes.Normal);
                    //File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DecodeAndReplaceWithNode(XmlDocument xmldoc, string directoryPath)
        {
            try
            {
                XmlNodeList nodeList = xmldoc.GetElementsByTagName("NOIDUNGFILE");
                string InnertText = string.Empty;

                for (int i = 0; i < nodeList.Count; i++)
                {
                    InnertText = nodeList[i].InnerText;
                    string DeCodeValue = Base64Decode(InnertText);
                    string outPut = RemoveFistLine(DeCodeValue);
                    ReplaceNodeByNode(xmldoc, nodeList[i], outPut);
                }
                Uri uri = null;
                if (xmldoc != null && !String.IsNullOrEmpty(xmldoc.BaseURI))
                {
                    uri = new Uri(xmldoc.BaseURI);
                }
                else
                {
                    uri = new Uri(directoryPath + "\\" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Millisecond + ".xml");
                }

                string filename = "";
                if (uri.IsFile)
                {
                    filename = System.IO.Path.GetFileName(uri.LocalPath);
                }
                CreateIfMissing(directoryPath + "\\Temp");
                directoryPathTemp = directoryPath + "\\Temp";
                xmldoc.Save(directoryPathTemp + "/" + filename);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ReplaceNodeByNode(XmlDocument doc, XmlNode oldElem, string newValue)
        {
            try
            {
                if (doc == null)
                {
                    return;
                }
                var root = doc.GetElementsByTagName("GIAMDINHHS")[0];
                if (root == null)
                {
                    return;
                }
                if (oldElem == null)
                {
                    return;
                }
                var newElem = doc.CreateElement(oldElem.Name);
                if (newElem == null)
                {
                    return;
                }
                XmlDocument XmlDocumentNew = new XmlDocument();
                XmlDocumentNew.LoadXml(newValue);
                if (oldElem.ParentNode != null)
                {
                    oldElem.ParentNode.ReplaceChild(newElem, oldElem);
                    newElem.AppendChild(newElem.OwnerDocument.ImportNode(XmlDocumentNew.FirstChild, true));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        public string Base64Decode(string base64EncodedData)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(base64EncodedData))
                {
                    var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                    result = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnBrowserFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "XML file|*.xml";
                op.Multiselect = true;
                op.ShowDialog();
                listUCXml130 = new List<UCXml130>();
                xtraTabControl1.TabPages.Clear();
                His.Bhyt.ExportXml.XML130.CreateXmlProcessor xmlProcessor = new His.Bhyt.ExportXml.XML130.CreateXmlProcessor(null);
                foreach (var item in op.FileNames)
                {
                    string xmlFile = File.ReadAllText(item);
                    var hoso = xmlProcessor.GetDataFromString(xmlFile);

                    foreach (var hoSo in hoso.THONGTINHOSO.DANHSACHHOSO.HOSO)
                    {
                        foreach (var fileHoSo in hoSo.FILEHOSO)
                        {
                            List<object> listObj = new List<object>();
                            switch (fileHoSo.LOAIHOSO)
                            {
                                case "XML1":
                                    His.Bhyt.ExportXml.XML130.XML1.CreateXmlMain xmlMain1 = new His.Bhyt.ExportXml.XML130.XML1.CreateXmlMain();
                                    listObj.Add(xmlMain1.RunXml1Data(fileHoSo.NOIDUNGFILE));
                                    break;
                                case "XML2":
                                    His.Bhyt.ExportXml.XML130.XML2.CreateXmlMain xmlMain2 = new His.Bhyt.ExportXml.XML130.XML2.CreateXmlMain();
                                    var xml2Data = xmlMain2.RunXml2Data(fileHoSo.NOIDUNGFILE);
                                    if (xml2Data != null)
                                        listObj.AddRange(xml2Data.DSACH_CHI_TIET_THUOC.CHI_TIET_THUOC);
                                    break;
                                case "XML3":
                                    var xml3Data = His.Bhyt.ExportXml.XML130.XML3.XML3Data.LoadFromXMLString(fileHoSo.NOIDUNGFILE);
                                    if (xml3Data != null)
                                    {
                                        listObj.AddRange(xml3Data.DSACH_CHI_TIET_DVKT.CHI_TIET_DVKT);
                                    }
                                    break;
                                case "XML4":
                                    His.Bhyt.ExportXml.XML130.XML4.CreateXmlMain xmlMain4 = new His.Bhyt.ExportXml.XML130.XML4.CreateXmlMain();
                                    listObj.AddRange(xmlMain4.RunXml4DetailData(fileHoSo.NOIDUNGFILE));
                                    break;
                                case "XML5":
                                    His.Bhyt.ExportXml.XML130.XML5.CreateXmlMain xmlMain5 = new His.Bhyt.ExportXml.XML130.XML5.CreateXmlMain();
                                    listObj.AddRange(xmlMain5.RunXml5DetailData(fileHoSo.NOIDUNGFILE));
                                    break;
                                case "XML6":
                                    His.Bhyt.ExportXml.XML130.XML6.CreateXmlMain xmlMain6 = new His.Bhyt.ExportXml.XML130.XML6.CreateXmlMain();
                                    var xml6Data = xmlMain6.RunXml6Data(fileHoSo.NOIDUNGFILE);
                                    if (xml6Data != null && xml6Data.DSACH_HO_SO_BENH_AN_CHAM_SOC_VA_DIEU_TRI_HIV_AIDS != null)
                                        listObj.AddRange(xml6Data.DSACH_HO_SO_BENH_AN_CHAM_SOC_VA_DIEU_TRI_HIV_AIDS.HO_SO_BENH_AN_CHAM_SOC_VA_DIEU_TRI_HIV_AIDS);
                                    break;
                                case "XML7":
                                    listObj.Add(His.Bhyt.ExportXml.XML130.XML7.XML7Data.LoadFromXMLString(fileHoSo.NOIDUNGFILE));
                                    break;
                                case "XML8":
                                    His.Bhyt.ExportXml.XML130.XML8.CreateXmlMain xmlMain8 = new His.Bhyt.ExportXml.XML130.XML8.CreateXmlMain();
                                    var xml8Data = xmlMain8.RunXml8Data(fileHoSo.NOIDUNGFILE);
                                    if (xml8Data != null)
                                        listObj.Add(xml8Data);
                                    break;
                                case "XML9":
                                    His.Bhyt.ExportXml.XML130.XML9.CreateXmlMain xmlMain9 = new His.Bhyt.ExportXml.XML130.XML9.CreateXmlMain();
                                    listObj.AddRange(xmlMain9.RunXml9DetailData(fileHoSo.NOIDUNGFILE));
                                    break;
                                case "XML10":
                                    His.Bhyt.ExportXml.XML130.XML10.CreateXmlMain xmlMain10 = new His.Bhyt.ExportXml.XML130.XML10.CreateXmlMain();
                                    var xml10Data = xmlMain10.RunXml10DetailData(fileHoSo.NOIDUNGFILE);
                                    if (xml10Data != null)
                                        listObj.Add(xml10Data);
                                    break;
                                case "XML11":
                                    His.Bhyt.ExportXml.XML130.XML11.CreateXmlMain xmlMain11 = new His.Bhyt.ExportXml.XML130.XML11.CreateXmlMain();
                                    var xml11Data = xmlMain11.RunXml11Data(fileHoSo.NOIDUNGFILE);
                                    if (xml11Data != null)
                                        listObj.Add(xml11Data);
                                    break;
                                case "XML12":
                                    His.Bhyt.ExportXml.XML130.XML12.CreateXmlMain xmlMain12 = new His.Bhyt.ExportXml.XML130.XML12.CreateXmlMain();
                                    listObj.AddRange(xmlMain12.RunXml12DetailsData(fileHoSo.NOIDUNGFILE));
                                    break;
                                case "XML13":
                                    His.Bhyt.ExportXml.XML130.XML13.CreateXmlMain xmlMain13 = new His.Bhyt.ExportXml.XML130.XML13.CreateXmlMain();
                                    if (xmlMain13 != null)
                                        listObj.Add(xmlMain13.RunXML13DetailsData(fileHoSo.NOIDUNGFILE));
                                    break;
                                case "XML14":
                                    His.Bhyt.ExportXml.XML130.XML14.CreateXmlMain xmlMain14 = new His.Bhyt.ExportXml.XML130.XML14.CreateXmlMain();
                                    if (xmlMain14 != null)
                                        listObj.Add(xmlMain14.RunXML14DetailsData(fileHoSo.NOIDUNGFILE));
                                    break;
                                case "XML15":
                                    His.Bhyt.ExportXml.XML130.XML15.CreateXmlMain xmlMain15 = new His.Bhyt.ExportXml.XML130.XML15.CreateXmlMain();
                                    if (xmlMain15 != null)
                                        listObj.AddRange(xmlMain15.RunXML15DetailsData(fileHoSo.NOIDUNGFILE));
                                    break;

                            }
                            UCXml130 uc = new UCXml130();
                            uc.Tag = fileHoSo.LOAIHOSO;
                            if (listUCXml130 != null && listUCXml130.Count > 0)
                            {
                                var ucExist = listUCXml130.FirstOrDefault(o => o.Tag.ToString() == uc.Tag.ToString());
                                if (ucExist != null)
                                {
                                    ucExist.LoadList(listObj);
                                }
                                else
                                {
                                    uc.LoadList(listObj);
                                    listUCXml130.Add(uc);
                                }
                            }
                            else
                            {
                                uc.LoadList(listObj);
                                listUCXml130.Add(uc);
                            }
                        }
                    }
                }
                listUCXml130 = listUCXml130.OrderBy(o => Convert.ToInt16(o.Tag.ToString().Substring(3))).ToList();
                if (chkViewXml130.Checked)
                {
                    foreach (var uc in listUCXml130)
                    {
                        XtraTabPage xtraTabPage = new XtraTabPage();
                        uc.Dock = DockStyle.Fill;
                        xtraTabPage.Controls.Add(uc);
                        xtraTabPage.Text = uc.Tag.ToString();
                        xtraTabControl1.TabPages.Add(xtraTabPage);
                    }
                }
                if (op.FileNames.Count() == 1)
                {
                    string fileNameSub = Path.GetDirectoryName(op.FileNames[0]);
                    string fileNameSubTemp = fileNameSub + "\\Temp";
                    directoryPathTempList.Add(fileNameSubTemp);

                    StartExecuteFile(op.FileNames[0], fileNameSub);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkViewXml130_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isFirstLoadForm)
                    return;
                xtraTabControl1.TabPages.Clear();
                if (chkViewXml130.Checked && listUCXml130 != null && listUCXml130.Count > 0)
                {
                    foreach (var uc in listUCXml130)
                    {
                        XtraTabPage xtraTabPage = new XtraTabPage();
                        uc.Dock = DockStyle.Fill;
                        xtraTabPage.Controls.Add(uc);
                        xtraTabPage.Text = uc.Tag.ToString();
                        xtraTabControl1.TabPages.Add(xtraTabPage);
                    }
                    xtraTabControl1.TabPages.Add(xtraTabPage__XMLfull);
                }
                else if (chkViewXmlCheckIn.Checked && ucXmlCheckIn != null && ucXmlCheckIn.Tag != null)
                {
                    XtraTabPage xtraTabPage = new XtraTabPage();
                    ucXmlCheckIn.Dock = DockStyle.Fill;
                    xtraTabPage.Controls.Add(ucXmlCheckIn);
                    xtraTabPage.Text = ucXmlCheckIn.Tag.ToString();
                    xtraTabControl1.TabPages.Add(xtraTabPage);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

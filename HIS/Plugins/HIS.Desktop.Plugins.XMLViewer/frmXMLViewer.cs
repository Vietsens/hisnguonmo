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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.XMLViewer.Base;
using HIS.Desktop.Plugins.XMLViewer.Config;
using HIS.Desktop.Plugins.XMLViewer.Validation;
using HIS.Desktop.Plugins.XMLViewer.XML.XML1;
using HIS.Desktop.Plugins.XMLViewer.XML.XML2;
using HIS.Desktop.Plugins.XMLViewer.XML.XML3;
using HIS.Desktop.Plugins.XMLViewer.XML.XML4;
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

namespace HIS.Desktop.Plugins.XMLViewer
{
    public partial class frmXMLViewer : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        string FilePath;
        //string directoryPath = "";
        string directoryPathTemp = "";
        List<string> directoryPathTempList = new List<string>();
        NumberStyles style = NumberStyles.Any;
        MemoryStream mmStream;
        List<XML1Data> dataSourceXML1Grid;
        List<XML2DetailData> dataSourceXML2Grid;
        List<XML3DetailData> dataSourceXML3Grid;
        List<XML4DetailData> dataSourceXML4Grid;
        List<XML5DetailData> dataSourceXML5Grid;
        public frmXMLViewer()
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

        public frmXMLViewer(Inventec.Desktop.Common.Modules.Module module, string data)
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

        public frmXMLViewer(Inventec.Desktop.Common.Modules.Module module, MemoryStream _mmStream)
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

        private void frmXMLViewer_Load(object sender, EventArgs e)
        {
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.SetIcon();
                this.LoadKeyFrmLanguage();
                dataSourceXML1Grid = new List<XML1Data>();
                dataSourceXML2Grid = new List<XML2DetailData>();
                dataSourceXML3Grid = new List<XML3DetailData>();
                dataSourceXML4Grid = new List<XML4DetailData>();
                dataSourceXML5Grid = new List<XML5DetailData>();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                if (!String.IsNullOrWhiteSpace(FilePath))
                {
                    string[] filePaths = FilePath.Split(';');
                    if (filePaths == null || filePaths.Count() == 0)
                    {
                        return;
                    }
                    string firstFilePath = filePaths.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o));
                    StartExecuteFile(firstFilePath, this.directoryPathTempList.FirstOrDefault(), true);
                }
                else if (mmStream != null)
                {
                    StartExecuteFile(mmStream, this.directoryPathTempList.FirstOrDefault(), true);
                }
                gridControlXML1.DataSource = dataSourceXML1Grid;
                gridControlXML2.DataSource = dataSourceXML2Grid;
                gridControlXML3.DataSource = dataSourceXML3Grid;
                gridControlXML4.DataSource = dataSourceXML4Grid;
                gridControlXML5.DataSource = dataSourceXML5Grid;
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
                string str = "";
                fileName = Files.FirstOrDefault().Name;

            }
            catch (Exception ex)
            {
                fileName = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return fileName;
        }

        private void StartExecuteFile(string directoryFile, string directoryPath, bool isOne)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(directoryFile);

                DecodeAndReplaceWithNode(xmldoc, directoryPath);

                if (!String.IsNullOrWhiteSpace(directoryPathTemp))
                {
                    string fileName = GetAllFileName(directoryPathTemp);
                    DisplayXml(directoryPathTemp + "/" + fileName, isOne);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void StartExecuteFile(MemoryStream mmStream, string dir, bool isOne)
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(mmStream);

                DecodeAndReplaceWithNode(xmldoc, dir);

                if (!String.IsNullOrWhiteSpace(directoryPathTemp))
                {
                    string fileName = GetAllFileName(directoryPathTemp);
                    DisplayXml(directoryPathTemp + "/" + fileName, isOne);
                }
                //else
                //{

                //    string fileName = GetAllFileName(directoryPathTemp);
                //    DisplayXml(directoryPathTemp + "/" + fileName);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadKeyFrmLanguage()
        {
            try
            {

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
                return "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void DisplayXml(string directoryViewFile, bool isOne)
        {
            try
            {
                if (isOne)
                {
                    xtraTabPage__XMLfull.PageVisible = true;
                    string xmlString = File.ReadAllText(@directoryViewFile);
                    //string a = File.ReadAllText(Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.FullName, @"resources\defaultss.xsl"));

                    // Load the xslt used by IE to render the xml
                    XslCompiledTransform xTrans = new XslCompiledTransform();
                    sample84(xTrans);
                    //xTrans.Load(Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.FullName, @"resources\defaultss.xsl"));


                    // Read the xml string.
                    StringReader sr = new StringReader(xmlString);
                    XmlReader xReader = XmlReader.Create(sr);

                    // Transform the XML data
                    MemoryStream ms = new MemoryStream();
                    xTrans.Transform(xReader, null, ms);

                    ms.Position = 0;

                    // Set to the document stream
                    webBrowser1.DocumentStream = ms;
                }
                else
                {
                    xtraTabPage__XMLfull.PageVisible = false;
                }

                XmlDocument xmldoc = ReadXmlFromDirectory(@directoryViewFile);

                FillDataToXML1(xmldoc);
                FillDataToXML2(xmldoc);
                FillDataToXML3(xmldoc);
                FillDataToXML4(xmldoc);
                FillDataToXML5(xmldoc);

                clearFolder(this.directoryPathTemp);
                //webBrowser1.Navigate(new Uri(directoryViewFile));

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
                //XDocument.Parse(newValue);
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
                dataSourceXML1Grid = new List<XML1Data>();
                dataSourceXML2Grid = new List<XML2DetailData>();
                dataSourceXML3Grid = new List<XML3DetailData>();
                dataSourceXML4Grid = new List<XML4DetailData>();
                dataSourceXML5Grid = new List<XML5DetailData>();
                foreach (var item in op.FileNames)
                {
                    string fileNameSub = Path.GetDirectoryName(item);
                    string fileNameSubTemp = fileNameSub + "\\Temp";
                    directoryPathTempList.Add(fileNameSubTemp);

                    StartExecuteFile(item, fileNameSub, false);
                }
                gridControlXML1.DataSource = dataSourceXML1Grid;
                gridControlXML2.DataSource = dataSourceXML2Grid;
                gridControlXML3.DataSource = dataSourceXML3Grid;
                gridControlXML4.DataSource = dataSourceXML4Grid;
                gridControlXML5.DataSource = dataSourceXML5Grid;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetValueByNode(string nodeStr, XmlDocument xmldoc)
        {
            string result = "";
            try
            {
                XmlNodeList xnList = xmldoc.SelectNodes("//TONG_HOP");

                //XmlNodeList nodeList = xmldoc.GetElementsByTagName(nodeStr);
                string Short_Fall = string.Empty;
                foreach (XmlNode node in xnList)
                {
                    Short_Fall = (node[nodeStr] != null ? node[nodeStr].InnerText : "");
                }
                result = Short_Fall;
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private XmlDocument ReadXmlFromDirectory(string dic)
        {
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                string xmlFile = File.ReadAllText(dic);
                xmldoc.LoadXml(xmlFile);

            }
            catch (Exception ex)
            {
                xmldoc = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return xmldoc;
        }

        private void FillDataToXML1(XmlDocument xmldoc)
        {
            Inventec.Common.Logging.LogSystem.Info("XML1________________________ " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => xmldoc), xmldoc));
            try
            {
                XML1Data xML1Data = new XML1Data();

                string MA_LKstr = GetValueByNode("MA_LK", xmldoc);
                xML1Data.MA_LK = MA_LKstr;
                string STTstr = GetValueByNode("STT", xmldoc);
                xML1Data.STT = STTstr;
                string MA_BNstr = GetValueByNode("MA_BN", xmldoc);
                xML1Data.MA_BN = MA_BNstr;
                string HO_TENstr = GetValueByNode("HO_TEN", xmldoc);
                xML1Data.HO_TEN = HO_TENstr;
                string NGAY_SINHstr = (GetValueByNode("NGAY_SINH", xmldoc));
                if (!String.IsNullOrEmpty(NGAY_SINHstr) && NGAY_SINHstr.Length > 4)
                {
                    xML1Data.NGAY_SINH = Inventec.Common.DateTime.Convert.TimeNumberToDateString(NGAY_SINHstr);
                }
                else
                {
                    xML1Data.NGAY_SINH = NGAY_SINHstr;
                }

                string GIOI_TINHstr = GetValueByNode("GIOI_TINH", xmldoc);
                xML1Data.GIOI_TINH = GIOI_TINHstr;
                string MA_THEstr = GetValueByNode("MA_THE", xmldoc);
                xML1Data.MA_THE = MA_THEstr;
                string MA_DKBDstr = GetValueByNode("MA_DKBD", xmldoc);
                xML1Data.MA_DKBD = MA_DKBDstr;

                // the tu
                string GT_THE_TUstr = GetValueByNode("GT_THE_TU", xmldoc);
                if (GT_THE_TUstr.Contains(";"))
                {
                    string[] GT_THE_TU_Lst = GT_THE_TUstr.Split(';');
                    List<string> lstTheTu = new List<string>();
                    if (GT_THE_TU_Lst != null && GT_THE_TU_Lst.Count() > 0)
                    {
                        foreach (var item in GT_THE_TU_Lst)
                        {
                            lstTheTu.Add(Inventec.Common.DateTime.Convert.TimeNumberToDateString(item));
                        }
                        string GT_THE_TUstr1 = string.Join(";", lstTheTu);
                        xML1Data.GT_THE_TU = GT_THE_TUstr1;
                    }
                }
                else
                {
                    xML1Data.GT_THE_TU = GT_THE_TUstr;
                }
                // the den
                string GT_THE_DENstr = GetValueByNode("GT_THE_DEN", xmldoc);
                if (GT_THE_DENstr.Contains(";"))
                {
                    string[] GT_THE_DEN_Lst = GT_THE_DENstr.Split(';');
                    List<string> lstTheDen = new List<string>();
                    if (GT_THE_DEN_Lst != null && GT_THE_DEN_Lst.Count() > 0)
                    {
                        foreach (var item in GT_THE_DEN_Lst)
                        {
                            lstTheDen.Add(Inventec.Common.DateTime.Convert.TimeNumberToDateString(item));
                        }
                        string GT_THE_DENstr1 = string.Join(";", lstTheDen);
                        xML1Data.GT_THE_DEN = GT_THE_DENstr1;
                    }
                }
                else
                {
                    xML1Data.GT_THE_DEN = GT_THE_DENstr;
                }

                string MIEN_CUNG_CTstr = GetValueByNode("MIEN_CUNG_CT", xmldoc);
                xML1Data.MIEN_CUNG_CT = MIEN_CUNG_CTstr;
                string TEN_BENHstr = GetValueByNode("TEN_BENH", xmldoc);
                xML1Data.TEN_BENH = TEN_BENHstr;
                string MA_BENHstr = GetValueByNode("MA_BENH", xmldoc);
                xML1Data.MA_BENH = MA_BENHstr;
                string MA_BENHKHACstr = GetValueByNode("MA_BENHKHAC", xmldoc);
                xML1Data.MA_BENHKHAC = MA_BENHKHACstr;
                string MA_LYDO_VVIENstr = GetValueByNode("MA_LYDO_VVIEN", xmldoc);
                xML1Data.MA_LYDO_VVIEN = MA_LYDO_VVIENstr;
                string MA_NOI_CHUYENstr = GetValueByNode("MA_NOI_CHUYEN", xmldoc);
                xML1Data.MA_NOI_CHUYEN = MA_NOI_CHUYENstr;
                string MA_TAI_NANstr = GetValueByNode("MA_TAI_NAN", xmldoc);
                xML1Data.MA_TAI_NAN = MA_TAI_NANstr;
                string NGAY_VAOstr = Inventec.Common.DateTime.Convert.TimeNumberToDateString((GetValueByNode("NGAY_VAO", xmldoc)));
                xML1Data.NGAY_VAO = Inventec.Common.DateTime.Convert.TimeNumberToDateString((GetValueByNode("NGAY_VAO", xmldoc)));
                string NGAY_RAstr = Inventec.Common.DateTime.Convert.TimeNumberToDateString((GetValueByNode("NGAY_RA", xmldoc)));
                xML1Data.NGAY_RA = Inventec.Common.DateTime.Convert.TimeNumberToDateString((GetValueByNode("NGAY_RA", xmldoc)));
                string SO_NGAY_DTRIstr = GetValueByNode("SO_NGAY_DTRI", xmldoc);
                xML1Data.SO_NGAY_DTRI = SO_NGAY_DTRIstr;
                string KET_QUA_DTRIstr = GetValueByNode("KET_QUA_DTRI", xmldoc);
                xML1Data.KET_QUA_DTRI = KET_QUA_DTRIstr;
                string TINH_TRANG_RVstr = GetValueByNode("TINH_TRANG_RV", xmldoc);
                xML1Data.TINH_TRANG_RV = TINH_TRANG_RVstr;
                string NGAY_TTOANstr = Inventec.Common.DateTime.Convert.TimeNumberToDateString((GetValueByNode("NGAY_TTOAN", xmldoc)));
                xML1Data.NGAY_TTOAN = NGAY_TTOANstr;
                string T_BHTTstr = GetValueByNode("T_BHTT", xmldoc);
                xML1Data.T_BHTT = T_BHTTstr;
                string T_THUOCstr = GetValueByNode("T_THUOC", xmldoc);
                xML1Data.T_THUOC = T_THUOCstr;
                string T_VTYTstr = GetValueByNode("T_VTYT", xmldoc);
                xML1Data.T_VTYT = T_VTYTstr;
                string T_TONGCHIstr = GetValueByNode("T_TONGCHI", xmldoc);
                xML1Data.T_TONGCHI = T_TONGCHIstr;
                string T_BNTTstr = GetValueByNode("T_BNTT", xmldoc);
                xML1Data.T_BNTT = T_BNTTstr;
                string T_BNCCTstr = GetValueByNode("T_BNCCT", xmldoc);
                xML1Data.T_BNCCT = T_BNCCTstr;
                string T_NGUONKHACstr = GetValueByNode("T_NGUONKHAC", xmldoc);
                xML1Data.T_NGUONKHAC = T_NGUONKHACstr;
                string T_NGOAIDSstr = GetValueByNode("T_NGOAIDS", xmldoc);
                xML1Data.T_NGOAIDS = T_NGOAIDSstr;
                string NAM_QTstr = GetValueByNode("NAM_QT", xmldoc);
                xML1Data.NAM_QT = NAM_QTstr;
                string THANG_QTstr = GetValueByNode("THANG_QT", xmldoc);
                xML1Data.THANG_QT = THANG_QTstr;
                string MA_LOAI_KCBstr = GetValueByNode("MA_LOAI_KCB", xmldoc);
                xML1Data.MA_LOAI_KCB = MA_LOAI_KCBstr;
                string MA_KHOAstr = GetValueByNode("MA_KHOA", xmldoc);
                xML1Data.MA_KHOA = MA_KHOAstr;
                string MA_CSKCBstr = GetValueByNode("MA_CSKCB", xmldoc);
                xML1Data.MA_CSKCB = MA_CSKCBstr;
                string MA_KHUVUCstr = GetValueByNode("MA_KHUVUC", xmldoc);
                xML1Data.MA_KHUVUC = MA_KHUVUCstr;
                string MA_PTTT_QTstr = GetValueByNode("MA_PTTT_QT", xmldoc);
                xML1Data.MA_PTTT_QT = MA_PTTT_QTstr;
                string CAN_NANGstr = GetValueByNode("CAN_NANG", xmldoc);
                xML1Data.CAN_NANG = CAN_NANGstr;
                string DIA_CHIstr = GetValueByNode("DIA_CHI", xmldoc);
                xML1Data.DIA_CHI = DIA_CHIstr;
                this.dataSourceXML1Grid.Add(xML1Data);
                Inventec.Common.Logging.LogSystem.Info("Dau vao XML1. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataSourceXML2Grid), dataSourceXML2Grid));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToXML2(XmlDocument xml)
        {
            try
            {
                XmlNodeList xnList = xml.SelectNodes("//DSACH_CHI_TIET_THUOC/CHI_TIET_THUOC");

                foreach (XmlNode xn in xnList)
                {
                    Decimal soluong, tBhtt, tBncct, tBntt, tNgoaiDs, tNguonKhac, DonGia, ThanhTien;

                    XML2DetailData xML2DetailData = new XML2DetailData();

                    xML2DetailData.DON_VI_TINH = xn["DON_VI_TINH"] != null ? xn["DON_VI_TINH"].InnerText : "";
                    xML2DetailData.DUONG_DUNG = xn["DUONG_DUNG"] != null ? xn["DUONG_DUNG"].InnerText : "";
                    xML2DetailData.HAM_LUONG = xn["HAM_LUONG"] != null ? xn["HAM_LUONG"].InnerText : "";
                    xML2DetailData.LIEU_DUNG = xn["LIEU_DUNG"] != null ? xn["LIEU_DUNG"].InnerText : "";
                    xML2DetailData.MA_BAC_SI = xn["MA_BAC_SI"] != null ? xn["MA_BAC_SI"].InnerText : "";
                    xML2DetailData.MA_BENH = xn["MA_BENH"] != null ? xn["MA_BENH"].InnerText : "";
                    xML2DetailData.MA_KHOA = xn["MA_KHOA"] != null ? xn["MA_KHOA"].InnerText : "";
                    xML2DetailData.MA_LK = xn["MA_LK"] != null ? xn["MA_LK"].InnerText : "";
                    xML2DetailData.MA_NHOM = xn["MA_NHOM"] != null ? xn["MA_NHOM"].InnerText : "";
                    xML2DetailData.MA_PTTT = xn["MA_PTTT"] != null ? xn["MA_PTTT"].InnerText : "";
                    xML2DetailData.MA_THUOC = xn["MA_THUOC"] != null ? xn["MA_THUOC"].InnerText : "";
                    xML2DetailData.MUC_HUONG = xn["MUC_HUONG"] != null ? xn["MUC_HUONG"].InnerText : "";
                    xML2DetailData.NGAY_YL = xn["NGAY_YL"] != null ? xn["NGAY_YL"].InnerText : "";
                    xML2DetailData.PHAM_VI = xn["PHAM_VI"] != null ? xn["PHAM_VI"].InnerText : "";
                    xML2DetailData.SO_DANG_KY = xn["SO_DANG_KY"] != null ? xn["SO_DANG_KY"].InnerText : "";

                    if (xn["DON_GIA"] != null && Decimal.TryParse((xn["DON_GIA"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out DonGia))
                        xML2DetailData.DON_GIA = DonGia;

                    if (xn["SO_LUONG"] != null && Decimal.TryParse((xn["SO_LUONG"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out soluong))
                        xML2DetailData.SO_LUONG = soluong;

                    xML2DetailData.STT = xn["STT"] != null ? xn["STT"].InnerText : "";

                    if (xn["T_BHTT"] != null && Decimal.TryParse((xn["T_BHTT"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tBhtt))
                        xML2DetailData.T_BHTT = tBhtt;

                    if (xn["T_BNCCT"] != null && Decimal.TryParse((xn["T_BNCCT"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tBncct))
                        xML2DetailData.T_BNCCT = tBncct;

                    if (xn["T_BNTT"] != null && Decimal.TryParse((xn["T_BNTT"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tBntt))
                        xML2DetailData.T_BNTT = tBntt;

                    if (xn["T_NGOAIDS"] != null && Decimal.TryParse((xn["T_NGOAIDS"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tNgoaiDs))
                        xML2DetailData.T_NGOAIDS = tNgoaiDs;

                    if (xn["T_NGUONKHAC"] != null && Decimal.TryParse((xn["T_NGUONKHAC"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tNguonKhac))
                        xML2DetailData.T_NGUONKHAC = tNguonKhac;

                    if (xn["THANH_TIEN"] != null && Decimal.TryParse((xn["THANH_TIEN"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out ThanhTien))
                        xML2DetailData.THANH_TIEN = ThanhTien;

                    xML2DetailData.TEN_THUOC = xn["TEN_THUOC"] != null ? xn["TEN_THUOC"].InnerText : "";
                    xML2DetailData.TT_THAU = xn["TT_THAU"] != null ? xn["TT_THAU"].InnerText : "";
                    xML2DetailData.TYLE_TT = xn["TYLE_TT"] != null ? xn["TYLE_TT"].InnerText : "";

                    dataSourceXML2Grid.Add(xML2DetailData);
                }
                Inventec.Common.Logging.LogSystem.Info("Dau vao XML2. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataSourceXML2Grid), dataSourceXML2Grid));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToXML3(XmlDocument xml)
        {
            try
            {
                XmlNodeList xnList = xml.SelectNodes("//DSACH_CHI_TIET_DVKT/CHI_TIET_DVKT");

                foreach (XmlNode xn in xnList)
                {
                    Decimal soluong, tBhtt, tBncct, tBntt, tNgoaiDs, tNguonKhac, DonGia, ThanhTien, tTranTT, mucHuong;

                    XML3DetailData xML3DetailData = new XML.XML3.XML3DetailData();
                    xML3DetailData.DON_VI_TINH = xn["DON_VI_TINH"] != null ? xn["DON_VI_TINH"].InnerText : "";
                    xML3DetailData.GOI_VTYT = xn["GOI_VTYT"] != null ? xn["GOI_VTYT"].InnerText : "";
                    xML3DetailData.MA_BAC_SI = xn["MA_BAC_SI"] != null ? xn["MA_BAC_SI"].InnerText : "";
                    xML3DetailData.MA_BENH = xn["MA_BENH"] != null ? xn["MA_BENH"].InnerText : "";
                    xML3DetailData.MA_DICH_VU = xn["MA_DICH_VU"] != null ? xn["MA_DICH_VU"].InnerText : "";
                    xML3DetailData.MA_GIUONG = xn["MA_GIUONG"] != null ? xn["MA_GIUONG"].InnerText : "";
                    xML3DetailData.MA_KHOA = xn["MA_KHOA"] != null ? xn["MA_KHOA"].InnerText : "";
                    xML3DetailData.MA_LK = xn["MA_LK"] != null ? xn["MA_LK"].InnerText : "";
                    xML3DetailData.MA_NHOM = xn["MA_NHOM"] != null ? xn["MA_NHOM"].InnerText : "";
                    xML3DetailData.MA_PTTT = xn["MA_PTTT"] != null ? xn["MA_PTTT"].InnerText : "";
                    xML3DetailData.MA_VAT_TU = xn["MA_VAT_TU"] != null ? xn["MA_VAT_TU"].InnerText : "";
                    xML3DetailData.NGAY_KQ = xn["NGAY_KQ"] != null ? xn["NGAY_KQ"].InnerText : "";
                    xML3DetailData.NGAY_YL = xn["NGAY_YL"] != null ? xn["NGAY_YL"].InnerText : "";
                    xML3DetailData.PHAM_VI = xn["PHAM_VI"] != null ? xn["PHAM_VI"].InnerText : "";

                    if (xn["MUC_HUONG"] != null && Decimal.TryParse((xn["MUC_HUONG"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out mucHuong))
                        xML3DetailData.MUC_HUONG = mucHuong;

                    if (xn["DON_GIA"] != null && Decimal.TryParse((xn["DON_GIA"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out DonGia))
                        xML3DetailData.DON_GIA = DonGia;

                    if (xn["SO_LUONG"] != null && Decimal.TryParse((xn["SO_LUONG"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out soluong))
                        xML3DetailData.SO_LUONG = soluong;

                    xML3DetailData.STT = xn["STT"].InnerText;

                    if (xn["T_BHTT"] != null && Decimal.TryParse((xn["T_BHTT"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tBhtt))
                        xML3DetailData.T_BHTT = tBhtt;

                    if (xn["T_BNCCT"] != null && Decimal.TryParse((xn["T_BNCCT"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tBncct))
                        xML3DetailData.T_BNCCT = tBncct;

                    if (xn["T_BNTT"] != null && Decimal.TryParse((xn["T_BNTT"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tBntt))
                        xML3DetailData.T_BNTT = tBntt;

                    if (xn["T_NGOAIDS"] != null && Decimal.TryParse((xn["T_NGOAIDS"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tNgoaiDs))
                        xML3DetailData.T_NGOAIDS = tNgoaiDs;

                    if (xn["T_NGUONKHAC"] != null && Decimal.TryParse((xn["T_NGUONKHAC"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tNguonKhac))
                        xML3DetailData.T_NGUONKHAC = tNguonKhac;

                    if (xn["THANH_TIEN"] != null && Decimal.TryParse((xn["THANH_TIEN"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out ThanhTien))
                        xML3DetailData.THANH_TIEN = ThanhTien;

                    if (xn["T_TRANTT"] != null && Decimal.TryParse((xn["T_TRANTT"].InnerText ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out tTranTT))
                        xML3DetailData.T_TRANTT = tTranTT;

                    xML3DetailData.STT = xn["STT"] != null ? xn["STT"].InnerText : "";

                    xML3DetailData.TEN_DICH_VU = xn["TEN_DICH_VU"] != null ? xn["TEN_DICH_VU"].InnerText : "";
                    xML3DetailData.TEN_VAT_TU = xn["TEN_VAT_TU"] != null ? xn["TEN_VAT_TU"].InnerText : "";
                    xML3DetailData.TT_THAU = xn["TT_THAU"] != null ? xn["TT_THAU"].InnerText : "";
                    xML3DetailData.TYLE_TT = xn["TYLE_TT"] != null ? xn["TYLE_TT"].InnerText : "";
                    dataSourceXML3Grid.Add(xML3DetailData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToXML4(XmlDocument xml)
        {
            try
            {
                XmlNodeList xnList = xml.SelectNodes("//DSACH_CHI_TIET_CLS/CHI_TIET_CLS");

                foreach (XmlNode xn in xnList)
                {
                    XML4DetailData xML4DetailData = new XML4DetailData();
                    xML4DetailData.GIA_TRI = xn["GIA_TRI"].InnerText;
                    xML4DetailData.KET_LUAN = xn["KET_LUAN"].InnerText;
                    xML4DetailData.MA_CHI_SO = xn["MA_CHI_SO"].InnerText;
                    xML4DetailData.MA_DICH_VU = xn["MA_DICH_VU"].InnerText;
                    xML4DetailData.MA_LK = xn["MA_LK"].InnerText;
                    xML4DetailData.MA_MAY = xn["MA_MAY"].InnerText;
                    xML4DetailData.MO_TA = xn["MO_TA"].InnerText;
                    xML4DetailData.NGAY_KQ = xn["NGAY_KQ"].InnerText;
                    xML4DetailData.STT = xn["STT"].InnerText;
                    xML4DetailData.TEN_CHI_SO = xn["TEN_CHI_SO"].InnerText;

                    dataSourceXML4Grid.Add(xML4DetailData);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToXML5(XmlDocument xml)
        {
            try
            {
                XmlNodeList xnList = xml.SelectNodes("//DSACH_CHI_TIET_DIEN_BIEN_BENH/CHI_TIET_DIEN_BIEN_BENH");

                foreach (XmlNode xn in xnList)
                {
                    XML5DetailData xML4DetailData = new XML5DetailData();
                    xML4DetailData.DIEN_BIEN = xn["DIEN_BIEN"].InnerText;
                    xML4DetailData.HOI_CHAN = xn["HOI_CHAN"].InnerText;
                    xML4DetailData.MA_LK = xn["MA_LK"].InnerText;
                    xML4DetailData.NGAY_YL = xn["NGAY_YL"].InnerText;
                    xML4DetailData.PHAU_THUAT = xn["PHAU_THUAT"].InnerText;
                    xML4DetailData.STT = xn["STT"].InnerText;


                    dataSourceXML5Grid.Add(xML4DetailData);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static object DeserializeXml(string xmlData, Type type)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(type);
            StringReader reader = new StringReader(xmlData);
            object obj = xmlSerializer.Deserialize(reader);

            return obj;
        }

        private static T ConvertNode<T>(XmlNode node) where T : class
        {
            MemoryStream stm = new MemoryStream();

            StreamWriter stw = new StreamWriter(stm);
            stw.Write(node.OuterXml);
            stw.Flush();

            stm.Position = 0;

            XmlSerializer ser = new XmlSerializer(typeof(T));
            T result = (ser.Deserialize(stm) as T);

            return result;
        }

        private void gridViewXML2_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (XML2DetailData)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "NGAY_YL_STR")
                        {
                            try
                            {
                                if (!String.IsNullOrWhiteSpace(data.NGAY_YL))
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Convert.ToInt64(data.NGAY_YL + "00"));
                                }
                                else
                                {
                                    e.Value = null;
                                }
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewXML3_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (XML3DetailData)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "NGAY_YL_STR")
                        {
                            try
                            {
                                if (!String.IsNullOrWhiteSpace(data.NGAY_YL))
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Convert.ToInt64(data.NGAY_YL + "00"));
                                }
                                else
                                {
                                    e.Value = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "NGAY_KQ_STR")
                        {
                            try
                            {
                                if (!String.IsNullOrWhiteSpace(data.NGAY_KQ))
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Convert.ToInt64(data.NGAY_KQ + "00"));
                                }
                                else
                                {
                                    e.Value = null;
                                }
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewXML4_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (XML4DetailData)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "NGAY_KQ_STR")
                        {
                            try
                            {
                                if (!String.IsNullOrWhiteSpace(data.NGAY_KQ))
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Convert.ToInt64(data.NGAY_KQ + "00"));
                                }
                                else
                                {
                                    e.Value = null;
                                }
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewXML5_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (XML5DetailData)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "NGAY_YL_STR")
                        {
                            try
                            {
                                if (!String.IsNullOrWhiteSpace(data.NGAY_YL))
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(Convert.ToInt64(data.NGAY_YL + "00"));
                                }
                                else
                                {
                                    e.Value = null;
                                }
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

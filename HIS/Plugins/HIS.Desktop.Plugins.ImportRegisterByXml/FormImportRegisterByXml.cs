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
using HIS.Desktop.Plugins.ImportRegisterByXml.Base;
using HIS.Desktop.Plugins.ImportRegisterByXml.XML.XML3;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;

namespace HIS.Desktop.Plugins.ImportRegisterByXml
{
    public partial class FormImportRegisterByXml : HIS.Desktop.Utility.FormBase
    {
        private string FolderPath;
        private string saveFileFolder = "success";
        private string mapFolder = "map";
        private string fileForMapAfterSave = @"map.txt";
        private string fileErrorAfterSave = @"error.txt";

        private string errorForCheck = "";

        private long roomId;
        private long departmentId;

        public FormImportRegisterByXml()
        {
            InitializeComponent();
        }

        public FormImportRegisterByXml(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.roomId = moduleData.RoomId;
            this.Text = moduleData.text;
        }

        private void FormImportRegisterByXml_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                var room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.roomId);
                if (room != null)
                {
                    this.departmentId = room.DEPARTMENT_ID;
                }

                if (Base.Config.DicRoomEmployee != null && Base.Config.DicRoomEmployee.Count > 0)
                {
                    BtnChooseExcelMap.Enabled = false;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnChooseExcelMap_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();
                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var ImpListProcessor = import.Get<Base.MapRoomEmployeeAdo>(0);
                        if (ImpListProcessor != null && ImpListProcessor.Count > 0)
                        {
                            Dictionary<string, string> dicRoomEmployee = new Dictionary<string, string>();
                            foreach (var item in ImpListProcessor)
                            {
                                if (!dicRoomEmployee.ContainsKey(item.MA_BAC_SI))
                                {
                                    dicRoomEmployee[item.MA_BAC_SI] = item.MA_PHONG_THUC_HIEN;
                                }
                            }
                            Base.Config.DicRoomEmployee = dicRoomEmployee;
                            BtnChooseExcelMap.Enabled = false;
                        }
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnChooseFolder_Click(object sender, EventArgs e)
        {
            try
            {
                using (var folderDialog = new FolderBrowserDialog())
                {
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        FolderPath = folderDialog.SelectedPath;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnChooseImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(FolderPath))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn folder Chứa XML4210");
                    return;
                }

                if (Base.Config.DicRoomEmployee == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn file excel ánh xạ \"mã bác sỹ - phòng thực hiện\"");
                    return;
                }

                WaitingManager.Show();
                var listFileXml = GetFile(FolderPath);

                if (listFileXml != null && listFileXml.Count > 0)
                {
                    long createSuccess = 0;
                    long createError = 0;
                    foreach (var fileName in listFileXml)
                    {
                        errorForCheck = "";
                        HisServiceReqExamRegisterSDO readXml = ProcessReadXml(fileName);
                        //File.Delete(fileName);

                        if (readXml == null)
                        {
                            var file = Path.GetFileName(fileName);

                            var value = file + "\t" + "readXml is null" + "\t" + errorForCheck;
                            WriteDataResult(value, fileErrorAfterSave);
                            createError += 1;
                            Inventec.Common.Logging.LogSystem.Error("readXml is null");
                            continue;
                        }

                        CommonParam param = new CommonParam();
                        var apiResult = new BackendAdapter(param).Post<HisServiceReqExamRegisterResultSDO>(HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_SERVICE_REQ_EXAMREGISTER, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, readXml, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                        if (apiResult != null)
                        {
                            var file = Path.GetFileName(fileName);
                            string successPath = Path.Combine(FolderPath, saveFileFolder);
                            if (!System.IO.Directory.Exists(successPath))
                                System.IO.Directory.CreateDirectory(successPath);
                            File.Copy(Path.Combine(FolderPath, file), Path.Combine(successPath, file), true);
                            File.Delete(Path.Combine(FolderPath, file));


                            var value = file + "\t" + apiResult.HisPatientProfile.HisTreatment.TREATMENT_CODE;
                            WriteDataResult(value, fileForMapAfterSave);
                            createSuccess += 1;
                        }
                        else
                        {
                            var file = Path.GetFileName(fileName);

                            var value = file + "\t" + string.Join("|", param.Messages) + "\t" + String.Join("|", param.BugCodes);
                            WriteDataResult(value, fileErrorAfterSave);

                            createError += 1;
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                        }
                    }
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format("Đã tiếp đón được {0} hồ sơ, lỗi {1} hồ sơ, trên tổng số {2} hồ sơ", createSuccess, createError, listFileXml.Count));
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void WriteDataResult(string value, string fileName)
        {
            try
            {
                var mapPath = Path.Combine(FolderPath, mapFolder);
                if (!System.IO.Directory.Exists(mapPath))
                    System.IO.Directory.CreateDirectory(mapPath);

                string fullFileName = Path.Combine(mapPath, fileName);
                if (!File.Exists(fullFileName))
                {
                    using (StreamWriter sw = new StreamWriter(fullFileName))
                    {
                        sw.WriteLine(" ");
                    }
                }

                List<string> result = new List<string>();
                using (StreamReader sr = new StreamReader(fullFileName))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        result.Add(line);
                    }
                }

                result.Add(value);
                using (StreamWriter sw = new StreamWriter(fullFileName))
                {
                    foreach (var item in result)
                    {
                        sw.WriteLine(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HisServiceReqExamRegisterSDO ProcessReadXml(string directoryViewFile)
        {
            HisServiceReqExamRegisterSDO result = null;
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(directoryViewFile);

                string directoryPathTemp = DecodeAndReplaceWithNode(xmldoc, directoryViewFile);

                if (!String.IsNullOrWhiteSpace(directoryPathTemp))
                {
                    result = DisplayXml(directoryPathTemp);
                }
                else
                {
                    errorForCheck += "Decode error ";
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private string DecodeAndReplaceWithNode(XmlDocument xmldoc, string directoryToSaveAs)
        {
            string result = "";
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
                Uri uri = new Uri(xmldoc.BaseURI);
                string filename = "";
                if (uri.IsFile)
                {
                    filename = System.IO.Path.GetFileName(uri.LocalPath);
                }

                result = CreateIfMissing(System.IO.Path.GetDirectoryName(directoryToSaveAs) + "\\Temp");
                result = result + "/" + filename;
                xmldoc.Save(result);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private string CreateIfMissing(string path)
        {
            string result = path;
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (Directory.Exists(path))
                {
                    File.SetAttributes(path, FileAttributes.Normal);
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
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

        private string Base64Decode(string base64EncodedData)
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

        private HisServiceReqExamRegisterSDO DisplayXml(string directoryViewFile)
        {
            HisServiceReqExamRegisterSDO result = null;
            try
            {
                string xmlString = System.IO.File.ReadAllText(directoryViewFile);

                XslCompiledTransform xTrans = new XslCompiledTransform();
                sample84(xTrans);

                // Read the xml string.
                StringReader sr = new StringReader(xmlString);
                XmlReader xReader = XmlReader.Create(sr);

                // Transform the XML data
                MemoryStream ms = new MemoryStream();
                xTrans.Transform(xReader, null, ms);

                ms.Position = 0;

                XmlDocument xmldoc = ReadXmlFromDirectory(directoryViewFile);

                result = FillDataToXML3(xmldoc);
                if (result != null)
                {
                    HisPatientProfileSDO patient = FillDataToXML1(xmldoc);
                    if (patient != null)
                    {
                        result.HisPatientProfile = patient;
                        result.IcdCode = patient.HisTreatment.ICD_CODE;
                        result.IcdName = patient.HisTreatment.ICD_NAME;
                        result.IcdSubCode = patient.HisTreatment.ICD_SUB_CODE;
                        result.IcdText = patient.HisTreatment.ICD_TEXT;
                    }
                    else
                    {
                        errorForCheck += "thong tin benh nhan loi ";
                        result = null;
                    }
                }
                else
                {
                    errorForCheck += "HisServiceReqExamRegisterSDO null ";
                }

                File.Delete(directoryViewFile);
            }
            catch (Exception ex)
            {
                errorForCheck += "DisplayXml ";
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private HisPatientProfileSDO FillDataToXML1(XmlDocument xmldoc)
        {
            HisPatientProfileSDO result = null;
            try
            {
                if (xmldoc != null)
                {
                    result = new HisPatientProfileSDO();
                    result.HisPatient = new MOS.EFMODEL.DataModels.HIS_PATIENT();
                    result.HisTreatment = new MOS.EFMODEL.DataModels.HIS_TREATMENT();
                    result.HisPatientTypeAlter = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER();

                    result.DepartmentId = this.departmentId;
                    result.RequestRoomId = this.roomId;
                    //result.HisPatient.PATIENT_CODE = GetValueByNode("MA_BN", xmldoc);


                    result.HisPatientTypeAlter.PATIENT_TYPE_ID = Base.Config.PatientTypeId_BHYT;

                    var maDkbd = GetValueByNode("MA_DKBD", xmldoc);
                    var mathe = GetValueByNode("MA_THE", xmldoc);
                    var hanTu = GetValueByNode("GT_THE_TU", xmldoc);
                    var hanDen = GetValueByNode("GT_THE_DEN", xmldoc);

                    var lstPatient = new List<HisPatientSDO>();
                    foreach (var item in mathe)
                    {
                        var patients = GetPatientByCard(mathe.Split(';').FirstOrDefault());
                        if (patients != null)
                        {
                            lstPatient.AddRange(patients);
                        }
                    }

                    if (lstPatient != null && lstPatient.Count > 0)
                    {
                        var patient = new MOS.EFMODEL.DataModels.HIS_PATIENT();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT>(patient, lstPatient.First());
                        result.HisPatient = patient;
                    }
                    else
                    {
                        string HO_TENstr = GetValueByNode("HO_TEN", xmldoc);
                        ProcessPatientName(ref result, HO_TENstr);

                        result.HisPatient.DOB = ProcessDateTime(GetValueByNode("NGAY_SINH", xmldoc));
                        string GIOI_TINHstr = GetValueByNode("GIOI_TINH", xmldoc);
                        result.HisPatient.GENDER_ID = !String.IsNullOrWhiteSpace(GIOI_TINHstr) && GIOI_TINHstr == "1" ? IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE : IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE;

                        result.HisPatient.ADDRESS = GetValueByNode("DIA_CHI", xmldoc);
                        ProcessAddress(result.HisPatient);
                    }

                    result.HisPatientTypeAlter.HEIN_CARD_NUMBER = mathe.Split(';').FirstOrDefault();
                    var timeFrom = ProcessDateTime(hanTu.Split(';').FirstOrDefault());
                    if (timeFrom > 0)
                    {
                        result.HisPatientTypeAlter.HEIN_CARD_FROM_TIME = timeFrom;
                    }

                    var timeto = ProcessDateTime(hanDen.Split(';').FirstOrDefault());
                    if (timeto > 0)
                    {
                        result.HisPatientTypeAlter.HEIN_CARD_TO_TIME = timeto;
                    }

                    result.HisPatientTypeAlter.HEIN_MEDI_ORG_CODE = maDkbd.Split(';').FirstOrDefault();

                    result.HisPatientTypeAlter.ADDRESS = GetValueByNode("DIA_CHI", xmldoc);

                    result.HisPatientTypeAlter.JOIN_5_YEAR = MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.FALSE;
                    result.HisPatientTypeAlter.PAID_6_MONTH = MOS.LibraryHein.Bhyt.HeinPaid6Month.HeinPaid6MonthCode.FALSE;

                    var miencungct = ProcessDateTime(GetValueByNode("MIEN_CUNG_CT", xmldoc));
                    if (miencungct > 0)
                    {
                        result.HisPatientTypeAlter.FREE_CO_PAID_TIME = miencungct;
                    }

                    if (result.HisPatientTypeAlter.FREE_CO_PAID_TIME.HasValue)
                    {
                        result.HisPatientTypeAlter.JOIN_5_YEAR = MOS.LibraryHein.Bhyt.HeinJoin5Year.HeinJoin5YearCode.TRUE;
                        result.HisPatientTypeAlter.PAID_6_MONTH = MOS.LibraryHein.Bhyt.HeinPaid6Month.HeinPaid6MonthCode.TRUE;
                    }

                    var mediorg = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(o => o.MEDI_ORG_CODE == result.HisPatientTypeAlter.HEIN_MEDI_ORG_CODE);
                    if (mediorg != null)
                    {
                        result.HisPatientTypeAlter.HEIN_MEDI_ORG_NAME = mediorg.MEDI_ORG_NAME;
                    }
                    string MA_LYDO_VVIENstr = GetValueByNode("MA_LYDO_VVIEN", xmldoc);

                    result.HisPatientTypeAlter.RIGHT_ROUTE_CODE = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE;

                    if (!maDkbd.Split(';').Contains(GetValueByNode("MA_CSKCB", xmldoc)))
                    {
                        result.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT;
                    }

                    if (MA_LYDO_VVIENstr == "3")
                    {
                        result.HisPatientTypeAlter.RIGHT_ROUTE_CODE = MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE;
                        result.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = null;
                    }
                    else if (MA_LYDO_VVIENstr == "4")
                    {
                        //result.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT;
                    }
                    else if (MA_LYDO_VVIENstr == "2")
                    {
                        result.HisTreatment.IS_EMERGENCY = 1;
                        result.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.EMERGENCY;
                    }

                    //string MIEN_CUNG_CTstr = GetValueByNode("MIEN_CUNG_CT", xmldoc);
                    result.HisTreatment.ICD_CODE = GetValueByNode("MA_BENH", xmldoc);
                    result.HisTreatment.ICD_SUB_CODE = GetValueByNode("MA_BENHKHAC", xmldoc);
                    result.TreatmentTime = ProcessDateTime(GetValueByNode("NGAY_VAO", xmldoc));

                    var tenbenhstr = GetValueByNode("TEN_BENH", xmldoc);
                    if (!String.IsNullOrWhiteSpace(tenbenhstr))
                    {
                        var benhs = tenbenhstr.Split(';');
                        result.HisTreatment.ICD_NAME = benhs[0];
                        if (benhs.Length > 2)
                        {
                            result.HisTreatment.ICD_TEXT = "";
                            for (int i = 1; i < benhs.Length; i++)
                            {
                                result.HisTreatment.ICD_TEXT = string.Format("{0};{1}", result.HisTreatment.ICD_TEXT, benhs[i]);
                            }
                        }
                    }

                    result.HisTreatment.TRANSFER_IN_MEDI_ORG_CODE = GetValueByNode("MA_NOI_CHUYEN", xmldoc);

                    if (!String.IsNullOrWhiteSpace(result.HisTreatment.TRANSFER_IN_MEDI_ORG_CODE))
                    {
                        result.HisTreatment.IS_TRANSFER_IN = 1;
                        var inMediorg = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(o => o.MEDI_ORG_CODE == result.HisTreatment.TRANSFER_IN_MEDI_ORG_CODE);
                        if (inMediorg != null)
                        {
                            result.HisTreatment.TRANSFER_IN_MEDI_ORG_NAME = inMediorg.MEDI_ORG_NAME;
                        }

                        result.HisTreatment.TRANSFER_IN_ICD_CODE = result.HisTreatment.ICD_CODE;
                        result.HisTreatment.TRANSFER_IN_ICD_NAME = result.HisTreatment.ICD_NAME;
                    }

                    //string KET_QUA_DTRIstr = GetValueByNode("KET_QUA_DTRI", xmldoc);
                    //if (KET_QUA_DTRIstr == "1") result.HisTreatment.TREATMENT_RESULT_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__KHOI;
                    //else if (KET_QUA_DTRIstr == "2") result.HisTreatment.TREATMENT_RESULT_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__DO;
                    //else if (KET_QUA_DTRIstr == "3") result.HisTreatment.TREATMENT_RESULT_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__KTD;
                    //else if (KET_QUA_DTRIstr == "4") result.HisTreatment.TREATMENT_RESULT_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__NANG;
                    //else if (KET_QUA_DTRIstr == "4") result.HisTreatment.TREATMENT_RESULT_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_RESULT.ID__CHET;

                    //string TINH_TRANG_RVstr = GetValueByNode("TINH_TRANG_RV", xmldoc);
                    //if (TINH_TRANG_RVstr == "2") result.HisTreatment.TREATMENT_END_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN;
                    //else if (TINH_TRANG_RVstr == "3") result.HisTreatment.TREATMENT_END_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__TRON;
                    //else if (TINH_TRANG_RVstr == "4") result.HisTreatment.TREATMENT_END_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN;
                    //else result.HisTreatment.TREATMENT_END_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN;

                    string MA_LOAI_KCBstr = GetValueByNode("MA_LOAI_KCB", xmldoc);
                    if (!string.IsNullOrWhiteSpace(MA_LOAI_KCBstr))
                    {
                        if (MA_LOAI_KCBstr == "1")
                        {
                            result.HisPatientTypeAlter.TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
                        }
                        else if (MA_LOAI_KCBstr == "2")
                        {
                            result.HisPatientTypeAlter.TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU;
                        }
                        else if (MA_LOAI_KCBstr == "3")
                        {
                            result.HisPatientTypeAlter.TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU;
                        }
                    }
                }
                else
                { errorForCheck += "Loi doc file xml1 "; }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<HisPatientSDO> GetPatientByCard(string card)
        {
            List<HisPatientSDO> result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientAdvanceFilter hisPatientFilter = new MOS.Filter.HisPatientAdvanceFilter();
                hisPatientFilter.HEIN_CARD_NUMBER__EXACT = card;
                result = new BackendAdapter(param).Get<List<HisPatientSDO>>("api/HisPatient/GetSdoAdvance", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, hisPatientFilter, param);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static void ProcessAddress(MOS.EFMODEL.DataModels.HIS_PATIENT entity)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(entity.COMMUNE_NAME) && String.IsNullOrWhiteSpace(entity.DISTRICT_NAME)
                    && String.IsNullOrWhiteSpace(entity.PROVINCE_NAME) && !String.IsNullOrWhiteSpace(entity.ADDRESS))
                {
                    string provice = "";
                    string provice_code = "";
                    string district = "";
                    string district_code = "";
                    int index_district = 0;
                    string commune = "";
                    int index_commune = 0;
                    string address = "";

                    var fullAddress = ProcessName(entity.ADDRESS);

                    foreach (var item in Config.DistrictCFG.INITIAL_NAMEs)
                    {
                        index_district = fullAddress.IndexOf(item);
                        if (index_district >= 0) break;
                    }

                    //tu vi tri huyen den cuoi lay ra tinh huyen
                    if (index_district >= 0)
                    {
                        var district_provice = fullAddress.Substring(index_district, fullAddress.Length - index_district);
                        List<SDA.EFMODEL.DataModels.SDA_DISTRICT> currentSdaDistrict = new List<SDA.EFMODEL.DataModels.SDA_DISTRICT>();

                        if (!String.IsNullOrWhiteSpace(district_provice))
                        {
                            var listProvince = Config.ProvinceCFG.SDA_PROVINCEs.Where(o => district_provice.Contains(o.PROVINCE_NAME)).ToList();
                            if (listProvince != null && listProvince.Count > 0)
                            {
                                var lisDistrict = Config.DistrictCFG.SDA_DISTRICTs.Where(o => listProvince.Select(s => s.ID).Contains(o.PROVINCE_ID)).ToList();
                                if (lisDistrict != null && lisDistrict.Count > 0)
                                {
                                    currentSdaDistrict = lisDistrict.Where(o => district_provice.Contains(o.DISTRICT_NAME)).ToList();
                                    if (currentSdaDistrict != null && currentSdaDistrict.Count > 0)
                                    {
                                        if (currentSdaDistrict.Count == 1)
                                        {
                                            district = string.Format("{0} {1}", currentSdaDistrict.First().INITIAL_NAME, currentSdaDistrict.First().DISTRICT_NAME);
                                            district_code = currentSdaDistrict.First().DISTRICT_CODE;
                                            provice = listProvince.FirstOrDefault(o => o.ID == currentSdaDistrict.First().PROVINCE_ID).PROVINCE_NAME;
                                            provice_code = listProvince.FirstOrDefault(o => o.ID == currentSdaDistrict.First().PROVINCE_ID).PROVINCE_CODE;
                                        }
                                    }
                                }
                            }
                        }

                        var address_commune = fullAddress.Substring(0, index_district);
                        if (!String.IsNullOrWhiteSpace(address_commune))
                        {
                            foreach (var item in Config.CommuneCFG.INITIAL_NAMEs)
                            {
                                index_commune = address_commune.IndexOf(item);
                                if (index_commune >= 0) break;
                            }

                            if (index_commune >= 0)
                            {
                                var currCommune = address_commune.Substring(index_commune, address_commune.Length - index_commune);

                                if (!String.IsNullOrWhiteSpace(currCommune))
                                {
                                    //xu lý ten dua ve khong co dau noi ( ",";"-")
                                    currCommune = ProcessNameRemoveChar(currCommune);

                                    List<SDA.EFMODEL.DataModels.SDA_COMMUNE> listCommune = new List<SDA.EFMODEL.DataModels.SDA_COMMUNE>();

                                    if (currentSdaDistrict != null && currentSdaDistrict.Count > 0)
                                    {
                                        listCommune = Config.CommuneCFG.SDA_COMMUNEs.Where(o => currentSdaDistrict.Select(s => s.ID).Contains(o.DISTRICT_ID)).ToList();
                                    }
                                    else
                                    {
                                        listCommune = Config.CommuneCFG.SDA_COMMUNEs;
                                    }

                                    var currentSdaCommune = listCommune.Where(o => currCommune.Contains(o.COMMUNE_NAME)).ToList();
                                    if (currentSdaCommune != null && currentSdaCommune.Count > 0)
                                    {
                                        SDA.EFMODEL.DataModels.SDA_COMMUNE sdaCommune = new SDA.EFMODEL.DataModels.SDA_COMMUNE();
                                        if (currentSdaCommune.Count > 1)
                                        {
                                            sdaCommune = currentSdaCommune.FirstOrDefault(o => string.Format("{0} {1}", o.INITIAL_NAME, o.COMMUNE_NAME) == currCommune);
                                        }
                                        else
                                        {
                                            sdaCommune = currentSdaCommune.First();
                                        }

                                        commune = string.Format("{0} {1}", sdaCommune.INITIAL_NAME, sdaCommune.COMMUNE_NAME);
                                        if (String.IsNullOrWhiteSpace(district) || String.IsNullOrWhiteSpace(provice))
                                        {
                                            var sdaDistric = Config.DistrictCFG.SDA_DISTRICTs.FirstOrDefault(o => o.ID == sdaCommune.DISTRICT_ID);

                                            district = string.Format("{0} {1}", sdaDistric.INITIAL_NAME, sdaDistric.DISTRICT_NAME);
                                            district_code = sdaDistric.DISTRICT_CODE;
                                            provice = Config.ProvinceCFG.SDA_PROVINCEs.FirstOrDefault(o => o.ID == sdaDistric.PROVINCE_ID).PROVINCE_NAME;
                                            provice_code = Config.ProvinceCFG.SDA_PROVINCEs.FirstOrDefault(o => o.ID == sdaDistric.PROVINCE_ID).PROVINCE_CODE;
                                        }
                                    }
                                    else
                                    {
                                        commune = currCommune;
                                    }
                                }

                                address = address_commune.Substring(0, index_commune);
                            }
                            else
                                address = address_commune;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(provice))
                    {
                        entity.PROVINCE_NAME = provice;
                        entity.PROVINCE_CODE = provice_code;
                    }

                    if (!string.IsNullOrWhiteSpace(district))
                    {
                        entity.DISTRICT_NAME = district;
                        entity.DISTRICT_CODE = district_code;
                    }

                    if (!String.IsNullOrWhiteSpace(commune))
                    {
                        entity.COMMUNE_NAME = commune;
                    }

                    if (!String.IsNullOrWhiteSpace(provice) || !string.IsNullOrWhiteSpace(district) || !String.IsNullOrWhiteSpace(commune))
                    {
                        entity.ADDRESS = address;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string ProcessNameRemoveChar(string name)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(name))
                {
                    var a = name.Trim();
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (Char.IsLetter(a[i]))
                        {
                            result += a[i];
                        }
                        else if (Char.IsWhiteSpace(a[i]))
                        {
                            result += a[i];
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        result = ProcessName(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result.Trim();
        }

        internal static string ProcessName(string data)
        {
            string result = null;
            try
            {
                if (!String.IsNullOrWhiteSpace(data))
                {
                    var listData = data.Split(' ');
                    if (listData != null && listData.Count() > 0)
                    {
                        result = "";
                        foreach (var item in listData)
                        {
                            if (item != "")
                            {
                                result += (Inventec.Common.String.Convert.FirstCharToUpper(item.ToLower()) + " ");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
                result = null;
            }

            if (result == null)
                return null;
            else
                return result.Trim();
        }

        private long ProcessDateTime(string NGAYstr)
        {
            long result = 0;
            try
            {
                if (!string.IsNullOrWhiteSpace(NGAYstr))
                {
                    string ngaynew = NGAYstr;
                    if (NGAYstr.Length < 14)
                    {
                        for (int i = 0; i < 14 - NGAYstr.Length; i++)
                        {
                            ngaynew += "0";
                        }
                    }
                    result = Convert.ToInt64(ngaynew);
                }
            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessPatientName(ref HisPatientProfileSDO result, string HO_TENstr)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(HO_TENstr))
                {
                    string[] name = HO_TENstr.Trim().Split(' ');
                    if (name.Count() > 1)
                    {
                        for (int i = 0; i < name.Length; i++)
                        {
                            if (i == name.Length - 1)
                            {
                                result.HisPatient.FIRST_NAME = name[i].ToUpper();
                            }
                            else
                            {
                                if (String.IsNullOrWhiteSpace(result.HisPatient.LAST_NAME))
                                    result.HisPatient.LAST_NAME = name[i].ToUpper();
                                else
                                    result.HisPatient.LAST_NAME = string.Format("{0} {1}", result.HisPatient.LAST_NAME, name[i].ToUpper());
                            }
                        }
                    }
                    else
                    {
                        result.HisPatient.FIRST_NAME = name[0].ToUpper();
                    }

                    result.HisPatient.FIRST_NAME = result.HisPatient.FIRST_NAME.Trim();
                    result.HisPatient.LAST_NAME = result.HisPatient.LAST_NAME.Trim();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HisServiceReqExamRegisterSDO FillDataToXML3(XmlDocument xml)
        {
            HisServiceReqExamRegisterSDO result = null;
            try
            {
                if (xml != null)
                {
                    XmlNodeList xnList = xml.SelectNodes("//DSACH_CHI_TIET_DVKT/CHI_TIET_DVKT");
                    List<XML3DetailData> dataSource = new List<XML3DetailData>();
                    System.Globalization.NumberStyles style = System.Globalization.NumberStyles.Any;
                    foreach (XmlNode xn in xnList)
                    {
                        Decimal soluong, tBhtt, tBncct, tBntt, tNgoaiDs, tNguonKhac, DonGia, ThanhTien, tTranTT, mucHuong;

                        XML3DetailData xML3DetailData = new XML.XML3.XML3DetailData();
                        xML3DetailData.DON_VI_TINH = GetDataNode(xn, "DON_VI_TINH");
                        xML3DetailData.GOI_VTYT = GetDataNode(xn, "GOI_VTYT");
                        xML3DetailData.MA_BAC_SI = GetDataNode(xn, "MA_BAC_SI");
                        xML3DetailData.MA_BENH = GetDataNode(xn, "MA_BENH");
                        xML3DetailData.MA_DICH_VU = GetDataNode(xn, "MA_DICH_VU");
                        xML3DetailData.MA_GIUONG = GetDataNode(xn, "MA_GIUONG");
                        xML3DetailData.MA_KHOA = GetDataNode(xn, "MA_KHOA");
                        xML3DetailData.MA_LK = GetDataNode(xn, "MA_LK");
                        xML3DetailData.MA_NHOM = GetDataNode(xn, "MA_NHOM");
                        xML3DetailData.MA_PTTT = GetDataNode(xn, "MA_PTTT");
                        xML3DetailData.MA_VAT_TU = GetDataNode(xn, "MA_VAT_TU");
                        xML3DetailData.NGAY_KQ = GetDataNode(xn, "NGAY_KQ");
                        xML3DetailData.NGAY_YL = GetDataNode(xn, "NGAY_YL");
                        xML3DetailData.PHAM_VI = GetDataNode(xn, "PHAM_VI");

                        if (Decimal.TryParse((GetDataNode(xn, "MUC_HUONG")).Replace('.', ','), style, LanguageManager.GetCulture(), out mucHuong))
                            xML3DetailData.MUC_HUONG = mucHuong;

                        if (Decimal.TryParse((GetDataNode(xn, "DON_GIA")).Replace('.', ','), style, LanguageManager.GetCulture(), out DonGia))
                            xML3DetailData.DON_GIA = DonGia;

                        if (Decimal.TryParse((GetDataNode(xn, "SO_LUONG")).Replace('.', ','), style, LanguageManager.GetCulture(), out soluong))
                            xML3DetailData.SO_LUONG = soluong;

                        if (Decimal.TryParse((GetDataNode(xn, "T_BHTT")).Replace('.', ','), style, LanguageManager.GetCulture(), out tBhtt))
                            xML3DetailData.T_BHTT = tBhtt;

                        if (Decimal.TryParse((GetDataNode(xn, "T_BNCCT")).Replace('.', ','), style, LanguageManager.GetCulture(), out tBncct))
                            xML3DetailData.T_BNCCT = tBncct;

                        if (Decimal.TryParse((GetDataNode(xn, "T_BNTT")).Replace('.', ','), style, LanguageManager.GetCulture(), out tBntt))
                            xML3DetailData.T_BNTT = tBntt;

                        if (Decimal.TryParse((GetDataNode(xn, "T_NGOAIDS")).Replace('.', ','), style, LanguageManager.GetCulture(), out tNgoaiDs))
                            xML3DetailData.T_NGOAIDS = tNgoaiDs;

                        if (Decimal.TryParse((GetDataNode(xn, "T_NGUONKHAC")).Replace('.', ','), style, LanguageManager.GetCulture(), out tNguonKhac))
                            xML3DetailData.T_NGUONKHAC = tNguonKhac;

                        if (Decimal.TryParse((GetDataNode(xn, "THANH_TIEN")).Replace('.', ','), style, LanguageManager.GetCulture(), out ThanhTien))
                            xML3DetailData.THANH_TIEN = ThanhTien;

                        if (Decimal.TryParse((GetDataNode(xn, "T_TRANTT")).Replace('.', ','), style, LanguageManager.GetCulture(), out tTranTT))
                            xML3DetailData.T_TRANTT = tTranTT;

                        xML3DetailData.STT = GetDataNode(xn, "STT");

                        xML3DetailData.TEN_DICH_VU = GetDataNode(xn, "TEN_DICH_VU");
                        xML3DetailData.TEN_VAT_TU = GetDataNode(xn, "TEN_VAT_TU");
                        xML3DetailData.TT_THAU = GetDataNode(xn, "TT_THAU");
                        xML3DetailData.TYLE_TT = GetDataNode(xn, "TYLE_TT");
                        dataSource.Add(xML3DetailData);
                    }

                    if (dataSource != null && dataSource.Count > 0)
                    {
                        dataSource = dataSource.OrderBy(o => o.NGAY_YL).ThenBy(o => o.STT).ToList();
                        foreach (var item in dataSource)
                        {
                            if (item.TYLE_TT == "100" && String.IsNullOrWhiteSpace(item.MA_VAT_TU) && !String.IsNullOrWhiteSpace(item.MA_DICH_VU) && Base.Config.ListHeinServiceTypeExam.Contains(item.MA_DICH_VU))
                            {
                                result = new HisServiceReqExamRegisterSDO();
                                result.ServiceReqDetails = new List<ServiceReqDetailSDO>();
                                var serviceReq = new ServiceReqDetailSDO();
                                serviceReq.Amount = item.SO_LUONG;
                                serviceReq.PatientTypeId = Base.Config.PatientTypeId_BHYT;
                                var listService = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>();
                                if (listService != null && listService.Count > 0)
                                {
                                    var services = listService.Where(o => o.HEIN_SERVICE_BHYT_CODE == item.MA_DICH_VU && o.HEIN_SERVICE_BHYT_NAME == item.TEN_DICH_VU
                                        && o.IS_LEAF == 1).ToList();

                                    string roomcode = Config.DicRoomEmployee.ContainsKey(item.MA_BAC_SI) ? Config.DicRoomEmployee[item.MA_BAC_SI] : "";
                                    string[] roomcodes = roomcode.Split(',');
                                    roomcodes = roomcodes.Distinct().ToArray();
                                    var rooms = new List<V_HIS_SERVICE_ROOM>();
                                    if (roomcodes.Count() > 0)
                                    {
                                        rooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>().Where(o => roomcodes.Contains(o.ROOM_CODE) && o.IS_LEAF == 1 && o.IS_ACTIVE == 1).ToList();
                                    }

                                    if (rooms != null && services != null)
                                    {
                                        List<long> serviceIds = new List<long>();
                                        serviceIds = services.Select(o => o.ID).ToList();

                                        var room = rooms.Where(o => serviceIds.Contains(o.SERVICE_ID)).ToList();
                                        if (room != null && room.Count > 0)
                                        {
                                            if (room.Count == 1)
                                            {
                                                serviceReq.RoomId = room.First().ROOM_ID;
                                                serviceReq.ServiceId = room.First().SERVICE_ID;
                                            }
                                            else
                                            {
                                                serviceReq.RoomId = room.FirstOrDefault().ROOM_ID;
                                                serviceReq.ServiceId = room.FirstOrDefault().SERVICE_ID;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        errorForCheck += "khong tim thay dich vu, phong rooms null |";
                                    }
                                }

                                if (serviceReq != null && serviceReq.RoomId.HasValue && serviceReq.RoomId.Value > 0)
                                {
                                    result.RequestRoomId = this.roomId;
                                    result.ServiceReqDetails.Add(serviceReq);
                                    result.IcdCode = item.MA_BENH;
                                    result.InstructionTime = ProcessDateTime(item.NGAY_YL);
                                    result.IsNoExecute = false;
                                    result.RequestLoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                    result.RequestUserName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                                }
                                else
                                {
                                    errorForCheck += "khong tim thay dich vu, phong serviceReq null |";
                                    result = null;
                                }
                                break;
                            }
                        }

                        if (result == null)
                        {
                            errorForCheck += "khong co dich vu kham |";
                        }
                    }
                    else
                    { errorForCheck += "khong co dich vu nao| "; }
                }
                else
                { errorForCheck += "Loi doc file xml3 xml null| "; }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private string GetDataNode(XmlNode xn, string p)
        {
            string result = "";
            try
            {
                if (xn != null && !String.IsNullOrWhiteSpace(p))
                {
                    result = xn[p].InnerText;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
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

        public static void sample84(XslCompiledTransform objXslTrans)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(XML.XLSDefault.xlsFormat) && XML.XLSDefault.xlsFormat.Length > 0 && objXslTrans != null)
                {
                    objXslTrans.Load(new XmlTextReader(new StringReader(XML.XLSDefault.xlsFormat)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private List<string> GetFile(string FolderPath)
        {
            List<string> result = new List<string>();
            try
            {
                if (!System.IO.Directory.Exists(FolderPath))
                {
                    throw new Exception("Khong ton tai folder can doc ket qua xet nghiem: " + FolderPath);
                }
                string[] fileEntries = System.IO.Directory.GetFiles(FolderPath).OrderBy(f => f).ToArray();

                if (fileEntries == null || fileEntries.Length == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Folder cần đọc không có file nào: " + FolderPath);
                }
                foreach (var item in fileEntries)
                {
                    if (item.Substring(item.Length - 4, 4).ToLower() == ".xml")
                    {
                        result.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

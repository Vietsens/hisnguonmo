using Aspose.Cells;
using DevExpress.Data.Browsing;
using DevExpress.XtraExport;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace HIS.Desktop.Plugins.ExportXmlQD130
{
    public partial class UCExportXml : HIS.Desktop.Utility.UserControlBase
    {
        string Aspose_Key =
            "PExpY2Vuc2U+DQogIDxEYXRhPg0KICAgIDxMaWNlbnNlZFRvPkFzcG9zZSBTY290bGFuZCB" +
            "UZWFtPC9MaWNlbnNlZFRvPg0KICAgIDxFbWFpbFRvPmJpbGx5Lmx1bmRpZUBhc3Bvc2UuY2" +
            "9tPC9FbWFpbFRvPg0KICAgIDxMaWNlbnNlVHlwZT5EZXZlbG9wZXIgT0VNPC9MaWNlbnNlV" +
            "HlwZT4NCiAgICA8TGljZW5zZU5vdGU+TGltaXRlZCB0byAxIGRldmVsb3BlciwgdW5saW1p" +
            "dGVkIHBoeXNpY2FsIGxvY2F0aW9uczwvTGljZW5zZU5vdGU+DQogICAgPE9yZGVySUQ+MTQ" +
            "wNDA4MDUyMzI0PC9PcmRlcklEPg0KICAgIDxVc2VySUQ+OTQyMzY8L1VzZXJJRD4NCiAgIC" +
            "A8T0VNPlRoaXMgaXMgYSByZWRpc3RyaWJ1dGFibGUgbGljZW5zZTwvT0VNPg0KICAgIDxQc" +
            "m9kdWN0cz4NCiAgICAgIDxQcm9kdWN0PkFzcG9zZS5Ub3RhbCBmb3IgLk5FVDwvUHJvZHVj" +
            "dD4NCiAgICA8L1Byb2R1Y3RzPg0KICAgIDxFZGl0aW9uVHlwZT5FbnRlcnByaXNlPC9FZGl" +
            "0aW9uVHlwZT4NCiAgICA8U2VyaWFsTnVtYmVyPjlhNTk1NDdjLTQxZjAtNDI4Yi1iYTcyLT" +
            "djNDM2OGYxNTFkNzwvU2VyaWFsTnVtYmVyPg0KICAgIDxTdWJzY3JpcHRpb25FeHBpcnk+M" +
            "jAxNTEyMzE8L1N1YnNjcmlwdGlvbkV4cGlyeT4NCiAgICA8TGljZW5zZVZlcnNpb24+My4w" +
            "PC9MaWNlbnNlVmVyc2lvbj4NCiAgICA8TGljZW5zZUluc3RydWN0aW9ucz5odHRwOi8vd3d" +
            "3LmFzcG9zZS5jb20vY29ycG9yYXRlL3B1cmNoYXNlL2xpY2Vuc2UtaW5zdHJ1Y3Rpb25zLm" +
            "FzcHg8L0xpY2Vuc2VJbnN0cnVjdGlvbnM+DQogIDwvRGF0YT4NCiAgPFNpZ25hdHVyZT5GT" +
            "zNQSHNibGdEdDhGNTlzTVQxbDFhbXlpOXFrMlY2RThkUWtJUDdMZFRKU3hEaWJORUZ1MXpP" +
            "aW5RYnFGZkt2L3J1dHR2Y3hvUk9rYzF0VWUwRHRPNmNQMVpmNkowVmVtZ1NZOGkvTFpFQ1R" +
            "Hc3pScUpWUVJaME1vVm5CaHVQQUprNWVsaTdmaFZjRjhoV2QzRTRYUTNMemZtSkN1YWoyTk" +
            "V0ZVJpNUhyZmc9PC9TaWduYXR1cmU+DQo8L0xpY2Vuc2U+";

        BackgroundWorker backgroundWorkerExel = null;
        string PathTempXml = null;
        bool IsProcessingExcel = false;
        public void ProcessDataExcel()
        {
            try
            {
                if (listSelection == null || listSelection.Count == 0) return;
                if (backgroundWorkerExel != null && backgroundWorkerExel.IsBusy)
                    return;
                backgroundWorkerExel = new System.ComponentModel.BackgroundWorker();
                backgroundWorkerExel.WorkerReportsProgress = true; // <-- Ensure this is set before using ReportProgress
                WaitingManager.Show();
                if (backgroundWorkerExel != null && backgroundWorkerExel.IsBusy)
                    return;
                SetLicenseForAsposeCell();
                PathTempXml = Path.Combine(Application.StartupPath + @"\Excel130", DateTime.Now.ToString("yyyyMMdd"));
                if (!System.IO.Directory.Exists(PathTempXml))
                    System.IO.Directory.CreateDirectory(PathTempXml);
                else
                    DeleteXmlFilesInPathTempXml();
                listWorkbooks = new List<WorkbookADO>();
                wokAll = new Workbook();
                Inventec.Common.Logging.LogSystem.Info("ProcessDataExcel Begin");
                this.backgroundWorkerExel.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerExel_DoWork);
                this.backgroundWorkerExel.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerExel_ProgressChanged);
                this.backgroundWorkerExel.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerExel_RunWorkerCompleted);
                Inventec.Common.Logging.LogSystem.Info("ProcessDataExcel End");
                this.backgroundWorkerExel.RunWorkerAsync();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void backgroundWorkerExel_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void backgroundWorkerExel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                foreach (var item in listWorkbooks)
                {
                    item.workbook.Save(PathTempXml + @"\" + item.TreatmentCode + ".xlsx");
                }
                if (listWorkbooks != null && listWorkbooks.Count > 0)
                {
                    wokAll.Worksheets.RemoveAt(0);
                    wokAll.Save(PathTempXml + @"\" + "DATA_XML_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");
                }
                MessageManager.Show(paramExcel, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string TreatmentCode = "";
        List<WorkbookADO> listWorkbooks = new List<WorkbookADO>();
        List<Workbook> listWorkbooks12 = new List<Workbook>();
        Workbook wokAll = new Workbook();
        CommonParam paramExcel = new CommonParam();
        string saveFileExcel = "";
        string saveFileExcel12 = "";
        private void backgroundWorkerExel_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                IsProcessingExcel = true;
                MemoryStream memoryStreamExcel = new MemoryStream();
                var success = this.GenerateXmlPlus(ref paramExcel, ref memoryStreamExcel, true, listSelection);
                if (!success)
                    return;
                paramExcel = new CommonParam();
                His.Bhyt.ExportXml.XML130.CreateXmlProcessor xmlProcessor = new His.Bhyt.ExportXml.XML130.CreateXmlProcessor(null);
                string xmlFile = File.ReadAllText(saveFileExcel);
                if (string.IsNullOrEmpty(xmlFile))
                    return;
                var hoso = xmlProcessor.GetDataFromString(xmlFile);
                foreach (var hoSo in hoso.THONGTINHOSO.DANHSACHHOSO.HOSO)
                {
                    // Tạo số lượng sheet theo số lượng fileHoSo
                    Workbook workbookCurrent = new Workbook();
                    int fileHoSoCount = hoSo.FILEHOSO.Count;
                    // Tạo sheet cho từng fileHoSo
                    for (int s = 0; s < fileHoSoCount; s++)
                    {
                        workbookCurrent.Worksheets.Add();
                    }

                    int i = 0;
                    TreatmentCode = "";
                    foreach (var fileHoSo in hoSo.FILEHOSO)
                    {
                        List<object> listObj = new List<object>();
                        switch (fileHoSo.LOAIHOSO)
                        {
                            case "XML1":
                                His.Bhyt.ExportXml.XML130.XML1.CreateXmlMain xmlMain1 = new His.Bhyt.ExportXml.XML130.XML1.CreateXmlMain();
                                listObj.Add(xmlMain1.RunXml1Data(fileHoSo.NOIDUNGFILE));
                                List<string> maLKList = new List<string>();
                                foreach (var obj in listObj)
                                {
                                    var prop = obj.GetType().GetProperty("MA_LK");
                                    if (prop != null)
                                    {
                                        var value = prop.GetValue(obj, null);
                                        if (value != null)
                                        {
                                            TreatmentCode = value.ToString();
                                            break;
                                        }
                                    }
                                }
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
                        var sheet = workbookCurrent.Worksheets[i];
                        sheet.Name = fileHoSo.LOAIHOSO;
                        Cells cells = sheet.Cells;
                        // Tạo các cột theo các trường trong listObj
                        if (listObj.Count > 0)
                        {
                            var firstObj = listObj[0];
                            var properties = firstObj.GetType().GetProperties();
                            // Tạo header
                            for (int col = 0; col < properties.Length; col++)
                            {
                                cells[0, col].Value = properties[col].Name;
                            }
                            // Push dữ liệu
                            for (int row = 0; row < listObj.Count; row++)
                            {
                                var obj = listObj[row];
                                for (int col = 0; col < properties.Length; col++)
                                {
                                    var value = properties[col].GetValue(obj, null);
                                    if (value is XmlCDataSection)
                                    {
                                        var cdata = value as XmlCDataSection;
                                        cells[row + 1, col].Value = cdata.Value;
                                    }
                                    else
                                    {
                                        cells[row + 1, col].Value = value;
                                    }
                                }
                            }


                            string sheetName = fileHoSo.LOAIHOSO;
                            Worksheet sheetAll;
                            int sheetIndex = wokAll.Worksheets == null || wokAll.Worksheets.Count == 0 || wokAll.Worksheets[sheetName] == null || wokAll.Worksheets[sheetName].Index == -1 ? -1 : wokAll.Worksheets[sheetName] != null ? wokAll.Worksheets[sheetName].Index : -1;
                            if (sheetIndex == -1)
                            {
                                sheetIndex = wokAll.Worksheets.Add();
                                sheetAll = wokAll.Worksheets[sheetIndex];
                                sheetAll.Name = sheetName;
                            }
                            else
                            {
                                sheetAll = wokAll.Worksheets[sheetIndex];
                            }

                            Cells cellsAll = sheetAll.Cells;
                            int startRow = 0;
                            if (cellsAll.MaxDataRow >= 0)
                                startRow = cellsAll.MaxDataRow + 1;

                            if (listObj.Count > 0)
                            {

                                // Nếu sheet mới, tạo header
                                if (startRow == 0)
                                {
                                    for (int col = 0; col < properties.Length; col++)
                                    {
                                        cellsAll[0, col].Value = properties[col].Name;
                                    }
                                    startRow = 1;
                                }

                                // Thêm dữ liệu vào dòng cuối cùng
                                for (int row = 0; row < listObj.Count; row++)
                                {
                                    var obj = listObj[row];
                                    for (int col = 0; col < properties.Length; col++)
                                    {
                                        var value = properties[col].GetValue(obj, null);
                                        if (value is XmlCDataSection)
                                        {
                                            var cdata = value as XmlCDataSection;
                                            cellsAll[startRow + row, col].Value = cdata.Value;
                                        }
                                        else
                                        {
                                            cellsAll[startRow + row, col].Value = value;
                                        }
                                    }
                                }
                            }

                        }
                        i++;
                    }

                    // Xóa 1 sheet cuối của workbookCurrent
                    if (workbookCurrent.Worksheets.Count >= 1)
                    {
                        workbookCurrent.Worksheets.RemoveAt(workbookCurrent.Worksheets.Count - 1);
                    }
                    listWorkbooks.Add(new WorkbookADO() { workbook = workbookCurrent, TreatmentCode = TreatmentCode });
                }
                Inventec.Common.Logging.LogSystem.Error("backgroundWorkerExel_DoWork__3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void DeleteXmlFilesInPathTempXml()
        {
            try
            {
                if (string.IsNullOrEmpty(PathTempXml) || !Directory.Exists(PathTempXml))
                    return;

                var xmlFiles = Directory.GetFiles(PathTempXml, "*", SearchOption.TopDirectoryOnly);
                foreach (var file in xmlFiles)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetLicenseForAsposeCell()
        {
            try
            {
                if (!String.IsNullOrEmpty(Aspose_Key))
                {
                    Stream Aspose_LStream = (Stream)new MemoryStream(Convert.FromBase64String(Aspose_Key));
                    Aspose.Cells.License license = new Aspose.Cells.License();
                    license.SetLicense(Aspose_LStream);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
    public class WorkbookADO
    {
        public Workbook workbook { get; set; }
        public string TreatmentCode { get; set; }
    }
}

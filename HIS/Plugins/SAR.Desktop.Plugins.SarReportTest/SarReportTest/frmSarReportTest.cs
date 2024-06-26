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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Common.Modules;
using Inventec.UC.Paging;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.BackendData;
using SAR.EFMODEL.DataModels;
using System.IO;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;
using FlexCel.XlsAdapter;
using SAR.Desktop.Plugins.SarReportTest.ADO;
using SAR.Filter;

namespace SAR.Desktop.Plugins.SarReportTest.SarReportTest
{
    public partial class frmSarReportTest : HIS.Desktop.Utility.FormBase
    {
        #region Declare reclare
        const string Status__Finish = "Đã chạy kiểm thử";
        const string Status__Running = "Đang chạy kiểm thử...";

        Module moduleData;
        PagingGrid pagingGrid;
        string pathDirectoryPower;
        string PathDestination;
        //List<ImportDataADO> DataFilePower;
        //List<ImportDataADO> DataFileDestination;
        List<ResultDataComparison> Resutl;
        string Status;
        List<V_SAR_REPORT> DataCurrent;
        List<ResultDataComparison> resultDataComparisonSelecteds;
        string erromess;
        #endregion

        public frmSarReportTest()
        {
            InitializeComponent();
        }

        public frmSarReportTest(Module module)
            : base(module)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = module;
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmSarReportTest_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                LoadData();
                string fd = FolderRootWorker.GetFolder();
                if (!String.IsNullOrEmpty(fd))
                {
                    txtPathSave.Text = fd;
                }
                EnableButton(true, false, false);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadData()
        {
            try
            {
                CommonParam param = new CommonParam();
                SarReportViewFilter fiter = new SarReportViewFilter();
                fiter.IS_REFERENCE_TESTING = 1;
                fiter.IS_EXCLUDE_FILTER_BY_LOGINNAME = true;//TODO
                fiter.REPORT_STT_ID = IMSys.DbConfig.SAR_RS.SAR_REPORT_STT.ID__HT;
                this.Resutl = new BackendAdapter(param).Get<List<ResultDataComparison>>(HisRequestUriStore.GET_VIEW_SAR_REPORT, ApiConsumers.SarConsumer, fiter, param);

                foreach (var item in this.Resutl)
                {
                    item.Status = "Chưa chạy kiểm thử";
                }
                gridViewResult.BeginUpdate();
                gridControlResult.DataSource = this.Resutl;
                gridViewResult.EndUpdate();

                gridViewResult.SelectAll();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = moduleData.text;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtPathSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtPathSave.Text = folderBrowserDialog1.SelectedPath.ToString();
                    FolderRootWorker.ChangeFolder(txtPathSave.Text);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            try
            {
                if (!DirectoryCreat())
                    return;

                txtContentError.Text = null;
                lblStatus.Text = "";
                this.resultDataComparisonSelecteds = new List<ResultDataComparison>();
                WaitingManager.Show();
                int[] rowHandles = gridViewResult.GetSelectedRows();
                bool valid = (rowHandles != null && rowHandles.Length > 0);
                if (valid)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (ResultDataComparison)gridViewResult.GetRow(i);
                        if (row != null)
                        {
                            resultDataComparisonSelecteds.Add(row);
                        }
                    }

                    foreach (var rdc in resultDataComparisonSelecteds)
                    {
                        DowloadFileExcelPower(rdc);

                        //Tải file excel về thư mục đích
                        if (rdc.Status != Status__Finish)
                        {
                            DowloadFileExcelDestination(rdc);
                        }
                    }

                    gridControlResult.RefreshDataSource();
                }

                EnableButton(false, true, true);

                WaitingManager.Hide();
                this.timer1.Enabled = true;
                this.timer1.Interval = 5000;//Fix
                this.timer1.Start();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void EnableButton(bool checking, bool refesh, bool stop)
        {
            try
            {
                btnCheck.Enabled = checking;
                btnRefesh.Enabled = refesh;
                btnStop.Enabled = stop;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void DowloadFileExcelPower(ResultDataComparison data)
        {
            try
            {
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(data.REPORT_URL);
                if (stream != null)
                {
                    System.IO.Directory.CreateDirectory(pathDirectoryPower + @"\" + data.REPORT_CODE);
                    saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls|Pdf file(*.pdf)|*.pdf";
                    saveFileDialog1.FileName = data.REPORT_URL.Split('\\').LastOrDefault();
                }
                var fileStream = new FileStream(pathDirectoryPower + @"\" + data.REPORT_CODE + @"\" + saveFileDialog1.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                stream.CopyTo(fileStream);

                //Tải thành công
                //Đọc file excel trong thu mục nguồn
                ReadFileExcelPower(pathDirectoryPower + @"\" + data.REPORT_CODE + @"\" + saveFileDialog1.FileName, data);
            }
            catch (Exception ex)
            {
                data.Difference = "Không tồn tại báo cáo nguồn";
                data.Status = Status__Finish;
                LogSystem.Warn(ex);
            }
        }

        private async Task DowloadFileExcelDestination(ResultDataComparison data)
        {
            try
            {
                CommonParam param = new CommonParam();
                System.IO.Directory.CreateDirectory(PathDestination + @"\" + data.REPORT_CODE);
                //Tạo báo cáo đích
                MRS.SDO.CreateReportSDO sarReport = new MRS.SDO.CreateReportSDO();
                sarReport.ReportTypeCode = (data.REPORT_TYPE_CODE != null) ? data.REPORT_TYPE_CODE : "";
                sarReport.ReportTemplateCode = (data.REPORT_TEMPLATE_CODE != null) ? data.REPORT_TEMPLATE_CODE : "";
                sarReport.Filter = Newtonsoft.Json.JsonConvert.DeserializeObject(data.JSON_FILTER);
                sarReport.BranchId = HIS.Desktop.LocalStorage.LocalData.BranchWorker.GetCurrentBranchId();
                sarReport.Loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                sarReport.Username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                sarReport.ReportName = (data.REPORT_NAME != null) ? data.REPORT_NAME : "";
                var result = await new BackendAdapter(param).PostAsync<SAR_REPORT>(HisRequestUriStore.SAR_REPORT_CREATEREQ, HIS.Desktop.ApiConsumer.ApiConsumers.MrsConsumer, sarReport, param);

                //Tải báo cáo đích về thư mục đích
                if (result != null && result.ID > 0)
                {
                    data.Status = "Đang xử lý tạo báo cáo đích phục vụ so sánh...";
                    data.ID_Des = result.ID;
                }
                else
                {
                    data.Status = "Tạo báo cáo đích để so sánh thất bại";
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private bool DirectoryCreat()
        {
            try
            {
                string path = txtPathSave.Text.Trim();
                if (System.IO.Directory.Exists(path))
                {
                    string time = DateTime.Now.ToString("dd-MM-yyyy-HHmmss");
                    pathDirectoryPower = path + @"\FolderPower_" + time;
                    PathDestination = path + @"\FolderDestination_" + time;
                    System.IO.Directory.CreateDirectory(pathDirectoryPower);
                    System.IO.Directory.CreateDirectory(PathDestination);
                }
                else
                {
                    MessageBox.Show("Xử lý thất bại. Đường dẫn lưu thư mục không tồn tại.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return true;
        }

        private void ReadFileExcelPower(string path, ResultDataComparison data)
        {
            try
            {
                data.DataFiles = new List<ImportDataADO>();
                XlsFile xEcel = new XlsFile(false);
                xEcel.Open(path);
                //this.DataFilePower = new List<ImportDataADO>();
                int CountSheet = xEcel.SheetCount;
                for (int i = 1; i < CountSheet; i++)
                {
                    int ColCount = xEcel.GetColCount(i, false);
                    int rowCount = xEcel.GetRowCount(i);
                    for (int col = 1; col <= ColCount; col++)
                    {
                        for (int row = 1; row <= rowCount; row++)
                        {
                            int d = 0;
                            ADO.ImportDataADO dataADO = new ADO.ImportDataADO();
                            var dataCell = xEcel.GetCellValue(i, row, col, ref d);
                            dataADO.ValueRowCell = (dataCell != null ? dataCell : "").ToString();
                            string RowCellName = new FlexCel.Core.TCellAddress(row, col, false, false).CellRef;
                            dataADO.RowCellName = RowCellName;
                            dataADO.SheetName = xEcel.GetSheetName(i);
                            data.DataFiles.Add(dataADO);
                        }
                    }
                }
                data.Status = "Đang đọc file báo cáo gốc...";
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ReadFileExcelDestination(string path, ResultDataComparison data)
        {
            try
            {
                data.DataFileDestinations = new List<ImportDataADO>();
                XlsFile xEcel = new XlsFile(false);
                xEcel.Open(path);
                int CountSheet = xEcel.SheetCount;
                //this.DataFileDestination = new List<ImportDataADO>();
                for (int i = 1; i < CountSheet; i++)
                {
                    int rowcount = xEcel.GetRowCount(i);
                    int ColCount = xEcel.GetColCount(i, false);

                    for (int col = 1; col <= ColCount; col++)
                    {
                        for (int row = 1; row <= rowcount; row++)
                        {
                            ADO.ImportDataADO dataADO = new ADO.ImportDataADO();
                            int d = 0;

                            string RowCellName = new FlexCel.Core.TCellAddress(row, col, false, false).CellRef;
                            var dataCell = xEcel.GetCellValue(i, row, col, ref d);
                            dataADO.ValueRowCell = (dataCell != null ? dataCell : "").ToString();
                            dataADO.RowCellName = RowCellName;
                            dataADO.SheetName = xEcel.GetSheetName(i);
                            data.DataFileDestinations.Add(dataADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ComparisonFile(ResultDataComparison dataComparison)
        {
            try
            {
                string differenceCompare = "";
                if (dataComparison.DataFileDestinations != null && dataComparison.DataFiles != null)
                {
                    var dataSheerPower = dataComparison.DataFiles.Select(o => o.SheetName).Distinct().ToList();
                    var dataSheetDes = dataComparison.DataFileDestinations.Select(o => o.SheetName).Distinct().ToList();
                    var SheetNamedif = dataSheerPower.Except(dataSheetDes).Concat(dataSheetDes.Except(dataSheerPower));
                    if (SheetNamedif != null && SheetNamedif != null)
                    {
                        foreach (var itemsheetname in SheetNamedif)
                        {
                            differenceCompare += differenceCompare = String.Format("{0}{1} không tồn tại", String.IsNullOrEmpty(differenceCompare) ? "" : "\r\n", itemsheetname);
                        }
                    }
                    var SheetNameSame = dataSheerPower.Intersect(dataSheetDes).ToList();
                    if (SheetNameSame != null)
                    {

                        foreach (var iSheet in SheetNameSame)
                        {

                            var Destination = dataComparison.DataFileDestinations.Where(o => o.SheetName == iSheet).ToList();
                            var Power = dataComparison.DataFiles.Where(o => o.SheetName == iSheet).ToList();
                            var DestinationNew = dataComparison.DataFileDestinations.Where(o => o.SheetName == iSheet).ToList();
                            var PowerNew = dataComparison.DataFiles.Where(o => o.SheetName == iSheet).ToList();
                            if (PowerNew != null && PowerNew.Count > 0)
                            {
                                ImportDataADO dataRemo = new ImportDataADO();
                                foreach (var itemPower in PowerNew)
                                {

                                    if (itemPower.ValueRowCell != null && itemPower.ValueRowCell.Length > 0)
                                    {
                                        bool exits = DestinationNew.Any(o => o.RowCellName == itemPower.RowCellName && o.ValueRowCell != null);
                                        if (exits == false)
                                        {
                                            differenceCompare += String.Format("{0}{1} các ô có nội dung sai lệch: {2}", String.IsNullOrEmpty(differenceCompare) ? "" : "\r\n", iSheet, itemPower.RowCellName);
                                            Power.Remove(itemPower);
                                        }

                                    }
                                }
                            }
                            if (DestinationNew != null && DestinationNew.Count > 0)
                            {
                                foreach (var itemDestination in DestinationNew)
                                {
                                    if (itemDestination.ValueRowCell != null && itemDestination.ValueRowCell.Length > 0)
                                    {

                                        bool exits = PowerNew.Any(o => o.RowCellName == itemDestination.RowCellName && o.ValueRowCell != null);
                                        if (exits == false)
                                        {
                                            differenceCompare += String.Format("{0}{1} các ô có nội dung sai lệch: {2}", String.IsNullOrEmpty(differenceCompare) ? "" : "\r\n", iSheet, itemDestination.RowCellName);
                                            Destination.Remove(itemDestination);
                                        }
                                    }
                                }
                            }
                            foreach (var des in Destination)
                            {
                                foreach (var pow in Power)
                                {
                                    if (des.RowCellName == pow.RowCellName && des.ValueRowCell.Trim() != pow.ValueRowCell.Trim())
                                    {
                                        differenceCompare += String.Format("{0}{1} các ô có nội dung sai lệch: {2}", String.IsNullOrEmpty(differenceCompare) ? "" : "\r\n", iSheet, pow.RowCellName);
                                    }
                                }
                            }


                        }
                    }
                }

                dataComparison.Difference = String.IsNullOrEmpty(differenceCompare) ? "Dữ liệu báo cáo nguồn & đích trùng khớp nhau" : differenceCompare.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }


        private void gridViewResult_Click(object sender, EventArgs e)
        {
            try
            {
                var datarow = (ResultDataComparison)gridViewResult.GetFocusedRow();
                if (datarow != null)
                {
                    txtContentError.Text = datarow.Difference;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void RuningTestReport()
        {
            try
            {
                //TODO
                if (this.resultDataComparisonSelecteds != null && this.resultDataComparisonSelecteds.Count > 0)
                {
                    bool isExistsRPRunning = this.resultDataComparisonSelecteds.Any(o => o.Status != Status__Finish);
                    if (isExistsRPRunning)
                    {
                        List<ResultDataComparison> rdcRunnings = new List<ResultDataComparison>();

                        var rpRunningSelecteds = this.resultDataComparisonSelecteds.Where(o => o.Status != Status__Finish).ToList();
                        int rpCount = rpRunningSelecteds != null ? rpRunningSelecteds.Count : 0;
                        Inventec.Common.Logging.LogSystem.Info("Con " + rpCount + " bao cao dang trong qua trinh chay kiem thu tu dong.");
                        lblStatus.Text = "Còn " + rpCount + " báo cáo đang trong quá trình chạy kiểm thử tự động.";

                        var ids = rpRunningSelecteds.Select(o => o.ID_Des).ToList();
                        if (rpCount <= 100)
                        {
                            CommonParam param = new CommonParam();
                            SarReportViewFilter fiter = new SarReportViewFilter();
                            fiter.IDs = ids;
                            fiter.IS_EXCLUDE_FILTER_BY_LOGINNAME = true;
                            //fiter.REPORT_STT_ID = IMSys.DbConfig.SAR_RS.SAR_REPORT_STT.ID__HT;
                            rdcRunnings = new BackendAdapter(param).Get<List<ResultDataComparison>>(HisRequestUriStore.GET_VIEW_SAR_REPORT, ApiConsumers.SarConsumer, fiter, param);
                        }
                        else
                        {
                            CommonParam param = new CommonParam();
                            SarReportViewFilter fiter = new SarReportViewFilter();
                            fiter.IS_EXCLUDE_FILTER_BY_LOGINNAME = true;
                            //fiter.REPORT_STT_ID = IMSys.DbConfig.SAR_RS.SAR_REPORT_STT.ID__HT;
                            rdcRunnings = new BackendAdapter(param).Get<List<ResultDataComparison>>(HisRequestUriStore.GET_VIEW_SAR_REPORT, ApiConsumers.SarConsumer, fiter, param).Where(o => ids.Contains(o.ID_Des)).ToList();
                        }

                        foreach (var item in rdcRunnings)
                        {
                            var rp = rpRunningSelecteds.Where(o => o.ID_Des == item.ID).FirstOrDefault();
                            if (item.REPORT_STT_ID == IMSys.DbConfig.SAR_RS.SAR_REPORT_STT.ID__HT && !String.IsNullOrEmpty(item.REPORT_URL))
                            {
                                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(item.REPORT_URL);
                                if (stream == null)
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu file rỗng");
                                    rp.Difference = "Dữ liệu báo cáo đích rỗng, không thể so sánh";
                                    rp.Status = Status__Finish;
                                    continue;
                                }
                                else
                                {
                                    System.IO.Directory.CreateDirectory(PathDestination + @"\" + rp.REPORT_CODE);
                                    saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls|Pdf file(*.pdf)|*.pdf";
                                    saveFileDialog1.FileName = item.REPORT_URL.Split('\\').LastOrDefault();
                                }

                                rp.Status = Status__Running;
                                gridControlResult.RefreshDataSource();

                                var fileStream = new FileStream(PathDestination + @"\" + rp.REPORT_CODE + @"\" + saveFileDialog1.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                                stream.CopyTo(fileStream);
                                //Đọc dữ liệu file excel trong thư mục đích
                                ReadFileExcelDestination(PathDestination + @"\" + rp.REPORT_CODE + @"\" + saveFileDialog1.FileName, rp);

                                ComparisonFile(rp);
                                rp.Status = Status__Finish;
                                gridControlResult.RefreshDataSource();
                            }
                            else if (item.REPORT_STT_ID == IMSys.DbConfig.SAR_RS.SAR_REPORT_STT.ID__DXL)
                            {
                                rp.Status = "Đang xử lý tạo báo cáo đích phục vụ so sánh...";
                                gridControlResult.RefreshDataSource();
                            }
                            else if (item.REPORT_STT_ID == IMSys.DbConfig.SAR_RS.SAR_REPORT_STT.ID__LOI || item.REPORT_STT_ID == IMSys.DbConfig.SAR_RS.SAR_REPORT_STT.ID__HUY || String.IsNullOrEmpty(item.REPORT_URL))
                            {
                                rp.Status = Status__Finish;
                                rp.Difference = "Tạo báo cáo đích phục vụ so sánh lỗi. Kiểm thử thất bại";
                                gridControlResult.RefreshDataSource();
                            }
                        }
                        gridControlResult.RefreshDataSource();
                    }
                    else
                    {
                        lblStatus.Text = "kết thúc quá trình chạy kiểm thử tự động.";
                        Inventec.Common.Logging.LogSystem.Info("Khong con bao cao de chay, tien trinh ket thuc");
                        gridControlResult.RefreshDataSource();
                        this.timer1.Stop();
                    }
                }
                else
                {
                    lblStatus.Text = "kết thúc quá trình chạy kiểm thử tự động.";
                    Inventec.Common.Logging.LogSystem.Info("Khong con bao cao de chay, tien trinh ket thuc");
                    this.timer1.Stop();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                bool isExistsRPRunning = this.resultDataComparisonSelecteds.Any(o => o.Status == Status__Running);
                if (!isExistsRPRunning)
                {
                    RuningTestReport();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("timer1_Tick. Dang chay kiem thu bao cao, vui long doi...");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                this.timer1.Stop();
                EnableButton(true, false, false);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {

                txtContentError.Text = null;
                lblStatus.Text = "";
                this.resultDataComparisonSelecteds = new List<ResultDataComparison>();
                WaitingManager.Show();
                int[] rowHandles = gridViewResult.GetSelectedRows();
                bool valid = (rowHandles != null && rowHandles.Length > 0);
                if (valid)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (ResultDataComparison)gridViewResult.GetRow(i);
                        if (row != null && row.Status == "Chưa chạy kiểm thử")
                        {
                            resultDataComparisonSelecteds.Add(row);
                        }
                    }
                }
                foreach (var rdc in resultDataComparisonSelecteds)
                {
                    DowloadFileExcelPower(rdc);

                    //Tải file excel về thư mục đích
                    DowloadFileExcelDestination(rdc);
                }

                gridControlResult.RefreshDataSource();
                EnableButton(false, true, true);

                WaitingManager.Hide();
                this.timer1.Enabled = true;
                this.timer1.Interval = 5000;//Fix
                this.timer1.Start();
                RuningTestReport();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        //private void Test()
        //{
        //    try
        //    {
        //        ReadFileExcelPower(@"C:\Users\tuteo\OneDrive\Máy tính\Book1.xlsx");
        //        ReadFileExcelDestination(@"C:\Users\tuteo\OneDrive\Máy tính\Book3.xlsx");
        //        ComparisonFile();
        //        this.txtContentError.Text = this.difference;

        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

    }
}

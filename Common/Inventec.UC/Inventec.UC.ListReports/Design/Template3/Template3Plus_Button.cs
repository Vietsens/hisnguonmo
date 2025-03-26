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
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.ListReports.Base;
using Inventec.UC.ListReports.Form;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ListReports.Design.Template3
{
    internal partial class Template3
    {
        private void btnEditReport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var report = (SAR.EFMODEL.DataModels.V_SAR_REPORT)gridViewListReports.GetFocusedRow();
                if (report != null)
                {
                    frmUpdate frmupdate = new frmUpdate(report, this._HasException);
                    frmupdate.ShowDialog();

                    //ButtonSearchAndPagingClick(true);
                    SetInitPaging();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDeleteReport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SAR.EFMODEL.DataModels.V_SAR_REPORT)gridViewListReports.GetFocusedRow();
                CommonParam param = new CommonParam();
                bool success = false;
                if (row != null)
                {
                    if (System.Windows.Forms.MessageBox.Show("Bạn có muốn hủy dữ liệu", "Thông báo", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        SAR.EFMODEL.DataModels.SAR_REPORT evReport = new SAR.EFMODEL.DataModels.SAR_REPORT();
                        Inventec.Common.Mapper.DataObjectMapper.Map<SAR.EFMODEL.DataModels.SAR_REPORT>(evReport, row);
                        success = new Sar.SarReport.Delete.SarReportDeleteFactory(param).createFactory(evReport).Delete();
                        if (success)
                        {
                            WaitingManager.Hide();
                            //ButtonSearchAndPagingClick(true);
                            SetInitPaging();
                        }
                        #region Show message
                        MessageManager.Show(this.ParentForm, new CommonParam(), true);
                        #endregion ;

                        #region Process has exception
                        if (_HasException != null) _HasException(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPublicReport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ProcessPublicAndPrivate(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSendReport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SAR.EFMODEL.DataModels.V_SAR_REPORT)gridViewListReports.GetFocusedRow();
                if (row != null)
                {
                    frmShare frmShare = new frmShare(row, _HasException);
                    frmShare.ShowDialog();

                    //ButtonSearchAndPagingClick(true);
                    SetInitPaging();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDownloadReport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SAR.EFMODEL.DataModels.V_SAR_REPORT)gridViewListReports.GetFocusedRow();
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(row.REPORT_URL);
                if (stream == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu file rỗng");
                }
                else
                {
                    WaitingManager.Show();
                    var extensionCheck = row.EXTENSION_RECEIVE;
                    if (extensionCheck == null) extensionCheck = "xlsx";
                    if (extensionCheck.ToLower() == "pdf")
                    {
                        saveFileDialog1.Filter = "pdf files (*.pdf)|*.pdf";
                        saveFileDialog1.FileName = row.REPORT_NAME + ".pdf";
                        saveFileDialog1.FileOk += saveFileDialog1_FileOk;
                    }
                    else
                    {
                        saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls|Pdf file(*.pdf)|*.pdf";
                        saveFileDialog1.FileName = row.REPORT_NAME + ".xlsx";

                    }
                    if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        var fileStream = new FileStream(@"" + saveFileDialog1.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                        if (fileStream == null)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng ");
                        }
                        else
                        {
                            var extension = Path.GetExtension(saveFileDialog1.FileName);
                            if (extension.ToLower() == ".xlsx" || extension.ToLower() == ".xls")
                            {
                                try
                                {
                                    stream.CopyTo(fileStream);
                                    WaitingManager.Hide();
                                    #region Show message
                                    MessageManager.Show(this.ParentForm, new CommonParam(), true);
                                    #endregion ;
                                    try
                                    {
                                        if (System.Windows.Forms.MessageBox.Show("Bạn có muốn mở file bây giờ không?", "Hỏi đáp", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                                            System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                                    }
                                    catch (Exception ex)
                                    {
                                        WaitingManager.Hide();
                                        Inventec.Common.Logging.LogSystem.Warn(ex);
                                    }

                                    fileStream.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    WaitingManager.Hide();
                                    Inventec.Common.Logging.LogSystem.Warn(ex);
                                }
                            }
                            if (extension.ToLower() == ".pdf")
                            {
                                try
                                {
                                    fileStream.Dispose();
                                    Workbook workbook = new Workbook();
                                    bool rs = workbook.LoadDocument(stream, DocumentFormat.OpenXml);
                                    using (FileStream pdfFileStream = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                                    {
                                        workbook.ExportToPdf(pdfFileStream);
                                    }
                                    workbook.Dispose();
                                    WaitingManager.Hide();
                                    #region Show message
                                    MessageManager.Show(this.ParentForm, new CommonParam(), true);
                                    #endregion ;
                                    try
                                    {
                                        if (System.Windows.Forms.MessageBox.Show("Bạn có muốn mở file bây giờ không?", "Hỏi đáp", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                                            System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                                    }
                                    catch (Exception ex)
                                    {
                                        WaitingManager.Hide();
                                        Inventec.Common.Logging.LogSystem.Warn(ex);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WaitingManager.Hide();
                                    Inventec.Common.Logging.LogSystem.Warn(ex);
                                }
                            }
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrivateReport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                ProcessPublicAndPrivate(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintReport_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SAR.EFMODEL.DataModels.V_SAR_REPORT)gridViewListReports.GetFocusedRow();
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(row.REPORT_URL);
                if (stream != null)
                {
                    stream.Position = 0;
                    string printType = (System.Configuration.ConfigurationManager.AppSettings["Inventec.UC.ListReports.PrintType"] ?? "").ToString();
                    if (printType == "1")
                    {
                        var print = new Inventec.Common.Print.frmPrintPreview(stream, null, null, "", 1);
                        print.Show();
                    }
                    else
                    {
                        var print = new Inventec.Common.FlexCelPrint.frmSetupPrintPreview(stream, "");
                        //print.Owner = this.ParentForm;
                        print.Show();
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu file rỗng");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemchkIsReferenceTesting_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var report = (SAR.EFMODEL.DataModels.V_SAR_REPORT)gridViewListReports.GetFocusedRow();
                CommonParam param = new CommonParam();
                bool success = false;
                if (report != null)
                {
                    WaitingManager.Show();
                    if (report.IS_REFERENCE_TESTING != null)
                        report.IS_REFERENCE_TESTING = null;
                    else
                        report.IS_REFERENCE_TESTING = 1;

                    SAR.EFMODEL.DataModels.SAR_REPORT upReport = new SAR.EFMODEL.DataModels.SAR_REPORT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<SAR.EFMODEL.DataModels.SAR_REPORT>(upReport, report);
                    var result =  Base.ApiConsumerStore.SarConsumer.Post<Inventec.Core.ApiResultObject<SAR.EFMODEL.DataModels.SAR_REPORT>>("/api/SarReport/Update", param, upReport);
                   
                   // Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => upReport), upReport));
                    if (result != null)
                    {
                        success = true;
                        SetInitPaging();
                    }
                    WaitingManager.Hide();

                    #region Show message
                    MessageManager.Show(this.ParentForm, new CommonParam(), true);
                    #endregion ;
                    #region has exception
                    if (_HasException != null) _HasException(param);
                    #endregion

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sv = (sender as System.Windows.Forms.SaveFileDialog);
            if (Path.GetExtension(saveFileDialog1.FileName).ToLower() != ".pdf")
            {
                e.Cancel = true;
                DevExpress.XtraEditors.XtraMessageBox.Show("Hãy nhập định dạng file là'pdf'");
                return;
            }
        }

        private void ProcessPublicAndPrivate(bool isPublic)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                var row = (SAR.EFMODEL.DataModels.V_SAR_REPORT)gridViewListReports.GetFocusedRow();
                WaitingManager.Show();
                if (isPublic)
                {
                    row.IS_PUBLIC = 1;
                }
                else
                {
                    row.IS_PUBLIC = null;
                }
                SAR.EFMODEL.DataModels.SAR_REPORT evReport = new SAR.EFMODEL.DataModels.SAR_REPORT();
                Inventec.Common.Mapper.DataObjectMapper.Map<SAR.EFMODEL.DataModels.SAR_REPORT>(evReport, row);
                var result = new Sar.SarReport.Public.SarReportPublicFactory(param).createFactory(evReport).Public();
                if (result != null)
                {
                    WaitingManager.Hide();
                    //ButtonSearchAndPagingClick(true);
                    SetInitPaging();
                    success = true;
                }

                #region Show message
                MessageManager.Show(this.ParentForm, new CommonParam(), true);
                #endregion ;

                #region Process has exception
                if (_HasException != null) _HasException(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItembtnCopy_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SAR.EFMODEL.DataModels.V_SAR_REPORT)gridViewListReports.GetFocusedRow();
                if (row != null && this._ReportCopy != null)
                {
                    this._ReportCopy(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

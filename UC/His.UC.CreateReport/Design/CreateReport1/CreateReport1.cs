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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Utils;
using Inventec.Desktop.Common.Message;
using SAR.EFMODEL.DataModels;
using System.IO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using His.UC.CreateReport.Data;
using DevExpress.XtraLayout;

namespace His.UC.CreateReport.Design.CreateReport1
{

    internal partial class CreateReport1 : UserControl
    {
        const int solanMax = 8;
        int index = 0;
        int positionHandleControl = -1;
        HIS.UC.FormType.GenerateRDO generateRDO;
        SAR_REPORT_TYPE reportType = new SAR_REPORT_TYPE();
        SAR_REPORT sarReportCreate = new SAR_REPORT();
        List<ReportTemplateADO> listReportTemplateADO;
        List<BaseLayoutItem> layoutControlItemAll = new List<BaseLayoutItem>();

        public CreateReport1(HIS.UC.FormType.GenerateRDO paramData)
        {
            InitializeComponent();
            try
            {
                WaitingManager.Show();

                this.generateRDO = paramData;
                if (paramData.DetailData is SAR_REPORT_TYPE)
                {
                    this.reportType = paramData.DetailData as SAR_REPORT_TYPE;
                }
                if (generateRDO == null) throw new ArgumentNullException("generateRDO is null");
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => generateRDO), generateRDO));
                this.CreateReport_Load();
                WaitingManager.Hide();

                CheckTimeCreate();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InitDataForCreateNoShow()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tmViewAway_Tick(object sender, EventArgs e)
        {
            if (index == solanMax || chkViewAway.Checked == false)
            {
                tmViewAway.Stop();
                btnSave.Enabled = true;
                if (index == solanMax)
                {
                    this.lbMessage.Text = Base.MessageUtil.GetMessage(MessageLang.Message.Enum.KhongViewNgayChiGuiDenDanhSachBaoCao);
                }
                else
                {
                    this.lbMessage.Text = "";
                }
                index = 0;
                return;
            }

            //Get trạng thái của báo cáo
            SAR_REPORT sarReportResult = CreateReportDelegate.DelegateStatusReport(this.sarReportCreate.ID);
            if (sarReportResult != null && sarReportResult.REPORT_STT_ID == IMSys.DbConfig.SAR_RS.SAR_REPORT_STT.ID__HT)
            {
                //View Báo cáo đó lên
                tmViewAway.Stop();
                btnSave.Enabled = true;

                this.lbMessage.Text = "";//Null
                this.ViewReport(sarReportResult);
                index = 0;
                return;
            }

            if (index % 2 == 0)
            {
                this.lbMessage.Text = Base.MessageUtil.GetMessage(MessageLang.Message.Enum.DangLoadHienThiView);
                this.lbMessage.ForeColor = Color.Red;
            }
            else
            {
                this.lbMessage.Text = "";
                this.lbMessage.ForeColor = Color.Black;
            }
            this.index += 1;
        }

        private void ViewReport(SAR_REPORT sarReportResult)
        {
            try
            {
                MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(sarReportResult.REPORT_URL);
                if (stream != null)
                {
                    stream.Position = 0;
                    string printType = (System.Configuration.ConfigurationManager.AppSettings["Inventec.UC.ListReports.PrintType"] ?? "").ToString();
                    if (printType == "1")
                    {
                        var print = new Inventec.Common.Print.frmPrintPreview(stream, null, null, "", 1);
                        print.ShowDialog();
                    }
                    else
                    {
                        var print = new Inventec.Common.FlexCelPrint.frmSetupPrintPreview(stream, "");
                        print.ShowDialog();
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu file rỗng");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void lcGenerateReportField_ClientSizeChanged(object sender, EventArgs e)
        {
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("lcGenerateReportField_Resize.lcGenerateReportField.Width___runtime size:", ("[" + lcGenerateReportField.Width + ", " + lcGenerateReportField.Height + "],ClientSize=" + lcGenerateReportField.ClientSize + ", MaximumSize=" + lcGenerateReportField.MaximumSize)) + ", design size: [1199, 529]");
        }

        private void gridViewReportTemplate_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsChecked")
                        {
                            ProcessButtonCheckAllRoom(hi);
                        }

                    }
                    else if (hi.HitTest == GridHitTest.RowCell)
                    {
                        if (hi.Column.FieldName == "IsChecked")
                        {
                            ProcessButtonCheckRoom(hi);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessButtonCheckRoom(GridHitInfo hi)
        {
            try
            {
                ReportTemplateADO dataRow = (ReportTemplateADO)hi.View.GetRow(hi.RowHandle);
                if (dataRow != null && !dataRow.IsChecked)
                {
                    listReportTemplateADO.ForEach(o => o.IsChecked = false);
                    dataRow.IsChecked = true;

                    grdReportTemplate.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessButtonCheckAllRoom(GridHitInfo hi)
        {
            try
            {
                bool isCheckAll = false;
                if ((this.grdReportTemplate.DataSource as List<ReportTemplateADO>) != null && (this.grdReportTemplate.DataSource as List<ReportTemplateADO>).Count > 0)
                {
                    var CheckedNum = (this.grdReportTemplate.DataSource as List<ReportTemplateADO>).Where(o => o.IsChecked == true).Count();
                    var ServiceNum = (this.grdReportTemplate.DataSource as List<ReportTemplateADO>).Count();
                    if ((CheckedNum > 0 && CheckedNum < ServiceNum) || CheckedNum == 0)
                    {
                        isCheckAll = true;
                        hi.Column.Image = imageCollection1.Images[1];
                    }

                    if (CheckedNum == ServiceNum)
                    {
                        isCheckAll = false;
                        hi.Column.Image = imageCollection1.Images[0];
                    }

                    if (isCheckAll)
                    {
                        foreach (var item in (this.grdReportTemplate.DataSource as List<ReportTemplateADO>))
                        {
                            if (item.ID != null)
                            {
                                item.IsChecked = true;
                            }
                        }
                        isCheckAll = false;
                    }
                    else
                    {
                        foreach (var item in (this.grdReportTemplate.DataSource as List<ReportTemplateADO>))
                        {
                            if (item.ID != null)
                            {
                                item.IsChecked = false;
                            }
                        }
                        isCheckAll = true;
                    }

                    grdReportTemplate.BeginUpdate();
                    grdReportTemplate.DataSource = (this.gridViewReportTemplate.DataSource as List<ReportTemplateADO>).OrderByDescending(o => (o.IsChecked)).ToList();
                    grdReportTemplate.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewReportTemplate_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                ReportTemplateADO reportTemplateRow = (ReportTemplateADO)gridViewReportTemplate.GetRow(e.RowHandle);
                if (reportTemplateRow != null && reportTemplateRow.IsChecked)
                {
                    e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateReport1_Load(object sender, EventArgs e)
        {

        }

        private void btnTutorial_Click(object sender, EventArgs e)
        {
            try
            {
                string reportTypeCode = this.reportType.REPORT_TYPE_CODE;
                string domain = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_CRM;

                Inventec.Common.Logging.LogSystem.Debug("Open brower - reportTypeCode: " + reportTypeCode + " Domain: " + domain);
                if (!string.IsNullOrEmpty(reportTypeCode) && !string.IsNullOrEmpty(domain))
                {
                    try
                    {
                        WaitingManager.Show();
                        System.Diagnostics.Process.Start(string.Format("{0}ords/f?p=104:5:::::P5_CODE:{1}", domain, reportTypeCode));
                        WaitingManager.Hide();
                    }
                    catch (Exception ex)
                    {
                        WaitingManager.Hide();
                        Inventec.Common.Logging.LogSystem.Error(ex);
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

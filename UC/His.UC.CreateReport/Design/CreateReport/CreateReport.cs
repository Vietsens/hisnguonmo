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

namespace His.UC.CreateReport.Design.CreateReport
{

    internal partial class CreateReport : UserControl
    {
        const int solanMax = 8;
        int index = 0;
        int positionHandleControl = -1;
        HIS.UC.FormType.GenerateRDO generateRDO;
        SAR_REPORT_TYPE reportType = new SAR_REPORT_TYPE();
        SAR_REPORT sarReportCreate = new SAR_REPORT();

        public CreateReport(HIS.UC.FormType.GenerateRDO paramData)
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

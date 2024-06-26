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
using His.UC.CreateReport;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ReportTypeList
{
    public partial class frmMainReport : Form
    {
        List<String> listTimeFromTo = new List<string>();
        List<String> listTimeDepartmentRoom = new List<string>();
        List<String> listTimeDepartmentTreatmentType = new List<string>();
        List<String> listTimeDepartmentType = new List<string>();
        List<String> listTimePatientType = new List<string>();
        List<String> listTimePatientTypeTreatmentType = new List<string>();
        List<String> listTimeTreatmentType = new List<string>();
        string reportTypeCode;
        MRS.SDO.CreateReportSDO sarReport;
        SAR.EFMODEL.DataModels.SAR_REPORT_TYPE reportType = new SAR.EFMODEL.DataModels.SAR_REPORT_TYPE();
        List<SAR_REPORT_TEMPLATE> reportTemplates = new List<SAR_REPORT_TEMPLATE>();
        His.UC.CreateReport.MainCreateReport MainCreateReport = new His.UC.CreateReport.MainCreateReport();
        object datailData;

        public frmMainReport()
        {
            InitializeComponent();
        }

        public frmMainReport(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE data)
        {
            try
            {
                this.reportTypeCode = data.REPORT_TYPE_CODE;
                this.MainPrint_Load();
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmMainReport(string reportTypeCode)
        {
            try
            {
                InitializeComponent();
                this.reportTypeCode = reportTypeCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmMainReport(string reportTypeCode, object detailData)
        {
            try
            {
                InitializeComponent();
                this.reportTypeCode = reportTypeCode;
                this.datailData = detailData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MainPrint_Load()
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                reportType = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>().FirstOrDefault(o => o.REPORT_TYPE_CODE == reportTypeCode);
                His.UC.CreateReport.Generate.GenerateRDO generateRDO = new His.UC.CreateReport.Generate.GenerateRDO();
                if (reportType != null)
                {
                    generateRDO.ReportTypeCode = reportType.REPORT_TYPE_CODE;
                }              
                generateRDO.DetailData = this.datailData;
                var formCreate = MainCreateReport.Generate(param, generateRDO);
                formCreate.Dock = DockStyle.Fill;
                CreateReportDelegate.ProcessCreateReport = ProcessCreateReportDelegate;
                this.Controls.Add(formCreate);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                
                Inventec.Common.Logging.LogSystem.Error(ex);
                
            }
        }             

        bool ProcessCreateReportDelegate(object sarReport)
        {
            try
            {
                CommonParam param = new CommonParam();
                var result = new BackendAdapter(param).Post<bool>(MrsRequestUriStore.MRS_REPORT_CREATE, ApiConsumers.MrsConsumer, sarReport, param);
                #region Show message
                MessageManager.Show(param, result);
                #endregion
                if (result)
                {
                    this.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }
    }
}

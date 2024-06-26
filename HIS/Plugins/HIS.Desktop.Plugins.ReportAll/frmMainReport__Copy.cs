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
using Inventec.Desktop.Common.Message;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ReportAll
{
    public partial class frmMainReport : HIS.Desktop.Utility.FormBase
    {
        public frmMainReport(SAR.EFMODEL.DataModels.SAR_REPORT_TYPE data, SAR.EFMODEL.DataModels.V_SAR_REPORT report)
        {
            try
            {
                this.reportType = data;
                this.reportPrevious = report;
                CommonParam param = new CommonParam();
                SarReportFilter filter = new SarReportFilter();
                filter.ID = this.reportPrevious.REPORT_TYPE_ID;
                //this.reportType = new BackendAdapter(param).Get<List<SAR_REPORT_TYPE>>("api/SarReportType/Get", ApiConsumer.ApiConsumers.SarConsumer, filter, param).FirstOrDefault();

                His.UC.CreateReport.CreateReportConfig.ReportTemplates = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<SAR_REPORT_TEMPLATE>>("api/SarReportTemplate/Get", ApiConsumers.SarConsumer, new SarReportTemplateFilter() { IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE, REPORT_TYPE_ID = reportType.ID },
                    null);

                this.SetDelegateForUCFormType();
                this.MainPrint_Load();
                InitializeComponent();
                vlCustomFormType = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ReportCreate.CustomFormType");
                if (vlCustomFormType == "1")
                {
                    this.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    //neu cao hon man hinh se thu nho lai vua nhin
                    if (height >= SystemInformation.WorkingArea.Size.Height)
                    {
                        height = SystemInformation.WorkingArea.Size.Height - 100;
                    }
                    this.Height = height;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

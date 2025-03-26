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
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using Inventec.Core;
using Inventec.UC.CreateReport.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Design.TemplateBetweenTimeFilterOnly
{
    internal partial class TemplateBetweenTimeFilterOnly
    {
        #region variable declare
        internal List<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE> lstReportTemplate;
        private WaitDialogForm waitLoad;
        int positionHandleControl = -1;
        MRS.SDO.CreateReportSDO sarReport;
        #endregion
        #region load
        private void LoadDataToReportTemplate(long reportTypeId)
        {
            try
            {
                CommonParam param = new CommonParam();
                Inventec.UC.CreateReport.Sar.SarReportTemplate.Get.SarReportTemplateGet sarReportTemplate = new Sar.SarReportTemplate.Get.SarReportTemplateGet(param);
                SAR.Filter.SarReportTemplateFilter reportTemplateFilter = new SAR.Filter.SarReportTemplateFilter();
                reportTemplateFilter.REPORT_TYPE_ID = reportTypeId;
                GlobalStore.reportTemplates = sarReportTemplate.Get(reportTemplateFilter);
                cboReportTemplate.Properties.DataSource = GlobalStore.reportTemplates;
                cboReportTemplate.Properties.DisplayMember = "REPORT_TEMPLATE_NAME";
                cboReportTemplate.Properties.ValueMember = "ID";
                cboReportTemplate.Properties.ForceInitialize();
                cboReportTemplate.Properties.Columns.Clear();
                cboReportTemplate.Properties.Columns.Add(new LookUpColumnInfo("REPORT_TEMPLATE_CODE", "", 100));
                cboReportTemplate.Properties.Columns.Add(new LookUpColumnInfo("REPORT_TEMPLATE_NAME", "", 200));
                cboReportTemplate.Properties.ShowHeader = false;
                cboReportTemplate.Properties.ImmediatePopup = true;
                cboReportTemplate.Properties.PopupWidth = 300;
                if (GlobalStore.reportTemplates.Count==1)
                {
                    cboReportTemplate.EditValue = GlobalStore.reportTemplates[0].ID;
                    txtReportTemplateCode.Text = GlobalStore.reportTemplates[0].REPORT_TEMPLATE_CODE;
                }

                if (_HasException != null) _HasException(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToReportType(string reportTypeCode)
        {
            try
            {
                CommonParam param = new CommonParam();
                Inventec.UC.CreateReport.Sar.SarReportType.Get.SarReportTypeGet sarReportType = new Sar.SarReportType.Get.SarReportTypeGet(param);
                SAR.Filter.SarReportTypeFilter reportTypeFilter = new SAR.Filter.SarReportTypeFilter();
                reportTypeFilter.REPORT_TYPE_CODE = reportTypeCode;
                var reportTypes = sarReportType.Get(reportTypeFilter);
                if (reportTypes != null && reportTypes.Count > 0)
                {
                    GlobalStore.reportType = reportTypes.FirstOrDefault();

                }
                lblReportTypeCode.Text = GlobalStore.reportType.REPORT_TYPE_CODE;
                lblReportTypeName.Text = GlobalStore.reportType.REPORT_TYPE_NAME;
                if (_HasException != null) _HasException(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDefaultDataFromTime()
        {
            try
            {
                DateTime FirstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dtTimeFrom.EditValue = FirstDayOfMonth;
                dtTimeTo.EditValue = DateTime.Now;
            }
            catch (Exception ex)
            {
               Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}

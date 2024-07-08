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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using His.UC.CreateReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.CreateReport.Loader
{
    class ReportTemplateLoader
    {
        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboReportTemplate)
        {
            try
            {
                cboReportTemplate.Properties.DataSource = CreateReportConfig.ReportTemplates;
                cboReportTemplate.Properties.DisplayMember = "REPORT_TEMPLATE_NAME";
                cboReportTemplate.Properties.ValueMember = "ID";

                cboReportTemplate.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboReportTemplate.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboReportTemplate.Properties.ImmediatePopup = true;
                cboReportTemplate.ForceInitialize();
                cboReportTemplate.Properties.View.Columns.Clear();
                cboReportTemplate.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboReportTemplate.Properties.View.Columns.AddField("REPORT_TEMPLATE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                GridColumn aColumnName = cboReportTemplate.Properties.View.Columns.AddField("REPORT_TEMPLATE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void LoadDataToCombo(DevExpress.XtraEditors.GridLookUpEdit cboReportTemplate, List<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE> listData)
        {
            try
            {
                cboReportTemplate.Properties.DataSource = listData;
                cboReportTemplate.Properties.DisplayMember = "REPORT_TEMPLATE_NAME";
                cboReportTemplate.Properties.ValueMember = "ID";

                cboReportTemplate.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboReportTemplate.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboReportTemplate.Properties.ImmediatePopup = true;
                cboReportTemplate.ForceInitialize();
                cboReportTemplate.Properties.View.Columns.Clear();
                cboReportTemplate.Properties.View.OptionsView.ShowColumnHeaders = false;

                GridColumn aColumnCode = cboReportTemplate.Properties.View.Columns.AddField("REPORT_TEMPLATE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 150;

                GridColumn aColumnName = cboReportTemplate.Properties.View.Columns.AddField("REPORT_TEMPLATE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 600;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadDataToComboReportTemplate(DevExpress.XtraEditors.LookUpEdit cboReportTemplate)
        {
            try
            {
                cboReportTemplate.Properties.DataSource = CreateReportConfig.ReportTemplates;
                cboReportTemplate.Properties.DisplayMember = "REPORT_TEMPLATE_NAME";
                cboReportTemplate.Properties.ValueMember = "ID";
                cboReportTemplate.Properties.ForceInitialize();
                cboReportTemplate.Properties.Columns.Clear();
                cboReportTemplate.Properties.Columns.Add(new LookUpColumnInfo("REPORT_TEMPLATE_CODE", "", 100));
                cboReportTemplate.Properties.Columns.Add(new LookUpColumnInfo("REPORT_TEMPLATE_NAME", "", 200));
                cboReportTemplate.Properties.ShowHeader = false;
                cboReportTemplate.Properties.ImmediatePopup = true;
                cboReportTemplate.Properties.DropDownRows = 10;
                cboReportTemplate.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadDataToComboReportTemplate(DevExpress.XtraEditors.LookUpEdit cboReportTemplate, List<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE> listData)
        {
            try
            {
                cboReportTemplate.Properties.DataSource = listData;
                cboReportTemplate.Properties.DisplayMember = "REPORT_TEMPLATE_NAME";
                cboReportTemplate.Properties.ValueMember = "ID";
                cboReportTemplate.Properties.ForceInitialize();
                cboReportTemplate.Properties.Columns.Clear();
                cboReportTemplate.Properties.Columns.Add(new LookUpColumnInfo("REPORT_TEMPLATE_CODE", "", 100));
                cboReportTemplate.Properties.Columns.Add(new LookUpColumnInfo("REPORT_TEMPLATE_NAME", "", 200));
                cboReportTemplate.Properties.ShowHeader = false;
                cboReportTemplate.Properties.ImmediatePopup = true;
                cboReportTemplate.Properties.DropDownRows = 10;
                cboReportTemplate.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

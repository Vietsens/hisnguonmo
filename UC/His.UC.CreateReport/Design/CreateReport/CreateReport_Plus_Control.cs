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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace His.UC.CreateReport.Design.CreateReport
{
    internal partial class CreateReport
    {
        private void txtReportTemplateCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (String.IsNullOrEmpty(txtReportTemplateCode.Text))
                    {
                        cboReportTemplate.EditValue = null;
                        cboReportTemplate.Focus();
                        cboReportTemplate.ShowPopup();
                    }
                    else
                    {
                        var listData = CreateReportConfig.ReportTemplates.Where(o => o.REPORT_TEMPLATE_CODE.Contains(txtReportTemplateCode.Text) || o.REPORT_TEMPLATE_NAME.Contains(txtReportTemplateCode.Text)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            txtReportTemplateCode.Text = listData.First().REPORT_TEMPLATE_CODE;
                            cboReportTemplate.EditValue = listData.First().ID;
                            txtReportName.Focus();
                            txtReportName.SelectAll();
                        }
                        else
                        {
                            cboReportTemplate.Focus();
                            cboReportTemplate.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReportTemplate_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboReportTemplate.EditValue != null)
                    {
                        var reportTemplate = CreateReportConfig.ReportTemplates.FirstOrDefault(f => f.ID == long.Parse(cboReportTemplate.EditValue.ToString()));
                        if (reportTemplate != null)
                        {
                            txtReportTemplateCode.Text = reportTemplate.REPORT_TEMPLATE_CODE;
                            txtReportName.Text = reportTemplate.REPORT_TEMPLATE_NAME;
                            txtReportName.Focus();
                            txtReportName.SelectAll();
                        }
                    }
                    else
                    {
                        cboReportTemplate.Focus();
                        cboReportTemplate.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtReportName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //dtTimeFrom.Focus();
                    //dtTimeFrom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

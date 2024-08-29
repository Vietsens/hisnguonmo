using Inventec.UC.CreateReport.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.CreateReport.Design.TemplateBetweenTimeFilterOnly2
{
    internal partial class TemplateBetweenTimeFilterOnly2
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
                        var data = GlobalStore.reportTemplates.Where(o => o.REPORT_TEMPLATE_CODE.Contains(txtReportTemplateCode.Text)).ToList();
                        if (data != null)
                        {
                            if (data.Count == 1)
                            {
                                cboReportTemplate.EditValue = data[0].ID;
                                txtReportTemplateCode.Text = data[0].REPORT_TEMPLATE_CODE;
                                txtReportName.SelectAll();
                                txtReportName.Focus();
                            }
                            else if (data.Count > 1)
                            {
                                cboReportTemplate.EditValue = null;
                                cboReportTemplate.Focus();
                                cboReportTemplate.ShowPopup();
                            }
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
                        SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE data = GlobalStore.reportTemplates.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboReportTemplate.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            txtReportTemplateCode.Text = data.REPORT_TEMPLATE_CODE;
                            txtReportName.Focus();
                            txtReportName.SelectAll();
                        }
                    }
                    else
                    {
                        txtReportName.Focus();
                        txtReportName.SelectAll();
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
                    dtTimeFrom.Focus();
                    dtTimeFrom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtTimeFrom.EditValue != null)
                    {
                        dtTimeTo.Focus();
                        dtTimeTo.ShowPopup();
                    }
                    else
                    {
                        dtTimeFrom.Focus();
                        dtTimeFrom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtTimeTo.EditValue != null)
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                    else
                    {
                        dtTimeTo.Focus();
                        dtTimeTo.ShowPopup();
                    }
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
                if (e.KeyCode== Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Inventec.UC.CreateReport.Config;
using Inventec.UC.CreateReport.Design.TemplateBetweenTimeFilterOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSS.APP.Modules.Report.CreateReport.UseMedicineReport
{
    public class CreateReportProcess
    {
        internal static void LoadDataToReportType(TemplateBetweenTimeFilterOnly control)
        {
            try
            {
                control.lblReportTypeCode.Text = GlobalStore.reportType.REPORT_TYPE_CODE;
                control.lblReportTypeName.Text = GlobalStore.reportType.REPORT_TYPE_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadDataToTemplateCombo(TemplateBetweenTimeFilterOnly control)
        {
            try
            {
                control.cboReportTemplate.Properties.DataSource = GlobalStore.reportTemplates;
                control.cboReportTemplate.Properties.DisplayMember = "REPORT_TEMPLATE_NAME";
                control.cboReportTemplate.Properties.ValueMember = "ID";
                control.cboReportTemplate.Properties.ForceInitialize();
                control.cboReportTemplate.Properties.Columns.Clear();
                control.cboReportTemplate.Properties.Columns.Add(new LookUpColumnInfo("REPORT_TEMPLATE_CODE", "", 100));
                control.cboReportTemplate.Properties.Columns.Add(new LookUpColumnInfo("REPORT_TEMPLATE_NAME", "", 400));
                control.cboReportTemplate.Properties.ShowHeader = false;
                control.cboReportTemplate.Properties.ImmediatePopup = true;
                control.cboReportTemplate.Properties.DropDownRows = 10;
                control.cboReportTemplate.Properties.PopupWidth = 800;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void LoadDataToTemplateByTypeId(TemplateBetweenTimeFilterOnly control)
        {
            var templates = GlobalStore.reportTemplates;
            control.cboReportTemplate.Properties.DisplayMember = "REPORT_TEMPLATE_NAME";
            control.cboReportTemplate.Properties.ValueMember = "ID";
            control.cboReportTemplate.Properties.Columns.Clear();
            control.cboReportTemplate.Properties.Columns.Add(new LookUpColumnInfo("REPORT_TEMPLATE_CODE", "", 100));
            control.cboReportTemplate.Properties.Columns.Add(new LookUpColumnInfo("REPORT_TEMPLATE_NAME", "", 400));
            control.cboReportTemplate.Properties.ShowHeader = false;
            control.cboReportTemplate.Properties.ImmediatePopup = true;
            control.cboReportTemplate.Properties.DropDownRows = 10;
            control.cboReportTemplate.Properties.PopupWidth = 800;
            //xem sau
            if (templates.Count == 1)
            {
                control.cboReportTemplate.EditValue = templates[0].ID;
                control.txtReportTemplateCode.Text = templates[0].REPORT_TEMPLATE_CODE;
            }
            else if (templates.Count > 1)
            {
                control.txtReportTemplateCode.Text = "";
            }
        }
        public static void SelectFirstRowPopup(LookUpEdit cbo)
        {
            try
            {
                if (cbo != null && cbo.IsPopupOpen)
                {
                    DevExpress.Utils.Win.IPopupControl popupEdit = cbo as DevExpress.Utils.Win.IPopupControl;
                    DevExpress.XtraEditors.Popup.PopupLookUpEditForm popupWindow = popupEdit.PopupWindow as DevExpress.XtraEditors.Popup.PopupLookUpEditForm;
                    if (popupWindow != null)
                    {
                        popupWindow.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal static void LoadReportTemplateCombo(string searchCode, bool isExpand, TemplateBetweenTimeFilterOnly control)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    control.cboReportTemplate.EditValue = null;
                    control.cboReportTemplate.Focus();
                    control.cboReportTemplate.ShowPopup();
                    SelectFirstRowPopup(control.cboReportTemplate);
                }
                else
                {
                    var data = GlobalStore.reportTemplates.Where(o => o.REPORT_TEMPLATE_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            control.cboReportTemplate.EditValue = data[0].ID;
                            control.txtReportTemplateCode.Text = data[0].REPORT_TEMPLATE_CODE;
                            control.txtReportName.SelectAll();
                            control.txtReportName.Focus();
                        }
                        else if (data.Count > 1)
                        {
                            control.cboReportTemplate.EditValue = null;
                            control.cboReportTemplate.Focus();
                            control.cboReportTemplate.ShowPopup();
                            SelectFirstRowPopup(control.cboReportTemplate);
                        }
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

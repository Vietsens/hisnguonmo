using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.CreateReport.Design.TemplateBetweenTimeFilterOnly.TemplateBetweenTimeFilterOnlyValidation
{
    class ReportTypeValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtReportTypeCode;
        internal DevExpress.XtraEditors.LookUpEdit cboReportType;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtReportTypeCode == null || cboReportType == null) return valid;
                if (string.IsNullOrEmpty(txtReportTypeCode.Text) || string.IsNullOrEmpty(cboReportType.Text)) return valid;
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
    }
}

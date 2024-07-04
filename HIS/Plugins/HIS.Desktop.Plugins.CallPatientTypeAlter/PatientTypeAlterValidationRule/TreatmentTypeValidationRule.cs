using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PatientTypeAlter
{
    class TreatmentTypeValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.LookUpEdit cboTreatmentType;
        internal DevExpress.XtraEditors.TextEdit txtTreatmentTypeCode;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtTreatmentTypeCode == null || cboTreatmentType == null) return valid;
                if (String.IsNullOrEmpty(txtTreatmentTypeCode.Text) || cboTreatmentType.EditValue == null)
                    return valid;

                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}

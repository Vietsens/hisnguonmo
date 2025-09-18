using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace His.UC.UCHein.Design.TemplateHeinBHYT1.ValidationRule
{
    class TemplateHeinBHYT1__PatientTypeCode__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.GridLookUpEdit cboPatientCode;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (cboPatientCode == null) return valid;
                if (cboPatientCode.Enabled  || cboPatientCode.EditValue == null)
                    return valid;
                if(cboPatientCode.EditValue.ToString() == "")
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

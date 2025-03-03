using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Numeric
{
    class NumericValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        //internal DevExpress.XtraEditors.GridLookUpEdit cboTreatmentType;
        public DevExpress.XtraEditors.SpinEdit numericUpDown1;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool result = false;
            try
            {
                if (numericUpDown1 == null) return result;
                if (numericUpDown1.EditValue == null) return result;
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

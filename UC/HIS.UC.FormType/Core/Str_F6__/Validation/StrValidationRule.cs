using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Str.Validation
{
    class StrValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        //internal DevExpress.XtraEditors.GridLookUpEdit cboTreatmentType;
        internal DevExpress.XtraEditors.TextEdit Str;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool result = false;
            try
            {
                 if (Str == null) return result;
                if (Str.Text.Trim() == "") return result;
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

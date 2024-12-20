using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Core.TrueOrFalse.Validation
{
    class TrueOrFalseValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal System.Windows.Forms.RadioButton radTrue;
        internal System.Windows.Forms.RadioButton radFalse;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (radTrue == null&&radFalse ==null) return valid;
                if (radTrue.Checked == false && radFalse.Checked == false) return valid;
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
    }
}

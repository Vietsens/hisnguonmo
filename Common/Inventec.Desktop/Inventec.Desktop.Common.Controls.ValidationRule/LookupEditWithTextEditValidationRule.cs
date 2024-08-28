using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Desktop.Common.Controls.ValidationRule
{
    public class LookupEditWithTextEditValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        public DevExpress.XtraEditors.TextEdit txtTextEdit;
        public DevExpress.XtraEditors.LookUpEdit cbo;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtTextEdit == null || cbo == null) return valid;
                if (String.IsNullOrEmpty(txtTextEdit.Text) || cbo.EditValue == null)
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

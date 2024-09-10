using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Feedback.Design.Template1.Validation
{
    class Author__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtAuthor;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {

            bool valid = false;
            try
            {
                if (txtAuthor == null) return valid;
                if (string.IsNullOrEmpty(txtAuthor.Text)) return valid;
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

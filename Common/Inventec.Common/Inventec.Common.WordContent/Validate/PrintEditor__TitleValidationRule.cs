using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Common.WordContent.Validate
{
    class PrintEditor__TitleValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtTitle;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtTitle == null) return valid;
                if (String.IsNullOrEmpty(txtTitle.Text))
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

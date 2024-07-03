using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter
{
    class WorkPlaceValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.GridLookUpEdit cbo;
        internal DevExpress.XtraEditors.TextEdit txt;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txt == null || cbo == null) return valid;
                if (String.IsNullOrEmpty(txt.Text) && cbo.EditValue == null)
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

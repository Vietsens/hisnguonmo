using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter.ValidationRule
{
    internal class PrimaryPatientTypeCodeValidate : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtPrimaryPatientTypeCode;
        internal DevExpress.XtraEditors.GridLookUpEdit cboPrimaryPatientTypeCode;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtPrimaryPatientTypeCode == null || txtPrimaryPatientTypeCode == null) return valid;
                if (String.IsNullOrEmpty(txtPrimaryPatientTypeCode.Text) || txtPrimaryPatientTypeCode.EditValue == null)
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

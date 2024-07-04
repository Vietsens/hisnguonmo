using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter
{
    class PatientTypeValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtMaDoiTuong;
        internal DevExpress.XtraEditors.LookUpEdit cboDoiTuong;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtMaDoiTuong == null || cboDoiTuong == null) return valid;
                if (String.IsNullOrEmpty(txtMaDoiTuong.Text) || cboDoiTuong.EditValue == null)
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

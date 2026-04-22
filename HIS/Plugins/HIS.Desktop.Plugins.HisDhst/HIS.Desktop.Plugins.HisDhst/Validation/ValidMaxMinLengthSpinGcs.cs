using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisDhst.Validation
{
    class ValidMaxMinLengthSpinGcs : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.SpinEdit spinGcs;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (spinGcs == null) return valid;

                // Khong bat buoc nhap — trong thi hop le
                if (spinGcs.EditValue == null || string.IsNullOrEmpty(spinGcs.Text))
                {
                    return true;
                }

                if (spinGcs.Value < 3 || spinGcs.Value > 15)
                {
                    ErrorText = "Điểm GCS phải nằm trong khoảng 3 - 15";
                    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    return valid;
                }
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

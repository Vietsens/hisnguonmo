using Inventec.Desktop.Common.LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PatientUpdate
{
    class ValidateBHXHCode : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.BaseControl txtControl;
        internal int? maxLength;
        internal int? minLength;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool success = false;
            try
            {
                if (!string.IsNullOrEmpty(txtControl.Text) && Encoding.UTF8.GetByteCount(txtControl.Text.Trim()) < minLength)
                {
                    base.ErrorText = "Mã BHXH phải nhập đủ 10 ký tự";
                    base.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    return success;
                }
                if (!string.IsNullOrEmpty(txtControl.Text) && Encoding.UTF8.GetByteCount(txtControl.Text.Trim()) > maxLength)
                {
                    base.ErrorText = "Trường dữ liệu vượt quá ký tự cho phép";
                    base.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    return success;
                }
                success = true;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }
    }
}

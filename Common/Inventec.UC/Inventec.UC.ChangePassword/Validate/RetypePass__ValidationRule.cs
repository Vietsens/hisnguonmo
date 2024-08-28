using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ChangePassword.Validate
{
    class RetypePass__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtRetypePass;
        internal DevExpress.XtraEditors.TextEdit txtNewPass;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtRetypePass == null) return valid;
                if (string.IsNullOrEmpty(txtRetypePass.Text))
                {
                    this.ErrorText = "Thiếu trường dữ liệu bắt buộc";
                    return valid;
                }
                if (string.IsNullOrEmpty(txtRetypePass.Text))
                {
                    this.ErrorText = "Thiếu trường dữ liệu bắt buộc";
                    return valid;
                }
                if (txtRetypePass.Text != txtNewPass.Text)
                {
                    this.ErrorText = "Mật khẩu nhập lại phải giống mật khẩu mới";
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

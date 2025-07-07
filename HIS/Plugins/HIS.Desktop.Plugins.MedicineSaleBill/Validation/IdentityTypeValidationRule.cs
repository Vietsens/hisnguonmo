using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicineSaleBill.Validation
{
    class IdentityTypeValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.GridLookUpEdit cboIdentityType;
        internal DevExpress.XtraEditors.TextEdit txtIdentityType;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (cboIdentityType == null || txtIdentityType == null) return valid;

                // Kiểm tra điều kiện: Nếu txtIdentityType có giá trị nhưng cboIdentityType không có giá trị
                if (!string.IsNullOrWhiteSpace(txtIdentityType.Text) && string.IsNullOrWhiteSpace(cboIdentityType.Text))
                {
                    ErrorText = "Bắt buộc chọn loại giấy tờ khi có thông tin định danh";
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

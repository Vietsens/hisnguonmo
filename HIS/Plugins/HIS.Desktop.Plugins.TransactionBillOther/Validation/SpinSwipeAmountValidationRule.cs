using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBillOther.Validation
{
    class SpinSwipeAmountValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        public SpinEdit spinSwipeAmount;
        public LookUpEdit cboPayForm;
        public override bool Validate(Control control1, object value)
        {
            bool valid = false;
            try
            {
                if (spinSwipeAmount == null)
                    return valid;

                if (spinSwipeAmount.Value == 0 && cboPayForm?.EditValue != null && Convert.ToInt64(cboPayForm.EditValue) == 9)
                {
                    ErrorText = "Số tiền quẹt thẻ phải lớn hơn 0";
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

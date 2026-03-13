using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using System;

namespace HIS.Desktop.Plugins.TransactionBill.Validtion
{
    public class SpinTransferAndSwipeAmountNewValidationRule : ValidationRule
    {
        public SpinEdit SpinTransferAmountNew { get; set; }
        public SpinEdit SpinSwipeAmountNew { get; set; }
        public Func<long> GetPayFormId { get; set; }

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            try
            {
                if (GetPayFormId == null
                    || SpinTransferAmountNew == null
                    || SpinSwipeAmountNew == null)
                    return true;

                var payFormId = GetPayFormId();
                if (payFormId != 9) 
                    return true;

                var ck = SpinTransferAmountNew.Value;
                var qt = SpinSwipeAmountNew.Value;

                if (ck <= 0 && qt <= 0)
                {
                    ErrorText = "Vui lòng nhập số tiền chuyển khoản hoặc số tiền quẹt thẻ.";
                    ErrorType = ErrorType.Warning;
                    return false;
                }

                return true;
            }
            catch
            {
                return true;
            }
        }
    }
}
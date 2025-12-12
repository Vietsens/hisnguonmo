using DevExpress.XtraTreeList.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne.Validation
{
    class SpinSoTienCKValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.SpinEdit spinSoTienCK;
        internal decimal soTienThu;
        internal bool isReceipt;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                //if (spinSoTienCK == null ||  spinSoTienCK.Value > soTienThu) return valid;
                if (spinSoTienCK.Value > soTienThu && isReceipt)
                {
                    ErrorText = "Số tiền chuyển khoản lớn hơn số tiền thanh toán của bệnh nhân";
                    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    return valid;
                }
                if(!isReceipt && spinSoTienCK.Value > soTienThu)
                {
                    ErrorText = "Số tiền quẹt thẻ lớn hơn số tiền thanh toán của bệnh nhân";
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

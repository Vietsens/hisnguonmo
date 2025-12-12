using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBillOther.Validation
{
    public class SpinToTalAmountValidationRule : ValidationRule
    {
        public SpinEdit spinTotalAmount;
        public LookUpEdit cboPayForm;
        public override bool Validate(Control control, object value)
        {
            try
            {
                decimal soTienCk = (value is decimal) ? (decimal)value : 0;
                decimal totalAmount = spinTotalAmount?.Value ?? 0;
                long payFormId = cboPayForm?.EditValue != null ? Convert.ToInt64(cboPayForm.EditValue) : 0;
                //if(soTienCk < 0)
                //{
                //    ErrorText = "Số tiền phải lớn hơn 0";
                //    ErrorType = ErrorType.Warning;
                //    return false;
                //}    
                if ((payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK || payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                    && soTienCk > totalAmount)
                {
                    ErrorText = (payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                        ? "Số tiền quẹt thẻ lớn hơn số tiền thanh toán của bệnh nhân"
                        : "Số tiền chuyển khoản lớn hơn số tiền thanh toán của bệnh nhân";
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

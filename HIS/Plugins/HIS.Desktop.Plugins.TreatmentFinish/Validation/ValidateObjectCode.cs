using DevExpress.XtraEditors.DXErrorProvider;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentFinish.Validation
{
    class ValidateObjectCode : ValidationRule
    {
        public override bool Validate(Control control, object value)
        {
            var txt = control as TextBox;
            if (txt == null) return false;
            string text = txt.Text;
          
            bool onlyDigitsAndDot = text.All(c => char.IsDigit(c) || c == '.');
          
            bool lengthValid = text.Length <= 10;
            if (!onlyDigitsAndDot)
            {
                ErrorText = "Ch? ???c nh?p s? và d?u ch?m.";
                ErrorType = ErrorType.Warning;
                return false;
            }
            if (!lengthValid)
            {
                ErrorText = "T?i ?a 10 ký t?.";
                ErrorType = ErrorType.Warning;
                return false;
            }
            return true;
        }
    }
}
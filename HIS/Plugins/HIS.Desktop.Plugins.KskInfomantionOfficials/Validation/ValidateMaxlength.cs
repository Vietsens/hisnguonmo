using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials.Validation
{
    public class ValidateMaxLength : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.BaseEdit textEdit;
        internal int? maxLength;
        internal bool isRequired = false;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (textEdit == null) return valid;

                if (isRequired && String.IsNullOrWhiteSpace(textEdit.Text))
                {
                    this.ErrorText = Resources.ResourceMessage.TruongDuLieuBatBuoc;
                    this.ErrorType = ErrorType.Warning;
                    return valid;
                }
                if (maxLength != null && !String.IsNullOrEmpty(textEdit.Text) && Inventec.Common.String.CountVi.Count(textEdit.Text) > maxLength)
                {
                    this.ErrorText = String.Format(Resources.ResourceMessage.VuotQuaDoDaiChoPhep, maxLength);
                    this.ErrorType = ErrorType.Warning;
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

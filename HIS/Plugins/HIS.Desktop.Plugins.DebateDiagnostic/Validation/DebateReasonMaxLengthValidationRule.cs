using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DebateDiagnostic
{
    class DebateReasonMaxLengthValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal int maxLength;

        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                string text = (value ?? "").ToString();
                if (text.Length > maxLength)
                {
                    ErrorText = Resources.ResourceMessage.LyDoHoiChanToiDa500KyTu;
                    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
    }
}

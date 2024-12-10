using His.UC.LibraryMessage;
using HIS.UC.FormType.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Core.HeinTreatmentTypeRadio.Validation
{
    class HeinTreatmentTypeValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.CheckEdit radioAll;
        internal DevExpress.XtraEditors.CheckEdit radioExam;
        internal DevExpress.XtraEditors.CheckEdit radioTreat;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (radioAll == null || radioExam == null || radioTreat == null) return valid;
                if (!radioAll.Checked && radioExam.Checked && radioTreat.Checked)
                {
                    ErrorText = MessageUtil.GetMessage(Message.Enum.TruongDuLieuBatBuoc);
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

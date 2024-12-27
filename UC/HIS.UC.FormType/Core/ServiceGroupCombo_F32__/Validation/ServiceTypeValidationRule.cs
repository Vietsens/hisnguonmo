using His.UC.LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Core.ServiceGroupCombo_F32__.Validation
{
    class ServiceTypeValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtServiceTypeCode;
        internal DevExpress.XtraEditors.LookUpEdit cboServiceType;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtServiceTypeCode == null || cboServiceType == null) return valid;
                if (String.IsNullOrEmpty(txtServiceTypeCode.Text)||cboServiceType.EditValue==null)
                {
                    ErrorText = HIS.UC.FormType.Base.MessageUtil.GetMessage(Message.Enum.ThieuTruongDuLieuBatBuoc);
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

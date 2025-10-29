using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InviteSpecialistExam.Validation
{
    class ValidationGridlookup : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cboKhoa;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (cboKhoa.EditValue == null || cboKhoa == null)
                {
                    this.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    return valid;
                }
                else
                {
                    valid = true;
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

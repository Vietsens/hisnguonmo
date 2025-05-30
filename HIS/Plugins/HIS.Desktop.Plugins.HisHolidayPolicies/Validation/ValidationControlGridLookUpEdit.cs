using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisHolidayPolicies.Validation
{
    class ValidationControlGridLookUpEdit : ValidationRule
    {
        internal GridLookUpEdit gridLookUpEdit;
        internal bool isVisible;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (!isVisible) return true; // Skip validation if not visible
                if (gridLookUpEdit.EditValue == null)
                {
                    this.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    return valid;
                }
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
    }
}

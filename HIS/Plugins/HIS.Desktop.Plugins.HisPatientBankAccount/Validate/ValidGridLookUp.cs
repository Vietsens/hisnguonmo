using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisPatientBankAccount.Validate
{
    class ValidGridLookUp : ValidationRule
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
                if (Inventec.Common.String.CountVi.Count(gridLookUpEdit.Text) > 100)
                {
                    this.ErrorText = "Độ dài mã vượt quá 100 ký tự";
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

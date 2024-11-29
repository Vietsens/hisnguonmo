using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisHivGroupPatient.Validation
{
    class ValidationControlTextEditPatientName : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtTen;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (string.IsNullOrEmpty(txtTen.Text))
                {
                    this.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    return valid;
                }
                else
                {
                    if (Inventec.Common.String.CountVi.Count(txtTen.Text) > 200)
                    {
                        this.ErrorText = "Độ dài tên vượt quá " + 200;
                        return valid;
                    }
                    else
                    {
                        if (Inventec.Common.String.CountVi.Count(txtTen.Text) > 200)
                        {
                            this.ErrorText = "Độ dài tên vượt quá " + 200;
                            return valid;
                        }
                        else
                        {
                            valid = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}

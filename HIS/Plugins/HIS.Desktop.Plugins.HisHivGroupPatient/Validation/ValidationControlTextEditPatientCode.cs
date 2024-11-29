using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Common.String;

namespace HIS.Desktop.Plugins.HisHivGroupPatient.Validation
{
    class ValidationControlTextEditPatientCode : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtMa;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (string.IsNullOrEmpty(txtMa.Text))
                {
                    this.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    return valid;
                }
                else
                {
                    // Kiểm tra ký tự không phải số
                    if (System.Text.RegularExpressions.Regex.IsMatch(txtMa.Text, "[^0-9]"))
                    {
                        this.ErrorText = "Mã chỉ được chứa ký tự số.";
                        return valid;
                    }

                    if (Inventec.Common.String.CountVi.Count(txtMa.Text) > 20)
                    {
                        this.ErrorText = "Độ dài mã vượt quá " + 20;
                        return valid;
                    }
                    else
                    {
                        if (Inventec.Common.String.CountVi.Count(txtMa.Text) > 20)
                        {
                            this.ErrorText = "Độ dài mã vượt quá " + 20;
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

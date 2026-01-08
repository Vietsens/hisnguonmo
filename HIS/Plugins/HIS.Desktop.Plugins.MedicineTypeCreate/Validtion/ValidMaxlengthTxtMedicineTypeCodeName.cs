using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.MedicineTypeCreate.Config;

namespace HIS.Desktop.Plugins.MedicineTypeCreate.Validtion
{
    class ValidMaxlengthTxtMedicineTypeCodeName : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtMedicineTypeCode;
        internal DevExpress.XtraEditors.TextEdit txtMedicineTypeName;
        internal bool isValidCode = true;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                // Kiểm tra config SERVICE_CODE_OPTION
                bool isCodeRequired = HisConfigCFG.ServiceCodeOption != "1"; // Nếu = 1 thì không bắt buộc mã
                
                // Nếu bắt buộc mã (config != 1) và đang ở chế độ cần validate mã
                if ((isValidCode && isCodeRequired && string.IsNullOrEmpty(txtMedicineTypeCode.Text)) 
                    || string.IsNullOrEmpty(txtMedicineTypeName.Text))
                {
                    this.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    return valid;
                }
                else
                {
                    if (Inventec.Common.String.CountVi.Count(txtMedicineTypeCode.Text) > 50 && Inventec.Common.String.CountVi.Count(txtMedicineTypeName.Text) > 3000)
                    {
                        this.ErrorText = "Độ dài mã vượt quá " + 50 + "||" + "Độ dài tên vượt quá " + 3000;
                        return valid;
                    }
                    else
                    {
                        var len = Inventec.Common.String.CountVi.Count(txtMedicineTypeName.Text);
                        var lenn = txtMedicineTypeName.Text.Length;
                        if (Inventec.Common.String.CountVi.Count(txtMedicineTypeCode.Text) > 50)
                        {
                            this.ErrorText = "Độ dài mã vượt quá " + 50;
                            return valid;
                        }
                        else if (len > 3000)
                        {
                            this.ErrorText = "Độ dài tên vượt quá " + 3000;
                            return valid;
                        }
                        else
                            valid = true;
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

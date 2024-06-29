/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if ((isValidCode && string.IsNullOrEmpty(txtMedicineTypeCode.Text)) || string.IsNullOrEmpty(txtMedicineTypeName.Text))
                {
                    this.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    return valid;
                }
                else
                {
                    if (Inventec.Common.String.CountVi.Count(txtMedicineTypeCode.Text) > 25 && Inventec.Common.String.CountVi.Count(txtMedicineTypeName.Text) > 1500)
                    {
                        this.ErrorText = "Độ dài mã vượt quá " + 25 + "||" + "Độ dài tên vượt quá " + 1500;
                        return valid;
                    }
                    else
                    {
                        var len = Inventec.Common.String.CountVi.Count(txtMedicineTypeName.Text);
                        var lenn = txtMedicineTypeName.Text.Length;
                        if (Inventec.Common.String.CountVi.Count(txtMedicineTypeCode.Text) > 25)
                        {
                            this.ErrorText = "Độ dài mã vượt quá " + 25;
                            return valid;
                        }
                        else if (len > 1500)
                        {
                            this.ErrorText = "Độ dài tên vượt quá " + 1500;
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
